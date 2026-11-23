using LifeSimulator.Core.Characters;

namespace LifeSimulator.Core.Relationships;

/// <summary>Interações do jogador com uma pessoa conhecida (GDD §4.3, §5.1).</summary>
public enum InteractionKind
{
    Talk,
    SpendTime,
    Compliment,
    Gift,
    Argue,
    Apologize,
    Flirt,
    AskOut,
    Propose,
    Marry,
    BreakUp,
    TryForBaby,
}

/// <summary>
/// Quais interações estão disponíveis para cada vínculo e momento —
/// amizade e amor "onde permite": flertar e pedir em namoro só
/// aparecem em vínculos que admitem romance.
/// </summary>
public static class Interactions
{
    public static string Label(InteractionKind kind) => kind switch
    {
        InteractionKind.Talk => "Conversar",
        InteractionKind.SpendTime => "Passar tempo junto",
        InteractionKind.Compliment => "Elogiar",
        InteractionKind.Gift => "Dar presente",
        InteractionKind.Argue => "Discutir",
        InteractionKind.Apologize => "Pedir desculpas",
        InteractionKind.Flirt => "Flertar",
        InteractionKind.AskOut => "Pedir em namoro",
        InteractionKind.Propose => "Pedir em casamento",
        InteractionKind.Marry => "Casar",
        InteractionKind.BreakUp => "Terminar",
        InteractionKind.TryForBaby => "Tentar ter filho",
        _ => kind.ToString(),
    };

    /// <summary>Custo de um presente: fração do salário mediano local.</summary>
    public static decimal GiftCost(Person actor) =>
        Math.Max(10m, decimal.Round(
            (actor.Residence?.MedianMonthlySalary ?? 1000m) * 0.02m));

    public static IReadOnlyList<InteractionKind> Available(
        Person actor, Person other, Relationship rel, int absoluteMonth)
    {
        var list = new List<InteractionKind>();
        if (!actor.IsAlive || !other.IsAlive)
            return list;

        var social = rel.Kind is not RelationshipKind.Rival;
        if (social)
        {
            list.Add(InteractionKind.Talk);
            list.Add(InteractionKind.SpendTime);
            list.Add(InteractionKind.Compliment);
            if (actor.Finances.Money >= GiftCost(actor))
                list.Add(InteractionKind.Gift);
        }
        list.Add(InteractionKind.Argue);

        var hurtRecently = rel.Trust < 40
            || rel.History.Any(h => h.Impact < 0 && absoluteMonth - h.AbsoluteMonth <= 24);
        if (hurtRecently || rel.Kind == RelationshipKind.ExPartner
            || rel.Kind == RelationshipKind.Rival)
            list.Add(InteractionKind.Apologize);

        var adults = actor.AgeYears >= 13 && other.AgeYears >= 13;
        var bothSingle = actor.Partner is null && other.Partner is null;

        switch (rel.Kind)
        {
            case RelationshipKind.Friend:
                if (adults)
                    list.Add(InteractionKind.Flirt);
                if (adults && bothSingle
                    && actor.Status == MaritalStatus.Single
                    && other.Status == MaritalStatus.Single
                    && rel.Love >= RelationshipRules.AskOutMinLove
                    && Mutual(rel, actor, other) is { } mutual
                    && mutual.Love >= RelationshipRules.AskOutMinLove)
                    list.Add(InteractionKind.AskOut);
                break;

            case RelationshipKind.ExPartner:
                if (adults && bothSingle)
                    list.Add(InteractionKind.Flirt);
                break;

            case RelationshipKind.Partner:
                if (actor.Status == MaritalStatus.Dating
                    && rel.Love >= RelationshipRules.ProposeMinLove
                    && MonthsTogether(rel, absoluteMonth)
                        >= RelationshipRules.MinDatingMonthsToPropose)
                    list.Add(InteractionKind.Propose);
                list.Add(InteractionKind.BreakUp);
                if (CanConceive(actor, other))
                    list.Add(InteractionKind.TryForBaby);
                break;

            case RelationshipKind.Fiance:
                if (MonthsTogether(rel, absoluteMonth)
                        >= RelationshipRules.MinEngagementMonthsToMarry)
                    list.Add(InteractionKind.Marry);
                list.Add(InteractionKind.BreakUp);
                if (CanConceive(actor, other))
                    list.Add(InteractionKind.TryForBaby);
                break;
        }

        return list;
    }

    /// <summary>Relação recíproca, se existir.</summary>
    public static Relationship? Mutual(Relationship rel, Person actor, Person other) =>
        other.Relationships.GetValueOrDefault(actor.Id);

    public static int MonthsTogether(Relationship rel, int absoluteMonth) =>
        rel.SinceMonth is { } since ? absoluteMonth - since : 0;

    /// <summary>O casal pode conceber: um lado precisa ser mulher em idade fértil.</summary>
    public static Person? MotherOf(Person a, Person b) =>
        a.Gender == Gender.Female ? a : b.Gender == Gender.Female ? b : null;

    public static bool CanConceive(Person a, Person b) =>
        a.Status is MaritalStatus.Engaged or MaritalStatus.Married
        && a.Partner == b
        && MotherOf(a, b) is { } mother
        && mother.AgeYears is >= 18 and <= 45
        && a.Children.Count + b.Children.Count < 6;
}
