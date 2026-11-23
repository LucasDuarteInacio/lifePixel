using LifeSimulator.Core.Activities;
using LifeSimulator.Core.Characters;
using LifeSimulator.Core.Events;
using LifeSimulator.Core.Genealogy;
using LifeSimulator.Core.Generation;
using LifeSimulator.Core.Relationships;
using LifeSimulator.Core.Time;
using LifeSimulator.Core.World;

namespace LifeSimulator.Core.Simulation;

/// <summary>
/// Orquestra o ciclo principal de gameplay (GDD §4.2): consultar a
/// situação, avançar o tempo, atualizar personagens e mundo, apresentar
/// acontecimentos e aplicar consequências — até a morte e a sucessão.
/// </summary>
public sealed class LifeSimulation
{
    private readonly Random _rng;
    private readonly LifeGenerator _generator;
    private readonly RelationshipDirector _director;
    private readonly List<PendingDecision> _pendingDecisions = new();

    /// <summary>Chance por período de um evento mundial (escala mensal).</summary>
    private const double MonthlyGlobalEventChance = 0.015;

    private LifeSimulation(int seed)
    {
        Seed = seed;
        _rng = new Random(seed);
        _generator = new LifeGenerator(new Random(seed ^ 0x5f3759df));
        _director = new RelationshipDirector(_rng, _generator);
        StartYear = DateTime.Now.Year;
    }

    public int Seed { get; }

    /// <summary>Meses decorridos desde o início da partida.</summary>
    public int AbsoluteMonth { get; private set; }

    /// <summary>Ano de calendário em que a dinastia começou.</summary>
    public int StartYear { get; }

    /// <summary>Ano corrente dentro da partida.</summary>
    public int CurrentYear => StartYear + AbsoluteMonth / 12;

    /// <summary>Mês do ano corrente (0–11).</summary>
    public int MonthOfYear => AbsoluteMonth % 12;

    /// <summary>Geração atual da dinastia (1 = fundador).</summary>
    public int Generation { get; private set; } = 1;

    public World.World World { get; } = new();
    public EventLog Log { get; } = new();
    public Person Player { get; private set; } = null!;

    /// <summary>Todas as pessoas conhecidas da partida (GDD §8.5).</summary>
    public FamilyRegistry Registry { get; } = new();
    public bool DynastyEnded { get; private set; }

    /// <summary>Vida aleatória — modo de início padrão do GDD §3.1.</summary>
    public static LifeSimulation NewRandomLife(int? seed = null)
    {
        var sim = new LifeSimulation(seed ?? Environment.TickCount);
        var country = sim.World.PickRandom(sim._rng);
        sim.Player = sim._generator.GenerateNewborn(country);
        sim.Registry.RegisterTree(sim.Player);
        sim.RecordBirth(country);
        return sim;
    }

    /// <summary>
    /// Vida personalizada (GDD §3.1): o jogador fixa nome e país — a
    /// moeda vem da nacionalidade — enquanto família e características
    /// seguem aleatórias.
    /// </summary>
    public static LifeSimulation NewCustomLife(NewLifeRequest request, int? seed = null)
    {
        var sim = new LifeSimulation(seed ?? Environment.TickCount);
        sim.Player = sim._generator.GenerateNewborn(request.Country,
            gender: request.Gender,
            firstName: request.FirstName, lastName: request.LastName);
        sim.Registry.RegisterTree(sim.Player);
        sim.RecordBirth(request.Country);
        return sim;
    }

    private void RecordBirth(Country country)
    {
        var (filiacao, adotado) = Player.Gender switch
        {
            Characters.Gender.Female => ("filha", "adotada"),
            Characters.Gender.NonBinary => ("filhe", "adotade"),
            _ => ("filho", "adotado"),
        };
        var parents = DescribeParents();
        var origin = Player.Origin ?? new BirthOrigin(OriginKind.Natural, null);

        var title = origin.Kind == OriginKind.Adopted ? "Adoção" : "Nascimento";
        var description = origin switch
        {
            { Kind: OriginKind.InVitro } =>
                $"{Player.FullName} nasceu em {country.Name} por fertilização in vitro, " +
                $"{filiacao} de {parents}.",
            { Kind: OriginKind.Adopted } =>
                $"{Player.FullName} foi {adotado} em {country.Name} por {parents}" +
                $"{UnionText(origin.Union)}.",
            { Union: UnionKind.Affair } =>
                $"{Player.FullName} nasceu em {country.Name}, fruto de um affair " +
                $"entre {parents}.",
            { Union: not null } =>
                $"{Player.FullName} nasceu em {country.Name}, {filiacao} de {parents}" +
                $"{UnionText(origin.Union)}.",
            _ =>
                $"{Player.FullName} nasceu em {country.Name}, {filiacao} de {parents}.",
        };

        Log.Record(0, 0, title, description, EventCategory.Life);
    }

