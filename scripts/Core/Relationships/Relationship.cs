namespace LifeSimulator.Core.Relationships;

public enum RelationshipKind
{
    Parent,
    Child,
    Sibling,
    Partner,
    Fiance,
    ExPartner,
    Friend,
    Rival,
    Relative,
}

/// <summary>Os seis indicadores independentes da relação (GDD §7.3).</summary>
public enum RelationshipIndicator
{
    Love,
    Friendship,
    Trust,
    Respect,
    Intimacy,
    Rivalry,
}

/// <summary>Acontecimento registrado no histórico de uma relação.</summary>
public sealed record RelationshipMemory(int AbsoluteMonth, string Text, int Impact);

/// <summary>
/// Relações com indicadores independentes (GDD §7.3): uma pessoa pode
/// amar alguém sem confiar nela. Cada indicador vai de 0 a 100 e a
/// relação guarda um histórico de acontecimentos.
/// </summary>
public sealed class Relationship
{
    private int _love;
    private int _friendship;
    private int _trust;
    private int _respect;
    private int _intimacy;
    private int _rivalry;

    public Relationship(Guid otherId, RelationshipKind kind)
    {
        OtherId = otherId;
        Kind = kind;
    }

    public Guid OtherId { get; }
    public RelationshipKind Kind { get; set; }

    /// <summary>Mês absoluto em que o vínculo atual começou (namoro, amizade).</summary>
    public int? SinceMonth { get; set; }

    /// <summary>Último mês com interação — alimenta a deriva por afastamento.</summary>
    public int? LastContactMonth { get; set; }

    public List<RelationshipMemory> History { get; } = new();

    public int Love { get => _love; set => _love = Clamp(value); }
    public int Friendship { get => _friendship; set => _friendship = Clamp(value); }
    public int Trust { get => _trust; set => _trust = Clamp(value); }
    public int Respect { get => _respect; set => _respect = Clamp(value); }
    public int Intimacy { get => _intimacy; set => _intimacy = Clamp(value); }
    public int Rivalry { get => _rivalry; set => _rivalry = Clamp(value); }

    public int Get(RelationshipIndicator indicator) => indicator switch
    {
        RelationshipIndicator.Love => Love,
        RelationshipIndicator.Friendship => Friendship,
        RelationshipIndicator.Trust => Trust,
        RelationshipIndicator.Respect => Respect,
        RelationshipIndicator.Intimacy => Intimacy,
        _ => Rivalry,
    };

    public void Adjust(RelationshipIndicator indicator, int delta)
    {
        switch (indicator)
        {
            case RelationshipIndicator.Love: Love += delta; break;
            case RelationshipIndicator.Friendship: Friendship += delta; break;
            case RelationshipIndicator.Trust: Trust += delta; break;
            case RelationshipIndicator.Respect: Respect += delta; break;
            case RelationshipIndicator.Intimacy: Intimacy += delta; break;
            default: Rivalry += delta; break;
        }
    }

    public void Record(int absoluteMonth, string text, int impact) =>
        History.Add(new RelationshipMemory(absoluteMonth, text, impact));

    private static int Clamp(int value) => Math.Clamp(value, 0, 100);
}
