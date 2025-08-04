import { EventTemplate } from '@/types/game';

/**
 * Biblioteca de eventos do jogo VidaPixel
 * Cada evento tem uma probabilidade e pode ter requisitos específicos
 */

export const gameEvents: EventTemplate[] = [
  // Eventos da infância (0-12 anos)
  {
    id: 'born',
    title: 'Você nasceu!',
    description: 'Bem-vindo ao mundo! Sua jornada começa agora.',
    type: 'neutral',
    effects: {},
    probability: 100,
    ageRange: { min: 0, max: 0 }
  },
  {
    id: 'first_words',
    title: 'Primeiras palavras',
    description: 'Você disse suas primeiras palavras! Seus pais estão muito orgulhosos.',
    type: 'positive',
    effects: { happiness: 5, intelligence: 2 },
    probability: 80,
    ageRange: { min: 1, max: 2 }
  },
  {
    id: 'first_steps',
    title: 'Primeiros passos',
    description: 'Você deu seus primeiros passos! Uma nova fase da vida começa.',
    type: 'positive',
    effects: { happiness: 3, health: 2 },
    probability: 85,
    ageRange: { min: 1, max: 2 }
  },
  {
    id: 'kindergarten',
    title: 'Jardim de infância',
    description: 'Você começou a frequentar o jardim de infância. Novos amigos e experiências!',
    type: 'positive',
    effects: { happiness: 4, intelligence: 3, popularity: 2 },
    probability: 90,
    ageRange: { min: 3, max: 5 }
  },
  {
    id: 'childhood_illness',
    title: 'Doença infantil',
    description: 'Você ficou doente, mas se recuperou rapidamente.',
    type: 'negative',
    effects: { health: -5, happiness: -2 },
    probability: 30,
    ageRange: { min: 0, max: 12 }
  },
  {
    id: 'talent_discovery',
    title: 'Descoberta de talento',
    description: 'Seus pais descobriram que você tem um talento especial!',
    type: 'positive',
    effects: { happiness: 6, intelligence: 3 },
    probability: 20,
    ageRange: { min: 3, max: 12 }
  },
  {
    id: 'family_support',
    title: 'Apoio da família',
    description: 'Sua família está sempre ao seu lado, te dando apoio e carinho.',
    type: 'positive',
    effects: { happiness: 8, health: 3 },
    probability: 60,
    ageRange: { min: 0, max: 18 }
  },
  {
    id: 'family_conflict',
    title: 'Conflito familiar',
    description: 'Houve uma discussão em casa. As relações estão um pouco tensas.',
    type: 'negative',
    effects: { happiness: -5, popularity: -2 },
    probability: 30,
    ageRange: { min: 10, max: 25 }
  },
  {
    id: 'sibling_birth',
    title: 'Nascimento de irmão',
    description: 'Você ganhou um novo irmão! A família cresceu.',
    type: 'positive',
    effects: { happiness: 4, popularity: 2 },
    probability: 15,
    ageRange: { min: 0, max: 15 }
  },
  {
    id: 'family_trip',
    title: 'Viagem em família',
    description: 'Sua família fez uma viagem incrível! Momentos especiais juntos.',
    type: 'positive',
    effects: { happiness: 10, intelligence: 2 },
    probability: 25,
    ageRange: { min: 5, max: 20 }
  },

  // Eventos da adolescência (13-17 anos)
  {
    id: 'puberty',
    title: 'Puberdade',
    description: 'As mudanças da adolescência começaram. É uma fase complicada.',
    type: 'neutral',
    effects: { happiness: -3, looks: 5 },
    probability: 100,
    ageRange: { min: 13, max: 15 }
  },
  {
    id: 'first_crush',
    title: 'Primeira paixão',
    description: 'Você se apaixonou pela primeira vez!',
    type: 'positive',
    effects: { happiness: 8, popularity: 3 },
    probability: 70,
    ageRange: { min: 13, max: 17 }
  },
  {
    id: 'school_bully',
    title: 'Bullying na escola',
    description: 'Você está sofrendo bullying na escola.',
    type: 'negative',
    effects: { happiness: -10, health: -3, popularity: -5 },
    probability: 25,
    ageRange: { min: 13, max: 17 }
  },
  {
    id: 'academic_excellence',
    title: 'Excelência acadêmica',
    description: 'Você se destacou academicamente!',
    type: 'positive',
    effects: { happiness: 5, intelligence: 8, popularity: 3 },
    probability: 15,
    ageRange: { min: 13, max: 17 }
  },
  {
    id: 'teen_rebellion',
    title: 'Rebeldia adolescente',
    description: 'Você está passando por uma fase rebelde.',
    type: 'negative',
    effects: { happiness: -5, popularity: -3 },
    probability: 40,
    ageRange: { min: 14, max: 17 }
  },

  // Eventos da juventude (18-25 anos)
  {
    id: 'high_school_graduation',
    title: 'Formatura do ensino médio',
    description: 'Você se formou no ensino médio!',
    type: 'positive',
    effects: { happiness: 10, intelligence: 5 },
    probability: 95,
    ageRange: { min: 17, max: 19 }
  },
  {
    id: 'college_admission',
    title: 'Admissão na faculdade',
    description: 'Você foi aceito na faculdade!',
    type: 'positive',
    effects: { happiness: 12, intelligence: 8, money: -5000 },
    requirements: { minIntelligence: 60 },
    probability: 60,
    ageRange: { min: 18, max: 20 }
  },
  {
    id: 'first_job',
    title: 'Primeiro emprego',
    description: 'Você conseguiu seu primeiro emprego!',
    type: 'positive',
    effects: { happiness: 8, money: 2000 },
    probability: 70,
    ageRange: { min: 18, max: 22 }
  },
  {
    id: 'college_dropout',
    title: 'Abandono da faculdade',
    description: 'Você decidiu abandonar a faculdade.',
    type: 'negative',
    effects: { happiness: -8, intelligence: -5 },
    requirements: { hasEducation: true },
    probability: 15,
    ageRange: { min: 19, max: 23 }
  },
  {
    id: 'serious_relationship',
    title: 'Relacionamento sério',
    description: 'Você começou um relacionamento sério.',
    type: 'positive',
    effects: { happiness: 10, popularity: 5 },
    probability: 50,
    ageRange: { min: 20, max: 25 }
  },

  // Eventos da vida adulta (26-50 anos)
  {
    id: 'marriage',
    title: 'Casamento',
    description: 'Você se casou!',
    type: 'positive',
    effects: { happiness: 15, money: -10000 },
    requirements: { hasRelationship: true },
    probability: 40,
    ageRange: { min: 25, max: 35 }
  },
  {
    id: 'child_birth',
    title: 'Nascimento de filho',
    description: 'Você teve um filho!',
    type: 'positive',
    effects: { happiness: 20, money: -5000 },
    requirements: { hasRelationship: true },
    probability: 30,
    ageRange: { min: 25, max: 40 }
  },
  {
    id: 'job_promotion',
    title: 'Promoção no trabalho',
    description: 'Você foi promovido no trabalho!',
    type: 'positive',
    effects: { happiness: 8, money: 5000 },
    requirements: { isEmployed: true },
    probability: 25,
    ageRange: { min: 26, max: 50 }
  },
  {
    id: 'job_loss',
    title: 'Perda do emprego',
    description: 'Você perdeu seu emprego.',
    type: 'negative',
    effects: { happiness: -15, money: -3000 },
    requirements: { isEmployed: true },
    probability: 10,
    ageRange: { min: 26, max: 50 }
  },
  {
    id: 'divorce',
    title: 'Divórcio',
    description: 'Você se divorciou.',
    type: 'negative',
    effects: { happiness: -20, money: -15000 },
    requirements: { hasRelationship: true },
    probability: 15,
    ageRange: { min: 30, max: 50 }
  },
  {
    id: 'inheritance',
    title: 'Herança',
    description: 'Você recebeu uma herança inesperada.',
    type: 'positive',
    effects: { happiness: 10, money: 50000 },
    probability: 5,
    ageRange: { min: 25, max: 60 }
  },

  // Eventos da meia-idade (51-65 anos)
  {
    id: 'midlife_crisis',
    title: 'Crise da meia-idade',
    description: 'Você está passando por uma crise da meia-idade.',
    type: 'negative',
    effects: { happiness: -10, health: -5 },
    probability: 30,
    ageRange: { min: 45, max: 55 }
  },
  {
    id: 'career_peak',
    title: 'Pico da carreira',
    description: 'Você atingiu o pico da sua carreira!',
    type: 'positive',
    effects: { happiness: 15, money: 10000 },
    requirements: { isEmployed: true },
    probability: 20,
    ageRange: { min: 45, max: 60 }
  },
  {
    id: 'health_issues',
    title: 'Problemas de saúde',
    description: 'Você começou a ter problemas de saúde.',
    type: 'negative',
    effects: { health: -15, happiness: -8 },
    probability: 25,
    ageRange: { min: 50, max: 65 }
  },

  // Eventos da terceira idade (66+ anos)
  {
    id: 'retirement',
    title: 'Aposentadoria',
    description: 'Você se aposentou!',
    type: 'positive',
    effects: { happiness: 10, money: 20000 },
    requirements: { isEmployed: true },
    probability: 90,
    ageRange: { min: 65, max: 70 }
  },
  {
    id: 'grandchildren',
    title: 'Netos',
    description: 'Você se tornou avô/avó!',
    type: 'positive',
    effects: { happiness: 15 },
    probability: 40,
    ageRange: { min: 60, max: 75 }
  },
  {
    id: 'serious_illness',
    title: 'Doença grave',
    description: 'Você foi diagnosticado com uma doença grave.',
    type: 'negative',
    effects: { health: -30, happiness: -20 },
    probability: 20,
    ageRange: { min: 70, max: 90 }
  },
  {
    id: 'death',
    title: 'Morte',
    description: 'Sua jornada chegou ao fim.',
    type: 'neutral',
    effects: {},
    probability: 5,
    ageRange: { min: 70, max: 100 }
  }
];

