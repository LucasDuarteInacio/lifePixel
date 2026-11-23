# GDD --- Simulador de Vida

**Versão:** 0.1 --- Documento de concepção\
**Data:** 21/11/2026\
**Status:** Em definição --- decisões das etapas 1 a 10 consolidadas;
etapa 11 pendente.

------------------------------------------------------------------------

## 1. Visão geral

### 1.1 Conceito

Jogo mobile de simulação de vida inspirado em *BitLife*, com identidade
própria. O jogador acompanha uma pessoa desde o nascimento até a morte,
tomando decisões que influenciam sua trajetória, seus relacionamentos,
sua condição financeira, sua saúde e o legado deixado para as próximas
gerações.

O universo será global, baseado em países reais. Cada partida representa
uma família/dinastia independente, cuja história pode continuar por meio
de descendentes do personagem principal.

### 1.2 Fantasia do jogador

Viver uma vida inteira e construir uma história familiar única, marcada
por escolhas, oportunidades, dificuldades, conquistas, perdas e
consequências que podem atravessar gerações.

### 1.3 Tom e ambientação

-   **Tom:** equilíbrio entre realismo, humor e drama.
-   **Universo:** mundo contemporâneo global, com países reais.
-   **Referência principal:** BitLife, sem intenção de reproduzir sua
    identidade ou seus sistemas de forma literal.
-   **Plataformas-alvo:** Android e iOS, com possibilidade futura de PC.
-   **Orientação:** vertical (portrait).
-   **Direção artística:** ilustração 2D.

### 1.4 Princípios de design

1.  **Liberdade de escolha:** o jogador deve ter diferentes caminhos
    possíveis.
2.  **Consequências:** decisões podem produzir efeitos imediatos ou
    tardios.
3.  **Mundo vivo:** NPCs e acontecimentos continuam evoluindo sem
    depender da interação direta do jogador.
4.  **Histórias emergentes:** sistemas interligados devem gerar
    trajetórias únicas.
5.  **Continuidade familiar:** a morte de um personagem pode iniciar uma
    nova fase da mesma dinastia.
6.  **Expansibilidade:** os sistemas devem ser modulares, permitindo
    adicionar conteúdo sem reconstruir o núcleo do jogo.

------------------------------------------------------------------------

## 2. Pilares de experiência

### 2.1 Vida completa

A partida começa no nascimento e acompanha o personagem pelas diferentes
fases da vida, até sua morte.

### 2.2 Simulação sistêmica

Família, educação, carreira, economia, saúde, relacionamentos e mundo
devem interagir. Um acontecimento em um sistema pode afetar vários
outros.

### 2.3 Personagens autônomos

NPCs possuem atributos, personalidade, objetivos, memória,
relacionamentos e trajetória própria. Eles podem estudar, trabalhar,
formar famílias, envelhecer e morrer sem a intervenção do jogador.

### 2.4 Dinastia

Após a morte do personagem controlado, o jogador poderá escolher
qualquer descendente vivo para continuar a partida. A história familiar
e o legado permanecem.

### 2.5 Rejogabilidade

Cada nova partida pode começar em uma família, país e condição de vida
diferentes. Eventos escritos e geração procedural ajudam a produzir
experiências variadas.

------------------------------------------------------------------------

## 3. Estrutura da partida

### 3.1 Modos de início

A criação de uma nova partida terá dois modos:

-   **Vida aleatória:** país, família, condições iniciais e
    características são determinados com componentes aleatórios.
-   **Vida personalizada:** o jogador escolhe algumas características
    iniciais, enquanto outros elementos continuam sujeitos à
    aleatoriedade.

Os detalhes exatos das opções de personalização ainda serão definidos.

### 3.2 Partidas e famílias

-   Cada espaço de salvamento representa uma vida/dinastia independente.
-   A partida pode continuar por várias gerações.
-   A escolha atual é manter uma vida por partida, e não múltiplos
    personagens principais simultâneos.
