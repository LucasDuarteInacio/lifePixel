"use client";

import React from 'react';

/**
 * Componente de loading para ser usado durante a hidratação
 */
export default function LoadingScreen() {
  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-900 via-purple-900 to-indigo-900 flex items-center justify-center">
      <div className="text-center">
        <div className="animate-spin rounded-full h-16 w-16 border-b-2 border-white mx-auto mb-4"></div>
        <div className="text-white text-xl">Carregando VidaPixel...</div>
      </div>
    </div>
  );
} 