using Godot;
using LifeSimulator.Core.Characters;
using LifeSimulator.Core.Events;
using LifeSimulator.Core.Genealogy;
using LifeSimulator.Core.Relationships;
using LifeSimulator.Core.Simulation;
using static LifeSimulator.UI.UiStyle;

namespace LifeSimulator.UI;

/// <summary>
/// Tela de relações (GDD §5.1, §7.3): pessoas conhecidas agrupadas por
/// vínculo — parceiro, família, amigos, rivais e ex — com os
/// indicadores de destaque, mais o histórico familiar (§7.2) e acesso
/// à árvore genealógica.
/// </summary>
public partial class Relationships : Control
{
	private const string MainScene = "res://scenes/main.tscn";
	private const string TreeScene = "res://scenes/family_tree.tscn";

	private LifeSimulation _sim = null!;
	private VBoxContainer _peopleList = null!;
	private VBoxContainer _historyList = null!;
	private ScrollContainer _peopleScroll = null!;
	private ScrollContainer _historyScroll = null!;
	private Button _peopleTab = null!;
	private Button _historyTab = null!;
	private PanelContainer _toast = null!;
	private Label _toastLabel = null!;
	private Tween? _toastTween;

	public override void _Ready()
	{
		_sim = GameSetup.Current ?? LifeSimulation.NewRandomLife();
		GameSetup.Current ??= _sim;
		_sim.EnsureExtendedFamily();

		var background = new ColorRect { Color = Bg };
		background.SetAnchorsPreset(LayoutPreset.FullRect);
		AddChild(background);

		var margin = new MarginContainer();
		margin.SetAnchorsPreset(LayoutPreset.FullRect);
		foreach (var side in new[] { "left", "right", "top", "bottom" })
			margin.AddThemeConstantOverride($"margin_{side}", 16);
		AddChild(margin);

		var screen = new VBoxContainer();
		screen.AddThemeConstantOverride("separation", 14);
		margin.AddChild(screen);

		screen.AddChild(BuildTopBar());
		screen.AddChild(BuildTabs());

		_peopleScroll = new ScrollContainer
		{
			SizeFlagsVertical = SizeFlags.ExpandFill,
			HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled,
		};
		_peopleList = new VBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };
		_peopleList.AddThemeConstantOverride("separation", 10);
		_peopleScroll.AddChild(_peopleList);
		screen.AddChild(_peopleScroll);