    private string DescribeParents() => (Player.Mother, Player.Father) switch
    {
        ({ } m, { } f) => $"{DescribeParent(m)} e {DescribeParent(f)}",
        ({ } m, null) => DescribeParent(m),
        (null, { } f) => DescribeParent(f),
        _ => "uma família desconhecida",
    };

    /// <summary>"Nome, X anos, Cargo" — ou "sem emprego" quando desempregado.</summary>
    private static string DescribeParent(Person parent) =>
        $"{parent.FullName}, {parent.AgeYears} anos, {parent.Job?.Title ?? "sem emprego"}";

    private static string UnionText(UnionKind? union) => union switch
    {
        UnionKind.Married => ", um casal casado",
        UnionKind.Dating => ", um casal de namorados",
        _ => "",
    };

    public int MonthsPerAdvance => TimeStep.MonthsPerAdvance(Player.AgeYears);

    /// <summary>Descendentes vivos elegíveis para continuar a dinastia.</summary>
    public IReadOnlyList<Person> SuccessionCandidates =>
        Player.LivingDescendants().ToList();

    public void AdvancePeriod()
    {
        if (!Player.IsAlive || DynastyEnded)
            return;

        var months = MonthsPerAdvance;
        AbsoluteMonth += months;

        AgeFamily(months);
        Player.Finances.ApplyMonths(months);
        Player.Job?.AdvanceMonths(months);
        ApplyHealthDrift(months);
        _director.Advance(this, months);
        SimulateMortality(months);
        RollWorldEvents(months);
        RollMilestones();
        RollMortality(months);
    }

    /// <summary>
    /// Aplica uma atividade instantânea (GDD §4.3 — enquanto custos e
    /// agenda não são detalhados, atividades não consomem tempo).
    /// Retorna a mensagem de feedback para a interface.
    /// </summary>
    public string ApplyActivity(ActivityKind activity)
    {
        if (!Player.IsAlive || DynastyEnded)
            return "";

        string title;
        string feedback;
        switch (activity)
        {
            case ActivityKind.Study:
                Player.Attributes.Adjust(AttributeKind.Intelligence, 2);
                Player.Attributes.Adjust(AttributeKind.Discipline, 1);
                title = "Estudo";
                feedback = "Você estudou. Inteligência +2.";
                break;
            case ActivityKind.Exercise:
                Player.Attributes.Adjust(AttributeKind.Fitness, 2);
                Player.Health.Physical += 2;
                title = "Exercício";
                feedback = "Você se exercitou. Saúde +2.";
                break;
            case ActivityKind.Socialize:
                Player.Health.Mental += 2;
                Player.Attributes.Adjust(AttributeKind.Communication, 1);
                title = "Vida social";
                feedback = Socialize();
                break;
            case ActivityKind.Rest:
                Player.Attributes.Adjust(AttributeKind.Fitness, 3);
                Player.Health.Mental += 1;
                title = "Descanso";
                feedback = "Você descansou. Energia recuperada.";
                break;
            default:
                return "";
        }

        Log.Record(AbsoluteMonth, Player.AgeYears, title, feedback, EventCategory.Life);
        return feedback;
    }

    /// <summary>Sucessão (GDD §3.3): qualquer descendente vivo pode assumir.</summary>
    public void Succeed(Person heir)
    {
        if (!heir.IsAlive)
            throw new InvalidOperationException("O sucessor precisa estar vivo.");
        Player = heir;
        Registry.RegisterTree(heir);
        Generation++;
        Log.Record(AbsoluteMonth, heir.AgeYears, "Nova geração",
            $"{heir.FullName} assume como protagonista da dinastia, aos {heir.AgeYears} anos.",
            EventCategory.Life);
    }

