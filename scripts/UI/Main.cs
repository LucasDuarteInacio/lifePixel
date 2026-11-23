using Godot;
using LifeSimulator.Core.Activities;
using LifeSimulator.Core.Characters;
using LifeSimulator.Core.Events;
using LifeSimulator.Core.Relationships;
using LifeSimulator.Core.Simulation;
using LifeSimulator.Core.Time;
using static LifeSimulator.UI.UiStyle;

namespace LifeSimulator.UI;

/// <summary>
/// Tela de início do jogo, seguindo o protótipo
/// docs/vertice-inicio-prototipo-godot.html: topo com marca e data,
/// cartão de perfil, indicadores, patrimônio, atividades, diário da
/// vida, avanço de tempo e navegação inferior.
/// </summary>
public partial class Main : Control
{
	private const int MaxLogEntries = 20;
	private const string NewGameScene = "res://scenes/new_game.tscn";

	private static readonly string[] MonthNames =
	{
		"JANEIRO", "FEVEREIRO", "MARÇO", "ABRIL", "MAIO", "JUNHO",
		"JULHO", "AGOSTO", "SETEMBRO", "OUTUBRO", "NOVEMBRO", "DEZEMBRO",
	};

	private LifeSimulation _sim = null!;

	private Label _dateLabel = null!;
	private Label _nameLabel = null!;
	private Label _statusLabel = null!;
	private Label _tagLabel = null!;
	private Label _ageFooter = null!;
	private Label _yearFooter = null!;
	private Label _genFooter = null!;
	private Label _moneyValue = null!;
	private Label _moneyDelta = null!;
	private readonly List<(ProgressBar Bar, Label Pct)> _stats = new();
	private VBoxContainer _logList = null!;
	private VBoxContainer _advanceArea = null!;
	private PanelContainer _toast = null!;
	private Label _toastLabel = null!;
	private Tween? _toastTween;

	public override void _Ready()
	{
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
			HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled,
		};
		screen.AddChild(scroll);

		var column = new VBoxContainer
		{
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
		};
		column.AddThemeConstantOverride("separation", 14);
		scroll.AddChild(column);

		column.AddChild(BuildProfileCard());
		column.AddChild(BuildStatsSection());
		column.AddChild(BuildMoneyCard());
		column.AddChild(BuildActivitiesSection());
		column.AddChild(BuildLogSection());

		_advanceArea = new VBoxContainer();
		_advanceArea.AddThemeConstantOverride("separation", 10);
		column.AddChild(_advanceArea);

		screen.AddChild(BuildBottomNav());
		BuildToast();

