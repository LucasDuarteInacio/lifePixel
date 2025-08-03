"use client";

import React from 'react';
import { Character } from '@/types/game';

interface EventsScreenProps {
  character: Character;
  onBackToGame: () => void;
}

/**
 * Tela de eventos com lista cronológica de todos os eventos da vida
 */
export default function EventsScreen({ character, onBackToGame }: EventsScreenProps) {
  const sortedEvents = [...character.events].sort((a, b) => a.age - b.age);

  const getEventTypeColor = (type: string) => {
    switch (type) {
      case 'positive':
        return 'bg-green-500/20 border-green-500/30 text-green-400';
      case 'negative':
        return 'bg-red-500/20 border-red-500/30 text-red-400';
      case 'neutral':
        return 'bg-blue-500/20 border-blue-500/30 text-blue-400';
      default:
        return 'bg-gray-500/20 border-gray-500/30 text-gray-400';
    }
  };

  const getEventTypeIcon = (type: string) => {
    switch (type) {
      case 'positive':
        return '✅';
      case 'negative':
        return '❌';
      case 'neutral':
        return 'ℹ️';
      default:
        return '📝';
    }
  };

  const formatDate = (date: Date) => {
    return date.toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-900 via-blue-900 to-purple-900">
      {/* Header */}
      <div className="bg-black/20 backdrop-blur-sm border-b border-white/10 p-4">
        <div className="max-w-6xl mx-auto flex items-center justify-between">
          <div className="flex items-center space-x-4">
            <h1 className="text-2xl font-bold text-white">Histórico de Eventos</h1>
            <div className="text-white/80">
              {character.name} - {character.age} anos
            </div>
          </div>
          <button
            onClick={onBackToGame}
            className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg transition-colors"
          >
            Voltar ao Jogo
          </button>
        </div>
      </div>

      <div className="max-w-4xl mx-auto p-6">
        {/* Estatísticas dos Eventos */}
        <div className="bg-white/10 backdrop-blur-lg rounded-xl p-6 border border-white/20 mb-6">
          <h2 className="text-xl font-semibold text-white mb-4">Resumo dos Eventos</h2>
          <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
            <div className="text-center">
              <div className="text-white/60 text-sm">Total de Eventos</div>
              <div className="text-white font-bold text-2xl">{character.events.length}</div>
            </div>
            <div className="text-center">
              <div className="text-white/60 text-sm">Eventos Positivos</div>
              <div className="text-green-400 font-bold text-2xl">
                {character.events.filter(e => e.type === 'positive').length}
              </div>
            </div>
            <div className="text-center">
              <div className="text-white/60 text-sm">Eventos Negativos</div>
              <div className="text-red-400 font-bold text-2xl">
                {character.events.filter(e => e.type === 'negative').length}
              </div>
            </div>
            <div className="text-center">
              <div className="text-white/60 text-sm">Eventos Neutros</div>
              <div className="text-blue-400 font-bold text-2xl">
                {character.events.filter(e => e.type === 'neutral').length}
              </div>
            </div>
          </div>
        </div>

        {/* Lista de Eventos */}
        <div className="bg-white/10 backdrop-blur-lg rounded-xl p-6 border border-white/20">
          <h2 className="text-xl font-semibold text-white mb-6">Cronologia da Vida</h2>
          
          {sortedEvents.length > 0 ? (
            <div className="space-y-4">
              {sortedEvents.map((event, index) => (
                <div
                  key={event.id}
                  className={`p-4 rounded-lg border ${getEventTypeColor(event.type)}`}
                >
                  <div className="flex items-start justify-between">
                    <div className="flex items-start space-x-3 flex-1">
                      <div className="text-2xl">{getEventTypeIcon(event.type)}</div>
                      <div className="flex-1">
                        <div className="flex items-center space-x-2 mb-1">
                          <h3 className="font-semibold text-white">{event.title}</h3>
                          <span className="text-white/60 text-sm">({event.age} anos)</span>
                        </div>
                        <p className="text-white/80 mb-2">{event.description}</p>
                        
                        {/* Efeitos do Evento */}
                        {Object.keys(event.effects).length > 0 && (
                          <div className="flex flex-wrap gap-2">
                            {event.effects.happiness && (
                              <span className={`px-2 py-1 rounded text-xs ${
                                event.effects.happiness > 0 ? 'bg-green-500/20 text-green-400' : 'bg-red-500/20 text-red-400'
                              }`}>
                                Felicidade {event.effects.happiness > 0 ? '+' : ''}{event.effects.happiness}
                              </span>
                            )}
                            {event.effects.health && (
                              <span className={`px-2 py-1 rounded text-xs ${
                                event.effects.health > 0 ? 'bg-green-500/20 text-green-400' : 'bg-red-500/20 text-red-400'
                              }`}>
                                Saúde {event.effects.health > 0 ? '+' : ''}{event.effects.health}
                              </span>
                            )}
                            {event.effects.money && (
                              <span className={`px-2 py-1 rounded text-xs ${
                                event.effects.money > 0 ? 'bg-green-500/20 text-green-400' : 'bg-red-500/20 text-red-400'
                              }`}>
                                Dinheiro {event.effects.money > 0 ? '+' : ''}R$ {event.effects.money.toLocaleString()}
                              </span>
                            )}
                            {event.effects.intelligence && (
                              <span className={`px-2 py-1 rounded text-xs ${
                                event.effects.intelligence > 0 ? 'bg-green-500/20 text-green-400' : 'bg-red-500/20 text-red-400'
                              }`}>
                                Inteligência {event.effects.intelligence > 0 ? '+' : ''}{event.effects.intelligence}
                              </span>
                            )}
                            {event.effects.looks && (
                              <span className={`px-2 py-1 rounded text-xs ${
                                event.effects.looks > 0 ? 'bg-green-500/20 text-green-400' : 'bg-red-500/20 text-red-400'
                              }`}>
                                Aparência {event.effects.looks > 0 ? '+' : ''}{event.effects.looks}
                              </span>
                            )}
                            {event.effects.popularity && (
                              <span className={`px-2 py-1 rounded text-xs ${
                                event.effects.popularity > 0 ? 'bg-green-500/20 text-green-400' : 'bg-red-500/20 text-red-400'
                              }`}>
                                Popularidade {event.effects.popularity > 0 ? '+' : ''}{event.effects.popularity}
                              </span>
                            )}
                          </div>
                        )}
                      </div>
                    </div>
                    <div className="text-white/40 text-xs ml-4">
                      {formatDate(event.timestamp)}
                    </div>
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <div className="text-center py-12">
              <div className="text-white/60 text-lg mb-2">Nenhum evento registrado ainda</div>
              <div className="text-white/40 text-sm">
                Avance os anos para ver eventos acontecerem na vida do seu personagem!
              </div>
            </div>
          )}
        </div>

        {/* Legenda */}
        <div className="mt-6 bg-white/5 rounded-lg p-4 border border-white/10">
          <h3 className="text-white font-semibold mb-3">Legenda</h3>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4 text-sm">
            <div className="flex items-center space-x-2">
              <span className="text-2xl">✅</span>
              <span className="text-white/80">Eventos Positivos</span>
            </div>
            <div className="flex items-center space-x-2">
              <span className="text-2xl">❌</span>
              <span className="text-white/80">Eventos Negativos</span>
            </div>
            <div className="flex items-center space-x-2">
              <span className="text-2xl">ℹ️</span>
              <span className="text-white/80">Eventos Neutros</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
} 