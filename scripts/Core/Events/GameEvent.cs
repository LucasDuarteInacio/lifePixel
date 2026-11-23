namespace LifeSimulator.Core.Events;

public enum EventCategory
{
    Life,
    Family,
    Relationship,
    Education,
    Career,
    Finance,
    Health,
    World,
}

/// <summary>
/// Acontecimento apresentado ao jogador (GDD §9). Neste estágio o
/// evento é narrativo; decisões e consequências encadeadas serão
/// adicionadas com o sistema de escolhas.
/// </summary>
public sealed record GameEvent(
    int AbsoluteMonth,
    int AgeYears,
    string Title,
    string Description,
    EventCategory Category);

/// <summary>Feed cronológico de acontecimentos (GDD §9.5, §13.4).</summary>
public sealed class EventLog
{
    private readonly List<GameEvent> _events = new();

    public IReadOnlyList<GameEvent> Events => _events;

    public void Add(GameEvent gameEvent) => _events.Add(gameEvent);

    public void Record(int absoluteMonth, int ageYears, string title,
        string description, EventCategory category) =>
        _events.Add(new GameEvent(absoluteMonth, ageYears, title, description, category));

    /// <summary>Mais recentes primeiro, para exibição no feed.</summary>
    public IEnumerable<GameEvent> LatestFirst => _events.AsEnumerable().Reverse();
}
