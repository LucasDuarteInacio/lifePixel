"use client";

import React from 'react';
import { Character } from '@/types/game';
import { useHydration } from '@/hooks/useHydration';

interface StatusScreenProps {
  character: Character;
  onBackToGame: () => void;
}

/**
 * Tela de status detalhado do personagem
 */
export default function StatusScreen({ character, onBackToGame }: StatusScreenProps) {
  const isHydrated = useHydration();

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

  const getEducationLevel = (level: string) => {
    switch (level) {
      case 'none': return 'Nenhuma';
      case 'elementary': return 'Fundamental';
      case 'high_school': return 'Ensino Médio';
      case 'college': return 'Faculdade';
      case 'university': return 'Universidade';
      case 'phd': return 'Doutorado';
      default: return 'Nenhuma';
    }
  };

  const getFormattedDate = (date: Date) => {
    if (!isHydrated) return date.toISOString().split('T')[0];
    return date.toLocaleDateString('pt-BR');
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-900 via-blue-900 to-purple-900">
      {/* Header */}
      <div className="bg-black/20 backdrop-blur-sm border-b border-white/10 p-4">
        <div className="max-w-6xl mx-auto flex items-center justify-between">
          <div className="flex items-center space-x-4">
            <h1 className="text-2xl font-bold text-white">Status Detalhado</h1>
            <div className="text-white/80">
              {character.firstName} {character.lastName} - {character.age} anos
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

      <div className="max-w-6xl mx-auto p-6">
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          {/* Informações Básicas */}
          <div className="bg-white/10 backdrop-blur-lg rounded-xl p-6 border border-white/20">
            <h2 className="text-xl font-semibold text-white mb-4">Informações Básicas</h2>
            <div className="space-y-3">
              <div className="flex justify-between">
                <span className="text-white/60">Nome:</span>
                <span className="text-white font-medium">{character.firstName} {character.lastName}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-white/60">Idade:</span>
                <span className="text-white font-medium">{character.age} anos</span>
              </div>
              <div className="flex justify-between">
                <span className="text-white/60">Gênero:</span>
                <span className="text-white font-medium">
                  {character.gender === 'male' ? 'Masculino' : 'Feminino'}
                </span>
              </div>
              <div className="flex justify-between">
                <span className="text-white/60">País:</span>
                <span className="text-white font-medium">{character.country}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-white/60">Status:</span>
                <span className={`font-medium ${character.isAlive ? 'text-green-400' : 'text-red-400'}`}>
                  {character.isAlive ? 'Vivo' : 'Falecido'}
                </span>
              </div>
              <div className="flex justify-between">
                <span className="text-white/60">Criado em:</span>
                <span className="text-white font-medium">
                  {getFormattedDate(character.createdAt)}
                </span>
              </div>
            </div>
          </div>

          {/* Estatísticas */}
          <div className="bg-white/10 backdrop-blur-lg rounded-xl p-6 border border-white/20">
            <h2 className="text-xl font-semibold text-white mb-4">Estatísticas</h2>
            <div className="space-y-3">
              <div className="flex justify-between">
                <span className="text-white/60">Felicidade:</span>
                <span className={`font-medium ${getStatusColor(character.stats.happiness)}`}>
                  {character.stats.happiness}%
                </span>
              </div>
              <div className="flex justify-between">
                <span className="text-white/60">Saúde:</span>
                <span className={`font-medium ${getStatusColor(character.stats.health)}`}>
                  {character.stats.health}%
                </span>
              </div>
              <div className="flex justify-between">
                <span className="text-white/60">Dinheiro:</span>
                <span className={`font-medium ${character.stats.money >= 0 ? 'text-green-400' : 'text-red-400'}`}>
                  {getMoneyDisplay(character.stats.money)}
                </span>
              </div>
              <div className="flex justify-between">
                <span className="text-white/60">Inteligência:</span>
                <span className={`font-medium ${getStatusColor(character.stats.intelligence)}`}>
                  {character.stats.intelligence}%
                </span>
              </div>
              <div className="flex justify-between">
                <span className="text-white/60">Aparência:</span>
                <span className={`font-medium ${getStatusColor(character.stats.looks)}`}>
                  {character.stats.looks}%
                </span>
              </div>
              <div className="flex justify-between">
                <span className="text-white/60">Popularidade:</span>
                <span className={`font-medium ${getStatusColor(character.stats.popularity)}`}>
                  {character.stats.popularity}%
                </span>
              </div>
            </div>
          </div>

          {/* Carreira */}
          <div className="bg-white/10 backdrop-blur-lg rounded-xl p-6 border border-white/20">
            <h2 className="text-xl font-semibold text-white mb-4">Carreira</h2>
            <div className="space-y-3">
              <div className="flex justify-between">
                <span className="text-white/60">Status:</span>
                <span className={`font-medium ${character.career.isEmployed ? 'text-green-400' : 'text-red-400'}`}>
                  {character.career.isEmployed ? 'Empregado' : 'Desempregado'}
                </span>
              </div>
              <div className="flex justify-between">
                <span className="text-white/60">Cargo:</span>
                <span className="text-white font-medium">
                  {character.career.job || 'Nenhum'}
                </span>
              </div>
              <div className="flex justify-between">
                <span className="text-white/60">Salário:</span>
                <span className="text-white font-medium">
                  {character.career.salary > 0 ? getMoneyDisplay(character.career.salary) : 'R$ 0'}
                </span>
              </div>
              <div className="flex justify-between">
                <span className="text-white/60">Experiência:</span>
                <span className="text-white font-medium">
                  {character.career.experience} anos
                </span>
              </div>
            </div>
          </div>

          {/* Educação */}
          <div className="bg-white/10 backdrop-blur-lg rounded-xl p-6 border border-white/20">
            <h2 className="text-xl font-semibold text-white mb-4">Educação</h2>
            <div className="space-y-3">
              <div className="flex justify-between">
                <span className="text-white/60">Nível:</span>
                <span className="text-white font-medium">
                  {getEducationLevel(character.education.level)}
                </span>
              </div>
              <div className="flex justify-between">
                <span className="text-white/60">Instituição:</span>
                <span className="text-white font-medium">
                  {character.education.institution || 'Nenhuma'}
                </span>
              </div>
              <div className="flex justify-between">
                <span className="text-white/60">Ano de Formatura:</span>
                <span className="text-white font-medium">
                  {character.education.graduationYear || 'N/A'}
                </span>
              </div>
            </div>
          </div>

          {/* Relacionamentos */}
          <div className="bg-white/10 backdrop-blur-lg rounded-xl p-6 border border-white/20 lg:col-span-2">
            <h2 className="text-xl font-semibold text-white mb-4">Família e Relacionamentos</h2>
            {character.relationships.length > 0 ? (
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                                 {character.relationships.map((relationship) => {
                                   // Função para obter o nome do tipo de relacionamento em português
                                   const getRelationshipTypeName = (type: string) => {
                                     switch (type) {
                                       case 'father': return 'Pai';
                                       case 'mother': return 'Mãe';
                                       case 'brother': return 'Irmão';
                                       case 'sister': return 'Irmã';
                                       case 'son': return 'Filho';
                                       case 'daughter': return 'Filha';
                                       case 'family': return 'Familiar';
                                       case 'friend': return 'Amigo';
                                       case 'romantic': return 'Romântico';
                                       case 'colleague': return 'Colega';
                                       default: return 'Outro';
                                     }
                                   };
                
                                     return (
                                     <div key={relationship.id} className="bg-white/5 rounded-lg p-3 border border-white/10">
                                       <div className="text-white font-medium">{relationship.name}</div>
                                       <div className="text-white/60 text-sm">
                                         {getRelationshipTypeName(relationship.type)}
                                       </div>
                       <div className="text-white/60 text-sm">
                         Gênero: {relationship.gender === 'male' ? 'Masculino' : 'Feminino'}
                       </div>
                       <div className="text-white/60 text-sm">
                         Status: {relationship.status === 'good' ? 'Bom' :
                                  relationship.status === 'neutral' ? 'Neutro' : 'Ruim'}
                       </div>
                       <div className="text-white/60 text-sm">
                         Idade: {relationship.age} anos
                       </div>
                       <div className="text-white/60 text-sm">
                         {relationship.isAlive ? 'Vivo' : 'Falecido'}
                       </div>
                     </div>
                   );
                })}
              </div>
            ) : (
              <div className="text-white/60 text-center py-8">
                Nenhum relacionamento registrado ainda.
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
} 