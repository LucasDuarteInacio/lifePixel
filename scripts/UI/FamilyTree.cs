using Godot;
using LifeSimulator.Core.Characters;
using LifeSimulator.Core.Genealogy;
using LifeSimulator.Core.Relationships;
using LifeSimulator.Core.Simulation;
using static LifeSimulator.UI.UiStyle;

namespace LifeSimulator.UI;

/// <summary>Desenha as arestas do grafo: casais e pais→filhos.</summary>
public partial class TreeEdges : Control
{
	public readonly List<(Vector2 From, Vector2 To, Color Color)> Edges = new();

	public override void _Draw()
	{
		foreach (var (from, to, color) in Edges)
			DrawLine(from, to, color, 3f, antialiased: true);
	}
}

/// <summary>
/// Árvore genealógica visual (GDD §7.1): nós por geração — avós, pais
/// e tios, ego e irmãos/primos/cônjuge, filhos e sobrinhos, netos —
/// conectados por arestas de casal e de filiação. Toque no nó abre o
/// detalhe da pessoa.
/// </summary>
public partial class FamilyTree : Control
{
	private const string BackScene = "res://scenes/relationships.tscn";

	private const int NodeW = 160;
	private const int NodeH = 118;
	private const int HGap = 26;
	private const int VGap = 150;

	private static readonly Color CoupleEdge = new("df8bc0");
	private static readonly Color BloodEdge = new("5a4a78");

	private LifeSimulation _sim = null!;
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

		var scroll = new ScrollContainer
		{
			SizeFlagsVertical = SizeFlags.ExpandFill,
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
		};
		screen.AddChild(scroll);

