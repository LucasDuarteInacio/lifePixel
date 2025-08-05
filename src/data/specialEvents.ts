import { SpecialEvent } from '@/types/game';
import { getRandomName, getRandomGender } from './names';
import { PROBABILITIES } from './probabilities';

/**
 * Eventos especiais interativos que requerem escolha do jogador
 * Todos os textos em português conforme solicitado
 */
export const specialEvents: SpecialEvent[] = [
  // Eventos de amizade
  {
    id: 'new_friend_school',
    title: 'Novo Amigo na Escola',
    description: 'Você conheceu {name} na escola e ele(a) quer ser seu(sua) amigo(a). Ele(a) parece ser uma pessoa legal e vocês têm interesses em comum.',
    type: 'positive',
    choices: [
      {
        id: 'accept_friend',
        text: 'Aceitar a amizade',
        effects: {
          happiness: 8,
          popularity: 5
        },
        relationshipAction: {
          type: 'add',
          relationship: {
            name: '{name}',
            type: 'friend',
            status: 'good',
            age: 0, // Will be set dynamically
            gender: 'male', // Will be set dynamically
            isAlive: true
          }
        }
      },
      {
        id: 'decline_friend',
        text: 'Recusar educadamente',
        effects: {
          happiness: -2,
          popularity: -1
        }
      }
    ],
    probability: PROBABILITIES.specialEvents.newFriendSchool,
    ageRange: { min: 6, max: 17 }
  },
  {
    id: 'new_friend_work',
    title: 'Colega de Trabalho',
    description: 'Você conheceu {name} no trabalho. Vocês se deram muito bem e ele(a) sugeriu que vocês se tornem amigos fora do ambiente profissional.',
    type: 'positive',
    choices: [
      {
        id: 'accept_colleague_friend',
        text: 'Aceitar a amizade',
        effects: {
          happiness: 6,
          popularity: 3
        },
        relationshipAction: {
          type: 'add',
          relationship: {
            name: '{name}',
            type: 'friend',
            status: 'good',
            age: 0,
            gender: 'male',
            isAlive: true
          }
        }
      },
      {
        id: 'keep_professional',
        text: 'Manter apenas relação profissional',
        effects: {
          happiness: -1,
          intelligence: 2
        }
      }
    ],
    requirements: {
      isEmployed: true
    },
    probability: PROBABILITIES.specialEvents.newFriendWork,
    ageRange: { min: 18, max: 65 }
  },
  {
    id: 'neighbor_friend',
    title: 'Vizinho Amigável',
    description: 'Seu vizinho {name} veio até você e sugeriu que vocês se tornem amigos. Ele(a) parece ser uma pessoa confiável e simpática.',
    type: 'positive',
    choices: [
      {
        id: 'accept_neighbor',
        text: 'Aceitar a amizade',
        effects: {
          happiness: 5,
          popularity: 2
        },
        relationshipAction: {
          type: 'add',
          relationship: {
            name: '{name}',
            type: 'friend',
            status: 'good',
            age: 0,
            gender: 'male',
            isAlive: true
          }
        }
      },
      {
        id: 'polite_decline',
        text: 'Agradecer mas manter distância',
        effects: {
          happiness: -1
        }
      }
    ],
    probability: PROBABILITIES.specialEvents.neighborFriend,
    ageRange: { min: 18, max: 80 }
  },
  // Eventos românticos
  {
    id: 'romantic_interest',
    title: 'Interesse Romântico',
    description: '{name} demonstrou interesse romântico em você. Vocês se conheceram recentemente e há uma química interessante entre vocês.',
    type: 'positive',
    choices: [
      {
        id: 'start_dating',
        text: 'Aceitar e começar a namorar',
        effects: {
          happiness: 12,
          popularity: 4
        },
        relationshipAction: {
          type: 'add',
          relationship: {
            name: '{name}',
            type: 'romantic',
            status: 'good',
            age: 0,
            gender: 'male',
            isAlive: true
          }
        },
        riskOutcome: {
          probability: PROBABILITIES.riskOutcomes.romanticInterestBetrayal,
          message: 'Você começou a namorar {name}, mas descobriu que ele(a) já estava em outro relacionamento. Isso causou muito drama e você se sentiu traído(a).',
          effects: {
            happiness: -15,
            popularity: -8
          }
        }
      },
      {
        id: 'just_friends',
        text: 'Sugerir que sejam apenas amigos',
        effects: {
          happiness: 2,
          popularity: 1
        },
        relationshipAction: {
          type: 'add',
          relationship: {
            name: '{name}',
            type: 'friend',
            status: 'neutral',
            age: 0,
            gender: 'male',
            isAlive: true
          }
        },
        riskOutcome: {
          probability: PROBABILITIES.riskOutcomes.romanticInterestRejectionBacklash,
          message: '{name} não aceitou bem ser rejeitado(a) romanticamente e começou a espalhar rumores sobre você.',
          effects: {
            happiness: -8,
            popularity: -12
          },
          relationshipAction: {
            type: 'add',
            relationship: {
              name: '{name}',
              type: 'friend',
              status: 'bad',
              age: 0,
              gender: 'male',
              isAlive: true
            }
          }
        }
      },
      {
        id: 'decline_romantic',
        text: 'Recusar gentilmente',
        effects: {
          happiness: -1
        }
      }
    ],
    probability: PROBABILITIES.specialEvents.romanticInterest,
    ageRange: { min: 16, max: 50 }
  },
  // Eventos de conflito
  {
    id: 'bully_confrontation',
    title: 'Confronto com Valentão',
    description: '{name} está te intimidando na escola. Outros alunos estão observando como você vai reagir a esta situação.',
    type: 'negative',
    choices: [
      {
        id: 'stand_up',
        text: 'Enfrentar o valentão',
        effects: {
          happiness: 5,
          popularity: 8,
          health: -5
        },
        riskOutcome: {
          probability: PROBABILITIES.riskOutcomes.bullyStandUpBackfire,
          message: 'Você tentou enfrentar {name}, mas ele era mais forte que você esperava. Você apanhou na frente de todos e ficou machucado.',
          effects: {
            happiness: -10,
            popularity: -3,
            health: -15
          }
        }
      },
      {
        id: 'avoid_conflict',
        text: 'Evitar o conflito',
        effects: {
          happiness: -8,
          popularity: -5,
          health: 2
        },
        riskOutcome: {
          probability: PROBABILITIES.riskOutcomes.bullyAvoidanceBackfire,
          message: 'Você tentou evitar {name}, mas ele te seguiu e te humilhou ainda mais na frente de outros alunos.',
          effects: {
            happiness: -15,
            popularity: -10,
            health: -2
          }
        }
      },
      {
        id: 'seek_help',
        text: 'Procurar ajuda de um adulto',
        effects: {
          happiness: -2,
          popularity: -2,
          intelligence: 3
        },
        riskOutcome: {
          probability: PROBABILITIES.riskOutcomes.bullySeekHelpBackfire,
          message: 'Você procurou ajuda, mas o adulto não levou a sério. {name} descobriu que você "dedurou" e agora está ainda mais irritado.',
          effects: {
            happiness: -8,
            popularity: -8,
            health: -3
          }
        }
      }
    ],
    probability: PROBABILITIES.specialEvents.bullyConfrontation,
    ageRange: { min: 10, max: 17 }
  },
  // Eventos de oportunidade
  {
    id: 'mentor_opportunity',
    title: 'Oportunidade de Mentoria',
    description: '{name}, uma pessoa experiente na sua área, ofereceu para ser seu(sua) mentor(a). Esta pode ser uma grande oportunidade para seu crescimento profissional.',
    type: 'positive',
    choices: [
      {
        id: 'accept_mentor',
        text: 'Aceitar a mentoria',
        effects: {
          happiness: 6,
          intelligence: 10,
          money: 1000
        },
        relationshipAction: {
          type: 'add',
          relationship: {
            name: '{name}',
            type: 'colleague',
            status: 'good',
            age: 0,
            gender: 'male',
            isAlive: true
          }
        },
        riskOutcome: {
          probability: PROBABILITIES.riskOutcomes.mentorDubiousPractices,
          message: 'Você aceitou {name} como mentor(a), mas descobriu que ele(a) tinha métodos questionáveis e tentou te envolver em práticas duvidosas no trabalho.',
          effects: {
            happiness: -12,
            intelligence: 2,
            money: -500
          },
          relationshipAction: {
            type: 'add',
            relationship: {
              name: '{name}',
              type: 'colleague',
              status: 'bad',
              age: 0,
              gender: 'male',
              isAlive: true
            }
          }
        }
      },
      {
        id: 'decline_mentor',
        text: 'Recusar a oferta',
        effects: {
          happiness: -2,
          intelligence: -1
        }
      }
    ],
    requirements: {
      isEmployed: true,
      minAge: 22
    },
    probability: PROBABILITIES.specialEvents.mentorOpportunity,
    ageRange: { min: 22, max: 45 }
  },
  // Eventos familiares
  {
    id: 'distant_relative',
    title: 'Parente Distante',
    description: 'Você descobriu que tem um parente distante chamado {name} que gostaria de se reconectar com a família. Ele(a) parece ser uma pessoa interessante.',
    type: 'neutral',
    choices: [
      {
        id: 'welcome_relative',
        text: 'Dar as boas-vindas à família',
        effects: {
          happiness: 7,
          popularity: 3
        },
        relationshipAction: {
          type: 'add',
          relationship: {
            name: '{name}',
            type: 'family',
            status: 'good',
            age: 0,
            gender: 'male',
            isAlive: true
          }
        }
      },
      {
        id: 'keep_distance',
        text: 'Manter distância',
        effects: {
          happiness: -1
        }
      }
    ],
    probability: PROBABILITIES.specialEvents.distantRelative,
    ageRange: { min: 18, max: 70 }
  }
];