    // ---------------------------------------------------------------
    // Relações: árvore sob demanda, vínculos, interações e decisões
    // ---------------------------------------------------------------

    /// <summary>Materializa a família estendida do protagonista (GDD §7.1).</summary>
    public void EnsureExtendedFamily()
    {
        _generator.EnsureExtendedFamily(Player, AbsoluteMonth);
        Registry.RegisterTree(Player);
    }

    /// <summary>Vínculo do jogador com a pessoa (criado sob demanda).</summary>
    public Relationship BondWith(Person other) =>
        Player.RelationshipTo(other, Kinship.KindFor(Player, other));

    public IReadOnlyList<InteractionKind> AvailableInteractions(Person other) =>
        Interactions.Available(Player, other, BondWith(other), AbsoluteMonth);

    /// <summary>
    /// Aplica uma interação do jogador sobre uma pessoa conhecida.
    /// Retorna o feedback para a interface.
    /// </summary>
    public string Interact(Person other, InteractionKind kind)
    {
        if (!Player.IsAlive || DynastyEnded)
            return "";
        var rel = BondWith(other);
        if (!Interactions.Available(Player, other, rel, AbsoluteMonth).Contains(kind))
            return "Indisponível no momento.";

        var repeatable = kind is InteractionKind.Talk or InteractionKind.SpendTime
            or InteractionKind.Compliment or InteractionKind.Gift
            or InteractionKind.Argue or InteractionKind.Apologize
            or InteractionKind.Flirt;
        if (repeatable && rel.LastContactMonth == AbsoluteMonth)
            return $"Você já interagiu com {other.FirstName} neste período. Avance o tempo.";

        var mutual = other.RelationshipTo(Player);
        if (repeatable)
            rel.LastContactMonth = mutual.LastContactMonth = AbsoluteMonth;

        var text = kind switch
        {
            InteractionKind.Talk => ApplyTalk(rel, mutual),
            InteractionKind.SpendTime => ApplySpendTime(rel, mutual),
            InteractionKind.Compliment => ApplyCompliment(other, rel, mutual),
            InteractionKind.Gift => ApplyGift(other, rel, mutual),
            InteractionKind.Argue => ApplyArgue(other, rel, mutual),
            InteractionKind.Apologize => ApplyApologize(other, rel, mutual),
            InteractionKind.Flirt => ApplyFlirt(other, rel, mutual),
            InteractionKind.AskOut => ApplyAskOut(other, rel, mutual),
            InteractionKind.Propose => ApplyPropose(other, rel, mutual),
            InteractionKind.Marry => ApplyMarry(other),
            InteractionKind.BreakUp => ApplyBreakUp(other, rel),
            InteractionKind.TryForBaby => ApplyTryForBaby(other),
            _ => "",
        };
        return text;
    }

    private string ApplyTalk(Relationship rel, Relationship mutual)
    {
        rel.Friendship += 3; rel.Love += 1; rel.Trust += 1;
        mutual.Friendship += 2; mutual.Love += 1;
        return "Vocês conversaram por um tempo.";
    }

    private string ApplySpendTime(Relationship rel, Relationship mutual)
    {
        rel.Friendship += 5; rel.Love += 2;
        if (RelationshipRules.IsRomantic(rel.Kind)) rel.Intimacy += 3;
        mutual.Friendship += 3; mutual.Love += 2;
        return "Vocês passaram um tempo juntos.";
    }

    private string ApplyCompliment(Person other, Relationship rel, Relationship mutual)
    {
        rel.Friendship += 4; rel.Respect += 2; rel.Love += 2;
        mutual.Friendship += 4; mutual.Love += 2;
        mutual.Record(AbsoluteMonth, $"{Player.FirstName} me elogiou", 10);
        return $"Você elogiou {other.FirstName}.";
    }

