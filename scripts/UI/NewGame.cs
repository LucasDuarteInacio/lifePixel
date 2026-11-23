using System.Globalization;
using System.Text;
using Godot;
using LifeSimulator.Core.Characters;
using LifeSimulator.Core.Simulation;
using LifeSimulator.Core.World;
using static LifeSimulator.UI.UiStyle;

namespace LifeSimulator.UI;

/// <summary>
/// Configuração de nova partida (GDD §3.1 — vida personalizada): o
/// jogador escolhe nome, sobrenome, gênero e país de nascimento — a
/// moeda vem da nacionalidade — antes de seguir para a tela principal.
/// </summary>
public partial class NewGame : Control
{
	private const string MainScene = "res://scenes/main.tscn";
	private const string MenuScene = "res://scenes/menu.tscn";
	private const int CountryRowsVisible = 5;
	private const int CountryRowHeight = 56;

	private readonly World _world = new();

	private LineEdit _firstName = null!;
	private LineEdit _lastName = null!;
	private LineEdit _countryFilter = null!;
	private VBoxContainer _countryRows = null!;
	private int _selectedCountry;
	private Gender _selectedGender = Gender.Female;
	private readonly Dictionary<Gender, Button> _genderButtons = new();
	private readonly Random _nameRng = new();
	private string _autoFirstName = "";
	private string _autoLastName = "";
	private Button _start = null!;
	private Label _hint = null!;

	public override void _Ready()
	{
		var background = new ColorRect { Color = Bg };
		background.SetAnchorsPreset(LayoutPreset.FullRect);
		AddChild(background);

		var center = new CenterContainer();
		center.SetAnchorsPreset(LayoutPreset.FullRect);
		AddChild(center);

		var box = new VBoxContainer();
		box.CustomMinimumSize = new Vector2(520, 0);
		box.AddThemeConstantOverride("separation", 16);
		center.AddChild(box);

		box.AddChild(Lbl("NOVA VIDA", 34, Txt, HorizontalAlignment.Center));
		box.AddChild(Lbl("Escolha quem você será ao nascer", 14, Muted,
			HorizontalAlignment.Center));

		var card = Card(Surface2, new Color("59417c"), 24);
		var fields = new VBoxContainer();
		fields.AddThemeConstantOverride("separation", 16);
		card.AddChild(fields);
		box.AddChild(card);

		_firstName = StyleLineEdit("Ex.: Helena");
		_firstName.TextChanged += _ => UpdateStartState();
		fields.AddChild(Field("NOME", _firstName));

		_lastName = StyleLineEdit("Ex.: Silva");
		_lastName.TextChanged += _ => UpdateStartState();
		fields.AddChild(Field("SOBRENOME", _lastName));

		fields.AddChild(Field("GÊNERO", BuildGenderPicker()));

		fields.AddChild(Field("PAÍS DE NASCIMENTO", BuildCountryPicker()));

		PrefillNames();

		_hint = Lbl("Preencha nome e sobrenome para continuar.", 13, Muted,
			HorizontalAlignment.Center);
		box.AddChild(_hint);

		_start = MenuButton("COMEÇAR A VIDA", "Nascer e iniciar a simulação",
			primary: true, OnStartPressed);
		_start.AddThemeColorOverride("font_disabled_color", Muted);
		box.AddChild(_start);

		box.AddChild(MenuButton("VOLTAR", "Retornar ao menu principal",
			primary: false, () => GetTree().ChangeSceneToFile(MenuScene)));

		UpdateStartState();
	}

	private void UpdateStartState()
	{
		var valid = _firstName.Text.Trim().Length > 0 && _lastName.Text.Trim().Length > 0;
		_start.Disabled = !valid;
		_hint.Visible = !valid;
	}

	// ---------------------------------------------------------------
	// Nomes sugeridos: sorteados do país e gênero escolhidos, mas
	// nunca sobrescrevem o que o jogador digitou
	// ---------------------------------------------------------------

	private void PrefillNames()
	{
		var country = _world.Countries[_selectedCountry];
		_autoFirstName = country.PickFirstName(_selectedGender, _nameRng);
		_autoLastName = country.PickSurname(_nameRng);
		_firstName.Text = _autoFirstName;
		_lastName.Text = _autoLastName;
	}

	private void RefreshAutoNames()
	{
		var country = _world.Countries[_selectedCountry];
		if (_firstName.Text.Length == 0 || _firstName.Text == _autoFirstName)
		{
			_autoFirstName = country.PickFirstName(_selectedGender, _nameRng);
			_firstName.Text = _autoFirstName;
		}
		if (_lastName.Text.Length == 0 || _lastName.Text == _autoLastName)
		{
			_autoLastName = country.PickSurname(_nameRng);
			_lastName.Text = _autoLastName;
		}
	}

	private void OnStartPressed()
	{
		GameSetup.Pending = new NewLifeRequest(
			_firstName.Text.Trim(), _lastName.Text.Trim(),
			_world.Countries[_selectedCountry], _selectedGender);
		GetTree().ChangeSceneToFile(MainScene);
	}

	// ---------------------------------------------------------------
	// Seletor de gênero: dois botões lado a lado, um sempre ativo
	// ---------------------------------------------------------------

	private Control BuildGenderPicker()
	{
		var row = new HBoxContainer();
		row.AddThemeConstantOverride("separation", 8);
		row.AddChild(BuildGenderButton(Gender.Female, "♀", "FEMININO"));
		row.AddChild(BuildGenderButton(Gender.Male, "♂", "MASCULINO"));
		UpdateGenderButtons();
		return row;
	}