/**
 * Função para obter eventos disponíveis baseado na idade e características do personagem
 */
export function getAvailableEvents(age: number, character: any): EventTemplate[] {
  return gameEvents.filter(event => {
    // Verificar faixa etária
    if (event.ageRange) {
      if (age < event.ageRange.min || age > event.ageRange.max) {
        return false;
      }
    }

    // Verificar requisitos
    if (event.requirements) {
      const req = event.requirements;
      
      if (req.minAge && age < req.minAge) return false;
      if (req.maxAge && age > req.maxAge) return false;
      if (req.minHappiness && character.stats.happiness < req.minHappiness) return false;
      if (req.minHealth && character.stats.health < req.minHealth) return false;
      if (req.minMoney && character.stats.money < req.minMoney) return false;
      if (req.minIntelligence && character.stats.intelligence < req.minIntelligence) return false;
      if (req.isEmployed !== undefined && character.career.isEmployed !== req.isEmployed) return false;
      if (req.hasEducation && character.education.level === 'none') return false;
      if (req.hasRelationship && character.relationships.length === 0) return false;
    }

    return true;
  });
}

/**
 * Função para gerar um evento aleatório baseado na probabilidade
 */
export function generateRandomEvent(age: number, character: any): EventTemplate | null {
  const availableEvents = getAvailableEvents(age, character);
  
  if (availableEvents.length === 0) {
    return null;
  }

  // Calcular probabilidade total
  const totalProbability = availableEvents.reduce((sum, event) => sum + event.probability, 0);
  const random = Math.random() * totalProbability;
  
  let currentSum = 0;
  for (const event of availableEvents) {
    currentSum += event.probability;
    if (random <= currentSum) {
      return event;
    }
  }

  return availableEvents[0]; // Fallback
} 