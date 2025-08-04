"use client";

import { useState, useEffect } from 'react';
import { GameState, Character } from '@/types/game';
import { getRandomName, getRandomGender, generateFamily } from '@/data/names';
import { getRandomCountry } from '@/data/countries';
import { generateRandomEvent } from '@/data/events';

const STORAGE_KEY = 'vidapixel_game_state';

/**
 * Hook personalizado para gerenciar o estado do jogo
 * Inclui persistência no localStorage e lógica de criação de personagem
 */
export function useGameState() {
  const [gameState, setGameState] = useState<GameState>({
    character: null,
    currentScreen: 'start',
    isLoading: true,
    error: null
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

    const character = { ...gameState.character };
    character.age += 1;
    character.lastSaved = new Date();

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
      if (event.effects.happiness) {
        character.stats.happiness = Math.max(0, Math.min(100, character.stats.happiness + event.effects.happiness));
      }
      if (event.effects.health) {
        character.stats.health = Math.max(0, Math.min(100, character.stats.health + event.effects.health));
      }
      if (event.effects.money) {
        character.stats.money += event.effects.money;
      }
      if (event.effects.intelligence) {
        character.stats.intelligence = Math.max(0, Math.min(100, character.stats.intelligence + event.effects.intelligence));
      }
      if (event.effects.looks) {
        character.stats.looks = Math.max(0, Math.min(100, character.stats.looks + event.effects.looks));
      }
      if (event.effects.popularity) {
        character.stats.popularity = Math.max(0, Math.min(100, character.stats.popularity + event.effects.popularity));
      }

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
      error: null
    });
  };

  return {
    gameState,
    createCharacter,
    advanceYear,
    setScreen,
    resetGame
  };
} 