		StartNewLife();
	}

	// ---------------------------------------------------------------
	// Construção do layout
	// ---------------------------------------------------------------

	private Control BuildTopBar()
	{
		var bar = new HBoxContainer();
		bar.AddThemeConstantOverride("separation", 10);

		var logo = new PanelContainer { CustomMinimumSize = new Vector2(56, 56) };
		logo.AddThemeStyleboxOverride("panel", PanelStyle(Purple2, Purple2, 11));
		var glyph = Lbl("✦", 30, Txt, HorizontalAlignment.Center);
		glyph.VerticalAlignment = VerticalAlignment.Center;
		logo.AddChild(glyph);
		bar.AddChild(logo);

		var brand = new VBoxContainer();
		brand.AddThemeConstantOverride("separation", 3);
		brand.AddChild(Lbl("LIFE PIXEL", 20, Txt));
		brand.AddChild(Lbl("SIMULADOR DE VIDA", 13, Muted));
		bar.AddChild(brand);

		bar.AddChild(HSpacer());

		_dateLabel = Lbl("", 14, Muted, HorizontalAlignment.Right);
		bar.AddChild(_dateLabel);

		var gear = new Button { Text = "⚙", CustomMinimumSize = new Vector2(48, 48) };
		StyleButton(gear, Surface, Stroke, 10, 22);
		gear.Pressed += () => ShowToast("Configurações em breve.");
		bar.AddChild(gear);

		return bar;
	}

	private Control BuildProfileCard()
	{
		var card = Card(Surface2, new Color("59417c"), 20);
		var col = new VBoxContainer();
		col.AddThemeConstantOverride("separation", 14);
		card.AddChild(col);

		var row = new HBoxContainer();
		row.AddThemeConstantOverride("separation", 18);
		col.AddChild(row);

		var portrait = new TextureRect
		{
			CustomMinimumSize = new Vector2(126, 144),
			ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
		};
		var gradient = new Gradient();
		gradient.SetColor(0, new Color("e7b9a5"));
		gradient.SetColor(1, new Color("a76e9e"));
		portrait.Texture = new GradientTexture2D
		{
			Gradient = gradient,
			Width = 126,
			Height = 144,
			FillFrom = Vector2.Zero,
			FillTo = Vector2.One,
		};
		var face = Lbl("🧑", 64, Txt, HorizontalAlignment.Center);
		face.VerticalAlignment = VerticalAlignment.Center;
		face.SetAnchorsPreset(LayoutPreset.FullRect);
		portrait.AddChild(face);
		row.AddChild(portrait);

		var info = new VBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };
		info.AddThemeConstantOverride("separation", 6);
		row.AddChild(info);
		info.AddChild(Lbl("SUA JORNADA", 13, Kicker));
		_nameLabel = Lbl("", 30, Txt);
		info.AddChild(_nameLabel);
		_statusLabel = Lbl("", 16, Muted);
		info.AddChild(_statusLabel);
		var tag = new PanelContainer();
		tag.AddThemeStyleboxOverride("panel", PanelStyle(TagBg, TagBg, 20, 6));
		_tagLabel = Lbl("", 13, TagText);
		tag.AddChild(_tagLabel);
		info.AddChild(tag);

		var divider = new ColorRect { Color = Stroke, CustomMinimumSize = new Vector2(0, 1) };
		col.AddChild(divider);

		var footer = new HBoxContainer();
		footer.AddThemeConstantOverride("separation", 8);
		_ageFooter = Lbl("0", 16, Txt);
		_yearFooter = Lbl("0", 16, Txt);
		_genFooter = Lbl("0", 16, Txt);
		footer.AddChild(FooterItem("IDADE", _ageFooter));
		footer.AddChild(HSpacer());
		footer.AddChild(FooterItem("ANO", _yearFooter));
		footer.AddChild(HSpacer());
		footer.AddChild(FooterItem("GERAÇÃO", _genFooter));
		col.AddChild(footer);

		return card;
	}

	private Control BuildStatsSection()
	{
		var box = new VBoxContainer();
		box.AddThemeConstantOverride("separation", 12);
		box.AddChild(SectionTitle("Como você está?", "INDICADORES"));

		var grid = new GridContainer { Columns = 2 };
		grid.AddThemeConstantOverride("h_separation", 12);
		grid.AddThemeConstantOverride("v_separation", 12);
		box.AddChild(grid);

		AddStat(grid, "❤ Saúde", StatHp);
		AddStat(grid, "☀ Felicidade", StatJoy);
		AddStat(grid, "✨ Inteligência", StatMind);
		AddStat(grid, "⚡ Energia", StatEnergy);
		return box;
	}

	private void AddStat(GridContainer grid, string name, Color color)
	{
		var card = Card(Surface, Stroke, 14);
		card.SizeFlagsHorizontal = SizeFlags.ExpandFill;
		var col = new VBoxContainer();
		col.AddThemeConstantOverride("separation", 8);
		card.AddChild(col);

		var line = new HBoxContainer();
		line.AddChild(Lbl(name, 15, new Color("ded5ed")));
		line.AddChild(HSpacer());
		var pct = Lbl("0%", 16, Txt);
		line.AddChild(pct);
		col.AddChild(line);

		var bar = new ProgressBar
		{
			MinValue = 0,
			MaxValue = 100,
			ShowPercentage = false,
			CustomMinimumSize = new Vector2(0, 9),
		};
		bar.AddThemeStyleboxOverride("background", PanelStyle(BarTrack, BarTrack, 9));
		bar.AddThemeStyleboxOverride("fill", PanelStyle(color, color, 9));
		col.AddChild(bar);

		grid.AddChild(card);
		_stats.Add((bar, pct));
	}

	private Control BuildMoneyCard()
	{
		var card = Card(Surface, Stroke, 20);
		var row = new HBoxContainer();
		card.AddChild(row);

		var left = new VBoxContainer();
		left.AddThemeConstantOverride("separation", 4);
		left.AddChild(Lbl("PATRIMÔNIO DISPONÍVEL", 13, Muted));
		_moneyValue = Lbl("", 26, Txt);
		left.AddChild(_moneyValue);
		row.AddChild(left);

		row.AddChild(HSpacer());
		_moneyDelta = Lbl("", 13, Green, HorizontalAlignment.Right);
		_moneyDelta.VerticalAlignment = VerticalAlignment.Center;
		row.AddChild(_moneyDelta);

		return card;
	}

	private Control BuildActivitiesSection()
	{
		var box = new VBoxContainer();
		box.AddThemeConstantOverride("separation", 12);
		box.AddChild(SectionTitle("O que deseja fazer?", "ATIVIDADES"));

		var grid = new GridContainer { Columns = 4 };
		grid.AddThemeConstantOverride("h_separation", 10);
		box.AddChild(grid);

		AddActivity(grid, "📚", "Estudar", ActivityKind.Study);
		AddActivity(grid, "🏃", "Exercitar", ActivityKind.Exercise);
		AddActivity(grid, "💬", "Socializar", ActivityKind.Socialize);
		AddActivity(grid, "🌙", "Descansar", ActivityKind.Rest);
		return box;
	}

	private void AddActivity(GridContainer grid, string symbol, string label, ActivityKind kind)
	{
		var button = new Button
		{
			CustomMinimumSize = new Vector2(0, 110),
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
		};
		StyleButton(button, Surface, Stroke, 12, 13);
		button.Pressed += () =>
		{
			var feedback = _sim.ApplyActivity(kind);
			if (feedback.Length > 0)
			{
				Refresh();
				ShowToast(feedback);
			}
		};

		var col = new VBoxContainer
		{
			MouseFilter = MouseFilterEnum.Ignore,
			Alignment = BoxContainer.AlignmentMode.Center,
		};
		col.SetAnchorsPreset(LayoutPreset.FullRect);
		col.AddThemeConstantOverride("separation", 8);
		col.AddChild(Lbl(symbol, 30, Txt, HorizontalAlignment.Center));
		col.AddChild(Lbl(label, 13, new Color("e0d8ed"), HorizontalAlignment.Center));
		button.AddChild(col);

		grid.AddChild(button);
	}

	private Control BuildLogSection()
	{
		var box = new VBoxContainer();
		box.AddThemeConstantOverride("separation", 12);
		box.AddChild(SectionTitle("Últimos acontecimentos", "DIÁRIO DA VIDA"));

		var card = Card(Surface, Stroke, 20);
		_logList = new VBoxContainer();
		_logList.AddThemeConstantOverride("separation", 14);
		card.AddChild(_logList);
		box.AddChild(card);
		return box;
	}

	private Control BuildLogItem(GameEvent gameEvent)
	{
		var row = new HBoxContainer();
		row.AddThemeConstantOverride("separation", 12);

		var icon = new PanelContainer { CustomMinimumSize = new Vector2(40, 40) };
		icon.AddThemeStyleboxOverride("panel", PanelStyle(Surface3, Surface3, 9));
		var glyph = Lbl(CategoryIcon(gameEvent.Category), 20, Txt, HorizontalAlignment.Center);
		glyph.VerticalAlignment = VerticalAlignment.Center;
		icon.AddChild(glyph);
		row.AddChild(icon);

		var col = new VBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };
		col.AddThemeConstantOverride("separation", 3);
		col.AddChild(Lbl(gameEvent.Title, 15, Txt));
		var desc = Lbl(gameEvent.Description, 13, Muted);
		desc.AutowrapMode = TextServer.AutowrapMode.WordSmart;
		col.AddChild(desc);
		col.AddChild(Lbl(
			$"ANO {_sim.StartYear + gameEvent.AbsoluteMonth / 12} · {gameEvent.AgeYears} ANOS",
			12, LogTime));
		row.AddChild(col);
		return row;
	}

	private Control BuildBottomNav()
	{
		var card = Card(Surface, Stroke, 8);
		var row = new HBoxContainer();
		row.AddThemeConstantOverride("separation", 4);
		card.AddChild(row);

		AddNavButton(row, "🏠", "Início", true, null);
		AddNavButton(row, "👤", "Personagem", false, () => ShowToast("Perfil do personagem em breve."));
		AddNavButton(row, "💜", "Relações", false,
			() => GetTree().ChangeSceneToFile("res://scenes/relationships.tscn"));
		AddNavButton(row, "🏆", "Conquistas", false, () => ShowToast("Conquistas em breve."));
		return card;
	}

	private void AddNavButton(HBoxContainer row, string icon, string text, bool active, Action? onPressed)
	{
		var button = new Button
		{
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
			CustomMinimumSize = new Vector2(0, 76),
		};
		StyleButton(button, Colors.Transparent, Colors.Transparent, 10, 13);
		if (onPressed is not null)
			button.Pressed += onPressed;

		var col = new VBoxContainer
		{
			MouseFilter = MouseFilterEnum.Ignore,
			Alignment = BoxContainer.AlignmentMode.Center,
		};
		col.SetAnchorsPreset(LayoutPreset.FullRect);
		col.AddThemeConstantOverride("separation", 4);
		col.AddChild(Lbl(icon, 22, Txt, HorizontalAlignment.Center));
		col.AddChild(Lbl(text, 13, active ? NavActive : Muted, HorizontalAlignment.Center));
		button.AddChild(col);
		row.AddChild(button);
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
		};
		_toast.AddThemeStyleboxOverride("panel", PanelStyle(ToastBg, ToastBorder, 10, 10));
		_toastLabel = Lbl("", 15, Txt, HorizontalAlignment.Center);
		_toastLabel.VerticalAlignment = VerticalAlignment.Center;
		_toast.AddChild(_toastLabel);
		AddChild(_toast);
	}

	private void BuildAdvanceArea()
	{
		foreach (var child in _advanceArea.GetChildren())
			child.QueueFree();

		var player = _sim.Player;
		if (player.IsAlive)
		{
			_advanceArea.AddChild(BigButton(
				"AVANÇAR NO TEMPO",
				$"Descubra o que a vida reserva para você ({TimeStep.StepLabel(player.AgeYears)}) →",
				OnAdvancePressed));
			return;
		}

		if (_sim.DynastyEnded)
		{
			_advanceArea.AddChild(Lbl(
				$"{player.FullName} morreu sem descendentes. A dinastia terminou.",
				15, Muted));
			_advanceArea.AddChild(BigButton(
				"COMEÇAR UMA NOVA VIDA", "Uma nova família espera por você",
				() => GetTree().ChangeSceneToFile(NewGameScene)));
			return;
		}

		var prompt = Lbl(
			$"{player.FirstName} morreu aos {player.AgeYears} anos. Escolha quem continua a dinastia:",
			15, Muted);
		prompt.AutowrapMode = TextServer.AutowrapMode.WordSmart;
		_advanceArea.AddChild(prompt);
		foreach (var heir in _sim.SuccessionCandidates)
		{
			var candidate = heir;
			_advanceArea.AddChild(BigButton(
				$"CONTINUAR COMO {candidate.FirstName.ToUpper()}",
				$"{candidate.FullName} · {candidate.AgeYears} anos",
				() => { _sim.Succeed(candidate); Refresh(); }));
		}
	}

	private Button BigButton(string title, string subtitle, Action onPressed)
	{
		var button = new Button { CustomMinimumSize = new Vector2(0, 74) };
		StyleButton(button, Purple2, Purple2, 12, 16, Surface3);
		button.Pressed += onPressed;

		var col = new VBoxContainer
		{
			MouseFilter = MouseFilterEnum.Ignore,
			Alignment = BoxContainer.AlignmentMode.Center,
		};
		col.SetAnchorsPreset(LayoutPreset.FullRect);
		col.AddThemeConstantOverride("separation", 4);
		col.AddChild(Lbl(title, 16, Txt, HorizontalAlignment.Center));
		col.AddChild(Lbl(subtitle, 13, new Color("d8c6ff"), HorizontalAlignment.Center));
		button.AddChild(col);
		return button;
	}

	// ---------------------------------------------------------------
	// Estado
	// ---------------------------------------------------------------

	private void StartNewLife()
	{
		if (GameSetup.Pending is { } request)
		{
			_sim = LifeSimulation.NewCustomLife(request);
			GameSetup.Pending = null;
			GameSetup.Current = _sim;
		}
		else
		{
			_sim = GameSetup.Current ?? LifeSimulation.NewRandomLife();
			GameSetup.Current ??= _sim;
		}
		Refresh();
	}

	private void OnAdvancePressed()
	{
		_sim.AdvancePeriod();
		Refresh();
		var player = _sim.Player;
		ShowToast(player.IsAlive
			? $"Você avançou para {player.AgeYears} anos."
			: $"{player.FirstName} morreu aos {player.AgeYears} anos.");
	}

	private void Refresh()
	{
		var player = _sim.Player;
		var country = player.Residence ?? player.BirthCountry;

		_dateLabel.Text = $"{MonthNames[_sim.MonthOfYear]}\n{_sim.CurrentYear}";
		_nameLabel.Text = player.FullName;
		var marital = RelationshipRules.StatusLabel(player.Status);
		var occupation = player.AgeYears < TimeStep.AdolescenceMaxYears ? "Estudante" : "Sem ocupação";
		_statusLabel.Text = $"{player.AgeYears} anos · {marital}\n{occupation} · {country?.Name ?? "?"}";
		_tagLabel.Text = $"✦ {TimeStep.PhaseLabel(player.AgeYears)}";
		_ageFooter.Text = player.AgeYears.ToString();
		_yearFooter.Text = _sim.CurrentYear.ToString();
		_genFooter.Text = _sim.Generation.ToString("D2");

		var values = new[]
		{
			player.Health.Physical,
			player.Health.Mental,
			player.Attributes[AttributeKind.Intelligence],
			player.Attributes[AttributeKind.Fitness],
		};
		for (var i = 0; i < _stats.Count; i++)
		{
			_stats[i].Bar.Value = values[i];
			_stats[i].Pct.Text = $"{values[i]}%";
		}

		_moneyValue.Text = country?.FormatMoney(player.Finances.Money)
			?? player.Finances.Money.ToString("N0");
		var delta = player.Finances.NetMonthly;
		_moneyDelta.Text = delta == 0
			? ""
			: $"{(delta > 0 ? "+" : "−")} {country?.FormatMoney(Math.Abs(delta))} este mês";
		_moneyDelta.AddThemeColorOverride("font_color",
			delta >= 0 ? Green : new Color("e07a7a"));

		foreach (var child in _logList.GetChildren())
			child.QueueFree();
		foreach (var gameEvent in _sim.Log.LatestFirst.Take(MaxLogEntries))
			_logList.AddChild(BuildLogItem(gameEvent));

		BuildAdvanceArea();
		ShowPendingDecision();
	}

	// ---------------------------------------------------------------
	// Decisões pendentes (pedidos de namoro/casamento de NPCs, §9)
	// ---------------------------------------------------------------

	private PendingDecision? _shownDecision;

	private void ShowPendingDecision()
	{
		var decision = _sim.PeekDecision();
		if (decision is null || decision == _shownDecision)
			return;
		_shownDecision = decision;

		var overlay = new ColorRect { Color = new Color(0f, 0f, 0f, 0.6f) };
		overlay.SetAnchorsPreset(LayoutPreset.FullRect);
		AddChild(overlay);

		var center = new CenterContainer();
		center.SetAnchorsPreset(LayoutPreset.FullRect);
		overlay.AddChild(center);

		var card = Card(Surface2, ToastBorder, 22);
		card.CustomMinimumSize = new Vector2(580, 0);
		center.AddChild(card);

		var col = new VBoxContainer();
		col.AddThemeConstantOverride("separation", 14);
		card.AddChild(col);

		col.AddChild(Lbl(decision.Title, 20, Kicker));
		var desc = Lbl(decision.Description, 15, Txt);
		desc.AutowrapMode = TextServer.AutowrapMode.WordSmart;
		col.AddChild(desc);

		for (var i = 0; i < decision.Options.Count; i++)
		{
			var index = i;
			var option = decision.Options[i];
			var button = new Button { CustomMinimumSize = new Vector2(0, 56) };
			StyleButton(button, index == 0 ? Purple2 : Surface,
				index == 0 ? Purple2 : Stroke, 12, 15);
			var label = Lbl(option.Label, 15, Txt, HorizontalAlignment.Center);
			label.VerticalAlignment = VerticalAlignment.Center;
			label.MouseFilter = MouseFilterEnum.Ignore;
			label.SetAnchorsPreset(LayoutPreset.FullRect);
			button.AddChild(label);
			button.Pressed += () =>
			{
				var outcome = _sim.ResolveDecision(decision, index);
				_shownDecision = null;
				overlay.QueueFree();
				Refresh();
				ShowToast(outcome);
			};
			col.AddChild(button);
		}
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

	private static string CategoryIcon(EventCategory category) => category switch
	{
		EventCategory.Family => "👪",
		EventCategory.Relationship => "💞",
		EventCategory.Education => "🎓",
		EventCategory.Career => "💼",
		EventCategory.Finance => "💰",
		EventCategory.Health => "🏥",
		EventCategory.World => "🌍",
		_ => "✦",
	};
}
