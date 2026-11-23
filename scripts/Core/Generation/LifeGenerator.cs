using LifeSimulator.Core.Careers;
using LifeSimulator.Core.Characters;
using LifeSimulator.Core.Genetics;
using LifeSimulator.Core.Relationships;
using LifeSimulator.Core.World;

namespace LifeSimulator.Core.Generation;

/// <summary>
/// Criação de novas vidas (GDD §3.1): o modo aleatório gera país,
/// família e características por sorteio; o personalizado permite
/// fixar algumas escolhas mantendo o resto aleatório.
/// </summary>
public sealed class LifeGenerator(Random rng)
{
    /// <summary>
    /// Gera um recém-nascido com pais (e possíveis irmãos), genoma
    /// herdado e atributos iniciais.
    /// </summary>
    public Person GenerateNewborn(
        Country country,
        Gender? gender = null,
        string? firstName = null,
        string? lastName = null)
    {
        var originKind = rng.NextDouble() switch
        {
            < 0.05 => OriginKind.InVitro,
            < 0.13 => OriginKind.Adopted,
            _ => OriginKind.Natural,
        };
        var (hasMother, hasFather) = originKind switch
        {
            OriginKind.InVitro => (true, false),
            // Adoção: casal (60%), só mãe (25%) ou só pai (15%).
            OriginKind.Adopted => rng.NextDouble() switch
            {
                < 0.60 => (true, true),
                < 0.85 => (true, false),
                _ => (false, true),
            },
            _ => (true, true),
        };
        var origin = new BirthOrigin(originKind,
            hasMother && hasFather ? RollUnion(originKind) : null);

        Person? mother = hasMother
            ? GenerateAdult(country, Gender.Female, rng.Next(18, 41))
            : null;
        Person? father = hasFather
            ? GenerateAdult(country, Gender.Male, Math.Clamp(
                (mother?.AgeYears ?? rng.Next(24, 42)) + rng.Next(-3, 6), 18, 70))
            : null;

        if (mother is not null && father is not null)
            LinkCouple(mother, father, origin.Union);

        var surname = lastName ?? father?.LastName ?? mother?.LastName
            ?? country.PickSurname(rng);
        var childGender = gender ?? (Gender)rng.Next(2);
        var child = new Person
        {
            FirstName = firstName ?? country.PickFirstName(childGender, rng),
            LastName = surname,
            Gender = childGender,
            AgeMonths = 0,
            BirthCountry = country,
            Residence = country,
            Mother = mother,
            Father = father,
            Origin = origin,
            Genome = origin.Kind == OriginKind.Natural && mother is not null && father is not null
                ? Genome.Inherit(mother.Genome, father.Genome, rng)
                : Genome.Random(rng),
        };

        mother?.Children.Add(child);
        father?.Children.Add(child);

        if (mother is not null)
        {
            var bond = child.RelationshipTo(mother, RelationshipKind.Parent);
            bond.Love = 90;
            bond.Trust = 80;
            mother.RelationshipTo(child, RelationshipKind.Child).Love = 95;
        }
        if (father is not null)
        {
            var rel = child.RelationshipTo(father, RelationshipKind.Parent);
            rel.Love = 85;
            rel.Trust = 75;
            father.RelationshipTo(child, RelationshipKind.Child).Love = 90;
        }

        // Irmãos mais velhos possíveis (LinkChild já liga os vínculos).
        var parent = (Person?)mother ?? father!;
        var siblingCount = rng.Next(100) switch
        {
            < 55 => 0,
            < 85 => 1,
            _ => 2,
        };
        for (var i = 0; i < siblingCount; i++)
            GenerateChild(mother, father, country,
                rng.Next(12, Math.Max(13, parent.AgeYears * 12 - 156)));

        // Atributos de nascimento: base neutra com influência genética leve.
        child.Attributes[AttributeKind.Health] = 50 + (child.Genome.ImmuneSystem - 50) / 5 + rng.Next(-10, 11);
        child.Attributes[AttributeKind.Appearance] = 40 + rng.Next(40);
        child.Attributes[AttributeKind.Intelligence] = 40 + rng.Next(30);
        child.Health.Physical = 60 + child.Genome.ImmuneSystem / 4;
        child.Health.Mental = 70;

        for (var i = rng.Next(1, 3); i > 0; i--)
            child.Traits.Add(RandomTrait());

        return child;
    }

