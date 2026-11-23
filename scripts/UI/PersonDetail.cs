using Godot;
using LifeSimulator.Core.Characters;
using LifeSimulator.Core.Genealogy;
using LifeSimulator.Core.Relationships;
using LifeSimulator.Core.Simulation;
using static LifeSimulator.UI.UiStyle;

namespace LifeSimulator.UI;

/// <summary>
/// Cartão de detalhe de uma pessoa conhecida: vínculo, indicadores
/// ativos da relação, interações disponíveis e histórico. Usado pela
/// tela de relações e pela árvore genealógica.
/// </summary>
public static class PersonDetail
{
	public static string Avatar(Person person) =>
		person.Gender switch
		{
			Gender.Female => "👩",
			Gender.Male => "👨",
			_ => "🧑",
		};

	public static void Show(Control host, LifeSimulation sim, Person person,
		Action<string> toast, Action? onChanged = null)
	{
		var overlay = new ColorRect { Color = new Color(0f, 0f, 0f, 0.55f) };
		overlay.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		host.AddChild(overlay);

		var center = new CenterContainer();
		center.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		overlay.AddChild(center);

		var card = Card(Surface2, Stroke, 20);
		card.CustomMinimumSize = new Vector2(620, 0);
		center.AddChild(card);

		var scroll = new ScrollContainer
		{
			CustomMinimumSize = new Vector2(0, 1120),
			HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled,
		};
		card.AddChild(scroll);

		var content = new VBoxContainer { SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
		content.AddThemeConstantOverride("separation", 14);
		scroll.AddChild(content);

		void Rebuild()
		{
			foreach (var child in content.GetChildren())
				child.QueueFree();
			BuildContent(sim, person, content, () =>
			{
				Rebuild();
				onChanged?.Invoke();
			}, toast);

			var close = new Button
			{
				Text = "Fechar",
				CustomMinimumSize = new Vector2(0, 44),
			};
			StyleButton(close, Surface, Stroke, 10, 14);
			close.Pressed += () => { overlay.QueueFree(); onChanged?.Invoke(); };
			content.AddChild(close);
		}

		Rebuild();
	}

	private static void BuildContent(LifeSimulation sim, Person person,
		VBoxContainer col, Action rebuild, Action<string> toast)
	{
		var player = sim.Player;
		var bond = player == person ? null : sim.BondWith(person);

		// Cabeçalho
		var header = new HBoxContainer();
		header.AddThemeConstantOverride("separation", 14);
		col.AddChild(header);

		var portrait = new PanelContainer { CustomMinimumSize = new Vector2(72, 84) };
		portrait.AddThemeStyleboxOverride("panel", PanelStyle(Surface3, Surface3, 12));
		var face = Lbl(person.IsAlive ? Avatar(person) : "🪦", 38, Txt,
			HorizontalAlignment.Center);
		face.VerticalAlignment = VerticalAlignment.Center;
		portrait.AddChild(face);
		header.AddChild(portrait);

		var info = new VBoxContainer { SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
		info.AddThemeConstantOverride("separation", 4);
		header.AddChild(info);
		info.AddChild(Lbl(
			person.FullName + (person.IsAlive ? "" : " †"), 22, Txt));
		info.AddChild(Lbl(person == player
			? "Você"
			: KinLabel.Label(player, person), 14, Kicker));

		var statusBits = new System.Text.StringBuilder();
		statusBits.Append(person.IsAlive ? $"{person.AgeYears} anos" : "falecido(a)");
		statusBits.Append($" · {RelationshipRules.StatusLabel(person.Status)}");
		if (person.Job is { } job)
			statusBits.Append($" · {job.Title}");
		if (person.Residence is { } home)
			statusBits.Append($" · {home.Name}");
		var statusLine = Lbl(statusBits.ToString(), 13, Muted);
		statusLine.AutowrapMode = TextServer.AutowrapMode.WordSmart;
		info.AddChild(statusLine);

		if (bond is null)
			return;

		// Indicadores ativos do vínculo
		col.AddChild(SectionTitle("Como está a relação", "INDICADORES"));
		foreach (var indicator in RelationshipRules.IndicatorsFor(bond.Kind))
		{
			var value = bond.Get(indicator);
			var line = new HBoxContainer();
			line.AddChild(Lbl(RelationshipRules.IndicatorLabel(indicator, bond.Kind),
				14, Muted));
			line.AddChild(HSpacer());
			line.AddChild(Lbl($"{value}%", 14, Txt));
			col.AddChild(line);

			var bar = new ProgressBar
			{
				MinValue = 0,
				MaxValue = 100,
				Value = value,
				ShowPercentage = false,
				CustomMinimumSize = new Vector2(0, 8),
			};
			bar.AddThemeStyleboxOverride("background",
				PanelStyle(BarTrack, BarTrack, 8));
			bar.AddThemeStyleboxOverride("fill",
				PanelStyle(Purple2, Purple2, 8));
			col.AddChild(bar);
		}

		if (bond.Kind == RelationshipKind.Friend)
			col.AddChild(Lbl(
				$"Vínculo: {RelationshipRules.TierLabel(
					RelationshipRules.TierOf(bond.Friendship))}", 13, Muted));

		// Interações
		var available = sim.AvailableInteractions(person);
		if (person.IsAlive && available.Count > 0)
		{
			col.AddChild(SectionTitle("O que fazer", "INTERAÇÕES"));
			var grid = new GridContainer { Columns = 2 };
			grid.AddThemeConstantOverride("h_separation", 8);
			grid.AddThemeConstantOverride("v_separation", 8);
			col.AddChild(grid);
			foreach (var kind in available)
			{
				var k = kind;
				var button = new Button
				{
					Text = Interactions.Label(kind),
					CustomMinimumSize = new Vector2(0, 52),
					SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				};
				StyleButton(button, Surface, Stroke, 10, 13);
				button.Pressed += () =>
				{
					var text = sim.Interact(person, k);
					if (text.Length > 0)
						toast(text);
					rebuild();
				};
				grid.AddChild(button);
			}
		}

		// Histórico da relação
		if (bond.History.Count > 0)
		{
			col.AddChild(SectionTitle("Histórico", "RELACIONAMENTO"));
			foreach (var entry in bond.History.TakeLast(8).Reverse())
			{
				var row = new VBoxContainer();
				row.AddThemeConstantOverride("separation", 2);
				row.AddChild(Lbl(entry.Text, 14, Txt));
				row.AddChild(Lbl(
					$"ANO {sim.StartYear + entry.AbsoluteMonth / 12}", 11, LogTime));
				col.AddChild(row);
			}
		}
	}
}
