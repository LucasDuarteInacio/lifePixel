using LifeSimulator.Core.Characters;
using LifeSimulator.Core.Events;
using LifeSimulator.Core.Genealogy;
using LifeSimulator.Core.Generation;
using LifeSimulator.Core.Relationships;
using LifeSimulator.Core.Simulation;
using LifeSimulator.Core.World;

namespace LifeSimulator.Tests;

/// <summary>Monta famílias manualmente para verificar o parentesco derivado.</summary>
public class KinshipTests
{
    private static Person P(Gender gender = Gender.Male, int ageYears = 30, string name = "X") =>
        new() { Gender = gender, AgeMonths = ageYears * 12, FirstName = name };

    private static (Person ego, Person mom, Person dad, Person grandma, Person uncle,
        Person auntInLaw, Person cousin, Person sis, Person nib, Person child,
        Person grandchild, Person husband, Person mil, Person sil) BuildFamily()
    {
        var ego = P(Gender.Female, 25, "Ego");
        var mom = P(Gender.Female, 55, "Mae");
        var dad = P(Gender.Male, 58, "Pai");
        var grandma = P(Gender.Female, 80, "Vo");
        var uncle = P(Gender.Male, 57, "Tio");
        var auntInLaw = P(Gender.Female, 55, "Tia");
        var cousin = P(Gender.Male, 28, "Primo");
        var sis = P(Gender.Female, 27, "Irma");
        var nib = P(Gender.Male, 5, "Sobr");
        var child = P(Gender.Male, 3, "Filho");
        var grandchild = P(Gender.Female, 0, "Neta");
        var husband = P(Gender.Male, 26, "Marido");
        var mil = P(Gender.Female, 60, "Sogra");
        var sil = P(Gender.Female, 30, "Cunha");

        ego.Mother = mom; ego.Father = dad;
        mom.Children.Add(ego); dad.Children.Add(ego);
        mom.Mother = grandma; grandma.Children.Add(mom);
        mom.Siblings.Add(uncle); uncle.Siblings.Add(mom);
        grandma.Children.Add(uncle); uncle.Mother = grandma;
        uncle.Partner = auntInLaw; auntInLaw.Partner = uncle;
        uncle.Children.Add(cousin); cousin.Father = uncle;
        cousin.Mother = auntInLaw; auntInLaw.Children.Add(cousin);
        ego.Siblings.Add(sis); sis.Siblings.Add(ego);
        mom.Children.Add(sis); dad.Children.Add(sis);
        sis.Mother = mom; sis.Father = dad;
        sis.Children.Add(nib); nib.Mother = sis;
        ego.Children.Add(child); child.Mother = ego;
        child.Children.Add(grandchild); grandchild.Mother = child;
        ego.Partner = husband; husband.Partner = ego;
        ego.Status = MaritalStatus.Married; husband.Status = MaritalStatus.Married;
        husband.Mother = mil; mil.Children.Add(husband);
        husband.Siblings.Add(sil); sil.Siblings.Add(husband);

        return (ego, mom, dad, grandma, uncle, auntInLaw, cousin, sis, nib,
            child, grandchild, husband, mil, sil);
    }

    [Fact]
    public void Derives_every_kin_degree()
    {
        var f = BuildFamily();

        Assert.Contains(f.grandma, Kinship.Grandparents(f.ego));
        Assert.Contains(f.uncle, Kinship.UnclesAunts(f.ego));
        Assert.Contains(f.cousin, Kinship.Cousins(f.ego));
        Assert.Contains(f.nib, Kinship.NiecesNephews(f.ego));
        Assert.Contains(f.grandchild, Kinship.Grandchildren(f.ego));
        Assert.Contains(f.mil, Kinship.InLaws(f.ego));
        Assert.Contains(f.sil, Kinship.InLaws(f.ego));
        Assert.Contains(f.grandma, Kinship.Ancestors(f.ego));
        Assert.Contains(f.grandchild, Kinship.Descendants(f.ego));
        Assert.True(Kinship.IsKin(f.ego, f.cousin));
        Assert.True(Kinship.IsKin(f.ego, f.mil));
    }

    [Fact]
    public void KinLabels_are_portuguese_and_gendered()
    {
        var f = BuildFamily();

        Assert.Equal("mãe", KinLabel.Label(f.ego, f.mom));
        Assert.Equal("pai", KinLabel.Label(f.ego, f.dad));
        Assert.Equal("avó", KinLabel.Label(f.ego, f.grandma));
        Assert.Equal("tio", KinLabel.Label(f.ego, f.uncle));
        Assert.Equal("primo", KinLabel.Label(f.ego, f.cousin));
        Assert.Equal("irmã", KinLabel.Label(f.ego, f.sis));
        Assert.Equal("sobrinho", KinLabel.Label(f.ego, f.nib));
        Assert.Equal("filho", KinLabel.Label(f.ego, f.child));
        Assert.Equal("neta", KinLabel.Label(f.ego, f.grandchild));
        Assert.Equal("marido", KinLabel.Label(f.ego, f.husband));
        Assert.Equal("sogra", KinLabel.Label(f.ego, f.mil));
        Assert.Equal("cunhada", KinLabel.Label(f.ego, f.sil));
    }

