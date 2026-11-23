namespace LifeSimulator.Core.Events;

/// <summary>Opção de uma decisão: o efeito retorna o texto do desfecho.</summary>
public sealed record DecisionOption(string Label, Func<string> Apply);

/// <summary>
/// Acontecimento com escolha (GDD §9): produzido pela simulação —
/// pedidos de namoro e casamento de NPCs, reações a traições — e
/// resolvido pelo jogador na interface. Primeira mecânica de decisão
/// do jogo; os <see cref="GameEvent"/>s registram o desfecho.
/// </summary>
public sealed class PendingDecision
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required IReadOnlyList<DecisionOption> Options { get; init; }
    public EventCategory Category { get; init; } = EventCategory.Relationship;

    /// <summary>Pessoa que originou a decisão, para evitar pedidos duplicados.</summary>
    public Guid? AboutPersonId { get; init; }

    /// <summary>Aplica a opção escolhida e retorna o texto do desfecho.</summary>
    public string Resolve(int optionIndex) => Options[optionIndex].Apply();
}
