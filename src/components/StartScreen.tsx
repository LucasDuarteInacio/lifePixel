"use client";

import React, { useState } from 'react';

import { getRandomCountry } from '@/data/countries';
import { countries } from '@/data/countries';
import { useHydration } from '@/hooks/useHydration';

interface StartScreenProps {
  onCreateCharacter: (data?: {
    firstName?: string;
    lastName?: string;
    gender?: 'male' | 'female';
    country?: string;
  }) => void;
  onContinueGame: () => void;
  hasExistingGame: boolean;
  character?: any; // Para mostrar histórico se existir
}

/**
 * Tela inicial do jogo com design moderno e histórico de eventos
 */
export default function StartScreen({ 
  onCreateCharacter, 
  onContinueGame, 
  hasExistingGame,
  character
}: StartScreenProps) {
  const [showCustomForm, setShowCustomForm] = useState(false);
  const [customFirstName, setCustomFirstName] = useState('');
  const [customLastName, setCustomLastName] = useState('');
  const [customGender, setCustomGender] = useState<'male' | 'female'>('male');
  const [customCountry, setCustomCountry] = useState('');
  const [activeTab, setActiveTab] = useState<'main' | 'history'>('main');
  const isHydrated = useHydration();

  const handleRandomCharacter = () => {
    if (!isHydrated) return;
    onCreateCharacter();
  };

  const handleCustomCharacter = () => {
    if (!isHydrated) return;
    
    if (customFirstName.trim()) {
      onCreateCharacter({
        firstName: customFirstName.trim(),
        lastName: customLastName.trim() || 'Silva',
        gender: customGender,
        country: customCountry || getRandomCountry().name
      });
    }
  };

  const getEventTypeIcon = (type: string) => {
    switch (type) {
      case 'positive': return '✅';
      case 'negative': return '❌';
      case 'neutral': return 'ℹ️';
      default: return '📝';
    }
  };

  const getEventTypeColor = (type: string) => {
    switch (type) {
      case 'positive': return 'border-green-500/30 bg-green-500/10';
      case 'negative': return 'border-red-500/30 bg-red-500/10';
      case 'neutral': return 'border-blue-500/30 bg-blue-500/10';
      default: return 'border-gray-500/30 bg-gray-500/10';
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-purple-900 to-slate-900">
      {/* Header */}
      <div className="bg-black/20 backdrop-blur-sm border-b border-white/10 p-6">
        <div className="max-w-7xl mx-auto">
          <div className="text-center">
            <h1 className="text-5xl font-bold bg-gradient-to-r from-blue-400 via-purple-400 to-pink-400 bg-clip-text text-transparent mb-2">
              LifePixel
            </h1>
            <p className="text-white/80 text-xl">Simulador de Vida</p>
          </div>
        </div>
      </div>

      {/* Navigation Tabs */}
      <div className="max-w-7xl mx-auto px-6 py-4">
        <div className="flex space-x-1 bg-white/10 rounded-lg p-1 backdrop-blur-sm">
          <button
            onClick={() => setActiveTab('main')}
            className={`flex-1 py-2 px-4 rounded-md font-medium transition-all ${
              activeTab === 'main'
                ? 'bg-white/20 text-white shadow-sm'
                : 'text-white/70 hover:text-white'
            }`}
          >
            Início
          </button>
          {hasExistingGame && (
            <button
              onClick={() => setActiveTab('history')}
              className={`flex-1 py-2 px-4 rounded-md font-medium transition-all ${
                activeTab === 'history'
                  ? 'bg-white/20 text-white shadow-sm'
                  : 'text-white/70 hover:text-white'
              }`}
            >
              Histórico
            </button>
          )}
        </div>
      </div>

      {/* Main Content */}
      <div className="max-w-7xl mx-auto px-6 pb-8">
        {activeTab === 'main' && (
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
            {/* Left Panel - Character Creation */}
            <div className="space-y-6">
              {/* Welcome Card */}
              <div className="bg-white/10 backdrop-blur-lg rounded-2xl p-8 border border-white/20">
                <div className="text-center mb-6">
                  <div className="text-6xl mb-4">🎮</div>
                  <h2 className="text-2xl font-bold text-white mb-2">Bem-vindo ao lifePixel</h2>
                  <p className="text-white/80">
                    Crie seu personagem e viva uma vida virtual cheia de surpresas!
                  </p>
                </div>

                {hasExistingGame && (
                  <button
                    onClick={onContinueGame}
                    className="w-full bg-gradient-to-r from-green-500 to-emerald-600 hover:from-green-600 hover:to-emerald-700 text-white font-semibold py-4 px-6 rounded-xl mb-4 transition-all transform hover:scale-105 shadow-lg"
                  >
                    🎯 Continuar Jogo Salvo
                  </button>
                )}

                <div className="space-y-4">
                  <button
                    onClick={handleRandomCharacter}
                    className="w-full bg-gradient-to-r from-blue-500 to-purple-600 hover:from-blue-600 hover:to-purple-700 text-white font-semibold py-4 px-6 rounded-xl transition-all transform hover:scale-105 shadow-lg"
                  >
                    🎲 Iniciar Nova Vida (Aleatório)
                  </button>

                  <button
                    onClick={() => setShowCustomForm(!showCustomForm)}
                    className="w-full bg-gradient-to-r from-purple-500 to-pink-600 hover:from-purple-600 hover:to-pink-700 text-white font-semibold py-4 px-6 rounded-xl transition-all transform hover:scale-105 shadow-lg"
                  >
                    ✨ Criar Personagem Personalizado
                  </button>
                </div>
              </div>

              {/* Custom Form */}
              {showCustomForm && (
                <div className="bg-white/10 backdrop-blur-lg rounded-2xl p-8 border border-white/20">
                  <h3 className="text-xl font-bold text-white mb-6">Personalizar Personagem</h3>
                  
                  <div className="space-y-4">
                    <div>
                      <label className="block text-white text-sm font-medium mb-2">
                        Primeiro Nome *
                      </label>
                      <input
                        type="text"
                        value={customFirstName}
                        onChange={(e) => setCustomFirstName(e.target.value)}
                        placeholder="Digite o primeiro nome"
                        className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-xl text-white placeholder-white/50 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
                      />
                    </div>

                    <div>
                      <label className="block text-white text-sm font-medium mb-2">
                        Sobrenome
                      </label>
                      <input
                        type="text"
                        value={customLastName}
                        onChange={(e) => setCustomLastName(e.target.value)}
                        placeholder="Digite o sobrenome (opcional)"
                        className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-xl text-white placeholder-white/50 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
                      />
                    </div>

                    <div className="grid grid-cols-2 gap-4">
                      <div>
                        <label className="block text-white text-sm font-medium mb-2">
                          Gênero
                        </label>
                        <select
                          value={customGender}
                          onChange={(e) => setCustomGender(e.target.value as 'male' | 'female')}
                          className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-xl text-white focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
                        >
                          <option value="male">Masculino</option>
                          <option value="female">Feminino</option>
                        </select>
                      </div>

                      <div>
                        <label className="block text-white text-sm font-medium mb-2">
                          País
                        </label>
                        <select
                          value={customCountry}
                          onChange={(e) => setCustomCountry(e.target.value)}
                          className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-xl text-white focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
                        >
                          <option value="">País aleatório</option>
                          {countries.map((country) => (
                            <option key={country.code} value={country.name}>
                              {country.name}
                            </option>
                          ))}
                        </select>
                      </div>
                    </div>

                    <button
                      onClick={handleCustomCharacter}
                      disabled={!customFirstName.trim()}
                      className="w-full bg-gradient-to-r from-green-500 to-emerald-600 hover:from-green-600 hover:to-emerald-700 disabled:from-gray-500 disabled:to-gray-600 disabled:cursor-not-allowed text-white font-semibold py-4 px-6 rounded-xl transition-all transform hover:scale-105 shadow-lg"
                    >
                      🚀 Criar Personagem
                    </button>
                  </div>
                </div>
              )}
            </div>

            {/* Right Panel - Features */}
            <div className="space-y-6">
              {/* Features Card */}
              <div className="bg-white/10 backdrop-blur-lg rounded-2xl p-8 border border-white/20">
                <h3 className="text-xl font-bold text-white mb-6">🎯 Recursos do Jogo</h3>
                
                <div className="space-y-4">
                  <div className="flex items-start space-x-3">
                    <div className="text-2xl">🎲</div>
                    <div>
                      <h4 className="text-white font-semibold">Eventos Aleatórios</h4>
                      <p className="text-white/70 text-sm">Mais de 30 eventos diferentes que afetam sua vida</p>
                    </div>
                  </div>
                  
                  <div className="flex items-start space-x-3">
                    <div className="text-2xl">📊</div>
                    <div>
                      <h4 className="text-white font-semibold">Estatísticas Detalhadas</h4>
                      <p className="text-white/70 text-sm">Acompanhe felicidade, saúde, dinheiro e muito mais</p>
                    </div>
                  </div>
                  
                  <div className="flex items-start space-x-3">
                    <div className="text-2xl">👥</div>
                    <div>
                      <h4 className="text-white font-semibold">Relacionamentos</h4>
                      <p className="text-white/70 text-sm">Construa família e amizades ao longo da vida</p>
                    </div>
                  </div>
                  
                  <div className="flex items-start space-x-3">
                    <div className="text-2xl">💼</div>
                    <div>
                      <h4 className="text-white font-semibold">Carreira e Educação</h4>
                      <p className="text-white/70 text-sm">Desenvolva sua carreira e educação</p>
                    </div>
                  </div>
                </div>
              </div>

              {/* Stats Card */}
              <div className="bg-white/10 backdrop-blur-lg rounded-2xl p-8 border border-white/20">
                <h3 className="text-xl font-bold text-white mb-6">📈 Estatísticas</h3>
                
                <div className="grid grid-cols-2 gap-4">
                  <div className="text-center">
                    <div className="text-3xl font-bold text-blue-400">30+</div>
                    <div className="text-white/70 text-sm">Eventos</div>
                  </div>
                  <div className="text-center">
                    <div className="text-3xl font-bold text-green-400">6</div>
                    <div className="text-white/70 text-sm">Estatísticas</div>
                  </div>
                  <div className="text-center">
                    <div className="text-3xl font-bold text-purple-400">∞</div>
                    <div className="text-white/70 text-sm">Possibilidades</div>
                  </div>
                  <div className="text-center">
                    <div className="text-3xl font-bold text-pink-400">100%</div>
                    <div className="text-white/70 text-sm">Diversão</div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        )}

        {/* History Tab */}
        {activeTab === 'history' && character && (
          <div className="space-y-6">
            {/* Character Info */}
            <div className="bg-white/10 backdrop-blur-lg rounded-2xl p-8 border border-white/20">
              <div className="flex items-center justify-between mb-6">
                <div>
                  <h2 className="text-2xl font-bold text-white">
                    {character.firstName} {character.lastName}
                  </h2>
                  <p className="text-white/70">
                    {character.age} anos • {character.country} • {character.gender === 'male' ? 'Masculino' : 'Feminino'}
                  </p>
                </div>
                <div className="text-right">
                  <div className={`text-lg font-semibold ${character.isAlive ? 'text-green-400' : 'text-red-400'}`}>
                    {character.isAlive ? 'Vivo' : 'Falecido'}
                  </div>
                  <div className="text-white/60 text-sm">
                    {character.events.length} eventos
                  </div>
                </div>
              </div>

              {/* Quick Stats */}
              <div className="grid grid-cols-2 md:grid-cols-6 gap-4">
                <div className="text-center">
                  <div className="text-white/60 text-sm">Felicidade</div>
                  <div className="text-white font-bold">{character.stats.happiness}%</div>
                </div>
                <div className="text-center">
                  <div className="text-white/60 text-sm">Saúde</div>
                  <div className="text-white font-bold">{character.stats.health}%</div>
                </div>
                <div className="text-center">
                  <div className="text-white/60 text-sm">Dinheiro</div>
                  <div className="text-white font-bold">
                    R$ {character.stats.money >= 0 ? character.stats.money.toLocaleString() : `-${Math.abs(character.stats.money).toLocaleString()}`}
                  </div>
                </div>
                <div className="text-center">
                  <div className="text-white/60 text-sm">Inteligência</div>
                  <div className="text-white font-bold">{character.stats.intelligence}%</div>
                </div>
                <div className="text-center">
                  <div className="text-white/60 text-sm">Aparência</div>
                  <div className="text-white font-bold">{character.stats.looks}%</div>
                </div>
                <div className="text-center">
                  <div className="text-white/60 text-sm">Popularidade</div>
                  <div className="text-white font-bold">{character.stats.popularity}%</div>
                </div>
              </div>
            </div>

            {/* Events History */}
            <div className="bg-white/10 backdrop-blur-lg rounded-2xl p-8 border border-white/20">
              <h3 className="text-xl font-bold text-white mb-6">📜 Cronologia da Vida</h3>
              
              {character.events.length > 0 ? (
                <div className="space-y-4 max-h-96 overflow-y-auto">
                  {[...character.events]
                    .sort((a, b) => a.age - b.age)
                    .map((event) => (
                      <div
                        key={event.id}
                        className={`p-4 rounded-xl border ${getEventTypeColor(event.type)}`}
                      >
                        <div className="flex items-start space-x-3">
                          <div className="text-2xl">{getEventTypeIcon(event.type)}</div>
                          <div className="flex-1">
                            <div className="flex items-center space-x-2 mb-1">
                              <h4 className="font-semibold text-white">{event.title}</h4>
                              <span className="text-white/60 text-sm">({event.age} anos)</span>
                            </div>
                            <p className="text-white/80 text-sm">{event.description}</p>
                          </div>
                        </div>
                      </div>
                    ))}
                </div>
              ) : (
                <div className="text-center py-12">
                  <div className="text-6xl mb-4">📝</div>
                  <div className="text-white/60 text-lg mb-2">Nenhum evento registrado ainda</div>
                  <div className="text-white/40 text-sm">
                    Avance os anos para ver eventos acontecerem na vida do seu personagem!
                  </div>
                </div>
              )}
            </div>
          </div>
        )}
      </div>

      {/* Footer */}
      <div className="mt-12 text-center pb-8">
        <p className="text-white/40 text-sm">
          Desenvolvido com React, TypeScript e Electron
        </p>
      </div>
    </div>
  );
} 