		_historyScroll = new ScrollContainer
		{
			SizeFlagsVertical = SizeFlags.ExpandFill,
			HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled,
			Visible = false,
		};
		_historyList = new VBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };
		_historyList.AddThemeConstantOverride("separation", 12);
		_historyScroll.AddChild(_historyList);
		screen.AddChild(_historyScroll);

		BuildToast();
		Refresh();
	}

	private Control BuildTopBar()
	{
		var bar = new HBoxContainer();
		bar.AddThemeConstantOverride("separation", 10);

		var back = new Button { Text = "←", CustomMinimumSize = new Vector2(48, 48) };
		StyleButton(back, Surface, Stroke, 10, 20);
		back.Pressed += () => GetTree().ChangeSceneToFile(MainScene);
		bar.AddChild(back);

		var brand = new VBoxContainer();
		brand.AddThemeConstantOverride("separation", 3);
		brand.AddChild(Lbl("RELAÇÕES", 20, Txt));
		brand.AddChild(Lbl("Família, amizades e amores", 13, Muted));
		bar.AddChild(brand);

		bar.AddChild(HSpacer());

		var tree = new Button
		{
			Text = "🌳 Árvore",
			CustomMinimumSize = new Vector2(0, 48),
		};
		StyleButton(tree, Surface, Stroke, 10, 15);
		tree.Pressed += () => GetTree().ChangeSceneToFile(TreeScene);
		bar.AddChild(tree);

		return bar;
	}

	private Control BuildTabs()
	{
		var row = new HBoxContainer();
		row.AddThemeConstantOverride("separation", 8);

		_peopleTab = new Button
		{
			Text = "Pessoas",
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
			CustomMinimumSize = new Vector2(0, 46),
		};
		_historyTab = new Button
		{
			Text = "Histórico familiar",
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
			CustomMinimumSize = new Vector2(0, 46),
		};

		_peopleTab.Pressed += () => ShowTab(people: true);
		_historyTab.Pressed += () => ShowTab(people: false);
		row.AddChild(_peopleTab);
		row.AddChild(_historyTab);
		StyleTabs(people: true);
		return row;
	}

	private void StyleTabs(bool people)
	{
		StyleButton(_peopleTab, people ? Purple2 : Surface,
			people ? Purple2 : Stroke, 10, 14);
		StyleButton(_historyTab, people ? Surface : Purple2,
			people ? Stroke : Purple2, 10, 14);
	}

	private void ShowTab(bool people)
	{
		_peopleScroll.Visible = people;
		_historyScroll.Visible = !people;
		StyleTabs(people);
	}

	// ---------------------------------------------------------------
	// Lista de pessoas
	// ---------------------------------------------------------------

	private void Refresh()
	{
		foreach (var child in _peopleList.GetChildren())
			child.QueueFree();
		foreach (var child in _historyList.GetChildren())
			child.QueueFree();

		BuildPeopleList();
		BuildHistoryList();
	}

	private void BuildPeopleList()
	{
		var player = _sim.Player;
		var shown = new HashSet<Guid> { player.Id };

		if (player.Partner is { } partner)
			AddGroup("PARCEIRO(A)", new[] { partner }, shown);

		var family = new List<Person>();
		void AddKin(IEnumerable<Person> people)
		{
			foreach (var p in people)
				if (!family.Contains(p))
					family.Add(p);
		}
		AddKin(Kinship.Parents(player));
		AddKin(player.Siblings);
		AddKin(player.Children);
		AddKin(Kinship.Grandparents(player));
		AddKin(Kinship.UnclesAunts(player));
		AddKin(Kinship.Cousins(player));
		AddKin(Kinship.NiecesNephews(player));
		AddKin(Kinship.Grandchildren(player));
		AddKin(Kinship.InLaws(player));
		AddGroup("FAMÍLIA", family, shown);

		var friends = KnownOf(RelationshipKind.Friend);
		AddGroup("AMIGOS", friends, shown);

		var rivals = KnownOf(RelationshipKind.Rival);
		AddGroup("RIVAIS", rivals, shown);

		var exes = KnownOf(RelationshipKind.ExPartner);
		AddGroup("EX", exes, shown);

		if (_peopleList.GetChildCount() == 0)
			_peopleList.AddChild(Lbl(
				"Ninguém por perto ainda. Socialize para conhecer pessoas.",
				14, Muted));
	}

	private List<Person> KnownOf(RelationshipKind kind) =>
		_sim.Player.Relationships.Values
			.Where(r => r.Kind == kind)
			.Select(r => _sim.Registry.Find(r.OtherId))
			.Where(p => p is not null)
			.Cast<Person>()
			.ToList();

	private void AddGroup(string title, IEnumerable<Person> people, HashSet<Guid> shown)
	{
		var group = people.Where(p => shown.Add(p.Id)).ToList();
		if (group.Count == 0)
			return;

		_peopleList.AddChild(Lbl(title, 13, Muted));
		foreach (var person in group.OrderByDescending(p => p.IsAlive)
			         .ThenByDescending(p => p.AgeMonths))
			_peopleList.AddChild(BuildPersonRow(person));
	}

	private Control BuildPersonRow(Person person)
	{
		var bond = _sim.BondWith(person);

		var button = new Button { CustomMinimumSize = new Vector2(0, 74) };
		StyleButton(button, Surface, Stroke, 12, 14);

		var row = new HBoxContainer { MouseFilter = MouseFilterEnum.Ignore };
		row.SetAnchorsPreset(LayoutPreset.FullRect);
		row.AddThemeConstantOverride("separation", 12);
		button.AddChild(row);

		var avatar = new PanelContainer { CustomMinimumSize = new Vector2(52, 52) };
		avatar.AddThemeStyleboxOverride("panel",
			PanelStyle(Surface3, Surface3, 10));
		var face = Lbl(person.IsAlive ? PersonDetail.Avatar(person) : "🪦",
			26, Txt, HorizontalAlignment.Center);
		face.VerticalAlignment = VerticalAlignment.Center;
		avatar.AddChild(face);
		row.AddChild(avatar);

		var info = new VBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };
		info.AddThemeConstantOverride("separation", 2);
		var name = person.FullName + (person.IsAlive ? "" : " †");
		info.AddChild(Lbl(name, 16, person.IsAlive ? Txt : Muted));
		var sub = person.IsAlive
			? $"{KinLabel.Label(_sim.Player, person)} · {person.AgeYears} anos"
			: KinLabel.Label(_sim.Player, person);
		info.AddChild(Lbl(sub, 13, Muted));
		row.AddChild(info);

		var values = new VBoxContainer();
		values.AddThemeConstantOverride("separation", 2);
		var indicators = RelationshipRules.IndicatorsFor(bond.Kind);
		if (person.IsAlive && indicators.Count > 0)
		{
			var first = indicators[0];
			values.AddChild(Lbl(
				$"{ShortLabel(first)} {bond.Get(first)}", 13, StatJoy,
				HorizontalAlignment.Right));
			if (indicators.Count > 1 && indicators[1] == RelationshipIndicator.Friendship)
				values.AddChild(Lbl($"Amizade {bond.Friendship}", 13, Green,
					HorizontalAlignment.Right));
			else if (indicators.Count > 1)
				values.AddChild(Lbl(
					$"{ShortLabel(indicators[1])} {bond.Get(indicators[1])}",
					13, Green, HorizontalAlignment.Right));
		}
		row.AddChild(values);

		button.Pressed += () => PersonDetail.Show(
			this, _sim, person, ShowToast, Refresh);
		return button;
	}

	private static string ShortLabel(RelationshipIndicator indicator) =>
		indicator switch
		{
			RelationshipIndicator.Love => "❤",
			RelationshipIndicator.Friendship => "Amizade",
			RelationshipIndicator.Trust => "Confiança",
			RelationshipIndicator.Respect => "Respeito",
			RelationshipIndicator.Intimacy => "Intim.",
			_ => "Rival.",
		};

	// ---------------------------------------------------------------
	// Histórico familiar (GDD §7.2)
	// ---------------------------------------------------------------

	private void BuildHistoryList()
	{
		var events = _sim.Log.Events
			.Where(e => e.Category is EventCategory.Family
				or EventCategory.Relationship)
			.Reverse()
			.Take(40)
			.ToList();

		if (events.Count == 0)
		{
			_historyList.AddChild(Lbl(
				"O histórico da família aparecerá aqui.", 14, Muted));
			return;
		}

		foreach (var gameEvent in events)
		{
			var card = Card(Surface, Stroke, 14);
			var col = new VBoxContainer();
			col.AddThemeConstantOverride("separation", 3);
			card.AddChild(col);
			col.AddChild(Lbl(gameEvent.Title, 15, Txt));
			var desc = Lbl(gameEvent.Description, 13, Muted);
			desc.AutowrapMode = TextServer.AutowrapMode.WordSmart;
			col.AddChild(desc);
			col.AddChild(Lbl(
				$"ANO {_sim.StartYear + gameEvent.AbsoluteMonth / 12} · " +
				$"{gameEvent.AgeYears} ANOS", 12, LogTime));
			_historyList.AddChild(card);
		}
	}

	private void BuildToast()
	{
		_toast = new PanelContainer
		{
			AnchorLeft = 0.5f,
			AnchorRight = 0.5f,
			AnchorTop = 1f,
			AnchorBottom = 1f,
			OffsetTop = -90,
			OffsetBottom = -24,
			CustomMinimumSize = new Vector2(280, 0),
			GrowHorizontal = GrowDirection.Both,
			GrowVertical = GrowDirection.Begin,
			MouseFilter = MouseFilterEnum.Ignore,
			Modulate = new Color(1, 1, 1, 0),
			ZIndex = 100,
		};
		_toast.AddThemeStyleboxOverride("panel",
			PanelStyle(ToastBg, ToastBorder, 10, 10));
		_toastLabel = Lbl("", 15, Txt, HorizontalAlignment.Center);
		_toastLabel.VerticalAlignment = VerticalAlignment.Center;
		_toast.AddChild(_toastLabel);
		AddChild(_toast);
	}

	private void ShowToast(string message)
	{
		_toastLabel.Text = message;
		_toastTween?.Kill();
		_toast.Modulate = new Color(1, 1, 1, 1);
		_toastTween = CreateTween();
		_toastTween.TweenInterval(2.4);
		_toastTween.TweenProperty(_toast, "modulate:a", 0f, 0.3);
	}
}
