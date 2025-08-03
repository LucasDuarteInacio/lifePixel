"use client";

import React, { useState } from 'react';
import { getRandomName, getRandomGender } from '@/data/names';
import { getRandomCountry } from '@/data/countries';
import { countries } from '@/data/countries';

interface StartScreenProps {
  onCreateCharacter: (data?: {
    name?: string;
    gender?: 'male' | 'female';
    country?: string;
  }) => void;
  onContinueGame: () => void;
  hasExistingGame: boolean;
}

/**
 * Tela inicial do jogo com opções para criar novo personagem ou continuar
 */
export default function StartScreen({ 
  onCreateCharacter, 
  onContinueGame, 
  hasExistingGame 
}: StartScreenProps) {
  const [showCustomForm, setShowCustomForm] = useState(false);
  const [customName, setCustomName] = useState('');
  const [customGender, setCustomGender] = useState<'male' | 'female'>('male');
  const [customCountry, setCustomCountry] = useState('');

  const handleRandomCharacter = () => {
    onCreateCharacter();
  };

  const handleCustomCharacter = () => {
    if (customName.trim()) {
      onCreateCharacter({
        name: customName.trim(),
        gender: customGender,
        country: customCountry || getRandomCountry().name
      });
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-900 via-purple-900 to-indigo-900 flex items-center justify-center p-4">
      <div className="bg-white/10 backdrop-blur-lg rounded-2xl p-8 max-w-md w-full shadow-2xl border border-white/20">
        <div className="text-center mb-8">
          <h1 className="text-4xl font-bold text-white mb-2">VidaPixel</h1>
          <p className="text-white/80 text-lg">Simulador de Vida</p>
        </div>

        {hasExistingGame && (
          <button
            onClick={onContinueGame}
            className="w-full bg-green-600 hover:bg-green-700 text-white font-semibold py-3 px-6 rounded-lg mb-4 transition-colors"
          >
            Continuar Jogo Salvo
          </button>
        )}

        <div className="space-y-4">
          <button
            onClick={handleRandomCharacter}
            className="w-full bg-blue-600 hover:bg-blue-700 text-white font-semibold py-3 px-6 rounded-lg transition-colors"
          >
            Iniciar Nova Vida (Aleatório)
          </button>

          <button
            onClick={() => setShowCustomForm(!showCustomForm)}
            className="w-full bg-purple-600 hover:bg-purple-700 text-white font-semibold py-3 px-6 rounded-lg transition-colors"
          >
            Criar Personagem Personalizado
          </button>
        </div>

        {showCustomForm && (
          <div className="mt-6 space-y-4 bg-white/5 rounded-lg p-4">
            <div>
              <label className="block text-white text-sm font-medium mb-2">
                Nome
              </label>
              <input
                type="text"
                value={customName}
                onChange={(e) => setCustomName(e.target.value)}
                placeholder="Digite o nome do personagem"
                className="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg text-white placeholder-white/50 focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>

            <div>
              <label className="block text-white text-sm font-medium mb-2">
                Gênero
              </label>
              <select
                value={customGender}
                onChange={(e) => setCustomGender(e.target.value as 'male' | 'female')}
                className="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-blue-500"
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
                className="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                <option value="">País aleatório</option>
                {countries.map((country) => (
                  <option key={country.code} value={country.name}>
                    {country.name}
                  </option>
                ))}
              </select>
            </div>

            <button
              onClick={handleCustomCharacter}
              disabled={!customName.trim()}
              className="w-full bg-green-600 hover:bg-green-700 disabled:bg-gray-500 disabled:cursor-not-allowed text-white font-semibold py-3 px-6 rounded-lg transition-colors"
            >
              Criar Personagem
            </button>
          </div>
        )}

        <div className="mt-8 text-center">
          <p className="text-white/60 text-sm">
            Desenvolvido com React, TypeScript e Electron
          </p>
        </div>
      </div>
    </div>
  );
} 