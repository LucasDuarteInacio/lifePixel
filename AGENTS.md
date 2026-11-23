# Simulador de Vida — Godot .NET

Simulador de vida 2D (estilo BitLife) para mobile e PC, baseado em
`docs/GDD_Simulador_de_Vida_v0.1.md`.

## Stack

- Godot 4.7 (.NET / C#) — `project.godot`, `LifeSimulator.csproj`
- .NET 8 (Godot.NET.Sdk 4.7.2); testes em .NET 11 com xUnit
- Viewport portrait 720×1560 (proporção do Galaxy S25 Ultra, 1440×3120),
  renderer `mobile`, stretch `canvas_items/expand`

## Comandos

```bash
dotnet build LifeSimulator.slnx   # compila jogo + testes
dotnet test tests/                # testes do núcleo de simulação
```

Abrir no editor: importar `project.godot` no **Godot 4.7 .NET** (versão
mono/.NET, não a padrão). Cena principal: `scenes/main.tscn`.

## Arquitetura

- `scripts/Core/` — simulação pura em C#, **sem dependência do Godot**,
  organizada pelos domínios do GDD §16.3 (Characters, Genetics,
  Relationships, Events, World, Economy, Health, Time, Careers,
  Generation, Simulation). Toda a lógica nova do jogo deve ficar aqui
  para ser testável.
- `scripts/UI/` — scripts Godot (Control/Node) que apenas refletem o
  estado da simulação.
- `scenes/` — cenas .tscn.
- `data/countries.json` — tabela de países (custo/salário de
  referência, nomes típicos), embutida no assembly via
  `<EmbeddedResource>` e carregada por `Core/World/World.cs`. O jogo
  usa só três moedas — USD, EUR e BRL (`Core/Economy/Currency.cs`);
  cada país usa uma delas, ou seja, a moeda vem da nacionalidade.
- `data/careers.json` — tabela de carreiras (GDD §5.3): cada carreira
  tem 10 níveis (1 = entrada, 10 = ápice) com cargo e fator salarial
  sobre a renda mediana do país. Embutida no assembly e carregada por
  `Core/Careers/CareerCatalog.cs`, que valida a trilha; o vínculo
  empregatício da pessoa é `Core/Careers/Employment.cs` (`Person.Job`).
- `scripts/Core/Genealogy/` — família e genealogia (GDD §7):
  `Kinship` deriva os graus do grafo `Person` (fonte única),
  `KinLabel` gera rótulos pt-BR com gênero e `FamilyRegistry` é o
  registro de todas as pessoas conhecidas — escopo da simulação
  profunda: todo NPC registrado envelhece e morre.
- `scripts/Core/Relationships/` — relações (GDD §5.1, §7.3):
  `Relationship` com 6 indicadores independentes + histórico;
  `RelationshipRules` define indicadores ativos por tipo, tiers de
  amizade (conhecido/amigo/melhor amigo) e limiares de progressão;
  `MaritalStatus` + `Couple` cobrem namoro → noivado → casamento →
  divórcio/viuvez; `Interactions` é o catálogo de ações do jogador;
  `RelationshipDirector` é a "IA" de background que deriva vínculos,
  casa/descasa NPCs e cria `PendingDecision`s (pedidos de namoro e
  casamento ao jogador, traições). A família estendida é materializada
  sob demanda por `LifeGenerator.EnsureExtendedFamily` (idempotente
  via flags `Person.AncestryEnsured`/`DescendantKinEnsured`).
- `assets/flags/` — bandeiras PNG por código ISO (`{code}.png`),
  exibidas na seleção de país da tela de novo jogo.
- `tests/` — testes xUnit referenciando o csproj do jogo. O Godot.NET.Sdk
  compila `**/*.cs` do diretório: `tests/` e `docs/` são excluídos via
  `<Compile Remove>` no csproj.

## Decisões provisórias (a revisar quando o GDD detalhar)

- Fases: infância até 12 anos (anual), adolescência 12–18 (semestral),
  adulto 18+ (mensal) — centralizado em `Core/Time/TimeStep.cs`.
- Atributos e indicadores de relação usam escala 0–100.
- Sucessão: qualquer descendente vivo elegível (`Person.LivingDescendants`).
- Origem familiar (`Person.Origin`, `Core/Characters/BirthOrigin.cs`):
  natural (pais casados, namorados ou affair), FIV (só a mãe) ou adoção
  (pai e/ou mãe). A união só existe com os dois pais; affair vira
  `RelationshipKind.ExPartner`. Apresentada no evento de nascimento.
