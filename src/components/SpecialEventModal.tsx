import { SpecialEvent, SpecialEventChoice } from '@/types/game';

interface SpecialEventModalProps {
  event: SpecialEvent;
  onChoice: (choice: SpecialEventChoice) => void;
  onCancel: () => void;
}

/**
 * Modal para eventos especiais interativos
 * Interface em português conforme solicitado
 */
export default function SpecialEventModal({ event, onChoice, onCancel }: SpecialEventModalProps) {
  return (
    <div className="fixed inset-0 bg-black/50 backdrop-blur-sm flex items-center justify-center z-50 p-4">
      <div className="bg-white/10 backdrop-blur-lg rounded-2xl p-6 max-w-md w-full shadow-2xl border border-white/20 animate-in fade-in-0 zoom-in-95 duration-300">
        {/* Header */}
        <div className="flex items-center justify-between mb-4">
          <h2 className="text-xl font-bold text-white">
            {event.title}
          </h2>
          <button
            onClick={onCancel}
            className="text-white/60 hover:text-white transition-colors p-1"
            aria-label="Fechar"
          >
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        {/* Event Type Badge */}
        <div className="mb-4">
          <span className={`inline-flex items-center px-3 py-1 rounded-full text-xs font-medium ${
            event.type === 'positive' 
              ? 'bg-green-500/20 text-green-300 border border-green-500/30'
              : event.type === 'negative'
              ? 'bg-red-500/20 text-red-300 border border-red-500/30'
              : 'bg-blue-500/20 text-blue-300 border border-blue-500/30'
          }`}>
            {event.type === 'positive' ? '✨ Oportunidade' : 
             event.type === 'negative' ? '⚠️ Desafio' : 
             '📋 Situação'}
          </span>
        </div>

        {/* Event Description */}
        <div className="mb-6">
          <p className="text-white/90 leading-relaxed">
            {event.description}
          </p>
        </div>

        {/* Choices */}
        <div className="space-y-3">
          <p className="text-white/80 text-sm font-medium mb-3">
            Como você deseja reagir?
          </p>
          
          {event.choices.map((choice, index) => (
            <button
              key={choice.id}
              onClick={() => onChoice(choice)}
              className={`w-full text-left p-4 rounded-lg border transition-all duration-200 hover:scale-[1.02] ${
                index === 0
                  ? 'bg-green-500/10 border-green-500/30 hover:bg-green-500/20 text-green-100'
                  : index === 1
                  ? 'bg-blue-500/10 border-blue-500/30 hover:bg-blue-500/20 text-blue-100'
                  : 'bg-red-500/10 border-red-500/30 hover:bg-red-500/20 text-red-100'
              }`}
            >
              <div className="flex items-start justify-between">
                <span className="font-medium">
                  {choice.text}
                </span>
                <div className="flex items-center gap-1 ml-2">
                  {choice.riskOutcome && (
                    <span className="text-xs bg-orange-500/20 text-orange-300 px-1.5 py-0.5 rounded border border-orange-500/30" title={`${choice.riskOutcome.probability}% chance de dar errado`}>
                      ⚠️ {choice.riskOutcome.probability}%
                    </span>
                  )}
                  <span className="text-xs opacity-60">
                    {index + 1}
                  </span>
                </div>
              </div>
              
              {/* Show risk warning if exists */}
              {choice.riskOutcome && (
                <div className="mt-2 p-2 bg-orange-500/10 border border-orange-500/20 rounded text-xs text-orange-200">
                  <div className="flex items-center gap-1 mb-1">
                    <span>⚠️</span>
                    <span className="font-medium">Risco ({choice.riskOutcome.probability}% chance):</span>
                  </div>
                  <p className="text-orange-300/80 leading-relaxed">
                    Pode dar errado e resultar em consequências negativas
                  </p>
                </div>
              )}

              {/* Show effects preview */}
              {(choice.effects.happiness || choice.effects.health || choice.effects.money ||
                choice.effects.intelligence || choice.effects.looks || choice.effects.popularity) && (
                <div className="mt-2 flex flex-wrap gap-2">
                  {choice.effects.happiness && (
                    <span className={`text-xs px-2 py-1 rounded ${
                      choice.effects.happiness > 0 ? 'bg-yellow-500/20 text-yellow-300' : 'bg-gray-500/20 text-gray-300'
                    }`}>
                      😊 {choice.effects.happiness > 0 ? '+' : ''}{choice.effects.happiness}
                    </span>
                  )}
                  {choice.effects.health && (
                    <span className={`text-xs px-2 py-1 rounded ${
                      choice.effects.health > 0 ? 'bg-green-500/20 text-green-300' : 'bg-red-500/20 text-red-300'
                    }`}>
                      ❤️ {choice.effects.health > 0 ? '+' : ''}{choice.effects.health}
                    </span>
                  )}
                  {choice.effects.money && (
                    <span className={`text-xs px-2 py-1 rounded ${
                      choice.effects.money > 0 ? 'bg-green-500/20 text-green-300' : 'bg-red-500/20 text-red-300'
                    }`}>
                      💰 {choice.effects.money > 0 ? '+' : ''}${choice.effects.money}
                    </span>
                  )}
                  {choice.effects.intelligence && (
                    <span className={`text-xs px-2 py-1 rounded ${
                      choice.effects.intelligence > 0 ? 'bg-purple-500/20 text-purple-300' : 'bg-gray-500/20 text-gray-300'
                    }`}>
                      🧠 {choice.effects.intelligence > 0 ? '+' : ''}{choice.effects.intelligence}
                    </span>
                  )}
                  {choice.effects.looks && (
                    <span className={`text-xs px-2 py-1 rounded ${
                      choice.effects.looks > 0 ? 'bg-pink-500/20 text-pink-300' : 'bg-gray-500/20 text-gray-300'
                    }`}>
                      ✨ {choice.effects.looks > 0 ? '+' : ''}{choice.effects.looks}
                    </span>
                  )}
                  {choice.effects.popularity && (
                    <span className={`text-xs px-2 py-1 rounded ${
                      choice.effects.popularity > 0 ? 'bg-blue-500/20 text-blue-300' : 'bg-gray-500/20 text-gray-300'
                    }`}>
                      👥 {choice.effects.popularity > 0 ? '+' : ''}{choice.effects.popularity}
                    </span>
                  )}
                </div>
              )}

              {/* Show relationship action preview */}
              {choice.relationshipAction && choice.relationshipAction.type === 'add' && (
                <div className="mt-2">
                  <span className="text-xs bg-purple-500/20 text-purple-300 px-2 py-1 rounded">
                    👥 Novo relacionamento
                  </span>
                </div>
              )}
            </button>
          ))}
        </div>

        {/* Footer */}
        <div className="mt-6 pt-4 border-t border-white/10">
          <p className="text-white/60 text-xs text-center">
            Suas escolhas afetam sua vida e relacionamentos
          </p>
        </div>
      </div>
    </div>
  );
}