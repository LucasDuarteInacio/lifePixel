# VidaPixel - Simulador de Vida

Um jogo de simulação de vida estilo BitLife desenvolvido com React, TypeScript, Tailwind CSS e Electron.

## 🎮 Sobre o Jogo

VidaPixel é um simulador de vida onde você pode criar um personagem e viver sua vida através de eventos aleatórios que acontecem a cada ano. O jogo inclui:

- **Criação de personagem** com nome, gênero e país personalizados ou aleatórios
- **Sistema de envelhecimento** - avance um ano por vez
- **Eventos aleatórios** que afetam estatísticas do personagem
- **Múltiplas telas**: Início, Jogo, Status e Eventos
- **Persistência de dados** usando localStorage
- **Interface moderna** com design responsivo

## 🚀 Tecnologias Utilizadas

- **React 18** - Framework principal
- **TypeScript** - Tipagem estática
- **Tailwind CSS** - Estilização
- **Next.js 15** - Framework React
- **Electron** - Aplicação desktop
- **LocalStorage** - Persistência de dados

## 📁 Estrutura do Projeto

```
vidapixel/
├── src/
│   ├── app/                 # Páginas Next.js
│   ├── components/          # Componentes React
│   │   ├── GameApp.tsx     # Componente principal
│   │   ├── StartScreen.tsx # Tela inicial
│   │   ├── GameScreen.tsx  # Tela do jogo
│   │   ├── StatusScreen.tsx # Tela de status
│   │   └── EventsScreen.tsx # Tela de eventos
│   ├── hooks/              # Hooks personalizados
│   │   └── useGameState.ts # Hook de estado do jogo
│   ├── types/              # Tipos TypeScript
│   │   └── game.ts         # Tipos do jogo
│   └── data/               # Dados do jogo
│       ├── events.ts       # Eventos do jogo
│       ├── countries.ts    # Lista de países
│       └── names.ts        # Nomes brasileiros
├── electron/               # Configuração Electron
│   └── main.js            # Processo principal
└── package.json           # Dependências e scripts
```

## 🎯 Funcionalidades

### Tela Inicial
- Criar personagem aleatório
- Criar personagem personalizado
- Continuar jogo salvo

### Tela Principal
- Visualizar informações do personagem
- Ver estatísticas em tempo real
- Avançar anos
- Acessar telas de Status e Eventos

### Tela de Status
- Informações detalhadas do personagem
- Estatísticas completas
- Carreira e educação
- Relacionamentos

### Tela de Eventos
- Cronologia completa da vida
- Estatísticas dos eventos
- Efeitos de cada evento
- Filtros por tipo

## 🎲 Sistema de Eventos

O jogo possui mais de 30 eventos diferentes organizados por faixa etária:

### Infância (0-12 anos)
- Nascimento, primeiras palavras, jardim de infância
- Doenças infantis, descoberta de talentos

### Adolescência (13-17 anos)
- Puberdade, primeira paixão, bullying
- Excelência acadêmica, rebeldia

### Juventude (18-25 anos)
- Formatura do ensino médio, faculdade
- Primeiro emprego, relacionamentos sérios

### Vida Adulta (26-50 anos)
- Casamento, filhos, promoções
- Perda de emprego, divórcio, heranças

### Meia-idade (51-65 anos)
- Crise da meia-idade, pico da carreira
- Problemas de saúde

### Terceira idade (66+ anos)
- Aposentadoria, netos
- Doenças graves, morte

## 🛠️ Instalação e Execução

### Pré-requisitos
- Node.js 18+ 
- npm ou yarn

### Instalação
```bash
# Clonar o repositório
git clone <url-do-repositorio>
cd vidapixel

# Instalar dependências
npm install
```

### Execução

#### Modo Desenvolvimento (Web)
```bash
npm run dev
```
Acesse http://localhost:3000

#### Modo Desenvolvimento (Desktop)
```bash
npm run electron-dev
```

#### Build para Produção
```bash
# Build web
npm run build

# Build desktop
npm run dist
```

## 📊 Estatísticas do Personagem

Cada personagem possui 6 estatísticas principais:

- **Felicidade** (0-100): Afetada por eventos positivos/negativos
- **Saúde** (0-100): Pode levar à morte se chegar a 0
- **Dinheiro**: Pode ser negativo, afetado por empregos e eventos
- **Inteligência** (0-100): Afeta chances de eventos educacionais
- **Aparência** (0-100): Influencia relacionamentos
- **Popularidade** (0-100): Afeta interações sociais

## 🎨 Design e Interface

- **Design responsivo** para diferentes tamanhos de tela
- **Tema escuro** com gradientes modernos
- **Animações suaves** e transições
- **Interface intuitiva** com navegação clara
- **Feedback visual** para eventos e ações

## 🔧 Configuração Electron

O projeto está configurado para funcionar como aplicação desktop:

- **Janela principal**: 1200x800 pixels
- **Título**: "VidaPixel - Simulador de Vida"
- **DevTools**: Habilitado em desenvolvimento
- **Build**: Configurado para Windows, Mac e Linux

## 📝 Scripts Disponíveis

```bash
npm run dev          # Desenvolvimento web
npm run build        # Build para produção
npm run start        # Servidor de produção
npm run lint         # Verificação de código
npm run electron     # Executar Electron
npm run electron-dev # Desenvolvimento Electron
npm run dist         # Build desktop
```

## 🚀 Próximas Funcionalidades

- [ ] Sistema de relacionamentos mais complexo
- [ ] Carreiras específicas com progressão
- [ ] Sistema de doenças e tratamentos
- [ ] Eventos baseados em localização
- [ ] Sistema de conquistas
- [ ] Múltiplos personagens
- [ ] Exportar/importar saves
- [ ] Sons e música de fundo

## 🤝 Contribuição

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo `LICENSE` para mais detalhes.

## 👨‍💻 Desenvolvido por

VidaPixel - Simulador de Vida
Desenvolvido com React, TypeScript e Electron
