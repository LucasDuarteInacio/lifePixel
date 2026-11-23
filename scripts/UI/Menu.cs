using Godot;
using static LifeSimulator.UI.UiStyle;

namespace LifeSimulator.UI;

/// <summary>
/// Menu principal: novo jogo, carregar jogo e sair.
/// "Carregar jogo" fica desabilitado até o sistema de salvamento
/// (GDD §15.3) ser implementado.
/// </summary>
public partial class Menu : Control
{
	private const string NewGameScene = "res://scenes/new_game.tscn";

	public override void _Ready()
	{
		var background = new ColorRect { Color = Bg };
		background.SetAnchorsPreset(LayoutPreset.FullRect);
		AddChild(background);

		var center = new CenterContainer();
		center.SetAnchorsPreset(LayoutPreset.FullRect);
		AddChild(center);

		var box = new VBoxContainer();
		box.CustomMinimumSize = new Vector2(460, 0);
		box.AddThemeConstantOverride("separation", 16);
		center.AddChild(box);

		var logoRow = new HBoxContainer { Alignment = BoxContainer.AlignmentMode.Center };
		var logo = new PanelContainer { CustomMinimumSize = new Vector2(88, 88) };
		logo.AddThemeStyleboxOverride("panel", PanelStyle(Purple2, Purple2, 18));
		var glyph = Lbl("✦", 44, Txt, HorizontalAlignment.Center);
		glyph.VerticalAlignment = VerticalAlignment.Center;
		logo.AddChild(glyph);
		logoRow.AddChild(logo);
		box.AddChild(logoRow);

		box.AddChild(Lbl("LIFE PIXEL", 40, Txt, HorizontalAlignment.Center));
		box.AddChild(Lbl("SIMULADOR DE VIDA", 15, Muted, HorizontalAlignment.Center));

		box.AddChild(new Control { CustomMinimumSize = new Vector2(0, 28) });

		var card = Card(Surface2, new Color("59417c"), 24);
		var buttons = new VBoxContainer();
		buttons.AddThemeConstantOverride("separation", 14);
		card.AddChild(buttons);
		box.AddChild(card);

		buttons.AddChild(MenuButton(
			"NOVO JOGO", "Comece uma vida do zero", primary: true,
			() => GetTree().ChangeSceneToFile(NewGameScene)));

		var load = MenuButton(
			"CARREGAR JOGO", "Disponível com o sistema de salvamento", primary: false);
		load.Disabled = true;
		load.TooltipText = "Disponível quando o sistema de salvamento for implementado.";
		buttons.AddChild(load);

		// "Sair" é convenção de desktop; em mobile/web o app é
		// encerrado pelo sistema e o botão não faz sentido.
		if (OS.GetName() is "Windows" or "macOS" or "Linux")
			buttons.AddChild(MenuButton("SAIR", "Encerrar o jogo", primary: false,
				() => GetTree().Quit()));
	}
}
