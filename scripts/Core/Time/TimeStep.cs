namespace LifeSimulator.Core.Time;

/// <summary>
/// Passagem de tempo definida no GDD §4.1: infância avança por ano,
/// adolescência por semestre e vida adulta por mês.
/// As faixas etárias ainda são provisórias (decisão pendente no GDD)
/// e ficam centralizadas aqui para ajuste.
/// </summary>
public static class TimeStep
{
    public const int ChildhoodMaxYears = 12;
    public const int AdolescenceMaxYears = 18;

    public static int MonthsPerAdvance(int ageYears) => ageYears switch
    {
        < ChildhoodMaxYears => 12,
        < AdolescenceMaxYears => 6,
        _ => 1,
    };

    public static string PhaseLabel(int ageYears) => ageYears switch
    {
        < ChildhoodMaxYears => "Infância",
        < AdolescenceMaxYears => "Adolescência",
        _ => "Vida adulta",
    };

    public static string StepLabel(int ageYears) => MonthsPerAdvance(ageYears) switch
    {
        12 => "1 ano",
        6 => "6 meses",
        _ => "1 mês",
    };
}
