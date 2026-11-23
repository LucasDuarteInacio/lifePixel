using LifeSimulator.Core.Characters;
using LifeSimulator.Core.Relationships;

namespace LifeSimulator.Core.Genealogy;

/// <summary>
/// Rótulo em pt-BR do que uma pessoa é para outra ("avó", "tio",
/// "prima", "sogra", "noivo"), usado na interface e nos eventos.
/// </summary>
public static class KinLabel
{
    public static string Label(Person ego, Person other)
    {
        if (other == ego)
            return "você";

        // Vínculos afetivos registrados.
        if (ego.Relationships.TryGetValue(other.Id, out var bond))
        {
            if (bond.Kind == RelationshipKind.ExPartner)
                return "ex-companheiro(a)";
            if (bond.Kind == RelationshipKind.Friend)
                return RelationshipRules.TierOf(bond.Friendship) == FriendshipTier.BestFriend
                    ? G(other, "melhor amigo", "melhor amiga")
                    : G(other, "amigo", "amiga");
            if (bond.Kind == RelationshipKind.Rival)
                return "rival";
        }

        // Cônjuge/parceiro atual.
        if (other == ego.Partner)
            return ego.Status switch
            {
                MaritalStatus.Dating => G(other, "namorado", "namorada", "namorade"),
                MaritalStatus.Engaged => G(other, "noivo", "noiva"),
                MaritalStatus.Married => G(other, "marido", "esposa", "cônjuge"),
                _ => G(other, "companheiro", "companheira", "companheire"),
            };

        // Família direta.
        if (other == ego.Mother) return "mãe";
        if (other == ego.Father) return "pai";
        if (ego.Children.Contains(other))
            return G(other, "filho", "filha", "filhe");
        if (ego.Siblings.Contains(other))
            return G(other, "irmão", "irmã", "irmãe");

        // Ascendentes.
        var grandparents = Kinship.Grandparents(ego).ToList();
        if (other.Children.Any(c => c == ego.Mother || c == ego.Father))
            return G(other, "avô", "avó");
        if (grandparents.Any(gp => gp.Mother == other || gp.Father == other))
            return G(other, "bisavô", "bisavó");
        if (Kinship.Ancestors(ego).Contains(other))
            return "ancestral";

        // Descendentes.
        if (other.Mother is { } m && ego.Children.Contains(m)
            || other.Father is { } f && ego.Children.Contains(f))
            return G(other, "neto", "neta", "nete");
        if (Kinship.Grandchildren(ego).Any(gc => gc.Mother == other || gc.Father == other))
            return G(other, "bisneto", "bisneta");
        if (Kinship.Descendants(ego).Contains(other))
            return "descendente";

        // Laterais.
        if (other.Siblings.Any(s => s == ego.Mother || s == ego.Father))
            return G(other, "tio", "tia");
        if (grandparents.Any(gp => other.Siblings.Contains(gp)))
            return G(other, "tio-avô", "tia-avó");
        if (other.Mother is { } om && Kinship.UnclesAunts(ego).Contains(om)
            || other.Father is { } of && Kinship.UnclesAunts(ego).Contains(of))
            return G(other, "primo", "prima");
        if (other.Mother is { } nm && ego.Siblings.Contains(nm)
            || other.Father is { } nf && ego.Siblings.Contains(nf))
            return G(other, "sobrinho", "sobrinha", "sobrinhe");
        // Tio/tia por afinidade: genitor de um primo sem ser tio de sangue.
        if (Kinship.Cousins(ego).Any(c => c.Mother == other || c.Father == other))
            return G(other, "tio", "tia");

        // Afinidade (família do cônjuge e cônjuges da família).
        if (ego.Partner is { } spouse)
        {
            if (other == spouse.Mother) return "sogra";
            if (other == spouse.Father) return "sogro";
            if (spouse.Siblings.Contains(other))
                return G(other, "cunhado", "cunhada");
        }
        if (ego.Siblings.Any(s => s.Partner == other))
            return G(other, "cunhado", "cunhada");
        if (ego.Children.Any(c => c.Partner == other))
            return G(other, "genro", "nora");

        // Parentesco distante sem rótulo próprio.
        if (Kinship.IsKin(ego, other))
            return "parente";

        return "conhecido(a)";
    }

    /// <summary>Escolhe a forma pelo gênero da pessoa rotulada.</summary>
    private static string G(Person person, string male, string female, string? neutral = null) =>
        person.Gender switch
        {
            Gender.Female => female,
            Gender.NonBinary => neutral ?? male,
            _ => male,
        };
}