    /// <summary>Filho de um casal já existente (usado para irmãos e descendentes).</summary>
    public Person GenerateChildOf(Person mother, Person? father, Country country, int ageMonths) =>
        GenerateChild(mother, father, country, ageMonths);

    /// <summary>Filho de um ou dois pais já existentes.</summary>
    private Person GenerateChild(Person? mother, Person? father, Country country, int ageMonths)
    {
        var gender = (Gender)rng.Next(2);
        var child = new Person
        {
            FirstName = country.PickFirstName(gender, rng),
            LastName = father?.LastName ?? mother!.LastName,
            Gender = gender,
            AgeMonths = ageMonths,
            BirthCountry = country,
            Residence = country,
            Mother = mother,
            Father = father,
            Origin = new BirthOrigin(OriginKind.Natural, UnionOf(mother, father)),
            Genome = mother is not null && father is not null
                ? Genome.Inherit(mother.Genome, father.Genome, rng)
                : Genome.Random(rng),
        };
        CopyAttributes(AttributeSet.Random(rng), child.Attributes);
        LinkChild(child, mother, father);
        return child;
    }

    /// <summary>
    /// Liga o filho aos pais e aos irmãos existentes dos dois lados,
    /// criando as relações recíprocas com vínculos iniciais.
    /// </summary>
    private void LinkChild(Person child, Person? mother, Person? father)
    {
        foreach (var parent in new[] { mother, father })
        {
            if (parent is null) continue;
            if (!parent.Children.Contains(child))
                parent.Children.Add(child);
            child.RelationshipTo(parent, RelationshipKind.Parent).Love = 80;
            parent.RelationshipTo(child, RelationshipKind.Child).Love = 85;
        }

        var siblings = new[] { mother, father }
            .Where(p => p is not null)
            .SelectMany(p => p!.Children)
            .Where(s => s != child)
            .Distinct();
        foreach (var sibling in siblings)
        {
            if (!child.Siblings.Contains(sibling))
                child.Siblings.Add(sibling);
            if (!sibling.Siblings.Contains(child))
                sibling.Siblings.Add(child);
            child.RelationshipTo(sibling, RelationshipKind.Sibling).Love = 60 + rng.Next(30);
            sibling.RelationshipTo(child, RelationshipKind.Sibling).Love = 60 + rng.Next(30);
        }
    }

    /// <summary>União dos pais na época do nascimento, derivada do estado civil.</summary>
    private static UnionKind? UnionOf(Person? mother, Person? father) =>
        (mother, father) switch
        {
            (null, null) => null,
            ({ } m, { } f) when m.Partner == f || f.Partner == m =>
                m.Status == MaritalStatus.Married || f.Status == MaritalStatus.Married
                    ? UnionKind.Married : UnionKind.Dating,
            ({ }, { }) => UnionKind.Affair,
            _ => null,
        };

    /// <summary>
    /// União do casal na época do nascimento ou da adoção (GDD §7):
    /// nascimento natural admite affair; adoção por casal é quase
    /// sempre uma união estável.
    /// </summary>
    private UnionKind RollUnion(OriginKind kind) => kind switch
    {
        OriginKind.Adopted => rng.NextDouble() < 0.75
            ? UnionKind.Married : UnionKind.Dating,
        _ => rng.NextDouble() switch
        {
            < 0.45 => UnionKind.Married,
            < 0.85 => UnionKind.Dating,
            _ => UnionKind.Affair,
        },
    };

    /// <summary>Vínculo entre os pais conforme a união: affair vira ex.</summary>
    private void LinkCouple(Person a, Person b, UnionKind? union)
    {
        if (union is UnionKind.Affair)
        {
            a.RelationshipTo(b, RelationshipKind.ExPartner).Love = 30 + rng.Next(30);
            b.RelationshipTo(a, RelationshipKind.ExPartner);
            return;
        }

        a.Partner = b;
        b.Partner = a;
        a.Status = b.Status =
            union == UnionKind.Married ? MaritalStatus.Married : MaritalStatus.Dating;

        var relAB = a.RelationshipTo(b, RelationshipKind.Partner);
        var relBA = b.RelationshipTo(a, RelationshipKind.Partner);
        var baseLove = union == UnionKind.Married ? 70 + rng.Next(30) : 55 + rng.Next(30);
        relAB.Love = relBA.Love = baseLove;
        relAB.Intimacy = relBA.Intimacy = 60 + rng.Next(40);
        relAB.Trust = relBA.Trust = 50 + rng.Next(40);
        // A união já existe há algum tempo.
        relAB.SinceMonth = relBA.SinceMonth = -rng.Next(12, 241);
    }