-   Quando não houver descendentes vivos elegíveis, a dinastia termina;
    o histórico deve permanecer consultável. (Regra proposta a partir da
    decisão de sucessão; validar no detalhamento.)

### 3.3 Sucessão

Quando o personagem principal morrer: - O jogador poderá escolher
qualquer descendente vivo para assumir como personagem principal. - A
continuidade não ficará limitada ao filho mais velho. - Regras
detalhadas de herança, patrimônio, guarda e sucessão ainda precisam ser
especificadas.

------------------------------------------------------------------------

## 4. Tempo e ciclo de jogo

### 4.1 Passagem do tempo definida

O avanço de tempo varia conforme a idade:

  Fase                    Regra definida
  ----------------------- ------------------
  Infância, até 12 anos   Avanço anual
  Adolescência            Avanço semestral
  Vida adulta             Avanço mensal

A idade exata de início e término da adolescência, assim como a
transição para a vida adulta, ainda precisa ser especificada.

### 4.2 Ciclo principal de gameplay --- proposta inicial

1.  Consultar a situação atual do personagem.
2.  Escolher atividades e compromissos.
3.  Avançar o tempo.
4.  Atualizar personagens, relações e mundo.
5.  Apresentar acontecimentos e decisões.
6.  Aplicar consequências e atualizar a trajetória.
7.  Repetir o ciclo até o fim da vida e eventual sucessão.

**Atenção:** a estrutura acima é uma proposta de ciclo de jogo. As
perguntas da Etapa 11 ainda não foram respondidas e podem alterar esse
fluxo.

### 4.3 Atividades e agenda

Foi escolhido um modelo híbrido, combinando: - Atividades selecionáveis
pelo jogador. - Agenda para compromissos e atividades programadas.

Exemplos de atividades possíveis: - Trabalhar e procurar emprego. -
Estudar e fazer cursos. - Praticar exercícios. - Descansar. - Sair com
amigos. - Desenvolver relacionamentos. - Viajar. - Cuidar da família.

A lista final, custos, duração, limites por período e regras de
agendamento ainda serão definidos.

------------------------------------------------------------------------

## 5. Sistemas principais do MVP

O projeto começará com um conjunto menor de sistemas, preparado para
expansão. Os seis sistemas escolhidos como núcleo inicial são:

### 5.1 Família e relacionamentos

Escopo previsto: - Pais, irmãos e parentes. - Amizades e inimizades. -
Namoro, casamento e divórcio. - Filhos e criação. - Conflitos, traições
e reconciliações. - Herança e relações familiares.

### 5.2 Educação e desenvolvimento

Escopo previsto: - Infância e escolarização. - Desempenho escolar. -
Habilidades e desenvolvimento pessoal. - Cursos e universidade.

### 5.3 Carreira e profissões

Visão de longo prazo: - Empregos e progressão profissional. - Promoções
e demissões. - Empreendedorismo e empresas. - Política, fama e
profissões especiais. - Criminalidade e atividades ilegais.

A primeira implementação deverá começar com um conjunto controlado de
carreiras, expandindo por módulos.

### 5.4 Economia e patrimônio

Na primeira versão, o sistema será simplificado. A visão inclui: -
Dinheiro, renda e despesas. - Salários e custo de vida. - Dívidas e
empréstimos. - Imóveis e veículos. - Empresas e investimentos. - Herança
e patrimônio familiar.

A profundidade exata do sistema financeiro inicial ainda será definida.

### 5.5 Saúde e bem-estar

Escopo previsto: - Saúde física e mental. - Hábitos e condicionamento. -
Doenças e condições de saúde. - Exercícios, descanso e envelhecimento. -
Riscos de morte associados à idade, saúde, hábitos e acontecimentos.

### 5.6 Mundo e sociedade

Escopo inicial: - Países reais. - Moedas próprias por país. -
Características básicas de cada país. - Viagens e imigração. - Cultura e
diferenças sociais. - Leis e acontecimentos sociais, conforme evolução
do projeto.

