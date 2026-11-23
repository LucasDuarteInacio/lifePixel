using Godot;

namespace LifeSimulator.UI;

/// <summary>
/// Tela de apresentação: mostra o nome do jogo por alguns segundos
/// (ou até um toque/clique) e segue para o menu principal.
/// </summary>
public partial class Splash : Control
{
	private const double DisplaySeconds = 2.5;
	private const string MenuScene = "res://scenes/menu.tscn";

	private bool _left;

	public override void _Ready()
	{
		var background = new ColorRect { Color = new Color(0.07f, 0.08f, 0.12f) };
		background.SetAnchorsPreset(LayoutPreset.FullRect);
		AddChild(background);

		var center = new CenterContainer();
		center.SetAnchorsPreset(LayoutPreset.FullRect);
		AddChild(center);

		var box = new VBoxContainer();
		box.AddThemeConstantOverride("separation", 16);
		center.AddChild(box);

		var title = new Label
		{
			Text = "Life Pixel",
			HorizontalAlignment = HorizontalAlignment.Center,
		};
		title.AddThemeFontSizeOverride("font_size", 96);
		box.AddChild(title);

		var subtitle = new Label
		{
			Text = "um simulador de vida",
			HorizontalAlignment = HorizontalAlignment.Center,
			Modulate = new Color(0.6f, 0.62f, 0.7f),
		};
		subtitle.AddThemeFontSizeOverride("font_size", 28);
		box.AddChild(subtitle);

		GetTree().CreateTimer(DisplaySeconds).Timeout += GoToMenu;
	}

	public override void _UnhandledInput(InputEvent inputEvent)
	{
		var tapped = inputEvent is InputEventScreenTouch { Pressed: true }
			or InputEventMouseButton { Pressed: true };
		if (tapped)
			GoToMenu();
	}

	private void GoToMenu()
	{
		if (_left)
			return;
		_left = true;
		GetTree().ChangeSceneToFile(MenuScene);
	}
}
