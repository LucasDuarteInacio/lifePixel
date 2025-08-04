import React from 'react';
import { Character } from '@/types/game';
import { useHydration } from '@/hooks/useHydration';

interface GameScreenProps {
  character: Character;
  onAdvanceYear: () => void;
  onShowStatus: () => void;
  onBackToStart: () => void;
}

/**
 * Tela principal do jogo com interface de simulação moderna
 */
export default function GameScreen({ 
  character, 
  onAdvanceYear, 
  onShowStatus, 
  onBackToStart 
}: GameScreenProps) {
  const isHydrated = useHydration();

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
    if (!isHydrated) return date.toISOString().split('T')[0];
    
    return date.toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

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

  const getStatusBarColor = (value: number) => {
    if (value >= 80) return 'bg-green-500';
    if (value >= 60) return 'bg-yellow-500';
    if (value >= 40) return 'bg-orange-500';
    return 'bg-red-500';
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-purple-900 to-slate-900">
      {/* Header */}
      <div className="bg-black/20 backdrop-blur-sm border-b border-white/10 p-6">
        <div className="max-w-7xl mx-auto flex items-center justify-between">
          <div className="flex items-center space-x-4">
            <h1 className="text-3xl font-bold bg-gradient-to-r from-blue-400 via-purple-400 to-pink-400 bg-clip-text text-transparent">
              LifePixel
            </h1>
            <div className="text-white/80 text-lg">
              {character.firstName} {character.lastName} • {character.age} anos
            </div>
          </div>
          <div className="flex space-x-3">
            <button
              onClick={onShowStatus}
              className="px-4 py-2 bg-gradient-to-r from-blue-500 to-blue-600 hover:from-blue-600 hover:to-blue-700 text-white rounded-xl transition-all transform hover:scale-105 shadow-lg"
            >
              📊 Status
            </button>
            <button
              onClick={onBackToStart}
              className="px-4 py-2 bg-gradient-to-r from-gray-500 to-gray-600 hover:from-gray-600 hover:to-gray-700 text-white rounded-xl transition-all transform hover:scale-105 shadow-lg"
            >
              🏠 Menu
            </button>
          </div>
        </div>
      </div>

      <div className="max-w-7xl mx-auto p-6">
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          {/* Painel Principal */}
          <div className="lg:col-span-2 space-y-6">
            {/* Informações do Personagem */}
            <div className="bg-white/10 backdrop-blur-lg rounded-2xl p-8 border border-white/20">
              <h2 className="text-2xl font-bold text-white mb-6">👤 Seu Personagem</h2>
              <div className="grid grid-cols-2 md:grid-cols-3 gap-6">
                <div className="text-center p-4 bg-white/5 rounded-xl">
                  <div className="text-white/60 text-sm mb-1">Nome</div>
                  <div className="text-white font-semibold">{character.firstName} {character.lastName}</div>
                </div>
                <div className="text-center p-4 bg-white/5 rounded-xl">
                  <div className="text-white/60 text-sm mb-1">Idade</div>
                  <div className="text-white font-semibold">{character.age} anos</div>
                </div>
                <div className="text-center p-4 bg-white/5 rounded-xl">
                  <div className="text-white/60 text-sm mb-1">País</div>
                  <div className="text-white font-semibold">{character.country}</div>
                </div>
                <div className="text-center p-4 bg-white/5 rounded-xl">
                  <div className="text-white/60 text-sm mb-1">Gênero</div>
                  <div className="text-white font-semibold">
                    {character.gender === 'male' ? 'Masculino' : 'Feminino'}
                  </div>
                </div>
                <div className="text-center p-4 bg-white/5 rounded-xl">
                  <div className="text-white/60 text-sm mb-1">Emprego</div>
                  <div className="text-white font-semibold">
                    {character.career.isEmployed ? character.career.job || 'Empregado' : 'Desempregado'}
                  </div>
                </div>
                <div className="text-center p-4 bg-white/5 rounded-xl">
                  <div className="text-white/60 text-sm mb-1">Educação</div>
                  <div className="text-white font-semibold">
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

            {/* Status Detalhado */}
            <div className="bg-white/10 backdrop-blur-lg rounded-2xl p-8 border border-white/20">
              <h2 className="text-2xl font-bold text-white mb-6">📊 Status Detalhado</h2>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div className="space-y-4">
                  <div>
                    <div className="flex justify-between items-center mb-2">
                      <span className="text-white/80">Felicidade</span>
                      <span className={`font-semibold ${getStatusColor(character.stats.happiness)}`}>
                        {character.stats.happiness}%
                      </span>
                    </div>
                    <div className="w-full bg-white/10 rounded-full h-3">
                      <div 
                        className={`h-3 rounded-full ${getStatusBarColor(character.stats.happiness)} transition-all duration-300`}
                        style={{ width: `${character.stats.happiness}%` }}
                      ></div>
                    </div>
                  </div>

                  <div>
                    <div className="flex justify-between items-center mb-2">
                      <span className="text-white/80">Saúde</span>
                      <span className={`font-semibold ${getStatusColor(character.stats.health)}`}>
                        {character.stats.health}%
                      </span>
                    </div>
                    <div className="w-full bg-white/10 rounded-full h-3">
                      <div 
                        className={`h-3 rounded-full ${getStatusBarColor(character.stats.health)} transition-all duration-300`}
                        style={{ width: `${character.stats.health}%` }}
                      ></div>
                    </div>
                  </div>

                  <div>
                    <div className="flex justify-between items-center mb-2">
                      <span className="text-white/80">Inteligência</span>
                      <span className={`font-semibold ${getStatusColor(character.stats.intelligence)}`}>
                        {character.stats.intelligence}%
                      </span>
                    </div>
                    <div className="w-full bg-white/10 rounded-full h-3">
                      <div 
                        className={`h-3 rounded-full ${getStatusBarColor(character.stats.intelligence)} transition-all duration-300`}
                        style={{ width: `${character.stats.intelligence}%` }}
                      ></div>
                    </div>
                  </div>
                </div>

                <div className="space-y-4">
                  <div>
                    <div className="flex justify-between items-center mb-2">
                      <span className="text-white/80">Aparência</span>
                      <span className={`font-semibold ${getStatusColor(character.stats.looks)}`}>
                        {character.stats.looks}%
                      </span>
                    </div>
                    <div className="w-full bg-white/10 rounded-full h-3">
                      <div 
                        className={`h-3 rounded-full ${getStatusBarColor(character.stats.looks)} transition-all duration-300`}
                        style={{ width: `${character.stats.looks}%` }}
                      ></div>
                    </div>
                  </div>

                  <div>
                    <div className="flex justify-between items-center mb-2">
                      <span className="text-white/80">Popularidade</span>
                      <span className={`font-semibold ${getStatusColor(character.stats.popularity)}`}>
                        {character.stats.popularity}%
                      </span>
                    </div>
                    <div className="w-full bg-white/10 rounded-full h-3">
                      <div 
                        className={`h-3 rounded-full ${getStatusBarColor(character.stats.popularity)} transition-all duration-300`}
                        style={{ width: `${character.stats.popularity}%` }}
                      ></div>
                    </div>
                  </div>

                  <div>
                    <div className="flex justify-between items-center mb-2">
                      <span className="text-white/80">Dinheiro</span>
                      <span className={`font-semibold ${character.stats.money >= 0 ? 'text-green-400' : 'text-red-400'}`}>
                        {getMoneyDisplay(character.stats.money)}
                      </span>
                    </div>
                    <div className="w-full bg-white/10 rounded-full h-3">
                      <div 
                        className={`h-3 rounded-full ${character.stats.money >= 0 ? 'bg-green-500' : 'bg-red-500'} transition-all duration-300`}
                        style={{ width: `${Math.min(Math.abs(character.stats.money) / 10000 * 100, 100)}%` }}
                      ></div>
                    </div>
                  </div>
                </div>
              </div>
            </div>

        {/* Lista de Eventos */}
        <div className="bg-white/10 backdrop-blur-lg rounded-xl p-6 border border-white/20">
          <h2 className="text-xl font-semibold text-white mb-6">Cronologia da Vida</h2>
          
          {character.events.length > 0 ? (
            <div className="space-y-4">
              {[...character.events].sort((a, b) => b.age - a.age).map((event) => (
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
                                Dinheiro {event.effects.money > 0 ? '+' : ''}R$ {isHydrated ? event.effects.money.toLocaleString() : event.effects.money}
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

          {/* Painel de Ações */}
          <div className="space-y-6">
            {/* Botão Principal */}
            <div className="bg-white/10 backdrop-blur-lg rounded-2xl p-8 border border-white/20">
              <button
                onClick={onAdvanceYear}
                disabled={!character.isAlive}
                className={`w-full py-6 px-8 rounded-xl font-bold text-xl transition-all transform hover:scale-105 shadow-lg ${
                  character.isAlive
                    ? 'bg-gradient-to-r from-blue-500 to-purple-600 hover:from-blue-600 hover:to-purple-700 text-white'
                    : 'bg-gray-600 text-gray-300 cursor-not-allowed'
                }`}
              >
                {character.isAlive ? '⏭️ Avançar Ano' : '💀 Fim da Vida'}
              </button>
              
              {!character.isAlive && (
                <div className="mt-6 text-center text-white/80">
                  <p className="mb-2">Seu personagem faleceu aos {character.age} anos.</p>
                  <p className="text-sm">Clique em "Menu" para começar uma nova vida.</p>
                </div>
              )}
            </div>

            {/* Estatísticas Rápidas */}
            <div className="bg-white/10 backdrop-blur-lg rounded-2xl p-8 border border-white/20">
              <h3 className="text-xl font-bold text-white mb-6">📈 Resumo</h3>
              <div className="space-y-4">
                <div className="flex justify-between items-center p-3 bg-white/5 rounded-lg">
                  <span className="text-white/70">Total de Eventos:</span>
                  <span className="text-white font-bold">{character.events.length}</span>
                </div>
                <div className="flex justify-between items-center p-3 bg-white/5 rounded-lg">
                  <span className="text-white/70">Relacionamentos:</span>
                  <span className="text-white font-bold">{character.relationships.length}</span>
                </div>
                <div className="flex justify-between items-center p-3 bg-white/5 rounded-lg">
                  <span className="text-white/70">Salário:</span>
                  <span className="text-white font-bold">
                    {character.career.salary > 0 ? getMoneyDisplay(character.career.salary) : 'R$ 0'}
                  </span>
                </div>
                <div className="flex justify-between items-center p-3 bg-white/5 rounded-lg">
                  <span className="text-white/70">Experiência:</span>
                  <span className="text-white font-bold">{character.career.experience} anos</span>
                </div>
              </div>
            </div>

            {/* Dicas */}
            <div className="bg-white/10 backdrop-blur-lg rounded-2xl p-8 border border-white/20">
              <h3 className="text-xl font-bold text-white mb-4">💡 Dicas</h3>
              <div className="space-y-3 text-sm text-white/80">
                <p>• Mantenha a felicidade alta para eventos positivos</p>
                <p>• Cuide da saúde para viver mais tempo</p>
                <p>• Invista em educação para melhores empregos</p>
                <p>• Construa relacionamentos para apoio</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
} 