    [Fact]
    public void KindFor_maps_direct_kin_and_defaults()
    {
        var f = BuildFamily();

        Assert.Equal(RelationshipKind.Parent, Kinship.KindFor(f.ego, f.mom));
        Assert.Equal(RelationshipKind.Child, Kinship.KindFor(f.ego, f.child));
        Assert.Equal(RelationshipKind.Sibling, Kinship.KindFor(f.ego, f.sis));
        Assert.Equal(RelationshipKind.Relative, Kinship.KindFor(f.ego, f.cousin));
        Assert.Equal(RelationshipKind.Friend, Kinship.KindFor(f.ego, P(name: "Stranger")));
    }
}

public class FamilyRegistryTests
{
    [Fact]
    public void RegisterTree_pulls_the_whole_connected_network()
    {
        var sim = LifeSimulation.NewRandomLife(seed: 9);
        var registry = sim.Registry;

        Assert.Contains(sim.Player, registry.All);
        if (sim.Player.Mother is { } mom) Assert.Contains(mom, registry.All);
        if (sim.Player.Father is { } dad) Assert.Contains(dad, registry.All);
        foreach (var sib in sim.Player.Siblings) Assert.Contains(sib, registry.All);
        Assert.All(registry.All, p => Assert.NotNull(registry.Find(p.Id)));
    }
}

public class CoupleTransitionTests
{
    private static (Person a, Person b) CoupleOf()
    {
        var a = new Person { Gender = Gender.Male, AgeMonths = 30 * 12 };
        var b = new Person { Gender = Gender.Female, AgeMonths = 28 * 12 };
        return (a, b);
    }

    [Fact]
    public void Dating_engaged_married_divorced_flow()
    {
        var (a, b) = CoupleOf();

        Couple.StartDating(a, b, 10);
        Assert.Equal(MaritalStatus.Dating, a.Status);
        Assert.Equal(b, a.Partner);
        Assert.Equal(RelationshipKind.Partner, a.Relationships[b.Id].Kind);
        Assert.Equal(10, a.Relationships[b.Id].SinceMonth);

        Couple.Engage(a, b, 20);
        Assert.Equal(MaritalStatus.Engaged, a.Status);
        Assert.Equal(RelationshipKind.Fiance, a.Relationships[b.Id].Kind);

        Couple.Marry(a, b, 30);
        Assert.Equal(MaritalStatus.Married, a.Status);
        Assert.Equal(RelationshipKind.Partner, a.Relationships[b.Id].Kind);

        Couple.BreakUp(a, b, 40);
        Assert.Equal(MaritalStatus.Divorced, a.Status);
        Assert.Equal(MaritalStatus.Divorced, b.Status);
        Assert.Null(a.Partner);
        Assert.Equal(RelationshipKind.ExPartner, a.Relationships[b.Id].Kind);
        Assert.True(a.Relationships[b.Id].History.Count >= 1);
    }

    [Fact]
    public void Death_of_spouse_widows_the_survivor()
    {
        var (a, b) = CoupleOf();
        Couple.StartDating(a, b, 0);
        Couple.Marry(a, b, 6);

        Couple.Widow(b, 12);

        Assert.Equal(MaritalStatus.Widowed, b.Status);
        Assert.Null(b.Partner);
        Assert.Contains(b.Relationships[a.Id].History,
            h => h.Text.Contains("viúv"));
    }
}

public class RelationshipRulesTests
{
    [Fact]
    public void Friendship_tiers_follow_the_thresholds()
    {
        Assert.Equal(FriendshipTier.Acquaintance,
            RelationshipRules.TierOf(RelationshipRules.FriendThreshold - 1));
        Assert.Equal(FriendshipTier.Friend,
            RelationshipRules.TierOf(RelationshipRules.FriendThreshold));
        Assert.Equal(FriendshipTier.BestFriend,
            RelationshipRules.TierOf(RelationshipRules.BestFriendThreshold));
    }

