using LifeSimulator.Core.World;

namespace LifeSimulator.Core.Careers;

/// <summary>
/// Vínculo empregatício de uma pessoa: a carreira seguida e o nível
/// atual de progressão nela (1 = entrada, 10 = ápice).
/// </summary>
public sealed class Employment
{
    public Employment(Career career, int level = Career.EntryLevel)
    {
        Career = career;
        Level = Math.Clamp(level, Career.EntryLevel, Career.MaxLevel);
    }

    public Career Career { get; }
    public int Level { get; private set; }
    public int MonthsInLevel { get; private set; }
    public bool AtTop => Level >= Career.MaxLevel;
    public string Title => Career.AtLevel(Level).Title;

    /// <summary>Salário mensal no país de residência.</summary>
    public decimal MonthlySalary(Country country) =>
        country.MedianMonthlySalary * Career.AtLevel(Level).SalaryFactor;

    /// <summary>Promove um nível; retorna false se já está no ápice.</summary>
    public bool Promote()
    {
        if (AtTop)
            return false;
        Level++;
        MonthsInLevel = 0;
        return true;
    }

    /// <summary>Rebaixa um nível; retorna false se já está na base.</summary>
    public bool Demote()
    {
        if (Level <= Career.EntryLevel)
            return false;
        Level--;
        MonthsInLevel = 0;
        return true;
    }

    public void AdvanceMonths(int months) => MonthsInLevel += months;
}
