using LifeSimulator.Core.Simulation;

namespace LifeSimulator.UI;

/// <summary>
/// Ponte entre a tela de novo jogo e a tela principal: guarda a
/// configuração de partida escolhida até a Main consumi-la (a troca de
/// cena do Godot não carrega parâmetros).
/// </summary>
public static class GameSetup
{
	public static NewLifeRequest? Pending { get; set; }

	/// <summary>
	/// Partida em andamento, compartilhada entre as telas (a troca de
	/// cena do Godot não carrega estado). Main reutiliza ao voltar.
	/// </summary>
	public static LifeSimulation? Current { get; set; }
}