    private string ApplyGift(Person other, Relationship rel, Relationship mutual)
    {
        var cost = Interactions.GiftCost(Player);
        Player.Finances.Money -= cost;
        var bonus = other.Traits.Contains(PersonalityTrait.Greedy) ? 4 : 0;
        rel.Friendship += 6 + bonus; rel.Love += 4 + bonus;
        mutual.Friendship += 4; mutual.Love += 3;
        mutual.Record(AbsoluteMonth, $"Recebeu um presente de {Player.FirstName}", 15);
        var country = Player.Residence ?? Player.BirthCountry;
        return $"Você deu um presente a {other.FirstName} ({country?.FormatMoney(cost)}).";
    }

    private string ApplyArgue(Person other, Relationship rel, Relationship mutual)
    {
        var damage = 6 + _rng.Next(6)
            + (other.Traits.Contains(PersonalityTrait.HotTempered) ? 4 : 0);
        rel.Friendship -= damage; rel.Trust -= damage / 2; rel.Love -= damage / 2;
        rel.Rivalry += 10;
        mutual.Friendship -= damage; mutual.Trust -= damage / 2;
        mutual.Record(AbsoluteMonth, $"{Player.FirstName} discutiu comigo", -damage);
        return $"Vocês discutiram. A relação esfriou.";
    }

    private string ApplyApologize(Person other, Relationship rel, Relationship mutual)
    {
        rel.Trust += 8; rel.Friendship += 5; rel.Rivalry -= 15;
        mutual.Trust += 8; mutual.Rivalry -= 15;
        rel.Record(AbsoluteMonth, "Reconciliação", 20);
        mutual.Record(AbsoluteMonth, $"{Player.FirstName} pediu desculpas", 20);
        if (rel.Kind == RelationshipKind.Rival && rel.Rivalry < 20)
        {
            rel.Kind = mutual.Kind = RelationshipKind.Friend;
            return $"Vocês fizeram as pazes — {other.FirstName} deixou de ser seu rival.";
        }
        return $"Você pediu desculpas a {other.FirstName}.";
    }

    private string ApplyFlirt(Person other, Relationship rel, Relationship mutual)
    {
        rel.Love += 6; rel.Intimacy += 3; rel.Friendship += 2;
        mutual.Love += 4;
        mutual.Record(AbsoluteMonth, $"{Player.FirstName} flertou comigo", 15);
        return $"Você flertou com {other.FirstName}.";
    }

    private string ApplyAskOut(Person other, Relationship rel, Relationship mutual)
    {
        if (mutual.Love >= RelationshipRules.AskOutMinLove)
        {
            Couple.StartDating(Player, other, AbsoluteMonth);
            Log.Record(AbsoluteMonth, Player.AgeYears, "Namoro",
                $"Você e {other.FullName} começaram a namorar.",
                EventCategory.Relationship);
            return $"{other.FirstName} aceitou! Vocês estão namorando.";
        }
        rel.Friendship -= 8;
        mutual.Love -= 10;
        mutual.Record(AbsoluteMonth, "Recusou o pedido de namoro", -25);
        return $"{other.FirstName} não correspondeu ao pedido.";
    }

    private string ApplyPropose(Person other, Relationship rel, Relationship mutual)
    {
        var acceptP = Math.Min(0.95,
            0.5 + mutual.Love / 200.0 + mutual.Trust / 300.0);
        if (_rng.NextDouble() < acceptP)
        {
            Couple.Engage(Player, other, AbsoluteMonth);
            Log.Record(AbsoluteMonth, Player.AgeYears, "Noivado",
                $"{other.FullName} aceitou seu pedido de casamento!",
                EventCategory.Relationship);
            return $"{other.FirstName} aceitou! Vocês estão noivos.";
        }
        rel.Love -= 10;
        mutual.Trust -= 8;
        mutual.Record(AbsoluteMonth, "Pedido de casamento recusado", -40);
        return $"{other.FirstName} disse que ainda não é o momento.";
    }

    private string ApplyMarry(Person other)
    {
        Couple.Marry(Player, other, AbsoluteMonth);
        Player.Remember(AbsoluteMonth, $"Casamento com {other.FirstName}", 80);
        other.Remember(AbsoluteMonth, $"Casamento com {Player.FirstName}", 80);
        Log.Record(AbsoluteMonth, Player.AgeYears, "Casamento",
            $"Você e {other.FullName} se casaram.",
            EventCategory.Relationship);
        return $"Vocês se casaram!";
    }

