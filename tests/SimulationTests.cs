using LifeSimulator.Core.Careers;
using LifeSimulator.Core.Characters;
using LifeSimulator.Core.Economy;
using LifeSimulator.Core.Generation;
using LifeSimulator.Core.Relationships;
using LifeSimulator.Core.Simulation;
using LifeSimulator.Core.Time;
using LifeSimulator.Core.World;

namespace LifeSimulator.Tests;

public class TimeStepTests
{
    [Theory]
    [InlineData(0, 12)]
    [InlineData(11, 12)]
    [InlineData(12, 6)]
    [InlineData(17, 6)]
    [InlineData(18, 1)]
    [InlineData(65, 1)]
    public void MonthsPerAdvance_follows_life_phase(int ageYears, int expectedMonths)
    {
        Assert.Equal(expectedMonths, TimeStep.MonthsPerAdvance(ageYears));
    }
}

public class MortalityTests
{
    [Fact]
    public void Risk_increases_with_age()
    {
        var young = Mortality.AnnualRisk(25, physicalHealth: 80, longevity: 50);
        var old = Mortality.AnnualRisk(85, physicalHealth: 80, longevity: 50);
        Assert.True(old > young);
    }

    [Fact]
    public void Risk_decreases_with_health()
    {
        var healthy = Mortality.AnnualRisk(50, physicalHealth: 95, longevity: 50);
        var frail = Mortality.AnnualRisk(50, physicalHealth: 10, longevity: 50);
        Assert.True(frail > healthy);
    }

    [Fact]
    public void PeriodRisk_grows_with_period_length()
    {
        var monthly = Mortality.PeriodRisk(1, 30, 80, 50);
        var yearly = Mortality.PeriodRisk(12, 30, 80, 50);
        Assert.True(yearly > monthly);
    }
}

public class RelationshipTests
{
    [Fact]
    public void Indicators_are_independent_and_clamped()
    {
        var rel = new Relationship(Guid.NewGuid(), RelationshipKind.Partner)
        {
            Love = 100,
            Trust = -20,
        };

        Assert.Equal(100, rel.Love);
        Assert.Equal(0, rel.Trust);
        Assert.Equal(0, rel.Friendship);
    }

    [Fact]
    public void History_records_memories()
    {
        var rel = new Relationship(Guid.NewGuid(), RelationshipKind.Friend);
        rel.Record(42, "Me emprestou dinheiro", 30);

        Assert.Single(rel.History);
        Assert.Equal("Me emprestou dinheiro", rel.History[0].Text);
    }
}

public class WorldTests
{
    [Fact]
    public void Countries_load_from_embedded_json()
    {
        var world = new World();

        Assert.True(world.Countries.Count >= 50);
        Assert.Equal(world.Countries.Count, world.Countries.Select(c => c.Code).Distinct().Count());
        Assert.Equal(3, Currency.All.Count);
        Assert.All(world.Countries, c =>
            Assert.Contains(c.CurrencyCode, new[] { "USD", "EUR", "BRL" }));
        Assert.All(world.Countries, c =>
        {
            Assert.False(string.IsNullOrWhiteSpace(c.Name));
            Assert.NotEmpty(c.MaleNames);
            Assert.NotEmpty(c.FemaleNames);
            Assert.NotEmpty(c.Surnames);
        });
    }
}

public class CareerTests
{
    [Fact]
    public void Careers_load_from_embedded_json_with_ten_levels()
    {
        Assert.Equal(5, CareerCatalog.All.Count);
        Assert.All(CareerCatalog.All, career =>
        {
            Assert.False(string.IsNullOrWhiteSpace(career.Name));
            Assert.Equal(Career.MaxLevel, career.Levels.Length);
            for (var i = 0; i < career.Levels.Length; i++)
                Assert.Equal(i + 1, career.Levels[i].Level);
        });
    }

