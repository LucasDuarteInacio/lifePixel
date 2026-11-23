namespace LifeSimulator.Core.Characters;

/// <summary>Como a pessoa entrou na família (GDD §7).</summary>
public enum OriginKind
{
    /// <summary>Nascimento biológico do casal.</summary>
    Natural,
    /// <summary>Fertilização in vitro — apenas a mãe é conhecida.</summary>
    InVitro,
    /// <summary>Adotado por um ou dois pais.</summary>
    Adopted,
}

/// <summary>União do casal na época do nascimento ou da adoção.</summary>
public enum UnionKind
{
    /// <summary>Um caso fora de um relacionamento estável.</summary>
    Affair,
    Dating,
    Married,
}

/// <summary>
/// Origem familiar da pessoa: a circunstância do nascimento e a união
/// dos pais na época (null quando há apenas um genitor). Registrada
/// no evento de nascimento e guardada para a biografia.
/// </summary>
public sealed record BirthOrigin(OriginKind Kind, UnionKind? Union);
