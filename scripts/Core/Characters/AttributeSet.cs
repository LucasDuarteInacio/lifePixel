namespace LifeSimulator.Core.Characters;

/// <summary>Indicadores numéricos do GDD §6.1, em escala 0–100.</summary>
public enum AttributeKind
{
    // Físicas
    Appearance,
    Health,
    Fitness,
    // Mentais
    Intelligence,
    Creativity,
    EmotionalStability,
    // Sociais
    Charisma,
    Empathy,
    Communication,
    // Comportamentais
    Discipline,
    Ambition,
    Impulsivity,
    Honesty,
}

public sealed class AttributeSet
{
    public const int Min = 0;
    public const int Max = 100;
    public const int Neutral = 50;

    private readonly Dictionary<AttributeKind, int> _values = new();

    public int this[AttributeKind kind]
    {
        get => _values.TryGetValue(kind, out var value) ? value : Neutral;
        set => _values[kind] = Math.Clamp(value, Min, Max);
    }

    public void Adjust(AttributeKind kind, int delta) => this[kind] += delta;

    public static AttributeSet Random(Random rng, int baseline = Neutral, int spread = 30)
    {
        var set = new AttributeSet();
        foreach (var kind in Enum.GetValues<AttributeKind>())
            set[kind] = baseline + rng.Next(-spread, spread + 1);
        return set;
    }
}
