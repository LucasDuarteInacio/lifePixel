# LifePixel - Simulador de Vida

Um jogo de simulação de vida estilo BitLife desenvolvido com React, TypeScript e Electron.

## 🚀 Tecnologias

- **React 18** - Biblioteca para interfaces de usuário
- **TypeScript** - Tipagem estática para JavaScript
- **Vite** - Build tool e dev server
- **Tailwind CSS** - Framework CSS utilitário
- **Electron** - Framework para aplicações desktop

## 📦 Instalação

```bash
# Instalar dependências
npm install
```

## 🛠️ Scripts Disponíveis

### Desenvolvimento
```bash
# Iniciar servidor de desenvolvimento (Vite)
npm run dev

# Iniciar aplicação Electron em modo desenvolvimento
npm run electron-dev

# Executar linting
npm run lint
```

### Build e Distribuição
```bash
# Build da aplicação web
npm run build

# Preview do build
npm run preview

# Build e empacotamento para Electron
npm run electron-pack

# Build para distribuição
npm run dist
```

## 🎮 Como Jogar

1. **Iniciar o Jogo**: Execute `npm run electron-dev` para abrir a aplicação
2. **Criar Personagem**: Escolha nome, gênero e país de origem
3. **Gerenciar Vida**: Tome decisões que afetam sua vida virtual
4. **Acompanhar Progresso**: Veja estatísticas e eventos da sua vida

## 🏗️ Estrutura do Projeto

```
src/
├── components/          # Componentes React
│   ├── GameApp.tsx     # Componente principal do jogo
│   ├── StartScreen.tsx # Tela inicial
│   ├── GameScreen.tsx  # Tela principal do jogo
│   ├── StatusScreen.tsx # Tela de status
│   ├── EventsScreen.tsx # Tela de eventos
│   └── LoadingScreen.tsx # Tela de carregamento
├── hooks/              # Custom hooks
│   ├── useGameState.ts # Hook para estado do jogo
│   └── useHydration.ts # Hook para hidratação
├── data/               # Dados do jogo
│   ├── countries.ts    # Lista de países
│   ├── events.ts       # Eventos do jogo
│   └── names.ts        # Nomes para personagens
├── types/              # Definições de tipos TypeScript
│   └── game.ts         # Tipos do jogo
├── App.tsx             # Componente raiz
├── main.tsx            # Ponto de entrada
└── globals.css         # Estilos globais
```

## 🔧 Configuração

### Desenvolvimento
- **Porta do Vite**: 5173
- **Hot Reload**: Ativado automaticamente
- **TypeScript**: Configurado com path mapping (`@/` para `src/`)

### Electron
- **Janela**: 1200x800 pixels
- **DevTools**: Automático em desenvolvimento
- **Build**: Gera executáveis para Windows, Mac e Linux

## 📝 Licença

Este projeto é privado e desenvolvido para fins educacionais.

## 🤝 Contribuição

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 🐛 Problemas Conhecidos

- Em desenvolvimento
