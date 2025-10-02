import React from 'react';
import { Card } from './Card';

export type CombatArc = 'preparation' | 'combat' | 'consequence';

interface CombatArcsProps {
  currentArc: CombatArc;
  combatStarted: boolean;
  combatEnded: boolean;
}

export const CombatArcs: React.FC<CombatArcsProps> = ({
  currentArc,
  combatStarted,
  combatEnded
}) => {
  const arcs = [
    {
      id: 'preparation' as CombatArc,
      name: 'Preparação',
      icon: '⚔️',
      description: 'Configure sua party e equipamentos',
      disabled: combatStarted
    },
    {
      id: 'combat' as CombatArc,
      name: 'Combate',
      icon: '⚡',
      description: 'Batalhe contra os inimigos',
      disabled: !combatStarted || combatEnded
    },
    {
      id: 'consequence' as CombatArc,
      name: 'Consequência',
      icon: '🏆',
      description: 'Veja os resultados e recompensas',
      disabled: !combatEnded
    }
  ];

  return (
    <Card className="bg-gradient-to-r from-purple-900/30 to-indigo-900/30 border-purple-500/30 mb-4">
      <div className="flex justify-between items-center">
        <h2 className="text-xl font-bold text-purple-400 mb-4">
          🎭 Arcos da Batalha
        </h2>
        <div className="text-sm text-gray-400">
          {combatStarted ? 'Combate Ativo' : combatEnded ? 'Batalha Finalizada' : 'Preparando'}
        </div>
      </div>
      
      <div className="flex space-x-2">
        {arcs.map((arc, index) => (
          <div key={arc.id} className="flex-1">
            <div
              className={`w-full p-3 rounded-lg transition-all duration-300 ${
                currentArc === arc.id
                  ? 'bg-gradient-to-r from-yellow-500/20 to-orange-500/20 border-2 border-yellow-500/50 text-yellow-300'
                  : arc.disabled
                  ? 'bg-gray-800/30 border border-gray-600/30 text-gray-500'
                  : 'bg-gray-700/30 border border-gray-500/30 text-gray-300'
              }`}
            >
              <div className="text-2xl mb-1">{arc.icon}</div>
              <div className="font-bold text-sm">{arc.name}</div>
              <div className="text-xs opacity-75">{arc.description}</div>
              {currentArc === arc.id && (
                <div className="mt-2 text-center">
                  <div className="text-xs bg-yellow-500/20 px-2 py-1 rounded animate-pulse">
                    ATIVO
                  </div>
                </div>
              )}
            </div>
            
            {/* Seta conectora */}
            {index < arcs.length - 1 && (
              <div className="flex justify-center mt-2">
                <div className={`text-lg ${
                  currentArc === arc.id ? 'text-yellow-400' : 'text-gray-500'
                }`}>→</div>
              </div>
            )}
          </div>
        ))}
      </div>
    </Card>
  );
};
