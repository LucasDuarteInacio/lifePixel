namespace LifeSimulator.Core.Simulation;

/// <summary>
/// Modelo intermediário de mortalidade (GDD §11.1): idade, saúde,
/// hábitos e predisposição genética influenciam o risco por período.
/// Não é uma simulação médica detalhada.
/// </summary>
public static class Mortality
{
    /// <summary>Risco anual de morte, de 0 a 1.</summary>
    public static double AnnualRisk(int ageYears, int physicalHealth, int longevity)
    {
        var baseRisk = ageYears switch
        {
            < 1 => 0.006,
            < 15 => 0.0004,
            < 40 => 0.001,
            < 60 => 0.005,
            _ => 0.015,
        };
        if (ageYears >= 60)
            baseRisk += Math.Pow(ageYears - 60, 2.2) * 0.00035;

        // Saúde 100 ~ x0.5, saúde 0 ~ x2.3.
        var healthFactor = Math.Pow(1.55, (60 - physicalHealth) / 40.0);

        // Predisposição genética de longevidade: x0.7 a x1.3.
        var geneFactor = 1.0 - (longevity - 50) * 0.006;

        return Math.Clamp(baseRisk * healthFactor * geneFactor, 0.0, 0.95);
    }

    /// <summary>Risco de morte para um período em meses.</summary>
    public static double PeriodRisk(int months, int ageYears, int physicalHealth, int longevity)
    {
        var annual = AnnualRisk(ageYears, physicalHealth, longevity);
        return 1.0 - Math.Pow(1.0 - annual, months / 12.0);
    }

    public static string PickCause(Random rng, int ageYears, int physicalHealth) =>
        (ageYears, physicalHealth) switch
        {
            ( >= 75, _) => "causas naturais",
            (_, <= 25) => rng.Next(2) == 0 ? "uma doença" : "complicações de saúde",
            _ => rng.Next(3) switch
            {
                0 => "um acidente",
                1 => "uma doença",
                _ => "complicações de saúde",
            },
        };
}
