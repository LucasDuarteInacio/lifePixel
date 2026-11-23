using LifeSimulator.Core.Characters;

namespace LifeSimulator.Core.Careers;

/// <summary>
/// Um degrau da trilha profissional: cargo exercido e salário,
/// como multiplicador sobre a renda mediana do país de residência.
/// </summary>
public sealed record CareerLevel(
    int Level,
    string Title,
    decimal SalaryFactor);

/// <summary>
/// Carreira parametrizada (GDD §5.3): trilha de dez níveis, do
/// primeiro emprego (1) ao ápice da profissão (10).
/// </summary>
public sealed record Career(
    string Id,
    string Name,
    string Description,
    /// <summary>Atributo que mais influencia a progressão.</summary>
    AttributeKind KeyAttribute,
    /// <summary>Idade mínima para ingressar na carreira.</summary>
    int MinAge,
    CareerLevel[] Levels)
{
    /// <summary>Nível máximo de qualquer carreira — o ápice.</summary>
    public const int MaxLevel = 10;

    /// <summary>Nível inicial de qualquer carreira.</summary>
    public const int EntryLevel = 1;

    public CareerLevel AtLevel(int level) =>
        Levels[Math.Clamp(level, EntryLevel, MaxLevel) - 1];
}
