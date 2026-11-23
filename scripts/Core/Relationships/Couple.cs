using LifeSimulator.Core.Characters;

namespace LifeSimulator.Core.Relationships;

/// <summary>
/// Transições de estado civil de um casal (GDD §5.1): namoro → noivado
/// → casamento, término/divórcio e viuvez. Cada transição atualiza
/// <see cref="Person.Status"/>, <see cref="Person.Partner"/> e o tipo
/// da relação nos dois lados, registrando o marco no histórico.
/// </summary>
public static class Couple
{
    public static void StartDating(Person a, Person b, int month)
    {
        a.Partner = b;
        b.Partner = a;
        a.Status = b.Status = MaritalStatus.Dating;

        var relAB = a.RelationshipTo(b, RelationshipKind.Partner);
        var relBA = b.RelationshipTo(a, RelationshipKind.Partner);
        relAB.Kind = relBA.Kind = RelationshipKind.Partner;
        relAB.SinceMonth = relBA.SinceMonth = month;
        relAB.Love = Math.Max(relAB.Love, 60);
        relBA.Love = Math.Max(relBA.Love, 55);
        relAB.Intimacy = Math.Max(relAB.Intimacy, 50);
        relBA.Intimacy = Math.Max(relBA.Intimacy, 50);
        relAB.Trust = Math.Max(relAB.Trust, 55);
        relBA.Trust = Math.Max(relBA.Trust, 55);
        relAB.Friendship = Math.Max(relAB.Friendship, 50);
        relBA.Friendship = Math.Max(relBA.Friendship, 50);

        relAB.Record(month, $"Começou a namorar {b.FirstName}", 50);
        relBA.Record(month, $"Começou a namorar {a.FirstName}", 50);
    }

    public static void Engage(Person a, Person b, int month)
    {
        a.Status = b.Status = MaritalStatus.Engaged;
        var relAB = a.RelationshipTo(b, RelationshipKind.Fiance);
        var relBA = b.RelationshipTo(a, RelationshipKind.Fiance);
        relAB.Kind = relBA.Kind = RelationshipKind.Fiance;
        relAB.SinceMonth = relBA.SinceMonth = month;
        relAB.Record(month, "Pedido de casamento aceito", 70);
        relBA.Record(month, "Pedido de casamento aceito", 70);
    }

    public static void Marry(Person a, Person b, int month)
    {
        a.Status = b.Status = MaritalStatus.Married;
        var relAB = a.RelationshipTo(b, RelationshipKind.Partner);
        var relBA = b.RelationshipTo(a, RelationshipKind.Partner);
        relAB.Kind = relBA.Kind = RelationshipKind.Partner;
        relAB.SinceMonth = relBA.SinceMonth = month;
        relAB.Record(month, "Casamento", 80);
        relBA.Record(month, "Casamento", 80);
    }

    /// <summary>Término de namoro/noivado ou divórcio de casamento.</summary>
    public static void BreakUp(Person a, Person b, int month)
    {
        var wasMarried = a.Status is MaritalStatus.Married;
        a.Status = b.Status = wasMarried ? MaritalStatus.Divorced : MaritalStatus.Single;
        a.Partner = null;
        b.Partner = null;

        var relAB = a.RelationshipTo(b, RelationshipKind.ExPartner);
        var relBA = b.RelationshipTo(a, RelationshipKind.ExPartner);
        relAB.Kind = relBA.Kind = RelationshipKind.ExPartner;
        relAB.SinceMonth = relBA.SinceMonth = month;
        relAB.Love = Math.Min(relAB.Love, 30);
        relBA.Love = Math.Min(relBA.Love, 30);
        relAB.Intimacy = Math.Min(relAB.Intimacy, 15);
        relBA.Intimacy = Math.Min(relBA.Intimacy, 15);
        relAB.Trust -= 20;
        relBA.Trust -= 20;

        var text = wasMarried ? "Divórcio" : "Término";
        relAB.Record(month, text, -60);
        relBA.Record(month, text, -60);
    }

    /// <summary>A pessoa perdeu o cônjuge: fica viúva e o vínculo permanece.</summary>
    public static void Widow(Person person, int month)
    {
        person.Status = MaritalStatus.Widowed;
        if (person.Partner is { } late)
        {
            person.RelationshipTo(late, RelationshipKind.Partner)
                .Record(month, "Ficou viúvo(a)", -80);
        }
        person.Partner = null;
    }
}
