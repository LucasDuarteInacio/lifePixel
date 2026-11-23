namespace LifeSimulator.Core.Relationships;

/// <summary>Nível de amizade derivado do indicador (GDD §7.3).</summary>
public enum FriendshipTier
{
    Acquaintance,
    Friend,
    BestFriend,
}

/// <summary>
/// Regras que ligam o tipo de relação aos indicadores relevantes e aos
/// limiares de progressão (amigo → melhor amigo, namoro → casamento).
/// "Onde permite": amizade é o valor de destaque nas relações
/// platônicas e amor nas românticas e familiares.
/// </summary>
public static class RelationshipRules
{
    public const int FriendThreshold = 40;
    public const int BestFriendThreshold = 75;

    /// <summary>Amor mútuo mínimo para um pedido de namoro fazer sentido.</summary>
    public const int AskOutMinLove = 35;

    /// <summary>Amor do proponente mínimo para pedir em casamento.</summary>
    public const int ProposeMinLove = 70;

    /// <summary>Meses mínimos de namoro antes do pedido de casamento.</summary>
    public const int MinDatingMonthsToPropose = 6;

    /// <summary>Meses mínimos de noivado antes do casamento.</summary>
    public const int MinEngagementMonthsToMarry = 3;

    /// <summary>Indicadores exibidos e simulados para cada tipo de relação.</summary>
    public static IReadOnlyList<RelationshipIndicator> IndicatorsFor(RelationshipKind kind) =>
        kind switch
        {
            RelationshipKind.Friend =>
                new[] { RelationshipIndicator.Friendship, RelationshipIndicator.Trust,
                    RelationshipIndicator.Respect },
            RelationshipKind.Partner or RelationshipKind.Fiance or RelationshipKind.ExPartner =>
                new[] { RelationshipIndicator.Love, RelationshipIndicator.Intimacy,
                    RelationshipIndicator.Friendship, RelationshipIndicator.Trust },
            RelationshipKind.Rival =>
                new[] { RelationshipIndicator.Rivalry, RelationshipIndicator.Respect },
            _ =>
                new[] { RelationshipIndicator.Love, RelationshipIndicator.Trust,
                    RelationshipIndicator.Respect },
        };

    public static bool IsRomantic(RelationshipKind kind) =>
        kind is RelationshipKind.Partner or RelationshipKind.Fiance
            or RelationshipKind.ExPartner;

    public static bool IsFamily(RelationshipKind kind) =>
        kind is RelationshipKind.Parent or RelationshipKind.Child
            or RelationshipKind.Sibling or RelationshipKind.Relative;

    /// <summary>
    /// Rótulo pt-BR do indicador no contexto da relação: o mesmo campo
    /// <c>Love</c> aparece como "Amor" no casal e "Afeição" na família.
    /// </summary>
    public static string IndicatorLabel(RelationshipIndicator indicator, RelationshipKind kind) =>
        indicator switch
        {
            RelationshipIndicator.Love => IsFamily(kind) ? "Afeição" : "Amor",
            RelationshipIndicator.Friendship => "Amizade",
            RelationshipIndicator.Trust => "Confiança",
            RelationshipIndicator.Respect => "Respeito",
            RelationshipIndicator.Intimacy => "Intimidade",
            _ => "Rivalidade",
        };

    public static FriendshipTier TierOf(int friendship) => friendship switch
    {
        >= BestFriendThreshold => FriendshipTier.BestFriend,
        >= FriendThreshold => FriendshipTier.Friend,
        _ => FriendshipTier.Acquaintance,
    };

    public static string TierLabel(FriendshipTier tier) => tier switch
    {
        FriendshipTier.BestFriend => "melhor amigo(a)",
        FriendshipTier.Friend => "amigo(a)",
        _ => "conhecido(a)",
    };

    public static string StatusLabel(MaritalStatus status) => status switch
    {
        MaritalStatus.Dating => "Namorando",
        MaritalStatus.Engaged => "Noivo(a)",
        MaritalStatus.Married => "Casado(a)",
        MaritalStatus.Divorced => "Divorciado(a)",
        MaritalStatus.Widowed => "Viúvo(a)",
        _ => "Solteiro(a)",
    };
}
