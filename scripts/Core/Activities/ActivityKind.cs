namespace LifeSimulator.Core.Activities;

/// <summary>
/// Atividades selecionáveis pelo jogador (GDD §4.3). Conjunto inicial
/// do protótipo de interface; o modelo final terá custos, duração,
/// limites por período e agendamento.
/// </summary>
public enum ActivityKind
{
    Study,
    Exercise,
    Socialize,
    Rest,
}