    [Fact]
    public void Salaries_never_regress_along_the_track()
    {
        Assert.All(CareerCatalog.All, career =>
        {
            for (var i = 1; i < career.Levels.Length; i++)
                Assert.True(career.Levels[i].SalaryFactor >= career.Levels[i - 1].SalaryFactor);
        });
    }

    [Fact]
    public void Employment_progresses_from_entry_to_apex()
    {
        var career = CareerCatalog.Find("tecnologia")!;
        var job = new Employment(career);

        Assert.Equal(Career.EntryLevel, job.Level);
        Assert.Equal(career.Levels[0].Title, job.Title);

        job.AdvanceMonths(6);
        Assert.Equal(6, job.MonthsInLevel);

        for (var i = 0; i < Career.MaxLevel + 5; i++)
            job.Promote();

        Assert.True(job.AtTop);
        Assert.Equal(Career.MaxLevel, job.Level);
        Assert.False(job.Promote());
        Assert.Equal("Diretor de tecnologia (CTO)", job.Title);
        Assert.True(job.Demote());
        Assert.Equal(Career.MaxLevel - 1, job.Level);
    }

    [Fact]
    public void Salary_scales_with_the_country_median()
    {
        var world = new World();
        var brasil = world.Countries.First(c => c.Code == "BR");
        var career = CareerCatalog.Find("medicina")!;
        var job = new Employment(career, level: 4);

        var expected = brasil.MedianMonthlySalary * career.AtLevel(4).SalaryFactor;
        Assert.Equal(expected, job.MonthlySalary(brasil));
    }

    [Fact]
    public void Generated_adults_get_careers_and_matching_income()
    {
        var world = new World();
        var brasil = world.Countries.First(c => c.Code == "BR");
        var generator = new LifeGenerator(new Random(3));
        var adults = Enumerable.Range(0, 20)
            .Select(_ => generator.GenerateAdult(brasil, Gender.Male, 40))
            .ToList();

        var employed = adults.Where(a => a.Job is not null).ToList();
        Assert.NotEmpty(employed);
        Assert.All(employed, a =>
            Assert.Equal(a.Job!.MonthlySalary(brasil), a.Finances.MonthlyIncome));
        Assert.All(adults.Where(a => a.Job is null),
            a => Assert.Equal(0m, a.Finances.MonthlyIncome));
    }
}

public class LifeSimulationTests
{
    [Fact]
    public void NewRandomLife_is_deterministic_for_same_seed()
    {
        var a = LifeSimulation.NewRandomLife(seed: 42);
        var b = LifeSimulation.NewRandomLife(seed: 42);

        Assert.Equal(a.Player.FullName, b.Player.FullName);
        Assert.Equal(a.Player.BirthCountry?.Code, b.Player.BirthCountry?.Code);
    }

    [Fact]
    public void NewCustomLife_uses_chosen_name_and_country()
    {
        var world = new World();
        var brasil = world.Countries.First(c => c.Code == "BR");

        var sim = LifeSimulation.NewCustomLife(
            new NewLifeRequest("Ada", "Lovelace", brasil), seed: 1);

        Assert.Equal("Ada Lovelace", sim.Player.FullName);
        Assert.Equal("BR", sim.Player.BirthCountry?.Code);
        Assert.Equal(Currency.Real, sim.Player.BirthCountry?.Currency);
        Assert.NotNull(sim.Player.Mother);
    }

    [Fact]
    public void NewCustomLife_uses_chosen_gender()
    {
        var world = new World();
        var sim = LifeSimulation.NewCustomLife(
            new NewLifeRequest("Ada", null, world.Countries[0], Gender.Female), seed: 1);

        Assert.Equal(Gender.Female, sim.Player.Gender);
    }

    [Fact]
    public void NewCustomLife_without_surname_inherits_family_name()
    {
        var world = new World();
        var sim = LifeSimulation.NewCustomLife(
            new NewLifeRequest("Ada", null, world.Countries[0]), seed: 1);

        var familySurname = sim.Player.Father?.LastName ?? sim.Player.Mother!.LastName;
        Assert.Equal(familySurname, sim.Player.LastName);
    }

