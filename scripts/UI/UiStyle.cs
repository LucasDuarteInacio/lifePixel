using Godot;

namespace LifeSimulator.UI;

/// <summary>
/// Paleta e helpers de estilo compartilhados pelas telas, seguindo o
/// protótipo docs/vertice-inicio-prototipo-godot.html.
/// </summary>
public static class UiStyle
{
	public static readonly Color Bg = new("100d1b");
	public static readonly Color Surface = new("191428");
	public static readonly Color Surface2 = new("211a33");
	public static readonly Color Surface3 = new("2b2141");
	public static readonly Color Stroke = new("3b2d55");
	public static readonly Color Purple2 = new("7650c7");
	public static readonly Color Txt = new("f4efff");
	public static readonly Color Muted = new("a99fba");
	public static readonly Color Green = new("68d6a2");
	public static readonly Color Kicker = new("c2a9f4");
	public static readonly Color TagText = new("d8c6ff");
	public static readonly Color TagBg = new("382851");
	public static readonly Color BarTrack = new("342a45");
	public static readonly Color NavActive = new("c9b1ff");
	public static readonly Color LogTime = new("b79ae8");
	public static readonly Color StatHp = new("5acb9a");
	public static readonly Color StatJoy = new("df8bc0");
	public static readonly Color StatMind = new("9b82ee");
	public static readonly Color StatEnergy = new("e6bd63");
	public static readonly Color ToastBg = new("302344");
	public static readonly Color ToastBorder = new("795db0");

	public static Label Lbl(string text, int size, Color color,
		HorizontalAlignment align = HorizontalAlignment.Left)
	{
		var label = new Label { Text = text, HorizontalAlignment = align };
		label.AddThemeFontSizeOverride("font_size", size);
		label.AddThemeColorOverride("font_color", color);
		return label;
	}

	public static Control HSpacer() =>
		new() { SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };

	public static Control SectionTitle(string title, string right)
	{
		var row = new HBoxContainer();
		row.AddChild(Lbl(title, 18, Txt));
		row.AddChild(HSpacer());
		row.AddChild(Lbl(right, 13, Muted));
		return row;
	}

	public static Control FooterItem(string label, Label value)
	{
		var box = new HBoxContainer();
		box.AddThemeConstantOverride("separation", 6);
		box.AddChild(Lbl(label, 12, Muted));
		box.AddChild(value);
		return box;
	}

	public static PanelContainer Card(Color bg, Color border, int padding)
	{
		var card = new PanelContainer();
		card.AddThemeStyleboxOverride("panel", PanelStyle(bg, border, 15, padding));
		return card;
	}

	public static StyleBoxFlat PanelStyle(Color bg, Color border, int radius, int padding = 0)
	{
		var style = new StyleBoxFlat
		{
			BgColor = bg,
			BorderColor = border,
			CornerRadiusTopLeft = radius,
			CornerRadiusTopRight = radius,
			CornerRadiusBottomLeft = radius,
			CornerRadiusBottomRight = radius,
			BorderWidthLeft = 1,
			BorderWidthTop = 1,
			BorderWidthRight = 1,
			BorderWidthBottom = 1,
			ContentMarginLeft = padding,
			ContentMarginTop = padding,
			ContentMarginRight = padding,
			ContentMarginBottom = padding,
		};
		return style;
	}

	public static LineEdit StyleLineEdit(string placeholder)
	{
		var edit = new LineEdit
		{
			PlaceholderText = placeholder,
			CustomMinimumSize = new Vector2(0, 56),
		};
		edit.AddThemeFontSizeOverride("font_size", 16);
		edit.AddThemeColorOverride("font_color", Txt);
		edit.AddThemeColorOverride("font_placeholder_color", Muted);
		edit.AddThemeColorOverride("caret_color", Txt);
		edit.AddThemeStyleboxOverride("normal", PanelStyle(Surface, Stroke, 10, 14));
		edit.AddThemeStyleboxOverride("focus", PanelStyle(Surface, Purple2, 10, 14));
		return edit;
	}

	public static void StyleButton(Button button, Color bg, Color border, int radius,
		int fontSize, Color? hoverBg = null)
	{
		button.AddThemeFontSizeOverride("font_size", fontSize);
		button.AddThemeColorOverride("font_color", Txt);
		button.AddThemeStyleboxOverride("normal", PanelStyle(bg, border, radius));
		button.AddThemeStyleboxOverride("hover", PanelStyle(hoverBg ?? Surface3, border, radius));
		button.AddThemeStyleboxOverride("pressed", PanelStyle(hoverBg ?? Surface3, border, radius));
		button.AddThemeStyleboxOverride("focus", new StyleBoxEmpty());
		button.AddThemeStyleboxOverride("disabled", PanelStyle(Surface, border, radius));
	}

	/// <summary>Botão grande com título e subtítulo, usado em menus.</summary>
	public static Button MenuButton(string title, string subtitle, bool primary,
		Action? onPressed = null)
	{
		var button = new Button { CustomMinimumSize = new Vector2(0, 84) };
		if (primary)
			StyleButton(button, Purple2, Purple2, 12, 16, Surface3);
		else
			StyleButton(button, Surface, Stroke, 12, 16);
		if (onPressed is not null)
			button.Pressed += onPressed;

		var col = new VBoxContainer
		{
			MouseFilter = Control.MouseFilterEnum.Ignore,
			Alignment = BoxContainer.AlignmentMode.Center,
		};
		col.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		col.AddThemeConstantOverride("separation", 4);
		col.AddChild(Lbl(title, 16, Txt, HorizontalAlignment.Center));
		col.AddChild(Lbl(subtitle, 13, primary ? TagText : Muted, HorizontalAlignment.Center));
		button.AddChild(col);
		return button;
	}
}