    [Fact]
    public void Indicators_vary_by_relationship_kind()
    {
        Assert.Contains(RelationshipIndicator.Friendship,
            RelationshipRules.IndicatorsFor(RelationshipKind.Friend));
        Assert.DoesNotContain(RelationshipIndicator.Love,
            RelationshipRules.IndicatorsFor(RelationshipKind.Friend));

        Assert.Contains(RelationshipIndicator.Love,
            RelationshipRules.IndicatorsFor(RelationshipKind.Partner));
        Assert.Contains(RelationshipIndicator.Intimacy,
            RelationshipRules.IndicatorsFor(RelationshipKind.Fiance));

        Assert.Equal("Afeição",
            RelationshipRules.IndicatorLabel(
                RelationshipIndicator.Love, RelationshipKind.Parent));
        Assert.Equal("Amor",
            RelationshipRules.IndicatorLabel(
                RelationshipIndicator.Love, RelationshipKind.Partner));
    }
}

public class ExtendedFamilyGenerationTests
{
    [Fact]
    public void EnsureExtendedFamily_materializes_grandparents_uncles_cousins()
    {
        var sim = LifeSimulation.NewRandomLife(seed: 21);
        var player = sim.Player;

        sim.EnsureExtendedFamily();

        var grandparents = Kinship.Grandparents(player).ToList();
        Assert.Equal(2 * Kinship.Parents(player).Count(), grandparents.Count);

        var uncles = Kinship.UnclesAunts(player).ToList();
        Assert.All(uncles, u => Assert.Contains(u.Mother ?? u.Father!, grandparents));

        var cousins = Kinship.Cousins(player).ToList();
        Assert.All(cousins, c =>
            Assert.Contains(c.Mother ?? c.Father!, uncles));

        // Consistência bidirecional: primo me considera primo também.
        Assert.All(cousins, c => Assert.Contains(player, Kinship.Cousins(c)));
    }

    [Fact]
    public void EnsureExtendedFamily_is_idempotent_and_deterministic()
    {
        var a = LifeSimulation.NewRandomLife(seed: 33);
        var b = LifeSimulation.NewRandomLife(seed: 33);

        a.EnsureExtendedFamily();
        b.EnsureExtendedFamily();
        var countBefore = a.Registry.All.Count;
        a.EnsureExtendedFamily();

        Assert.Equal(countBefore, a.Registry.All.Count);
        Assert.Equal(b.Registry.All.Count, a.Registry.All.Count);
        Assert.Equal(
            Kinship.Grandparents(a.Player).Select(p => p.FullName).OrderBy(n => n),
            Kinship.Grandparents(b.Player).Select(p => p.FullName).OrderBy(n => n));
    }
}

public class RelationshipDirectorTests
{
    private static Person PartnerFor(LifeSimulation sim, int ageYears = 22)
    {
        var player = sim.Player;
        player.AgeMonths = Math.Max(player.AgeMonths, 20 * 12);
        player.Gender = Gender.Male;
        var npc = new Person
        {
            FirstName = "Ana",
            Gender = Gender.Female,
            AgeMonths = ageYears * 12,
            Residence = player.Residence,
        };
        sim.Registry.Register(npc);
        return npc;
    }

    [Fact]
    public void Drift_cools_relationships_over_time()
    {
        var sim = LifeSimulation.NewRandomLife(seed: 11);
        var parent = sim.Player.Mother ?? sim.Player.Father!;
        var bond = sim.Player.Relationships[parent.Id];
        bond.Love = 50;

        for (var i = 0; i < 5; i++)
            sim.AdvancePeriod();

        Assert.True(bond.Love < 50, $"Esperava deriva, Love = {bond.Love}");
        Assert.True(bond.Love >= 40, "Laço familiar não cai abaixo do chão");
    }

    [Fact]
    public void Npc_partner_eventually_proposes_to_player()
    {
        var sim = LifeSimulation.NewRandomLife(seed: 5);
        var npc = PartnerFor(sim);
        Couple.StartDating(sim.Player, npc, sim.AbsoluteMonth - 12);
        npc.RelationshipTo(sim.Player).Love = 90;

        PendingDecision? decision = null;
        for (var i = 0; i < 48 && decision is null; i++)
        {
            sim.AdvancePeriod();
            decision = sim.PeekDecision();
        }

        Assert.NotNull(decision);
        Assert.Equal("Pedido de casamento", decision.Title);

        var outcome = sim.ResolveDecision(decision, 0);
        Assert.Contains("noivos", outcome);
        Assert.Equal(MaritalStatus.Engaged, sim.Player.Status);
        Assert.Equal(RelationshipKind.Fiance,
            sim.Player.Relationships[npc.Id].Kind);
    }