    private string ApplyBreakUp(Person other, Relationship rel)
    {
        var wasMarried = Player.Status == MaritalStatus.Married;
        Couple.BreakUp(Player, other, AbsoluteMonth);
        Player.Remember(AbsoluteMonth,
            wasMarried ? $"Divórcio de {other.FirstName}" : $"Término com {other.FirstName}", -60);
        Log.Record(AbsoluteMonth, Player.AgeYears,
            wasMarried ? "Divórcio" : "Término",
            wasMarried
                ? $"Você e {other.FullName} se divorciaram."
                : $"Você e {other.FullName} terminaram.",
            EventCategory.Relationship);
        return wasMarried ? "Vocês se divorciaram." : "Vocês terminaram.";
    }

    private string ApplyTryForBaby(Person other)
    {
        if (_rng.NextDouble() >= 0.35)
            return "Ainda não foi dessa vez. Tentem novamente.";

        var mother = Interactions.MotherOf(Player, other)!;
        var father = mother == Player ? other : Player;
        var child = _generator.GenerateChildOf(
            mother, father, Player.Residence ?? Player.BirthCountry!, 0);
        Registry.Register(child);
        Player.Remember(AbsoluteMonth, $"Nascimento de {child.FirstName}", 70);
        Log.Record(AbsoluteMonth, Player.AgeYears, "Novo membro da família",
            $"Nasceu {child.FullName}, filho(a) de você e {other.FirstName}.",
            EventCategory.Family);
        return $"Nasceu {child.FullName}!";
    }

    /// <summary>
    /// Socializar (GDD §4.3): descansa a mente e pode render uma nova
    /// amizade — colega de escola ou de trabalho.
    /// </summary>
    private string Socialize()
    {
        var friends = Player.Relationships.Values
            .Where(r => r.Kind == RelationshipKind.Friend)
            .Select(r => Registry.Find(r.OtherId))
            .Where(p => p is { IsAlive: true })
            .Cast<Person>()
            .ToList();

        if (_rng.NextDouble() < 0.35)
        {
            var country = Player.Residence ?? Player.BirthCountry!;
            var peer = _generator.GeneratePeer(country,
                Math.Max(4, Player.AgeYears + _rng.Next(-3, 4)));
            var rel = Player.RelationshipTo(peer, RelationshipKind.Friend);
            rel.Friendship = 30 + _rng.Next(26);
            rel.SinceMonth = rel.LastContactMonth = AbsoluteMonth;
            var bond = peer.RelationshipTo(Player, RelationshipKind.Friend);
            bond.Friendship = rel.Friendship - _rng.Next(10);
            bond.SinceMonth = bond.LastContactMonth = AbsoluteMonth;
            Registry.Register(peer);
            return $"Você conheceu {peer.FullName} e fizeram amizade.";
        }

        if (friends.Count > 0)
        {
            var friend = friends[_rng.Next(friends.Count)];
            var rel = BondWith(friend);
            rel.Friendship += 4;
            rel.LastContactMonth = AbsoluteMonth;
            return $"Você saiu com {friend.FirstName}. Amizade +4.";
        }

        return "Você socializou. Felicidade +2.";
    }

    // ---------------------------------------------------------------
    // Decisões pendentes (GDD §9 — escolhas do jogador)
    // ---------------------------------------------------------------

    internal void EnqueueDecision(PendingDecision decision) =>
        _pendingDecisions.Add(decision);

    internal bool HasPendingDecisionAbout(Guid personId) =>
        _pendingDecisions.Any(d => d.AboutPersonId == personId);

    /// <summary>Próxima decisão aguardando o jogador, ou null.</summary>
    public PendingDecision? PeekDecision() =>
        _pendingDecisions.Count > 0 ? _pendingDecisions[0] : null;

    /// <summary>Aplica a opção escolhida e registra o desfecho no feed.</summary>
    public string ResolveDecision(PendingDecision decision, int optionIndex)
    {
        _pendingDecisions.Remove(decision);
        var outcome = decision.Resolve(optionIndex);
        Log.Record(AbsoluteMonth, Player.AgeYears, decision.Title,
            outcome, decision.Category);
        return outcome;
    }

    private void AgeFamily(int months)
    {
        Player.AgeMonths += months;
        foreach (var person in Registry.All)
            if (person.IsAlive && person != Player)
                person.AgeMonths += months;
    }

