#!/bin/bash

echo "🚀 Iniciando LifePixel em modo desenvolvimento..."

# Verificar se as dependências estão instaladas
if [ ! -d "node_modules" ]; then
    echo "📦 Instalando dependências..."
    npm install
fi

# Iniciar o servidor de desenvolvimento
echo "🌐 Iniciando servidor de desenvolvimento..."
npm run dev 