    [Fact]
    public void Npc_couples_progress_from_dating_to_marriage()
    {
        var sim = LifeSimulation.NewRandomLife(seed: 17);
        var gen = new LifeGenerator(new Random(1));
        var country = sim.Player.Residence!;
        var a = gen.GenerateAdult(country, Gender.Male, 30);
        var b = gen.GenerateAdult(country, Gender.Female, 28);
        Couple.StartDating(a, b, sim.AbsoluteMonth - 12);
        a.RelationshipTo(b).Love = b.RelationshipTo(a).Love = 85;
        sim.Registry.Register(a);
        sim.Registry.Register(b);

        for (var i = 0; i < 12; i++)
            sim.AdvancePeriod();

        Assert.True(
            a.Status is MaritalStatus.Engaged or MaritalStatus.Married,
            $"Esperava progressão, status = {a.Status}");
    }
}

public class InteractionTests
{
    private static (LifeSimulation sim, Person friend) SimWithFriend(
        int seed = 4, int love = 60)
    {
        var sim = LifeSimulation.NewRandomLife(seed);
        sim.Player.AgeMonths = Math.Max(sim.Player.AgeMonths, 20 * 12);
        var friend = new Person
        {
            FirstName = "Caio",
            Gender = Gender.Male,
            AgeMonths = 21 * 12,
            Residence = sim.Player.Residence,
        };
        sim.Registry.Register(friend);
        var rel = sim.BondWith(friend);
        rel.Kind = RelationshipKind.Friend;
        rel.Friendship = 60;
        rel.Love = love;
        var mutual = friend.RelationshipTo(sim.Player, RelationshipKind.Friend);
        mutual.Friendship = 55;
        mutual.Love = love;
        return (sim, friend);
    }

    [Fact]
    public void AskOut_with_mutual_love_starts_dating()
    {
        var (sim, friend) = SimWithFriend();

        Assert.Contains(InteractionKind.AskOut,
            sim.AvailableInteractions(friend));
        var text = sim.Interact(friend, InteractionKind.AskOut);

        Assert.Contains("namorando", text);
        Assert.Equal(MaritalStatus.Dating, sim.Player.Status);
        Assert.Equal(friend, sim.Player.Partner);
        Assert.Equal(RelationshipKind.Partner,
            sim.Player.Relationships[friend.Id].Kind);
    }

    [Fact]
    public void Engaged_couple_can_marry_after_the_waiting_period()
    {
        var (sim, friend) = SimWithFriend();
        Couple.StartDating(sim.Player, friend, sim.AbsoluteMonth - 12);
        Couple.Engage(sim.Player, friend, sim.AbsoluteMonth - 6);

        Assert.Contains(InteractionKind.Marry,
            sim.AvailableInteractions(friend));
        var text = sim.Interact(friend, InteractionKind.Marry);

        Assert.Contains("casaram", text);
        Assert.Equal(MaritalStatus.Married, sim.Player.Status);
        Assert.Equal(RelationshipKind.Partner,
            sim.Player.Relationships[friend.Id].Kind);
    }

    [Fact]
    public void Gift_costs_money_and_raises_bond()
    {
        var (sim, friend) = SimWithFriend();
        sim.Player.Finances.Money = 1000m;
        var rel = sim.Player.Relationships[friend.Id];
        var before = rel.Friendship;

        var text = sim.Interact(friend, InteractionKind.Gift);

        Assert.True(sim.Player.Finances.Money < 1000m);
        Assert.True(rel.Friendship > before);
        Assert.Contains("presente", text);
    }

    [Fact]
    public void TryForBaby_registers_the_child_in_the_family()
    {
        var (sim, friend) = SimWithFriend();
        sim.Player.Gender = Gender.Female;
        friend.Gender = Gender.Male;
        Couple.StartDating(sim.Player, friend, sim.AbsoluteMonth - 24);
        Couple.Engage(sim.Player, friend, sim.AbsoluteMonth - 12);
        Couple.Marry(sim.Player, friend, sim.AbsoluteMonth - 6);

        var born = false;
        for (var i = 0; i < 40 && !born; i++)
            born = sim.Interact(friend, InteractionKind.TryForBaby)
                .Contains("Nasceu");

        Assert.True(born, "O casal deveria conseguir um filho em 40 tentativas");
        var child = Assert.Single(sim.Player.Children);
        Assert.Contains(child, sim.Registry.All);
        Assert.Equal(RelationshipKind.Child,
            sim.Player.Relationships[child.Id].Kind);
    }

    [Fact]
    public void Repeatable_interactions_are_limited_to_once_per_period()
    {
        var (sim, friend) = SimWithFriend();

        sim.Interact(friend, InteractionKind.Talk);
        var again = sim.Interact(friend, InteractionKind.Talk);

        Assert.Contains("já interagiu", again);
    }
}