------------------------------------------------------------------------

## 6. Personagens e atributos

### 6.1 Categorias de atributos

O jogo combinará indicadores numéricos e traços de personalidade.

**Características físicas** - Aparência. - Saúde. - Condicionamento
físico.

**Características mentais** - Inteligência. - Criatividade. -
Estabilidade emocional.

**Características sociais** - Carisma. - Empatia. - Comunicação.

**Características comportamentais** - Disciplina. - Ambição. -
Impulsividade. - Honestidade.

Os nomes, escalas, fórmulas, limites e efeitos dos atributos ainda serão
especificados.

### 6.2 Personalidade

Além dos atributos numéricos, os personagens terão traços de
personalidade, por exemplo: - Introvertido. - Ambicioso. - Impulsivo. -
Generoso.

A personalidade deve influenciar decisões, preferências, reações a
eventos e comportamento social. A lista definitiva de traços ainda será
construída.

### 6.3 Genética e aparência

Foi definido um sistema de herança genética com: - Características
físicas herdadas dos pais. - Variações genéticas. - Predisposições. -
Semelhanças familiares. - Personalização avançada da aparência. -
Envelhecimento visual dos personagens.

A genética deve influenciar a aparência e predisposições, sem determinar
rigidamente o destino do personagem. A herança de inteligência e outras
características complexas precisa de regras próprias e cuidadosas.

### 6.4 Envelhecimento visual

Os personagens terão retratos ilustrados em 2D que mudam conforme
envelhecem. A aparência deve preservar a identidade do personagem ao
longo das fases da vida e permitir reconhecer semelhanças entre
gerações.

------------------------------------------------------------------------

## 7. Família e genealogia

### 7.1 Árvore genealógica

A árvore genealógica será completa, incluindo: - Pais e filhos. -
Irmãos. - Avós. - Tios e primos. - Sobrinhos e netos. - Cônjuges e
ex-cônjuges. - Descendentes de gerações posteriores.

### 7.2 Histórico familiar

O histórico poderá registrar: - Nascimentos e mortes. - Casamentos e
separações. - Filhos e descendentes. - Relações importantes. -
Patrimônio e heranças. - Acontecimentos marcantes.

### 7.3 Relacionamentos

As relações não serão representadas por um único indicador. Haverá
indicadores independentes, como: - Amor. - Amizade. - Confiança. -
Respeito. - Intimidade. - Rivalidade.

Também haverá histórico de acontecimentos que afetam a relação. Uma
pessoa pode, por exemplo, amar alguém e não confiar nela.

------------------------------------------------------------------------

## 8. NPCs e simulação autônoma

### 8.1 Autonomia

NPCs poderão: - Estudar e desenvolver habilidades. - Conseguir empregos
e mudar de carreira. - Construir relacionamentos. - Casar e ter
filhos. - Tomar decisões financeiras. - Enfrentar dificuldades e
oportunidades. - Envelhecer e morrer.

### 8.2 Tomada de decisão

As decisões dos NPCs deverão considerar: - Personalidade. - Atributos. -
Objetivos de vida. - Necessidades. - Condição financeira. -
Relacionamentos. - Memória e experiências anteriores. - Oportunidades
disponíveis. - Acontecimentos recentes.

### 8.3 Objetivos dinâmicos

Os personagens terão objetivos que podem mudar ao longo da vida.
Exemplos: - Enriquecer. - Formar uma família. - Tornar-se famoso. -
Viajar. - Ter reconhecimento profissional. - Viver tranquilamente. -
Alcançar poder político.

Os objetivos devem evoluir conforme experiências, idade, condições e
oportunidades.

### 8.4 Memória e biografia

NPCs terão memória de acontecimentos relevantes. Experiências passadas
poderão influenciar suas decisões e relações futuras.

Exemplos: - Perda de familiares. - Conflitos na infância. - Experiências
afetivas. - Abandono dos estudos. - Dificuldades financeiras. -
Conquistas profissionais.

