using LifeSimulator.Core.Characters;
using LifeSimulator.Core.World;

namespace LifeSimulator.Core.Simulation;

/// <summary>
/// Escolhas do jogador para uma vida personalizada (GDD §3.1): nome,
/// gênero e país de nascimento são fixos — a moeda vem da
/// nacionalidade — enquanto família e características permanecem
/// aleatórias. Sobrenome nulo herda o da família; gênero nulo sorteia.
/// </summary>
public sealed record NewLifeRequest(
    string FirstName,
    string? LastName,
    Country Country,
    Gender? Gender = null);