    public Person GenerateAdult(Country country, Gender gender, int ageYears)
    {
        var person = new Person
        {
            FirstName = country.PickFirstName(gender, rng),
            LastName = country.PickSurname(rng),
            Gender = gender,
            AgeMonths = ageYears * 12 + rng.Next(12),
            BirthCountry = country,
            Residence = country,
            Genome = Genome.Random(rng),
        };
        CopyAttributes(AttributeSet.Random(rng), person.Attributes);
        for (var i = rng.Next(1, 4); i > 0; i--)
            person.Traits.Add(RandomTrait());
        person.Health.Physical = 50 + person.Genome.ImmuneSystem / 3 - Math.Max(0, ageYears - 45) / 2;
        person.Finances.Money = country.MedianMonthlySalary * rng.Next(0, 24);

        // Emprego: a maioria dos adultos segue uma carreira, com nível
        // proporcional aos anos de experiência (GDD §5.3, §8).
        if (ageYears >= 18 && rng.NextDouble() < 0.8)
        {
            var career = CareerCatalog.PickRandom(rng);
            if (ageYears >= career.MinAge)
            {
                var level = 1 + (ageYears - career.MinAge) / 4 + rng.Next(-1, 2);
                person.Job = new Employment(career, level);
                person.Finances.MonthlyIncome = person.Job.MonthlySalary(country);
            }
        }
        person.Finances.MonthlyExpenses = country.MonthlyCostOfLiving * (decimal)(0.6 + rng.NextDouble() * 0.7);
        return person;
    }

    /// <summary>
    /// Pessoa da mesma faixa etária, para novas amizades (GDD §5.1):
    /// colega de escola na infância, de trabalho na vida adulta.
    /// </summary>
    public Person GeneratePeer(Country country, int ageYears, Gender? gender = null)
    {
        var g = gender ?? (Gender)rng.Next(2);
        if (ageYears >= 18)
            return GenerateAdult(country, g, ageYears);

        var peer = new Person
        {
            FirstName = country.PickFirstName(g, rng),
            LastName = country.PickSurname(rng),
            Gender = g,
            AgeMonths = ageYears * 12 + rng.Next(12),
            BirthCountry = country,
            Residence = country,
            Genome = Genome.Random(rng),
        };
        CopyAttributes(AttributeSet.Random(rng), peer.Attributes);
        for (var i = rng.Next(1, 3); i > 0; i--)
            peer.Traits.Add(RandomTrait());
        peer.Health.Physical = 60 + peer.Genome.ImmuneSystem / 4;
        return peer;
    }

    /// <summary>
    /// Família estendida sob demanda (GDD §7.1): materializa avós,
    /// tios e primos — e parceiros/filhos de irmãos e filhos adultos —
    /// na primeira consulta. Idempotente pelas flags
    /// <see cref="Person.AncestryEnsured"/>/<see cref="Person.DescendantKinEnsured"/>.
    /// </summary>
    public void EnsureExtendedFamily(Person ego, int absoluteMonth)
    {
        EnsureAncestors(ego.Mother, ego, absoluteMonth);
        EnsureAncestors(ego.Father, ego, absoluteMonth);
        foreach (var sibling in ego.Siblings.ToList())
            EnsurePartnerAndChildren(sibling, ego, absoluteMonth);
        foreach (var child in ego.Children.ToList())
            EnsurePartnerAndChildren(child, ego, absoluteMonth);
    }