    private void ApplyHealthDrift(int months)
    {
        // Registry.All já inclui o jogador.
        foreach (var person in Registry.All)
            if (person.IsAlive)
                person.Health.Drift(months,
                    person.Attributes[AttributeKind.Fitness],
                    person.Genome.ImmuneSystem);
    }

    /// <summary>
    /// Mortalidade dos NPCs registrados (GDD §8): o luto é narrado com o
    /// parentesco ("Sua avó…"), e o cônjuge sobrevivente fica viúvo.
    /// </summary>
    private void SimulateMortality(int months)
    {
        foreach (var person in Registry.All)
        {
            if (!person.IsAlive || person == Player)
                continue;

            var risk = Mortality.PeriodRisk(months, person.AgeYears,
                person.Health.Physical, person.Genome.Longevity);
            if (_rng.NextDouble() >= risk)
                continue;

            person.Die(AbsoluteMonth,
                Mortality.PickCause(_rng, person.AgeYears, person.Health.Physical));

            if (person.Partner is { IsAlive: true } spouse)
            {
                Couple.Widow(spouse, AbsoluteMonth);
                if (spouse == Player)
                    Log.Record(AbsoluteMonth, Player.AgeYears, "Luto",
                        $"Seu(sua) companheiro(a) {person.FirstName} morreu de " +
                        $"{person.CauseOfDeath}. Você está viúvo(a).",
                        EventCategory.Relationship);
            }

            if (Kinship.IsKin(Player, person)
                || Player.Relationships.ContainsKey(person.Id))
            {
                var bond = Player.Relationships.GetValueOrDefault(person.Id);
                var closeness = bond?.Love ?? 50;
                var subject = char.ToUpper(KinLabel.Label(Player, person)[0])
                    + KinLabel.Label(Player, person)[1..];
                Log.Record(AbsoluteMonth, Player.AgeYears, "Luto",
                    $"{subject} {person.FirstName} morreu de {person.CauseOfDeath}, " +
                    $"aos {person.AgeYears} anos.",
                    EventCategory.Family);
                Player.Remember(AbsoluteMonth, $"Morte de {person.FirstName}",
                    -(30 + closeness / 2));
            }
        }
    }

    private void RollWorldEvents(int months)
    {
        var chance = 1.0 - Math.Pow(1.0 - MonthlyGlobalEventChance, months);
        var worldEvent = World.RollGlobalEvent(_rng, chance);
        if (worldEvent is not null)
            Log.Record(AbsoluteMonth, Player.AgeYears, worldEvent.Value.Title,
                worldEvent.Value.Description, EventCategory.World);
    }

    /// <summary>Marcos de vida simples por fase (a expandir com eventos reais).</summary>
    private void RollMilestones()
    {
        var age = Player.AgeYears;
        if (age >= 6 && !Player.Memory.Any(m => m.Text.Contains("escola")))
        {
            Log.Record(AbsoluteMonth, age, "Primeiro dia de aula",
                $"{Player.FirstName} começou a estudar na escola.", EventCategory.Education);
            Player.Remember(AbsoluteMonth, "Começou a escola", 30);
        }
    }

    private void RollMortality(int months)
    {
        var risk = Mortality.PeriodRisk(months, Player.AgeYears,
            Player.Health.Physical, Player.Genome.Longevity);
        if (_rng.NextDouble() >= risk)
            return;

        var cause = Mortality.PickCause(_rng, Player.AgeYears, Player.Health.Physical);
        Player.Die(AbsoluteMonth, cause);
        if (Player.Partner is { IsAlive: true } spouse)
            Couple.Widow(spouse, AbsoluteMonth);
        Log.Record(AbsoluteMonth, Player.AgeYears, "Falecimento",
            $"{Player.FullName} morreu de {cause}, aos {Player.AgeYears} anos.",
            EventCategory.Life);

        if (SuccessionCandidates.Count == 0)
        {
            DynastyEnded = true;
            Log.Record(AbsoluteMonth, Player.AgeYears, "Fim da dinastia",
                "Sem descendentes vivos, a história desta família chegou ao fim. " +
                "O histórico permanece disponível no feed.",
                EventCategory.Life);
        }
    }
}