### 8.5 Níveis de simulação

Para controlar o custo de processamento, fica registrada a proposta de
três níveis:

1.  **NPCs próximos:** simulação profunda, memória, relacionamentos e
    acontecimentos.
2.  **NPCs relevantes:** atualização periódica de carreira, família e
    objetivos.
3.  **População geral:** simulação estatística de demografia, economia e
    acontecimentos.

Essa divisão é uma diretriz arquitetural proposta e deverá ser validada
em protótipos de desempenho.

------------------------------------------------------------------------

## 9. Eventos e narrativa

### 9.1 Modelo de eventos

O jogo combinará: - Eventos escritos manualmente. - Eventos gerados
proceduralmente a partir de modelos, atributos e variáveis. - Eventos
encadeados, em que uma decisão pode iniciar consequências ao longo de
meses ou anos.

### 9.2 Contextualização

Eventos devem considerar o estado atual do personagem e do mundo. Não
devem ser apresentados como textos aleatórios sem relação com a
história.

### 9.3 Consequências

Uma decisão poderá afetar múltiplos sistemas. Por exemplo, uma demissão
pode: - Reduzir a renda. - Aumentar o risco de endividamento. - Afetar a
saúde mental. - Prejudicar relacionamentos. - Alterar objetivos
profissionais.

### 9.4 Conteúdo

O jogo poderá abordar temas realistas e difíceis, incluindo crimes,
vícios, acidentes, doenças, problemas financeiros e situações
controversas, sempre com consequências coerentes no mundo simulado.

### 9.5 Apresentação dos eventos

A forma final de apresentação ainda está pendente. As opções
consideradas incluem feed cronológico, notificações e janelas de eventos
especiais.

------------------------------------------------------------------------

## 10. Mundo global e acontecimentos

### 10.1 Países

A primeira abordagem será implementar países com sistemas próprios em
profundidade controlada, com possibilidade de evolução para simulação
mais detalhada de cidades e regiões.

Na primeira versão, cada país poderá ter: - Moeda. - Idioma(s), conforme
escopo futuro. - Custo de vida simplificado. - Salários de referência. -
Características sociais e culturais básicas.

### 10.2 Economia por país

Foi escolhida uma economia simplificada, com moedas próprias por país.
Não está definido, neste momento, que haverá simulação completa de
inflação, juros, mercado financeiro ou crises econômicas.

### 10.3 Eventos mundiais

Haverá eventos globais aleatórios. Exemplos: - Crises econômicas. -
Pandemias. - Guerras. - Crises políticas. - Novas tecnologias. -
Desastres naturais.

A escolha atual é por eventos globais aleatórios; a inclusão de eventos
regionais e seus efeitos sistêmicos poderá ser detalhada posteriormente.

------------------------------------------------------------------------

## 11. Mortalidade e continuidade

### 11.1 Morte

A mortalidade será influenciada por: - Idade. - Saúde. - Hábitos. -
Acontecimentos. - Acidentes e doenças.

A escolha atual é um modelo intermediário, no qual esses fatores
influenciam o risco. Não foi escolhida uma simulação médica extremamente
detalhada.

### 11.2 Dinastia

Após a morte do personagem principal, o jogador poderá escolher qualquer
descendente vivo para continuar.

### 11.3 Legado

O histórico familiar deve permanecer relevante entre gerações. O escopo
exato da transmissão de patrimônio, reputação, relações e outros
elementos será definido em uma etapa específica.

------------------------------------------------------------------------

## 12. Progressão, desafios e conquistas

O jogo será livre, mas contará com: - Desafios opcionais. -
Conquistas. - Objetivos de longo prazo. - Conteúdos e possibilidades
desbloqueáveis.

As conquistas poderão incluir objetivos como: - Tornar-se bilionário. -
Ter uma família numerosa. - Manter uma dinastia por várias gerações. -
Conhecer todos os continentes. - Alcançar idade avançada. - Tornar-se
uma celebridade. - Construir uma empresa de sucesso.

