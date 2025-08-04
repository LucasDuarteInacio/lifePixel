"use client";

import React from 'react';
import { Character } from '@/types/game';
import { useHydration } from '@/hooks/useHydration';

interface GameScreenProps {
  character: Character;
  onAdvanceYear: () => void;
  onShowStatus: () => void;
  onShowEvents: () => void;
  onBackToStart: () => void;
}

/**
 * Tela principal do jogo com interface de simulação
 */
export default function GameScreen({ 
  character, 
  onAdvanceYear, 
  onShowStatus, 
  onShowEvents, 
  onBackToStart 
}: GameScreenProps) {
  const isHydrated = useHydration();

  const latestEvent = character.events[character.events.length - 1];

  const getStatusColor = (value: number) => {
    if (value >= 80) return 'text-green-400';
    if (value >= 60) return 'text-yellow-400';
    if (value >= 40) return 'text-orange-400';
    return 'text-red-400';
  };

  const getMoneyDisplay = (money: number) => {
    if (!isHydrated) return money >= 0 ? `R$ ${money}` : `-R$ ${Math.abs(money)}`;
    
    if (money >= 0) {
      return `R$ ${money.toLocaleString()}`;
    }
    return `-R$ ${Math.abs(money).toLocaleString()}`;
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-900 via-blue-900 to-purple-900">
      {/* Header */}
      <div className="bg-black/20 backdrop-blur-sm border-b border-white/10 p-4">
        <div className="max-w-6xl mx-auto flex items-center justify-between">
          <div className="flex items-center space-x-4">
            <h1 className="text-2xl font-bold text-white">VidaPixel</h1>
            <div className="text-white/80">
              {character.firstName} {character.lastName} - {character.age} anos
            </div>
          </div>
          <div className="flex space-x-2">
            <button
              onClick={onShowStatus}
              className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg transition-colors"
            >
              Status
            </button>
            <button
              onClick={onShowEvents}
              className="px-4 py-2 bg-purple-600 hover:bg-purple-700 text-white rounded-lg transition-colors"
            >
              Eventos
            </button>
            <button
              onClick={onBackToStart}
              className="px-4 py-2 bg-gray-600 hover:bg-gray-700 text-white rounded-lg transition-colors"
            >
              Menu
            </button>
          </div>
        </div>
      </div>

      <div className="max-w-6xl mx-auto p-6">
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          {/* Painel Principal */}
          <div className="lg:col-span-2 space-y-6">
            {/* Informações do Personagem */}
            <div className="bg-white/10 backdrop-blur-lg rounded-xl p-6 border border-white/20">
              <h2 className="text-xl font-semibold text-white mb-4">Seu Personagem</h2>
              <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
                <div className="text-center">
                  <div className="text-white/60 text-sm">Nome</div>
                  <div className="text-white font-medium">{character.firstName} {character.lastName}</div>
                </div>
                <div className="text-center">
                  <div className="text-white/60 text-sm">Idade</div>
                  <div className="text-white font-medium">{character.age} anos</div>
                </div>
                <div className="text-center">
                  <div className="text-white/60 text-sm">País</div>
                  <div className="text-white font-medium">{character.country}</div>
                </div>
                <div className="text-center">
                  <div className="text-white/60 text-sm">Gênero</div>
                  <div className="text-white font-medium">
                    {character.gender === 'male' ? 'Masculino' : 'Feminino'}
                  </div>
                </div>
                <div className="text-center">
                  <div className="text-white/60 text-sm">Emprego</div>
                  <div className="text-white font-medium">
                    {character.career.isEmployed ? character.career.job || 'Empregado' : 'Desempregado'}
                  </div>
                </div>
                <div className="text-center">
                  <div className="text-white/60 text-sm">Educação</div>
                  <div className="text-white font-medium">
                    {character.education.level === 'none' ? 'Nenhuma' : 
                     character.education.level === 'elementary' ? 'Fundamental' :
                     character.education.level === 'high_school' ? 'Ensino Médio' :
                     character.education.level === 'college' ? 'Faculdade' :
                     character.education.level === 'university' ? 'Universidade' :
                     character.education.level === 'phd' ? 'Doutorado' : 'Nenhuma'}
                  </div>
                </div>
              </div>
            </div>

            {/* Status Rápido */}
            <div className="bg-white/10 backdrop-blur-lg rounded-xl p-6 border border-white/20">
              <h2 className="text-xl font-semibold text-white mb-4">Status</h2>
              <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
                <div className="text-center">
                  <div className="text-white/60 text-sm">Felicidade</div>
                  <div className={`font-medium ${getStatusColor(character.stats.happiness)}`}>
                    {character.stats.happiness}%
                  </div>
                </div>
                <div className="text-center">
                  <div className="text-white/60 text-sm">Saúde</div>
                  <div className={`font-medium ${getStatusColor(character.stats.health)}`}>
                    {character.stats.health}%
                  </div>
                </div>
                <div className="text-center">
                  <div className="text-white/60 text-sm">Dinheiro</div>
                  <div className={`font-medium ${character.stats.money >= 0 ? 'text-green-400' : 'text-red-400'}`}>
                    {getMoneyDisplay(character.stats.money)}
                  </div>
                </div>
                <div className="text-center">
                  <div className="text-white/60 text-sm">Inteligência</div>
                  <div className={`font-medium ${getStatusColor(character.stats.intelligence)}`}>
                    {character.stats.intelligence}%
                  </div>
                </div>
                <div className="text-center">
                  <div className="text-white/60 text-sm">Aparência</div>
                  <div className={`font-medium ${getStatusColor(character.stats.looks)}`}>
                    {character.stats.looks}%
                  </div>
                </div>
                <div className="text-center">
                  <div className="text-white/60 text-sm">Popularidade</div>
                  <div className={`font-medium ${getStatusColor(character.stats.popularity)}`}>
                    {character.stats.popularity}%
                  </div>
                </div>
              </div>
            </div>

            {/* Último Evento */}
            {latestEvent && (
              <div className="bg-white/10 backdrop-blur-lg rounded-xl p-6 border border-white/20">
                <h2 className="text-xl font-semibold text-white mb-4">Último Evento</h2>
                <div className={`p-4 rounded-lg ${
                  latestEvent.type === 'positive' ? 'bg-green-500/20 border border-green-500/30' :
                  latestEvent.type === 'negative' ? 'bg-red-500/20 border border-red-500/30' :
                  'bg-blue-500/20 border border-blue-500/30'
                }`}>
                  <div className="text-white font-medium mb-2">{latestEvent.title}</div>
                  <div className="text-white/80">{latestEvent.description}</div>
                  <div className="text-white/60 text-sm mt-2">
                    Idade: {latestEvent.age} anos
                  </div>
                </div>
              </div>
            )}
          </div>

          {/* Painel de Ações */}
          <div className="space-y-6">
            {/* Botão Principal */}
            <div className="bg-white/10 backdrop-blur-lg rounded-xl p-6 border border-white/20">
              <button
                onClick={onAdvanceYear}
                disabled={!character.isAlive}
                className={`w-full py-4 px-6 rounded-lg font-semibold text-lg transition-all transform hover:scale-105 ${
                  character.isAlive
                    ? 'bg-gradient-to-r from-blue-600 to-purple-600 hover:from-blue-700 hover:to-purple-700 text-white'
                    : 'bg-gray-600 text-gray-300 cursor-not-allowed'
                }`}
              >
                {character.isAlive ? 'Avançar Ano' : 'Fim da Vida'}
              </button>
              
              {!character.isAlive && (
                <div className="mt-4 text-center text-white/80">
                  <p>Seu personagem faleceu aos {character.age} anos.</p>
                  <p className="text-sm">Clique em "Menu" para começar uma nova vida.</p>
                </div>
              )}
            </div>

            {/* Estatísticas Rápidas */}
            <div className="bg-white/10 backdrop-blur-lg rounded-xl p-6 border border-white/20">
              <h3 className="text-lg font-semibold text-white mb-4">Resumo</h3>
              <div className="space-y-3">
                <div className="flex justify-between">
                  <span className="text-white/60">Total de Eventos:</span>
                  <span className="text-white font-medium">{character.events.length}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-white/60">Relacionamentos:</span>
                  <span className="text-white font-medium">{character.relationships.length}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-white/60">Salário:</span>
                  <span className="text-white font-medium">
                    {character.career.salary > 0 ? getMoneyDisplay(character.career.salary) : 'R$ 0'}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
} 