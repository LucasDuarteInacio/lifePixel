namespace LifeSimulator.Core.Economy;

/// <summary>
/// Economia simplificada do MVP (GDD §5.4): dinheiro, renda, despesas
/// e dívidas. Valores na moeda do país de residência.
/// </summary>
public sealed class Finances
{
    public decimal Money { get; set; }
    public decimal MonthlyIncome { get; set; }
    public decimal MonthlyExpenses { get; set; }
    public decimal Debt { get; set; }

    public decimal NetMonthly => MonthlyIncome - MonthlyExpenses;

    public void ApplyMonths(int months)
    {
        Money += NetMonthly * months;
        if (Money < 0)
        {
            Debt += -Money;
            Money = 0;
        }
    }
}
