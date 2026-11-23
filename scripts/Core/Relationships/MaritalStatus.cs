namespace LifeSimulator.Core.Relationships;

/// <summary>
/// Estado civil da pessoa (GDD §5.1): namoro → noivado → casamento,
/// com término, divórcio e viuvez. O parceiro atual fica em
/// <see cref="Characters.Person.Partner"/>; ex-cônjuges permanecem
/// em <see cref="Characters.Person.Relationships"/> como
/// <see cref="RelationshipKind.ExPartner"/>.
/// </summary>
public enum MaritalStatus
{
    Single,
    Dating,
    Engaged,
    Married,
    Divorced,
    Widowed,
}