	private Button BuildGenderButton(Gender gender, string glyph, string label)
	{
		var button = new Button
		{
			CustomMinimumSize = new Vector2(0, 56),
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
		};
		button.Pressed += () =>
		{
			_selectedGender = gender;
			UpdateGenderButtons();
			RefreshAutoNames();
		};

		var content = new HBoxContainer
		{
			MouseFilter = MouseFilterEnum.Ignore,
			Alignment = BoxContainer.AlignmentMode.Center,
		};
		content.SetAnchorsPreset(LayoutPreset.FullRect);
		content.AddThemeConstantOverride("separation", 10);
		content.AddChild(Lbl(glyph, 20, Txt));
		content.AddChild(Lbl(label, 15, Txt));
		button.AddChild(content);

		_genderButtons[gender] = button;
		return button;
	}

	private void UpdateGenderButtons()
	{
		foreach (var (gender, button) in _genderButtons)
		{
			var selected = gender == _selectedGender;
			StyleButton(button,
				selected ? Surface3 : Surface,
				selected ? Purple2 : Stroke,
				10, 15);
		}
	}

	// ---------------------------------------------------------------
	// Seletor de país: filtro por texto + lista rolável de 5 linhas
	// ---------------------------------------------------------------

	private Control BuildCountryPicker()
	{
		var box = new VBoxContainer();
		box.AddThemeConstantOverride("separation", 8);

		_countryFilter = StyleLineEdit("Filtrar país…");
		_countryFilter.TextChanged += _ => RebuildCountryRows();
		box.AddChild(_countryFilter);

		var listCard = Card(Surface, Stroke, 6);
		var scroll = new ScrollContainer
		{
			CustomMinimumSize = new Vector2(0, CountryRowsVisible * CountryRowHeight),
			HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled,
		};
		listCard.AddChild(scroll);

		_countryRows = new VBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };
		_countryRows.AddThemeConstantOverride("separation", 4);
		scroll.AddChild(_countryRows);
		box.AddChild(listCard);

		_selectedCountry = DefaultCountryIndex();
		RebuildCountryRows();
		return box;
	}

	private int DefaultCountryIndex()
	{
		for (var i = 0; i < _world.Countries.Count; i++)
			if (_world.Countries[i].Code == "BR")
				return i;
		return 0;
	}

	private void RebuildCountryRows()
	{
		foreach (var child in _countryRows.GetChildren())
			child.QueueFree();

		var filter = Fold(_countryFilter.Text);
		var shown = 0;
		for (var i = 0; i < _world.Countries.Count; i++)
		{
			var country = _world.Countries[i];
			if (filter.Length > 0 && !Fold(country.Name).Contains(filter))
				continue;
			_countryRows.AddChild(BuildCountryRow(i, country));
			shown++;
		}
		if (shown == 0)
			_countryRows.AddChild(Lbl("Nenhum país encontrado.", 14, Muted));
	}

	private Button BuildCountryRow(int index, Country country)
	{
		var selected = index == _selectedCountry;
		var row = new Button
		{
			CustomMinimumSize = new Vector2(0, CountryRowHeight - 4),
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
		};
		StyleButton(row,
			selected ? Surface3 : Surface,
			selected ? Purple2 : Colors.Transparent,
			8, 15);
		row.Pressed += () =>
		{
			_selectedCountry = index;
			RebuildCountryRows();
			RefreshAutoNames();
		};

		var margin = new MarginContainer { MouseFilter = MouseFilterEnum.Ignore };
		margin.SetAnchorsPreset(LayoutPreset.FullRect);
		foreach (var side in new[] { "margin_left", "margin_right" })
			margin.AddThemeConstantOverride(side, 12);
		row.AddChild(margin);

		var content = new HBoxContainer { MouseFilter = MouseFilterEnum.Ignore };
		content.AddThemeConstantOverride("separation", 12);
		margin.AddChild(content);

		var flag = GD.Load<Texture2D>(
			$"res://assets/flags/{country.Code.ToLowerInvariant()}.png");
		if (flag is not null)
		{
			content.AddChild(new TextureRect
			{
				Texture = flag,
				CustomMinimumSize = new Vector2(40, 28),
				ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
				StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
				SizeFlagsVertical = SizeFlags.ShrinkCenter,
				MouseFilter = MouseFilterEnum.Ignore,
			});
		}

		var name = Lbl(country.Name, 15, Txt);
		name.SizeFlagsVertical = SizeFlags.ShrinkCenter;
		content.AddChild(name);

		if (selected)
		{
			var spacer = HSpacer();
			spacer.MouseFilter = MouseFilterEnum.Ignore;
			content.AddChild(spacer);
			var check = Lbl("✓", 16, NavActive);
			check.SizeFlagsVertical = SizeFlags.ShrinkCenter;
			content.AddChild(check);
		}

		return row;
	}

	/// <summary>Minúsculas sem acentos, para o filtro casar "mexico" com "México".</summary>
	private static string Fold(string text) => string.Concat(
		text.Normalize(NormalizationForm.FormD)
			.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark))
		.ToLowerInvariant();

	private static Control Field(string caption, Control input)
	{
		var box = new VBoxContainer();
		box.AddThemeConstantOverride("separation", 6);
		box.AddChild(Lbl(caption, 13, Muted));
		box.AddChild(input);
		return box;
	}
}