    /// <summary>Pais e irmãos do genitor — os avós e tios do ego.</summary>
    private void EnsureAncestors(Person? parent, Person ego, int absoluteMonth)
    {
        if (parent is null || parent.AncestryEnsured)
            return;
        parent.AncestryEnsured = true;
        var country = parent.BirthCountry ?? ego.BirthCountry!;

        var gm = GenerateAdult(country, Gender.Female,
            parent.AgeYears + rng.Next(20, 45));
        var gf = GenerateAdult(country, Gender.Male,
            Math.Max(18, gm.AgeYears + rng.Next(-4, 8)));
        parent.Mother = gm;
        parent.Father = gf;
        gm.Children.Add(parent);
        gf.Children.Add(parent);
        parent.RelationshipTo(gm, RelationshipKind.Parent).Love = 80;
        parent.RelationshipTo(gf, RelationshipKind.Parent).Love = 75;
        gm.RelationshipTo(parent, RelationshipKind.Child).Love = 85;
        gf.RelationshipTo(parent, RelationshipKind.Child).Love = 85;

        var gmDead = RollRetrospectiveDeath(gm, absoluteMonth);
        var gfDead = RollRetrospectiveDeath(gf, absoluteMonth);
        if (!gmDead && !gfDead)
        {
            LinkCouple(gm, gf, UnionKind.Married);
        }
        else
        {
            // Vínculo passado registrado; o sobrevivente fica viúvo.
            gm.Partner = gf;
            gf.Partner = gm;
            gm.Status = gf.Status = MaritalStatus.Married;
            if (!gmDead) Couple.Widow(gm, absoluteMonth - rng.Next(1, 61));
            if (!gfDead) Couple.Widow(gf, absoluteMonth - rng.Next(1, 61));
        }

        // Tios: irmãos do genitor (filhos dos avós) com suas famílias.
        var uncleCount = rng.Next(100) switch
        {
            < 25 => 0,
            < 60 => 1,
            < 90 => 2,
            _ => 3,
        };
        var uncleMaxAge = Math.Max(13, gm.AgeMonths - 15 * 12);
        for (var i = 0; i < uncleCount; i++)
        {
            var ageMonths = Math.Clamp(
                parent.AgeMonths + rng.Next(-15 * 12, 16 * 12), 12, uncleMaxAge);
            var uncle = GenerateChild(gm, gf, country, ageMonths);
            EnsurePartnerAndChildren(uncle, ego, absoluteMonth);
        }
    }

    /// <summary>Cônjuge e filhos de um adulto, sorteados uma única vez.</summary>
    private void EnsurePartnerAndChildren(Person person, Person ego, int absoluteMonth)
    {
        if (person.DescendantKinEnsured)
            return;
        if (!person.IsAlive)
        {
            person.DescendantKinEnsured = true;
            return;
        }
        // Menor de idade: rola só quando atingir a vida adulta.
        if (person.AgeYears < 16)
            return;
        person.DescendantKinEnsured = true;

        var country = person.Residence ?? ego.Residence!;
        if (person.Partner is null && rng.NextDouble() < 0.65)
        {
            var partner = GenerateAdult(country, CompatibleGender(person),
                Math.Max(16, person.AgeYears + rng.Next(-5, 6)));
            var union = person.AgeYears >= 24 && rng.NextDouble() < 0.7
                ? UnionKind.Married : UnionKind.Dating;
            LinkCouple(person, partner, union);
        }

        if (person.Partner is { IsAlive: true } spouse)
        {
            var childCount = rng.Next(100) switch
            {
                < 45 => 0,
                < 80 => 1,
                < 95 => 2,
                _ => 3,
            };
            var (mother, father) = person.Gender == Gender.Female
                ? (person, spouse)
                : spouse.Gender == Gender.Female
                    ? (spouse, person)
                    : (person, spouse);
            var maxAge = Math.Max(13, mother.AgeMonths - 16 * 12);
            for (var i = 0; i < childCount; i++)
                GenerateChild(mother, father, country, rng.Next(12, maxAge));
        }
    }

    private Gender CompatibleGender(Person person) => person.Gender switch
    {
        Gender.Female => Gender.Male,
        Gender.Male => Gender.Female,
        _ => (Gender)rng.Next(2),
    };

    /// <summary>Avós podem entrar na árvore já falecidos, com causa retroativa.</summary>
    private bool RollRetrospectiveDeath(Person person, int absoluteMonth)
    {
        var p = person.AgeYears switch
        {
            >= 85 => 0.8,
            >= 70 => 0.55,
            >= 60 => 0.15,
            _ => 0.0,
        };
        if (rng.NextDouble() >= p)
            return false;
        person.Die(absoluteMonth - rng.Next(1, 241),
            Simulation.Mortality.PickCause(rng, person.AgeYears, 25));
        return true;
    }

    private PersonalityTrait RandomTrait() =>
        (PersonalityTrait)rng.Next(Enum.GetValues<PersonalityTrait>().Length);

    private static void CopyAttributes(AttributeSet source, AttributeSet target)
    {
        foreach (var kind in Enum.GetValues<AttributeKind>())
            target[kind] = source[kind];
    }
}
