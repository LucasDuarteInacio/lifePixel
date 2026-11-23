namespace LifeSimulator.Core.Health;

/// <summary>
/// Saúde e bem-estar (GDD §5.5): saúde física e mental em escala
/// 0–100, mais condições de saúde adquiridas ao longo da vida.
/// </summary>
public sealed class HealthState
{
    private int _physical = 70;
    private int _mental = 70;

    public int Physical { get => _physical; set => _physical = Clamp(value); }
    public int Mental { get => _mental; set => _mental = Clamp(value); }

    /// <summary>Doenças e condições de saúde ativas (ex.: "Asma", "Depressão").</summary>
    public List<string> Conditions { get; } = new();

    public bool HasCondition(string condition) => Conditions.Contains(condition);

    /// <summary>
    /// Desgaste natural por período: hábitos e predisposições genéticas
    /// serão incorporados conforme o detalhamento do GDD.
    /// </summary>
    public void Drift(int months, int fitness, int immuneSystem)
    {
        var decay = months * (100 - fitness) / 4000.0;
        Physical -= (int)Math.Round(decay);
        if (immuneSystem < 30 && months >= 12)
            Physical -= 1;
    }

    private static int Clamp(int value) => Math.Clamp(value, 0, 100);
}
