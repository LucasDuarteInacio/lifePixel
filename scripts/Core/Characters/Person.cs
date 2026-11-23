using LifeSimulator.Core.Careers;
using LifeSimulator.Core.Economy;
using LifeSimulator.Core.Genealogy;
using LifeSimulator.Core.Genetics;
using LifeSimulator.Core.Health;
using LifeSimulator.Core.Relationships;
using LifeSimulator.Core.World;

namespace LifeSimulator.Core.Characters;

public enum Gender
{
    Female,
    Male,
    NonBinary,
}

/// <summary>
/// Pessoa simulada: o jogador controla uma por partida, mas familiares
/// e NPCs usam a mesma estrutura e vivem de forma autônoma (GDD §8).
/// </summary>
public sealed class Person
{
    public Guid Id { get; } = Guid.NewGuid();

    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string FullName => $"{FirstName} {LastName}";
    public Gender Gender { get; set; }

    /// <summary>Idade em meses — granularidade exigida pelo avanço mensal na vida adulta.</summary>
    public int AgeMonths { get; set; }
    public int AgeYears => AgeMonths / 12;

    public AttributeSet Attributes { get; } = new();
    public HashSet<PersonalityTrait> Traits { get; } = new();
    public Genome Genome { get; set; } = new();
    public HealthState Health { get; } = new();
    public Finances Finances { get; } = new();
    public Employment? Job { get; set; }
    public Country? BirthCountry { get; set; }
    public Country? Residence { get; set; }
    /// <summary>Origem familiar: natural, FIV ou adoção (GDD §7).</summary>
    public BirthOrigin? Origin { get; set; }

    /// <summary>Estado civil (GDD §5.1): solteiro, namorando, noivo, casado…</summary>
    public MaritalStatus Status { get; set; } = MaritalStatus.Single;

    /// <summary>Avós e tios desta pessoa já foram materializados sob demanda.</summary>
    public bool AncestryEnsured { get; set; }

    /// <summary>Cônjuge e filhos próprios já foram materializados sob demanda.</summary>
    public bool DescendantKinEnsured { get; set; }

    public bool IsAlive { get; private set; } = true;
    public string? CauseOfDeath { get; private set; }
    public int? DeathMonth { get; private set; }

    // Família e genealogia (GDD §7)
    public Person? Mother { get; set; }
    public Person? Father { get; set; }
    public Person? Partner { get; set; }
    public List<Person> Children { get; } = new();
    public List<Person> Siblings { get; } = new();

    /// <summary>Relações indexadas pelo Id da outra pessoa.</summary>
    public Dictionary<Guid, Relationship> Relationships { get; } = new();

    /// <summary>Memória e biografia (GDD §8.4).</summary>
    public List<MemoryEntry> Memory { get; } = new();

    public Relationship RelationshipTo(Person other, RelationshipKind kind)
    {
        if (!Relationships.TryGetValue(other.Id, out var rel))
        {
            rel = new Relationship(other.Id, kind);
            Relationships[other.Id] = rel;
        }
        return rel;
    }

    /// <summary>Vínculo com o grau derivado do grafo familiar (<see cref="Kinship"/>).</summary>
    public Relationship RelationshipTo(Person other) =>
        RelationshipTo(other, Kinship.KindFor(this, other));

    /// <summary>Descendentes vivos elegíveis à sucessão (GDD §3.3, §11.2).</summary>
    public IEnumerable<Person> LivingDescendants()
    {
        foreach (var child in Children)
        {
            if (child.IsAlive)
                yield return child;
            foreach (var descendant in child.LivingDescendants())
                yield return descendant;
        }
    }

    public void Die(int absoluteMonth, string cause)
    {
        IsAlive = false;
        CauseOfDeath = cause;
        DeathMonth = absoluteMonth;
    }

    public void Remember(int absoluteMonth, string text, int emotionalImpact) =>
        Memory.Add(new MemoryEntry(absoluteMonth, AgeYears, text, emotionalImpact));
}
