using LifeSimulator.Core.Characters;
using LifeSimulator.Core.Events;
using LifeSimulator.Core.Genealogy;
using LifeSimulator.Core.Generation;
using LifeSimulator.Core.Simulation;

namespace LifeSimulator.Core.Relationships;

/// <summary>
/// A "IA" que move as relações do mundo em background (GDD §5.1, §8):
/// a cada avanço de tempo ela deriva os indicadores de todos os
/// vínculos registrados — inclusive NPC↔NPC —, resolve afetos e
/// conflitos de casais, forma novos casais entre solteiros, gera
/// filhos e toma iniciativas com o jogador (pedido de namoro ou de
/// casamento viram <see cref="PendingDecision"/>).
/// </summary>
public sealed class RelationshipDirector(Random rng, LifeGenerator generator)
{
    public void Advance(LifeSimulation sim, int months)
    {
        var month = sim.AbsoluteMonth;
        DriftAll(sim, months, month);

        foreach (var person in sim.Registry.All.ToList())
        {
            if (!person.IsAlive)
                continue;

            if (person.Partner is { IsAlive: true } partner
                && string.CompareOrdinal(
                    person.Id.ToString(), partner.Id.ToString()) < 0)
            {
                CoupleStep(sim, person, partner, months, month);
            }
            else if (person != sim.Player && person.Partner is null)
            {
                SingleStep(sim, person, months, month);
            }

            if (person != sim.Player)
                Initiative(sim, person, months, month);
        }
    }

    // ---------------------------------------------------------------
    // Deriva: afastamento esfria os vínculos (GDD §4.2 — "atualizar
    // relações"), com chão para laços de família.
    // ---------------------------------------------------------------

    private void DriftAll(LifeSimulation sim, int months, int month)
    {
        foreach (var person in sim.Registry.All)
        {
            if (!person.IsAlive)
                continue;
            foreach (var rel in person.Relationships.Values)
            {
                if (sim.Registry.Find(rel.OtherId) is not { IsAlive: true })
                    continue;

                var family = RelationshipRules.IsFamily(rel.Kind);
                var romantic = rel.Kind is RelationshipKind.Partner
                    or RelationshipKind.Fiance;
                var distant = !romantic && rel.LastContactMonth is { } last
                    && month - last > 18;

                var factor = (family ? 0.5 : 1.0)
                    * (distant ? 2.0 : 1.0)
                    * (romantic ? 0.6 : 1.0);
                var floor = rel.Kind switch
                {
                    _ when family && rel.Kind != RelationshipKind.Rival => 40,
                    RelationshipKind.Friend => 10,
                    _ => 0,
                };

                Decay(rel, RelationshipIndicator.Love, 0.35 * factor * months, floor);
                Decay(rel, RelationshipIndicator.Intimacy,
                    (romantic ? 0.3 : 0.6) * factor * months, 0);
                Decay(rel, RelationshipIndicator.Friendship,
                    0.25 * factor * months, floor);
                Decay(rel, RelationshipIndicator.Trust, 0.06 * months, 0);
                Decay(rel, RelationshipIndicator.Respect, 0.04 * months, 0);
                Decay(rel, RelationshipIndicator.Rivalry, 0.3 * months, 0);
            }
        }
    }

    private void Decay(Relationship rel, RelationshipIndicator indicator,
        double amount, int floor)
    {
        var whole = (int)amount;
        if (rng.NextDouble() < amount - whole)
            whole++;
        if (whole <= 0 || rel.Get(indicator) <= floor)
            return;
        var value = Math.Max(floor, rel.Get(indicator) - whole);
        rel.Adjust(indicator, value - rel.Get(indicator));
    }

    // ---------------------------------------------------------------
    // Casais: afetos, conflitos, traições, progressão, filhos, término.
    // ---------------------------------------------------------------