A escolha atual é que conquistas desbloqueiem conteúdos e
possibilidades. A estrutura exata de recompensas ainda será definida.

------------------------------------------------------------------------

## 13. Interface e experiência do usuário

### 13.1 Direção visual

-   Ilustração 2D.
-   Retratos expressivos.
-   Personagens com envelhecimento visual.
-   Diversidade de características físicas e culturais.
-   Interface híbrida, combinando texto e elementos visuais.

### 13.2 Navegação

A navegação será organizada em: - Um painel principal. - Acesso a telas
específicas de cada sistema.

### 13.3 Orientação

A interface será prioritariamente vertical (portrait).

### 13.4 Tela principal

A proposta inicial é apresentar: - Retrato do personagem. - Indicadores
importantes. - Acesso rápido aos sistemas. - Feed cronológico de
acontecimentos.

Essa composição é uma recomendação ainda não confirmada pelo usuário.

------------------------------------------------------------------------

## 14. Monetização

Modelo escolhido: - Jogo gratuito. - Anúncios opcionais. - Compras
internas. - Expansões.

Ainda será necessário definir: - Quais recompensas podem ser obtidas por
anúncios. - Quais recursos serão vendidos. - Conteúdos de expansões. -
Limites para evitar desequilíbrio entre jogadores. - Estratégia de
monetização sem prejudicar a experiência principal.

------------------------------------------------------------------------

## 15. Plataforma, conectividade e salvamento

### 15.1 Plataformas

-   Android.
-   iOS.
-   Possibilidade futura de PC.

### 15.2 Conectividade

Modelo híbrido: - O jogo deverá funcionar offline. - Quando houver
internet, poderá sincronizar dados na nuvem.

A política de conflitos de sincronização, autenticação e backup ainda
será especificada.

### 15.3 Salvamento

-   Uma vida/dinastia por partida.
-   O jogador poderá manter partidas independentes.
-   A escolha de um único personagem principal por partida não impede
    que a dinastia tenha muitos NPCs e descendentes.

### 15.4 Motor de jogo

Ainda não definido. Godot, Unity e outras alternativas serão avaliadas
conforme: - Desempenho em dispositivos móveis. - Facilidade de
desenvolver interface 2D. - Suporte a Android e iOS. - Persistência e
sincronização. - Ferramentas de testes e depuração. - Facilidade de
manutenção e expansão. - Necessidades de animação e renderização.

Nenhuma tecnologia deve ser considerada decisão final antes dessa
avaliação.

------------------------------------------------------------------------

## 16. Escopo e estratégia de desenvolvimento

### 16.1 Visão final

A visão de longo prazo contempla: - Vida completa. - Dinastias. - NPCs
autônomos. - Genética e envelhecimento. - Relacionamentos complexos. -
Educação e carreiras. - Economia e patrimônio. - Países e acontecimentos
globais. - Eventos narrativos encadeados. - Progressão e conquistas.

### 16.2 MVP

O MVP deverá começar com um conjunto menor de sistemas, mas a lista
exata de funcionalidades e limites ainda não foi aprovada.

A recomendação inicial apresentada foi criar uma vida completa com
poucos sistemas, para validar nascimento, envelhecimento, morte e
sucessão antes de ampliar a simulação. Essa recomendação ainda precisa
ser confirmada.

### 16.3 Arquitetura modular

A arquitetura deverá separar, no mínimo, os seguintes domínios: -
Personagens e atributos. - Genética e aparência. - Família e
genealogia. - Relacionamentos. - Tempo e envelhecimento. - Atividades e
agenda. - Eventos e consequências. - Educação. - Carreira. - Economia e
patrimônio. - Saúde e mortalidade. - Mundo e países. - NPCs e
autonomia. - Progressão e conquistas. - Salvamento e sincronização. -
Interface e apresentação.

Essa divisão é uma proposta conceitual para orientar o desenho técnico,
não uma arquitetura de código já aprovada.

