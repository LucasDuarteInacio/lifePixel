using LifeSimulator.Core.Characters;
using LifeSimulator.Core.Relationships;

namespace LifeSimulator.Core.Genealogy;

/// <summary>
/// Parentesco derivado do grafo de <see cref="Person"/> (GDD §7.1):
/// graus são calculados sob demanda a partir de mãe/pai, irmãos,
/// filhos e cônjuge — fonte única, sem estado duplicado. O tipo de
/// <see cref="Relationship"/> descreve o vínculo afetivo; o grau de
/// parentesco vem daqui.
/// </summary>
public static class Kinship
{
    public static IEnumerable<Person> Parents(Person person)
    {
        if (person.Mother is not null) yield return person.Mother;
        if (person.Father is not null) yield return person.Father;
    }

    public static IEnumerable<Person> Grandparents(Person person) =>
        Parents(person).SelectMany(Parents);

    public static IEnumerable<Person> UnclesAunts(Person person) =>
        Parents(person).SelectMany(p => p.Siblings);

    public static IEnumerable<Person> Cousins(Person person) =>
        UnclesAunts(person).SelectMany(u => u.Children);

    public static IEnumerable<Person> NiecesNephews(Person person) =>
        person.Siblings.SelectMany(s => s.Children);

    public static IEnumerable<Person> Grandchildren(Person person) =>
        person.Children.SelectMany(c => c.Children);

    /// <summary>Todos os ascendentes (pais, avós, bisavós…), incluindo falecidos.</summary>
    public static IEnumerable<Person> Ancestors(Person person)
    {
        var seen = new HashSet<Person>();
        var queue = new Queue<Person>(Parents(person));
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (!seen.Add(current)) continue;
            yield return current;
            foreach (var p in Parents(current)) queue.Enqueue(p);
        }
    }

    /// <summary>Todos os descendentes (filhos, netos, bisnetos…), incluindo falecidos.</summary>
    public static IEnumerable<Person> Descendants(Person person)
    {
        var seen = new HashSet<Person>();
        var queue = new Queue<Person>(person.Children);
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (!seen.Add(current)) continue;
            yield return current;
            foreach (var c in current.Children) queue.Enqueue(c);
        }
    }

    /// <summary>Cônjuges dos filhos e parentes do cônjuge atual.</summary>
    public static IEnumerable<Person> InLaws(Person person)
    {
        foreach (var parent in person.Partner is { } partner ? Parents(partner) : [])
            yield return parent;
        if (person.Partner is { } spouse)
            foreach (var sib in spouse.Siblings)
                yield return sib;
        foreach (var sib in person.Siblings)
            if (sib.Partner is not null)
                yield return sib.Partner;
        foreach (var child in person.Children)
            if (child.Partner is not null)
                yield return child.Partner;
    }

    /// <summary>Toda a família conhecida da pessoa (sangue + afinidade).</summary>
    public static IEnumerable<Person> KnownFamily(Person person)
    {
        var seen = new HashSet<Person>();
        IEnumerable<Person> All()
        {
            foreach (var p in Parents(person)) yield return p;
            foreach (var p in person.Siblings) yield return p;
            foreach (var p in person.Children) yield return p;
            foreach (var p in Grandparents(person)) yield return p;
            foreach (var p in UnclesAunts(person)) yield return p;
            foreach (var p in Cousins(person)) yield return p;
            foreach (var p in NiecesNephews(person)) yield return p;
            foreach (var p in Grandchildren(person)) yield return p;
            foreach (var p in InLaws(person)) yield return p;
            if (person.Partner is not null) yield return person.Partner;
        }
        foreach (var p in All())
            if (seen.Add(p))
                yield return p;
    }

    public static bool IsKin(Person ego, Person other) => KnownFamily(ego).Contains(other);

    /// <summary>
    /// Tipo de vínculo para uma relação ainda não registrada: graus
    /// diretos viram Parent/Child/Sibling, o restante da família vira
    /// Relative e desconhecidos vira Friend (a afinação define o tipo).
    /// </summary>
    public static RelationshipKind KindFor(Person ego, Person other)
    {
        if (other == ego.Mother || other == ego.Father) return RelationshipKind.Parent;
        if (ego.Children.Contains(other)) return RelationshipKind.Child;
        if (ego.Siblings.Contains(other)) return RelationshipKind.Sibling;
        if (other == ego.Partner)
            return ego.Status == MaritalStatus.Engaged
                ? RelationshipKind.Fiance : RelationshipKind.Partner;
        if (IsKin(ego, other)) return RelationshipKind.Relative;
        return RelationshipKind.Friend;
    }

    /// <summary>Vínculo no sentido inverso (o que A é de B quando B é X de A).</summary>
    public static RelationshipKind Mirror(RelationshipKind kind) => kind switch
    {
        RelationshipKind.Parent => RelationshipKind.Child,
        RelationshipKind.Child => RelationshipKind.Parent,
        _ => kind,
    };
}