    private void CoupleStep(LifeSimulation sim, Person a, Person b,
        int months, int month)
    {
        var relAB = a.Relationships.GetValueOrDefault(b.Id);
        var relBA = b.Relationships.GetValueOrDefault(a.Id);
        if (relAB is null || relBA is null)
            return;

        var involvesPlayer = a == sim.Player || b == sim.Player;
        var avgLove = (relAB.Love + relBA.Love) / 2.0;

        if (rng.NextDouble() < 0.08 * months)
        {
            relAB.Love += 3; relAB.Intimacy += 3;
            relBA.Love += 3; relBA.Intimacy += 3;
        }

        var conflictP = 0.02;
        if (a.Traits.Contains(PersonalityTrait.Impulsive)
            || b.Traits.Contains(PersonalityTrait.Impulsive)
            || a.Traits.Contains(PersonalityTrait.HotTempered)
            || b.Traits.Contains(PersonalityTrait.HotTempered))
            conflictP += 0.015;
        if (relAB.Trust < 30 || relBA.Trust < 30)
            conflictP += 0.02;
        if (rng.NextDouble() < conflictP * months)
        {
            var damage = 5 + rng.Next(10);
            relAB.Love -= damage; relAB.Trust -= damage / 2;
            relBA.Love -= damage; relBA.Trust -= damage / 2;
            relAB.Record(month, "Briga", -damage);
            relBA.Record(month, "Briga", -damage);
            if (involvesPlayer && damage >= 10)
                sim.Log.Record(month, sim.Player.AgeYears, "Discussão",
                    $"Você e {PartnerName(sim, a, b)} brigaram feio.",
                    EventCategory.Relationship);
        }

        TryBetrayal(sim, a, b, relAB, relBA, months, month);
        TryBetrayal(sim, b, a, relBA, relAB, months, month);

        var avgAfter = (relAB.Love + relBA.Love) / 2.0;
        if (avgAfter < 18 && rng.NextDouble() < 0.15 * months)
        {
            var wasMarried = a.Status == MaritalStatus.Married;
            Couple.BreakUp(a, b, month);
            if (involvesPlayer)
                sim.Log.Record(month, sim.Player.AgeYears,
                    wasMarried ? "Divórcio" : "Término",
                    wasMarried
                        ? "O casamento chegou ao fim."
                        : "O relacionamento chegou ao fim.",
                    EventCategory.Relationship);
            else if (CloseTo(sim, a))
                sim.Log.Record(month, sim.Player.AgeYears,
                    wasMarried ? "Divórcio" : "Término",
                    $"{Subject(sim, a)} se separou" +
                        $"{(b.Gender == Gender.Female ? " da parceira" : " do parceiro")}.",
                    EventCategory.Family);
            return;
        }

        if (!involvesPlayer)
            ProgressCouple(sim, a, b, relAB, relBA, avgAfter, months, month);

        TryConception(sim, a, b, months, month);
    }

    /// <summary>Traição: impulsivo e desleal com amor baixo (GDD §5.1).</summary>
    private void TryBetrayal(LifeSimulation sim, Person cheater, Person betrayed,
        Relationship relCheater, Relationship relBetrayed, int months, int month)
    {
        if (!cheater.Traits.Contains(PersonalityTrait.Impulsive)
            || cheater.Traits.Contains(PersonalityTrait.Loyal)
            || relCheater.Love >= 45
            || rng.NextDouble() >= 0.004 * months)
            return;

        relCheater.Record(month, "Traiu o parceiro", -30);
        relBetrayed.Record(month, "Descobriu uma traição", -70);
        relBetrayed.Love -= 30;
        relBetrayed.Trust -= 40;
        relBetrayed.Intimacy -= 20;

        if (betrayed == sim.Player && !sim.HasPendingDecisionAbout(cheater.Id))
        {
            var monthCopy = month;
            sim.EnqueueDecision(new PendingDecision
            {
                Title = "Traição",
                Description = $"Você descobriu que {cheater.FirstName} te traiu.",
                Category = EventCategory.Relationship,
                AboutPersonId = cheater.Id,
                Options = new DecisionOption[]
                {
                    new("Perdoar", () =>
                    {
                        var bond = sim.Player.RelationshipTo(cheater);
                        bond.Trust -= 10;
                        bond.Record(monthCopy, "Perdoou uma traição", -10);
                        return "Você decidiu perdoar. A confiança ficou abalada.";
                    }),
                    new("Terminar", () =>
                    {
                        var wasMarried = sim.Player.Status == MaritalStatus.Married;
                        Couple.BreakUp(sim.Player, cheater, monthCopy);
                        return wasMarried
                            ? "Você pediu o divórcio." : "Você terminou o relacionamento.";
                    }),
                },
            });
        }
    }

