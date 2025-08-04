"use client";

import React from 'react';
import { useGameState } from '@/hooks/useGameState';
import { useHydration } from '@/hooks/useHydration';
import StartScreen from './StartScreen';
import GameScreen from './GameScreen';
import StatusScreen from './StatusScreen';
import EventsScreen from './EventsScreen';
import LoadingScreen from './LoadingScreen';

/**
 * Componente principal do jogo que gerencia todas as telas
 */
export default function GameApp() {
  const { gameState, createCharacter, advanceYear, setScreen, resetGame } = useGameState();
  const isHydrated = useHydration();

  // Don't render anything until we're hydrated
  if (!isHydrated) {
    return <LoadingScreen />;
  }

  // Loading screen
  if (gameState.isLoading) {
    return <LoadingScreen />;
  }

  // Error screen
  if (gameState.error) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-red-900 via-purple-900 to-indigo-900 flex items-center justify-center">
        <div className="bg-white/10 backdrop-blur-lg rounded-2xl p-8 max-w-md w-full shadow-2xl border border-white/20 text-center">
          <h1 className="text-2xl font-bold text-white mb-4">Erro</h1>
          <p className="text-white/80 mb-6">{gameState.error}</p>
          <button
            onClick={resetGame}
            className="bg-red-600 hover:bg-red-700 text-white font-semibold py-3 px-6 rounded-lg transition-colors"
          >
            Reiniciar Jogo
          </button>
        </div>
      </div>
    );
  }

  // Renderizar tela baseada no estado atual
  switch (gameState.currentScreen) {
    case 'start':
      return (
        <StartScreen
          onCreateCharacter={createCharacter}
          onContinueGame={() => setScreen('game')}
          hasExistingGame={!!gameState.character}
        />
      );

    case 'game':
      if (!gameState.character) {
        setScreen('start');
        return null;
      }
      return (
        <GameScreen
          character={gameState.character}
          onAdvanceYear={advanceYear}
          onShowStatus={() => setScreen('status')}
          onShowEvents={() => setScreen('events')}
          onBackToStart={() => setScreen('start')}
        />
      );

    case 'status':
      if (!gameState.character) {
        setScreen('start');
        return null;
      }
      return (
        <StatusScreen
          character={gameState.character}
          onBackToGame={() => setScreen('game')}
        />
      );

    case 'events':
      if (!gameState.character) {
        setScreen('start');
        return null;
      }
      return (
        <EventsScreen
          character={gameState.character}
          onBackToGame={() => setScreen('game')}
        />
      );

    default:
      setScreen('start');
      return null;
  }
} 