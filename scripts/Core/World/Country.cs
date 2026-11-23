using LifeSimulator.Core.Characters;
using LifeSimulator.Core.Economy;

namespace LifeSimulator.Core.World;

/// <summary>
/// País real com moeda do jogo (USD, EUR ou BRL), custo de vida e
/// salário de referência (GDD §10.1–10.2). Carrega também nomes
/// típicos para a geração de personagens.
/// </summary>
public sealed record Country(
    string Code,
    string Name,
    string CurrencyCode,
    /// <summary>Despesa mensal básica de referência.</summary>
    decimal MonthlyCostOfLiving,
    /// <summary>Salário mensal mediano de referência.</summary>
    decimal MedianMonthlySalary,
    string[] MaleNames,
    string[] FemaleNames,
    string[] Surnames)
{
    public Currency Currency => Currency.FromCode(CurrencyCode);

    public string PickFirstName(Gender gender, Random rng) => gender switch
    {
        Gender.Female => FemaleNames[rng.Next(FemaleNames.Length)],
        Gender.Male => MaleNames[rng.Next(MaleNames.Length)],
        _ => (rng.Next(2) == 0 ? MaleNames : FemaleNames)[rng.Next(MaleNames.Length)],
    };

    public string PickSurname(Random rng) => Surnames[rng.Next(Surnames.Length)];

    public string FormatMoney(decimal amount) => Currency.FormatMoney(amount);
}
