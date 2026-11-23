namespace LifeSimulator.Core.Economy;

/// <summary>
/// Moeda do jogo: só existem três — real, dólar e euro — e cada país
/// usa uma delas (GDD §10.2). Não há conversão cambial.
/// </summary>
public sealed record Currency(string Code, string Symbol, string Name)
{
    public static readonly Currency Real = new("BRL", "R$", "Real brasileiro");
    public static readonly Currency Dollar = new("USD", "$", "Dólar americano");
    public static readonly Currency Euro = new("EUR", "€", "Euro");

    public static IReadOnlyList<Currency> All { get; } = [Real, Dollar, Euro];

    public static Currency FromCode(string code) =>
        All.FirstOrDefault(c => c.Code == code)
        ?? throw new ArgumentException($"Moeda desconhecida: {code}");

    public string FormatMoney(decimal amount) =>
        $"{Symbol} {amount:N0} {Code}";
}