/**
 * Função para obter eventos especiais disponíveis baseado na idade e características do personagem
 */
export function getAvailableSpecialEvents(age: number, character: any): SpecialEvent[] {
  return specialEvents.filter(event => {
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
 * Função para gerar um evento especial aleatório
 */
export function generateRandomSpecialEvent(age: number, character: any): SpecialEvent | null {
  const availableEvents = getAvailableSpecialEvents(age, character);
  
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
      // Personalizar o evento com um nome aleatório
      const personalizedEvent = personalizeSpecialEvent(event, character);
      return personalizedEvent;
    }
  }

  return personalizeSpecialEvent(availableEvents[0], character); // Fallback
}

/**
 * Personalizar evento especial com nomes aleatórios
 */
function personalizeSpecialEvent(event: SpecialEvent, character: any): SpecialEvent {
  const gender = getRandomGender();
  const nameData = getRandomName(gender);
  const fullName = `${nameData.firstName} ${nameData.lastName}`;
  
  // Calcular idade baseada na idade do personagem
  let age = character.age;
  if (event.id.includes('school')) {
    age = character.age + Math.floor(Math.random() * 3) - 1; // ±1 ano
  } else if (event.id.includes('work')) {
    age = character.age + Math.floor(Math.random() * 10) - 5; // ±5 anos
  } else {
    age = character.age + Math.floor(Math.random() * 20) - 10; // ±10 anos
  }
  age = Math.max(1, age); // Garantir idade mínima

  const personalizedEvent: SpecialEvent = {
    ...event,
    title: event.title.replace('{name}', nameData.firstName),
    description: event.description.replace('{name}', fullName),
    choices: event.choices.map(choice => ({
      ...choice,
      relationshipAction: choice.relationshipAction ? {
        ...choice.relationshipAction,
        relationship: choice.relationshipAction.relationship ? {
          ...choice.relationshipAction.relationship,
          name: fullName,
          age: age,
          gender: gender
        } : undefined
      } : undefined
    }))
  };

  return personalizedEvent;
}