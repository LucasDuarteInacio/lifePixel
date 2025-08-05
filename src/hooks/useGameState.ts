import { useState, useEffect } from 'react';
import { GameState, Character, SpecialEvent, SpecialEventChoice, Relationship } from '@/types/game';
import { getRandomName, getRandomGender, generateFamily } from '@/data/names';
import { getRandomCountry } from '@/data/countries';
import { generateRandomEvent } from '@/data/events';
import { generateRandomSpecialEvent } from '@/data/specialEvents';
import { getDeathProbabilityByAge, getSpecialEventChance } from '@/data/probabilities';

const STORAGE_KEY = 'lifePixel_game_state';

/**
 * Hook personalizado para gerenciar o estado do jogo
 * Inclui persistência no localStorage e lógica de criação de personagem
 */
export function useGameState() {
  const [gameState, setGameState] = useState<GameState>({
    character: null,
    currentScreen: 'start',
    isLoading: true,
    error: null,
    activeSpecialEvent: null
  });

  const [isClient, setIsClient] = useState(false);

  // Ensure we're on the client side
  useEffect(() => {
    setIsClient(true);
  }, []);

  // Carregar estado salvo do localStorage
  useEffect(() => {
    if (!isClient) return;

    try {
      const savedState = localStorage.getItem(STORAGE_KEY);
      if (savedState) {
        const parsed = JSON.parse(savedState);
        // Converter strings de data de volta para objetos Date
        if (parsed.character) {
          parsed.character.createdAt = new Date(parsed.character.createdAt);
          parsed.character.lastSaved = new Date(parsed.character.lastSaved);
          parsed.character.events = parsed.character.events.map((event: any) => ({
            ...event,
            timestamp: new Date(event.timestamp)
          }));
        }
        setGameState({ ...parsed, isLoading: false });
      } else {
        setGameState(prev => ({ ...prev, isLoading: false }));
      }
    } catch (error) {
      console.error('Erro ao carregar estado do jogo:', error);
      setGameState(prev => ({ ...prev, isLoading: false, error: 'Erro ao carregar dados' }));
    }
  }, [isClient]);

  // Salvar estado no localStorage sempre que mudar
  useEffect(() => {
    if (!gameState.isLoading && isClient) {
      try {
        localStorage.setItem(STORAGE_KEY, JSON.stringify(gameState));
      } catch (error) {
        console.error('Erro ao salvar estado do jogo:', error);
      }
    }
  }, [gameState, isClient]);

  /**
   * Criar um novo personagem
   */
  const createCharacter = (customData?: {
    firstName?: string;
    lastName?: string;
    gender?: 'male' | 'female';
    country?: string;
  }) => {
    // Only generate random data on the client side
    if (!isClient) return;

    const gender = customData?.gender || getRandomGender();
    const nameData = customData?.firstName ? 
      { firstName: customData.firstName, lastName: customData.lastName || 'Silva' } :
      getRandomName(gender);
    const country = customData?.country || getRandomCountry().name;

    const character: Character = {
      id: `char_${isClient ? Date.now() : 0}_${isClient ? Math.random().toString(36).substr(2, 9) : 'temp'}`,
      firstName: nameData.firstName,
      lastName: nameData.lastName,
      gender,
      country,
      age: 0,
      isAlive: true,
      stats: {
        happiness: 50,
        health: 100,
        money: 0,
        intelligence: 50,
        looks: 50,
        popularity: 30
      },
      relationships: generateFamily(0, nameData.lastName), // Gerar família inicial
      career: {
        job: null,
        salary: 0,
        experience: 0,
        isEmployed: false
      },
      education: {
        level: 'none',
        institution: null,
        graduationYear: null
      },
      events: [],
      createdAt: isClient ? new Date() : new Date(0),
      lastSaved: isClient ? new Date() : new Date(0)
    };

    setGameState(prev => ({
      ...prev,
      character,
      currentScreen: 'game'
    }));
  };

  /**
   * Avançar um ano na vida do personagem
   */
  const advanceYear = () => {
    if (!gameState.character || !gameState.character.isAlive || !isClient) return;

    const currentCharacter = gameState.character;
    const newAge = currentCharacter.age + 1;
    
    // Process relationships first to avoid circular reference
    const updatedRelationships = currentCharacter.relationships.map(relationship => {
      const updatedRelationship: Relationship = {
        ...relationship,
        // Only age living people
        age: relationship.isAlive ? relationship.age + 1 : relationship.age
      };
      
      return updatedRelationship;
    });

    // Create a deep copy of the character to ensure proper state immutability
    const character: Character = {
      ...currentCharacter,
      age: newAge,
      lastSaved: new Date(),
      stats: { ...currentCharacter.stats },
      career: { ...currentCharacter.career },
      education: { ...currentCharacter.education },
      events: [...currentCharacter.events],
      relationships: updatedRelationships
    };

    // Now process death checks for relationships
    character.relationships.forEach(relationship => {
      // Calculate death probability based on age (only if still alive)
      if (relationship.isAlive) {
        const deathProbability = getDeathProbabilityByAge(relationship.age);
        
        // Roll for death
        if (Math.random() < deathProbability) {
          relationship.isAlive = false;
          
          // Add a death event to character's history
          const deathEvent = {
            id: `relationship_death_${relationship.id}_${character.age}_${Date.now()}`,
            title: `${relationship.name} faleceu`,
            description: `Seu ${getRelationshipTypeInPortuguese(relationship.type)} ${relationship.name} faleceu aos ${relationship.age} anos.`,
            age: character.age,
            type: 'negative' as const,
            effects: {
              happiness: -10,
              health: -5
            },
            timestamp: new Date()
          };
          
          console.log(deathEvent)
          character.events.push(deathEvent);
          
          // Apply grief effects
          applyEventEffects(character, deathEvent.effects);
        }
      }
    });


    // Verificar se deve gerar um evento especial
    const shouldGenerateSpecialEvent = Math.random() < getSpecialEventChance();
    if (shouldGenerateSpecialEvent) {
      const specialEvent = generateRandomSpecialEvent(character.age, character);
      if (specialEvent) {
        // Save the character state with aged relationships before showing special event
        setGameState(prev => ({
          ...prev,
          character,
          activeSpecialEvent: specialEvent
        }));
        return; // Não processar evento normal se há evento especial
      }
    }

    // Gerar evento aleatório para este ano
    const event = generateRandomEvent(character.age, character);
    if (event) {
      const gameEvent = {
        id: `${event.id}_${character.age}_${isClient ? Date.now() : 0}_${isClient ? Math.random().toString(36).substr(2, 5) : 'temp'}`,
        title: event.title,
        description: event.description,
        age: character.age,
        type: event.type,
        effects: event.effects,
        timestamp: isClient ? new Date() : new Date(0)
      };

      character.events.push(gameEvent);

      // Aplicar efeitos do evento
      applyEventEffects(character, event.effects);

      // Verificar se o personagem morreu
      if (event.id === 'death' || character.stats.health <= 0) {
        character.isAlive = false;
      }
    }

    setGameState(prev => ({
      ...prev,
      character
    }));
  };

  /**
   * Aplicar efeitos de um evento aos stats do personagem
   */
  const applyEventEffects = (character: Character, effects: any) => {
    if (effects.happiness) {
      character.stats.happiness = Math.max(0, Math.min(100, character.stats.happiness + effects.happiness));
    }
    if (effects.health) {
      character.stats.health = Math.max(0, Math.min(100, character.stats.health + effects.health));
    }
    if (effects.money) {
      character.stats.money += effects.money;
    }
    if (effects.intelligence) {
      character.stats.intelligence = Math.max(0, Math.min(100, character.stats.intelligence + effects.intelligence));
    }
    if (effects.looks) {
      character.stats.looks = Math.max(0, Math.min(100, character.stats.looks + effects.looks));
    }
    if (effects.popularity) {
      character.stats.popularity = Math.max(0, Math.min(100, character.stats.popularity + effects.popularity));
    }
  };


  /**
   * Get relationship type name in Portuguese
   */
  const getRelationshipTypeInPortuguese = (type: string): string => {
    switch (type) {
      case 'father': return 'pai';
      case 'mother': return 'mãe';
      case 'brother': return 'irmão';
      case 'sister': return 'irmã';
      case 'son': return 'filho';
      case 'daughter': return 'filha';
      case 'family': return 'familiar';
      case 'friend': return 'amigo(a)';
      case 'romantic': return 'parceiro(a)';
      case 'colleague': return 'colega';
      default: return 'conhecido(a)';
    }
  };

  /**
   * Processar escolha de evento especial
   */
  const handleSpecialEventChoice = (choice: SpecialEventChoice) => {
    if (!gameState.character || !gameState.activeSpecialEvent) return;

    // Create a deep copy of the character to ensure proper state immutability
    const character: Character = {
      ...gameState.character,
      stats: { ...gameState.character.stats },
      career: { ...gameState.character.career },
      education: { ...gameState.character.education },
      events: [...gameState.character.events],
      relationships: gameState.character.relationships.map(rel => ({ ...rel }))
    };
    let actualEffects = choice.effects;
    let actualRelationshipAction = choice.relationshipAction;
    let outcomeMessage = `${gameState.activeSpecialEvent.description} - Escolha: ${choice.text}`;
    let eventType = gameState.activeSpecialEvent.type;

    // Verificar se há risco de resultado negativo
    if (choice.riskOutcome && isClient) {
      const riskRoll = Math.random() * 100;
      if (riskRoll < choice.riskOutcome.probability) {
        // Resultado negativo ocorreu
        actualEffects = choice.riskOutcome.effects;
        actualRelationshipAction = choice.riskOutcome.relationshipAction || choice.relationshipAction;
        outcomeMessage = choice.riskOutcome.message;
        eventType = 'negative'; // Forçar tipo negativo quando dá errado
        
        // Personalizar mensagem com nome do evento
        const eventName = gameState.activeSpecialEvent.title.split(' ')[0]; // Pegar primeiro nome
        outcomeMessage = outcomeMessage.replace('{name}', eventName);
      }
    }
    
    // Aplicar efeitos (normais ou de risco)
    applyEventEffects(character, actualEffects);

    // Processar ação de relacionamento se existir
    if (actualRelationshipAction) {
      const action = actualRelationshipAction;
      
      if (action.type === 'add' && action.relationship) {
        // Adicionar novo relacionamento
        const newRelationship: Relationship = {
          id: `rel_${isClient ? Date.now() : 0}_${isClient ? Math.random().toString(36).substr(2, 9) : 'temp'}`,
          ...action.relationship
        };
        character.relationships.push(newRelationship);
      } else if (action.type === 'remove' && action.relationshipId) {
        // Remover relacionamento
        character.relationships = character.relationships.filter(rel => rel.id !== action.relationshipId);
      } else if (action.type === 'modify' && action.relationshipId && action.statusChange) {
        // Modificar relacionamento existente
        const relationshipIndex = character.relationships.findIndex(rel => rel.id === action.relationshipId);
        if (relationshipIndex !== -1) {
          character.relationships[relationshipIndex].status = action.statusChange;
        }
      }
    }

    // Adicionar evento ao histórico
    const gameEvent = {
      id: `special_${gameState.activeSpecialEvent.id}_${character.age}_${isClient ? Date.now() : 0}`,
      title: gameState.activeSpecialEvent.title,
      description: outcomeMessage,
      age: character.age,
      type: eventType,
      effects: actualEffects,
      timestamp: isClient ? new Date() : new Date(0)
    };

    character.events.push(gameEvent);
    character.lastSaved = new Date();

    // Limpar evento especial ativo
    setGameState(prev => ({
      ...prev,
      character,
      activeSpecialEvent: null
    }));
  };

  /**
   * Cancelar evento especial (fechar modal sem escolher)
   */
  const cancelSpecialEvent = () => {
    setGameState(prev => ({
      ...prev,
      activeSpecialEvent: null
    }));
  };

  /**
   * Mudar para uma tela específica
   */
  const setScreen = (screen: GameState['currentScreen']) => {
    setGameState(prev => ({
      ...prev,
      currentScreen: screen
    }));
  };

  /**
   * Reiniciar o jogo (deletar dados salvos)
   */
  const resetGame = () => {
    localStorage.removeItem(STORAGE_KEY);
    setGameState({
      character: null,
      currentScreen: 'start',
      isLoading: false,
      error: null,
      activeSpecialEvent: null
    });
  };

  return {
    gameState,
    createCharacter,
    advanceYear,
    setScreen,
    resetGame,
    handleSpecialEventChoice,
    cancelSpecialEvent
  };
} 