    [Fact]
    public void Newborn_has_family_and_country()
    {
        var sim = LifeSimulation.NewRandomLife(seed: 7);

        Assert.True(sim.Player.IsAlive);
        Assert.Equal(0, sim.Player.AgeYears);
        Assert.NotNull(sim.Player.Residence);
        Assert.NotNull(sim.Player.Mother);
    }

    [Fact]
    public void Birth_event_introduces_player_and_parents()
    {
        var world = new World();
        var brasil = world.Countries.First(c => c.Code == "BR");
        var sim = LifeSimulation.NewCustomLife(
            new NewLifeRequest("Ada", "Lovelace", brasil), seed: 7);

        var birth = Assert.Single(sim.Log.Events);
        Assert.Equal("Nascimento", birth.Title);
        Assert.Contains("Ada Lovelace", birth.Description);
        Assert.Contains("Brasil", birth.Description);

        if (sim.Player.Mother is { } mother)
        {
            Assert.Contains(mother.FullName, birth.Description);
            Assert.Contains($"{mother.AgeYears} anos", birth.Description);
            Assert.Contains(mother.Job?.Title ?? "sem emprego", birth.Description);
        }

        if (sim.Player.Father is { } father)
        {
            Assert.Contains(father.FullName, birth.Description);
            Assert.Contains($"{father.AgeYears} anos", birth.Description);
            Assert.Contains(father.Job?.Title ?? "sem emprego", birth.Description);
        }
    }

    [Fact]
    public void Birth_origin_is_stored_and_consistent_with_the_family()
    {
        var seen = new HashSet<OriginKind>();
        for (var seed = 0; seed < 300 && seen.Count < 3; seed++)
        {
            var sim = LifeSimulation.NewRandomLife(seed);
            var origin = sim.Player.Origin!;
            var birth = sim.Log.Events[0];
            seen.Add(origin.Kind);

            switch (origin.Kind)
            {
                case OriginKind.InVitro:
                    Assert.NotNull(sim.Player.Mother);
                    Assert.Null(sim.Player.Father);
                    Assert.Contains("fertilização in vitro", birth.Description);
                    break;
                case OriginKind.Adopted:
                    Assert.True(sim.Player.Mother is not null || sim.Player.Father is not null);
                    Assert.Equal("Adoção", birth.Title);
                    Assert.Contains("adotad", birth.Description);
                    break;
                default:
                    Assert.NotNull(sim.Player.Mother);
                    Assert.NotNull(sim.Player.Father);
                    Assert.Equal("Nascimento", birth.Title);
                    Assert.Contains(origin.Union switch
                    {
                        UnionKind.Affair => "affair",
                        UnionKind.Dating => "namorados",
                        _ => "casado",
                    }, birth.Description);
                    break;
            }

            // União só existe quando há os dois pais.
            if (sim.Player.Mother is null || sim.Player.Father is null)
                Assert.Null(origin.Union);
        }
        Assert.Equal(3, seen.Count);
    }

    [Fact]
    public void AdvancePeriod_ages_newborn_by_one_year()
    {
        var sim = LifeSimulation.NewRandomLife(seed: 7);
        sim.AdvancePeriod();

        Assert.Equal(12, sim.Player.AgeMonths);
        Assert.Equal(12, sim.AbsoluteMonth);
    }

    [Fact]
    public void Full_life_eventually_ends_in_death_or_dynasty()
    {
        var sim = LifeSimulation.NewRandomLife(seed: 7);
        for (var i = 0; i < 12 * 150 && sim.Player.IsAlive; i++)
            sim.AdvancePeriod();

        Assert.False(sim.Player.IsAlive);
        Assert.NotNull(sim.Player.CauseOfDeath);
    }

    [Fact]
    public void Succession_requires_living_heir()
    {
        var sim = LifeSimulation.NewRandomLife(seed: 7);
        var dead = new Person();
        dead.Die(0, "teste");
        Assert.Throws<InvalidOperationException>(() => sim.Succeed(dead));
    }
}
