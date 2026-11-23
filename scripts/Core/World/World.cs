using System.Text.Json;

namespace LifeSimulator.Core.World;

/// <summary>
/// Mundo global (GDD §10): países reais e eventos globais aleatórios.
/// A tabela de países é carregada de data/countries.json, embutido no
/// assembly para manter o núcleo independente do Godot. A moeda é uma
/// escolha do jogador (USD, EUR ou BRL), não do país — ver
/// Economy.Currency.
/// </summary>
public sealed class World
{
    private const string CountriesResource = "LifeSimulator.countries.json";

    private static readonly (string Title, string Description)[] GlobalEventPool =
    {
        ("Crise econômica global", "Mercados despencam e o custo de vida sobe em vários países."),
        ("Pandemia", "Uma nova doença se espalha pelo mundo e muda a rotina das pessoas."),
        ("Revolução tecnológica", "Uma nova tecnologia transforma empregos e o dia a dia."),
        ("Desastre natural", "Um grande desastre natural atinge manchetes no mundo inteiro."),
        ("Tensão geopolítica", "Conflitos entre países dominam as notícias."),
        ("Boom econômico", "A economia global entra em um período de crescimento."),
    };

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    };

    public IReadOnlyList<Country> Countries { get; } = LoadCountries();

    public Country PickRandom(Random rng) => Countries[rng.Next(Countries.Count)];

    /// <summary>Evento mundial aleatório (GDD §10.3), ou null.</summary>
    public (string Title, string Description)? RollGlobalEvent(Random rng, double chance)
    {
        if (rng.NextDouble() >= chance)
            return null;
        return GlobalEventPool[rng.Next(GlobalEventPool.Length)];
    }

    /// <summary>Carrega a tabela de países embutida (data/countries.json).</summary>
    public static IReadOnlyList<Country> LoadCountries()
    {
        var stream = typeof(World).Assembly.GetManifestResourceStream(CountriesResource)
            ?? throw new InvalidOperationException(
                $"Recurso '{CountriesResource}' não encontrado no assembly.");
        using var reader = new StreamReader(stream);
        return ParseCountries(reader.ReadToEnd());
    }

    /// <summary>Interpreta a tabela de países a partir de JSON.</summary>
    public static IReadOnlyList<Country> ParseCountries(string json) =>
        JsonSerializer.Deserialize<List<Country>>(json, JsonOptions)
            ?? throw new InvalidDataException("Tabela de países vazia ou inválida.");
}