    /// <summary>Namoro → noivado → casamento entre NPCs (GDD §8.1).</summary>
    private void ProgressCouple(LifeSimulation sim, Person a, Person b,
        Relationship relAB, Relationship relBA, double avgLove,
        int months, int month)
    {
        if (a.Status == MaritalStatus.Dating && avgLove >= 60
            && Interactions.MonthsTogether(relAB, month)
                >= RelationshipRules.MinDatingMonthsToPropose
            && rng.NextDouble() < 0.10 * months)
        {
            Couple.Engage(a, b, month);
            if (CloseTo(sim, a) || CloseTo(sim, b))
                sim.Log.Record(month, sim.Player.AgeYears, "Noivado",
                    $"{Subject(sim, a)} ficou noivo(a) de {b.FirstName}.",
                    EventCategory.Family);
        }
        else if (a.Status == MaritalStatus.Engaged
            && Interactions.MonthsTogether(relAB, month)
                >= RelationshipRules.MinEngagementMonthsToMarry
            && rng.NextDouble() < 0.15 * months)
        {
            Couple.Marry(a, b, month);
            if (CloseTo(sim, a) || CloseTo(sim, b))
                sim.Log.Record(month, sim.Player.AgeYears, "Casamento",
                    $"{Subject(sim, a)} se casou com {b.FirstName}.",
                    EventCategory.Family);
        }
    }