------------------------------------------------------------------------

## 17. Decisões pendentes

### Etapa 11 --- Core Gameplay Loop

As perguntas abaixo ainda não foram respondidas:

**46. O jogador poderá avançar o tempo sem realizar atividades?** - A)
Sim, pode simplesmente avançar. - B) Não, precisa cumprir atividades. -
C) Sim, mas atividades podem ser agendadas para acontecer
automaticamente.

**47. Como será a quantidade de atividades disponíveis?** - A) Poucas
atividades, com bastante profundidade. - B) Muitas atividades, com
diferentes níveis de complexidade. - C) Começar com poucas e adicionar
novas conforme as atualizações.

**48. Como será a tela principal?** - A) Feed cronológico de
acontecimentos. - B) Painel com indicadores e atalhos. - C) Painel com
retrato, indicadores e feed cronológico.

**49. Como serão apresentados acontecimentos fora da tela principal?** -
A) Tudo aparece apenas no feed. - B) Eventos importantes aparecem em
janelas próprias. - C) Sistema misto: feed, notificações e eventos
especiais.

**50. Qual será a prioridade do primeiro protótipo jogável?** - A) Uma
vida completa, mas com poucos sistemas. - B) Simulação profunda dos
primeiros 18 anos. - C) Personagem adulto para testar carreira,
dinheiro, relacionamentos e eventos. - D) Pequeno mundo com família,
NPCs autônomos e passagem do tempo.

### Outras definições necessárias

1.  Faixas etárias e transições exatas.
2.  Escalas e fórmulas dos atributos.
3.  Lista de traços de personalidade.
4.  Regras de genética e herança.
5.  Regras de sucessão e patrimônio.
6.  Quantidade inicial de países e profissões.
7.  Estrutura de atividades, custos e duração.
8.  Modelo de dados e persistência.
9.  Estratégia de sincronização offline/online.
10. Motor de jogo e ferramentas.
11. Classificação indicativa e diretrizes de conteúdo.
12. Regras de monetização.

------------------------------------------------------------------------

## 18. Glossário

  -----------------------------------------------------------------------
  Termo                               Significado
  ----------------------------------- -----------------------------------
  GDD                                 Game Design Document; documento de
                                      design do jogo

  MVP                                 Produto mínimo viável, primeira
                                      versão que valida o núcleo do jogo

  NPC                                 Personagem não controlado
                                      diretamente pelo jogador

  Dinastia                            Linha familiar que continua através
                                      das gerações

  Core Gameplay Loop                  Ciclo de ações que se repete
                                      durante a experiência

  Evento procedural                   Evento montado a partir de modelos,
                                      regras e variáveis

  Atributo                            Indicador numérico de uma
                                      característica do personagem

  Traço de personalidade              Característica comportamental que
                                      influencia decisões

  Genealogia                          Registro das relações e
                                      descendência familiar
  -----------------------------------------------------------------------

------------------------------------------------------------------------

## 19. Resumo executivo

O projeto é um simulador de vida mobile, em 2D e orientação vertical,
ambientado em um mundo global baseado em países reais. O jogador
acompanha uma pessoa desde o nascimento até a morte, toma decisões,
desenvolve relacionamentos, estuda, trabalha, constrói patrimônio e
enfrenta acontecimentos que influenciam sua trajetória.

O diferencial central será a combinação de: - NPCs com vidas
autônomas. - Personalidades, objetivos e memórias. - Genética e
envelhecimento visual. - Relações com histórico e indicadores
independentes. - Eventos escritos e procedurais, com consequências
encadeadas. - Continuidade da partida por meio de descendentes.

O projeto será desenvolvido de forma modular, começando por um MVP de
escopo controlado e evoluindo em direção à simulação de vida e dinastias
mais completa.

**Status do documento:** concepção inicial consolidada até a Etapa 10. A
Etapa 11 e diversas especificações de implementação permanecem em
aberto.
