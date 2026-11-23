using System.Text.Json;
using System.Text.Json.Serialization;

namespace LifeSimulator.Core.Careers;

/// <summary>
/// Catálogo de carreiras (GDD §5.3): conjunto controlado de trilhas
/// parametrizadas em data/careers.json, embutido no assembly para
/// manter o núcleo independente do Godot.
/// </summary>
public static class CareerCatalog
{
    private const string CareersResource = "LifeSimulator.careers.json";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    public static IReadOnlyList<Career> All { get; } = Load();

    public static Career? Find(string id) =>
        All.FirstOrDefault(c => c.Id == id);

    public static Career PickRandom(Random rng) => All[rng.Next(All.Count)];

    /// <summary>Carrega a tabela de carreiras embutida (data/careers.json).</summary>
    public static IReadOnlyList<Career> Load()
    {
        var stream = typeof(CareerCatalog).Assembly.GetManifestResourceStream(CareersResource)
            ?? throw new InvalidOperationException(
                $"Recurso '{CareersResource}' não encontrado no assembly.");
        using var reader = new StreamReader(stream);
        return Parse(reader.ReadToEnd());
    }

    /// <summary>Interpreta e valida a tabela de carreiras a partir de JSON.</summary>
    public static IReadOnlyList<Career> Parse(string json)
    {
        var careers = JsonSerializer.Deserialize<List<Career>>(json, JsonOptions)
            ?? throw new InvalidDataException("Tabela de carreiras vazia ou inválida.");
        foreach (var career in careers)
            Validate(career);
        return careers;
    }

    /// <summary>Garante a trilha 1–10 sequencial, sem regressão salarial.</summary>
    private static void Validate(Career career)
    {
        if (career.Levels.Length != Career.MaxLevel)
            throw new InvalidDataException(
                $"Carreira '{career.Id}' precisa de exatamente {Career.MaxLevel} níveis.");

        for (var i = 0; i < career.Levels.Length; i++)
        {
            var level = career.Levels[i];
            if (level.Level != i + 1)
                throw new InvalidDataException(
                    $"Níveis de '{career.Id}' fora de ordem no nível {i + 1}.");
            if (i > 0 && level.SalaryFactor < career.Levels[i - 1].SalaryFactor)
                throw new InvalidDataException(
                    $"Salário regredindo no nível {level.Level} de '{career.Id}'.");
        }
    }
}