    /// <summary>
    /// Concepção: casamentos geram filhos com chance mensal; noivados,
    /// raramente. Inclui o casal do jogador (casado).
    /// </summary>
    private void TryConception(LifeSimulation sim, Person a, Person b,
        int months, int month)
    {
        var rate = a.Status switch
        {
            MaritalStatus.Married => 0.015,
            MaritalStatus.Engaged => 0.004,
            _ => 0.0,
        };
        if (rate <= 0 || rng.NextDouble() >= rate * months)
            return;
        if (Interactions.MotherOf(a, b) is not { } mother
            || mother.AgeYears is < 18 or > 42
            || a.Children.Count + b.Children.Count >= 5)
            return;

        var father = mother == a ? b : a;
        var country = mother.Residence ?? sim.Player.Residence!;
        var child = generator.GenerateChildOf(mother, father, country, 0);
        sim.Registry.Register(child);

        var involvesPlayer = a == sim.Player || b == sim.Player;
        if (involvesPlayer)
            sim.Log.Record(month, sim.Player.AgeYears, "Novo membro da família",
                $"Nasceu {child.FullName}, filho(a) de {a.FirstName} e {b.FirstName}.",
                EventCategory.Family);
        else if (CloseTo(sim, a) || CloseTo(sim, b))
            sim.Log.Record(month, sim.Player.AgeYears, "Novo membro da família",
                $"{Subject(sim, ClosestTo(sim, a, b))} teve {(
                    child.Gender == Gender.Female ? "uma filha" : "um filho")}, {child.FirstName}.",
                EventCategory.Family);
    }

    // ---------------------------------------------------------------
    // Solteiros NPC encontram parceiros (GDD §8.1).
    // ---------------------------------------------------------------

    private void SingleStep(LifeSimulation sim, Person person, int months, int month)
    {
        if (person.AgeYears is < 16 or > 85)
            return;

        var p = 0.005;
        if (person.AgeYears is >= 20 and <= 45) p *= 2.0;
        else if (person.AgeYears >= 55) p *= 0.4;
        if (person.Traits.Contains(PersonalityTrait.Introverted)) p *= 0.6;
        if (person.Traits.Contains(PersonalityTrait.Extroverted)) p *= 1.5;
        if (person.Traits.Contains(PersonalityTrait.Loyal)
            && person.Status == MaritalStatus.Widowed) p *= 0.4;
        if (rng.NextDouble() >= p * months)
            return;

        var country = person.Residence ?? sim.Player.Residence!;
        var partner = generator.GenerateAdult(country, CompatibleGender(person),
            Math.Max(16, person.AgeYears + rng.Next(-6, 7)));
        Couple.StartDating(person, partner, month);
        sim.Registry.Register(partner);

        if (CloseTo(sim, person))
            sim.Log.Record(month, sim.Player.AgeYears, "Novo relacionamento",
                $"{Subject(sim, person)} começou a namorar {partner.FirstName}.",
                EventCategory.Family);
    }

    // ---------------------------------------------------------------
    // Iniciativas com o jogador: pedidos, marco de melhor amigo,
    // provocações de rivais.
    // ---------------------------------------------------------------

    private void Initiative(LifeSimulation sim, Person npc, int months, int month)
    {
        var player = sim.Player;
        if (!player.IsAlive)
            return;
        var bondNP = npc.Relationships.GetValueOrDefault(player.Id);
        var bondPN = player.Relationships.GetValueOrDefault(npc.Id);
        if (bondNP is null || bondPN is null)
            return;

        // Pedido de casamento ao jogador (GDD §5.1 — "ser pedido").
        if (npc == player.Partner && player.Status == MaritalStatus.Dating
            && bondNP.Love >= RelationshipRules.ProposeMinLove
            && Interactions.MonthsTogether(bondPN, month)
                >= RelationshipRules.MinDatingMonthsToPropose
            && rng.NextDouble() < 0.25 * months
            && !sim.HasPendingDecisionAbout(npc.Id))
        {
            var monthCopy = month;
            sim.EnqueueDecision(new PendingDecision
            {
                Title = "Pedido de casamento",
                Description = $"{npc.FirstName} se ajoelhou e te pediu em casamento!",
                Category = EventCategory.Relationship,
                AboutPersonId = npc.Id,
                Options = new DecisionOption[]
                {
                    new("Aceitar", () =>
                    {
                        Couple.Engage(player, npc, monthCopy);
                        return $"Vocês estão noivos! {npc.FirstName} não cabe em si de felicidade.";
                    }),
                    new("Recusar", () =>
                    {
                        bondPN.Love -= 10;
                        bondNP.Trust -= 8;
                        bondNP.Record(monthCopy, "Teve o pedido de casamento recusado", -40);
                        return "Você recusou o pedido. O clima pesou entre vocês.";
                    }),
                },
            });
        }
        // Amigo(a) com sentimentos pede em namoro.
        else if (bondNP.Kind == RelationshipKind.Friend
            && bondNP.Love >= 45 && bondPN.Love >= 30
            && player.Status == MaritalStatus.Single && player.Partner is null
            && npc.AgeYears >= 13 && player.AgeYears >= 13
            && rng.NextDouble() < 0.06 * months
            && !sim.HasPendingDecisionAbout(npc.Id))
        {
            var monthCopy = month;
            sim.EnqueueDecision(new PendingDecision
            {
                Title = "Pedido de namoro",
                Description = $"{npc.FirstName} confessou sentimentos e te pediu em namoro.",
                Category = EventCategory.Relationship,
                AboutPersonId = npc.Id,
                Options = new DecisionOption[]
                {
                    new("Aceitar", () =>
                    {
                        Couple.StartDating(player, npc, monthCopy);
                        return $"Vocês começaram a namorar.";
                    }),
                    new("Recusar", () =>
                    {
                        bondNP.Friendship -= 10;
                        bondNP.Love -= 15;
                        bondNP.Record(monthCopy, "Teve o pedido de namoro recusado", -30);
                        return "Você recusou, mas tentou preservar a amizade.";
                    }),
                },
            });
        }

        // Marco de melhor amigo (GDD §7.3 — histórico da relação).
        if (bondPN.Kind == RelationshipKind.Friend
            && bondPN.Friendship >= RelationshipRules.BestFriendThreshold
            && !bondPN.History.Any(h => h.Text.Contains("melhor amig")))
        {
            bondPN.Record(month, $"Virou melhor amigo(a) de {npc.FirstName}", 30);
            bondNP.Record(month, $"Virou melhor amigo(a) de {player.FirstName}", 30);
            sim.Log.Record(month, player.AgeYears, "Amizade",
                $"{npc.FirstName} agora é seu(sua) melhor amigo(a).",
                EventCategory.Relationship);
        }

        // Provocação de rival.
        if (bondNP.Kind == RelationshipKind.Rival && bondNP.Rivalry >= 40
            && rng.NextDouble() < 0.03 * months)
        {
            bondPN.Friendship -= 5;
            bondPN.Record(month, $"{npc.FirstName} provocou você", -15);
            bondNP.Rivalry += 5;
            sim.Log.Record(month, player.AgeYears, "Rivalidade",
                $"{npc.FirstName} provocou você em público.",
                EventCategory.Relationship);
        }
    }

    // ---------------------------------------------------------------
    // Auxiliares
    // ---------------------------------------------------------------

    private static bool CloseTo(LifeSimulation sim, Person person) =>
        person == sim.Player
        || Kinship.IsKin(sim.Player, person)
        || sim.Player.Relationships.ContainsKey(person.Id);

    private static Person ClosestTo(LifeSimulation sim, Person a, Person b) =>
        CloseTo(sim, a) ? a : b;

    /// <summary>"Irmã Ana", "Avô Pedro" — sujeito com parentesco.</summary>
    private static string Subject(LifeSimulation sim, Person person)
    {
        var label = KinLabel.Label(sim.Player, person);
        return $"{char.ToUpper(label[0])}{label[1..]} {person.FirstName}";
    }

    private static string PartnerName(LifeSimulation sim, Person a, Person b) =>
        a == sim.Player ? b.FirstName : a.FirstName;

    private Gender CompatibleGender(Person person) => person.Gender switch
    {
        Gender.Female => Gender.Male,
        Gender.Male => Gender.Female,
        _ => (Gender)rng.Next(2),
    };
}
