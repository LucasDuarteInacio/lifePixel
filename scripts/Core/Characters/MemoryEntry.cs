namespace LifeSimulator.Core.Characters;

/// <summary>
/// Memória e biografia do personagem (GDD §8.4): acontecimentos
/// relevantes que podem influenciar decisões e relações futuras.
/// </summary>
public sealed record MemoryEntry(
    int AbsoluteMonth,
    int AgeYears,
    string Text,
    int EmotionalImpact); // -100 (traumático) a 100 (marcante positivamente)