		var content = new Control();
		scroll.AddChild(content);
		BuildGraph(content);
		BuildToast();
	}

	private Control BuildTopBar()
	{
		var bar = new HBoxContainer();
		bar.AddThemeConstantOverride("separation", 10);

		var back = new Button { Text = "←", CustomMinimumSize = new Vector2(48, 48) };
		StyleButton(back, Surface, Stroke, 10, 20);
		back.Pressed += () => GetTree().ChangeSceneToFile(BackScene);
		bar.AddChild(back);

		var brand = new VBoxContainer();
		brand.AddThemeConstantOverride("separation", 3);
		brand.AddChild(Lbl("ÁRVORE GENEALÓGICA", 20, Txt));
		brand.AddChild(Lbl("Gerações da sua dinastia", 13, Muted));
		bar.AddChild(brand);
		return bar;
	}

	// ---------------------------------------------------------------
	// Grafo
	// ---------------------------------------------------------------

	private void BuildGraph(Control content)
	{
		var ego = _sim.Player;
		var rows = BuildRows(ego);
		var maxRow = rows.Max(r => r.Count);
		var width = Mathf.Max(680, maxRow * (NodeW + HGap) + HGap);
		var height = rows.Count * VGap + NodeH / 2;
		content.CustomMinimumSize = new Vector2(width, height);

		var edges = new TreeEdges { CustomMinimumSize = new Vector2(width, height) };
		content.AddChild(edges);

		var positions = new Dictionary<Person, Vector2>();
		var nodeCenters = new Dictionary<Person, Vector2>();
		for (var r = 0; r < rows.Count; r++)
		{
			var row = rows[r];
			var rowWidth = row.Count * (NodeW + HGap) - HGap;
			var x0 = (width - rowWidth) / 2f;
			for (var i = 0; i < row.Count; i++)
			{
				var person = row[i];
				var pos = new Vector2(x0 + i * (NodeW + HGap), r * VGap);
				positions[person] = pos;
				nodeCenters[person] = pos + new Vector2(NodeW / 2f, NodeH / 2f);
				content.AddChild(BuildNode(person, ego, pos));
			}
		}

		// Arestas de casal: uma por par posicionado.
		var couples = new HashSet<(Guid, Guid)>();
		foreach (var (person, center) in nodeCenters)
		{
			if (person.Partner is not { } partner || !nodeCenters.TryGetValue(partner, out var other))
				continue;
			var pair = string.CompareOrdinal(person.Id.ToString(), partner.Id.ToString()) < 0
				? (person.Id, partner.Id) : (partner.Id, person.Id);
			if (couples.Add(pair))
				edges.Edges.Add((center, other, CoupleEdge));
		}

		// Arestas de filiação: do ponto médio dos pais ao topo do filho.
		foreach (var (child, childTop) in positions)
		{
			var parents = new[] { child.Mother, child.Father }
				.Where(p => p is not null && positions.ContainsKey(p))
				.Select(p => positions[p!] + new Vector2(NodeW / 2f, NodeH))
				.ToList();
			if (parents.Count == 0)
				continue;
			var from = new Vector2(
				parents.Average(p => p.X), parents.Average(p => p.Y));
			var to = childTop + new Vector2(NodeW / 2f, 0);
			edges.Edges.Add((from, to, BloodEdge));
		}
	}

	private List<List<Person>> BuildRows(Person ego)
	{
		var placed = new HashSet<Person>();
		List<Person> Row(params IEnumerable<Person>[] groups)
		{
			var row = new List<Person>();
			foreach (var g in groups)
				foreach (var p in g)
					if (placed.Add(p))
						row.Add(p);
			return row;
		}
		static IEnumerable<Person> WithPartner(Person p)
		{
			yield return p;
			if (p.Partner is not null) yield return p.Partner;
		}

		var parents = Kinship.Parents(ego).ToList();
		var uncles = Kinship.UnclesAunts(ego).ToList();
		var cousins = Kinship.Cousins(ego).ToList();
		var niblings = Kinship.NiecesNephews(ego).ToList();

		var rows = new List<List<Person>>
		{
			// -2: avós
			Row(Kinship.Grandparents(ego)),
			// -1: pais, sogros, tios e parceiros
			Row(parents.SelectMany(WithPartner),
				ego.Partner is { } s ? Kinship.Parents(s) : [],
				uncles.SelectMany(WithPartner)),
			// 0: ego + cônjuge, irmãos e primos com parceiros
			Row(new[] { ego }.Concat(ego.Partner is { } sp ? new[] { sp } : []),
				ego.Siblings.SelectMany(WithPartner),
				cousins.SelectMany(WithPartner)),
			// +1: filhos e sobrinhos com parceiros, filhos dos primos
			Row(ego.Children.SelectMany(WithPartner),
				niblings.SelectMany(WithPartner),
				cousins.SelectMany(c => c.Children)),
			// +2: netos e sobrinhos-netos
			Row(Kinship.Grandchildren(ego),
				niblings.SelectMany(n => n.Children)),
		};

		rows.RemoveAll(r => r.Count == 0);
		return rows;
	}

	private Control BuildNode(Person person, Person ego, Vector2 pos)
	{
		var isEgo = person == ego;
		var button = new Button
		{
			Position = pos,
			Size = new Vector2(NodeW, NodeH),
		};
		StyleButton(button,
			isEgo ? Surface3 : Surface,
			isEgo ? Purple2 : Stroke, 12, 12);

		var col = new VBoxContainer
		{
			MouseFilter = MouseFilterEnum.Ignore,
			Alignment = BoxContainer.AlignmentMode.Center,
		};
		col.SetAnchorsPreset(LayoutPreset.FullRect);
		col.AddThemeConstantOverride("separation", 2);
		button.AddChild(col);

		col.AddChild(Lbl(person.IsAlive ? PersonDetail.Avatar(person) : "🪦",
			26, Txt, HorizontalAlignment.Center));

		var name = Lbl(person.FirstName, 14,
			person.IsAlive ? Txt : Muted, HorizontalAlignment.Center);
		name.TextOverrunBehavior = TextServer.OverrunBehavior.TrimEllipsis;
		name.CustomMinimumSize = new Vector2(NodeW - 12, 0);
		col.AddChild(name);

		var sub = isEgo
			? "você"
			: person.IsAlive
				? $"{KinLabel.Label(ego, person)} · {person.AgeYears}"
				: KinLabel.Label(ego, person);
		var subLabel = Lbl(sub, 11, Muted, HorizontalAlignment.Center);
		subLabel.TextOverrunBehavior = TextServer.OverrunBehavior.TrimEllipsis;
		subLabel.CustomMinimumSize = new Vector2(NodeW - 12, 0);
		col.AddChild(subLabel);

		button.Pressed += () => PersonDetail.Show(
			this, _sim, person, ShowToast);
		return button;
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
