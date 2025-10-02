import React from 'react';
import { Card } from './Card';
import { Button } from './Button';
import { Hero } from '../services/heroService';
import { Quest } from '../services/questService';

interface CombatPreparationProps {
  heroes: Hero[];
  selectedHeroes: number[];
  onHeroToggle: (heroId: number) => void;
  quest: Quest;
  onStartCombat: () => void;
  loading: boolean;
}

export const CombatPreparation: React.FC<CombatPreparationProps> = ({
  heroes,
  selectedHeroes,
  onHeroToggle,
  quest,
  onStartCombat,
  loading
}) => {
  return (
    <div className="space-y-6">
      {/* Informações da Quest */}
      <Card className="bg-gradient-to-r from-blue-900/30 to-cyan-900/30 border-blue-500/30">
        <h3 className="text-xl font-bold text-blue-400 mb-4 flex items-center gap-2">
          📜 Missão Selecionada
        </h3>
        <div className="space-y-3">
          <div>
            <h4 className="text-lg font-bold text-white">{quest.name}</h4>
            <p className="text-gray-300 text-sm mt-1">{quest.description}</p>
          </div>
          <div className="grid grid-cols-2 md:grid-cols-4 gap-4 text-sm">
            <div>
              <span className="text-gray-400">Dificuldade:</span>
              <div className="font-bold text-yellow-400">{quest.difficulty}</div>
            </div>
            <div>
              <span className="text-gray-400">Nível Requerido:</span>
              <div className="font-bold text-green-400">{quest.requiredLevel}</div>
            </div>
            <div>
              <span className="text-gray-400">Recompensa XP:</span>
              <div className="font-bold text-purple-400">{quest.experienceReward}</div>
            </div>
            <div>
              <span className="text-gray-400">Recompensa Ouro:</span>
              <div className="font-bold text-yellow-400">{quest.goldReward}g</div>
            </div>
          </div>
        </div>
      </Card>

      {/* Seleção de Heróis */}
      <Card className="bg-gradient-to-r from-green-900/30 to-emerald-900/30 border-green-500/30">
        <h3 className="text-xl font-bold text-green-400 mb-4 flex items-center gap-2">
          🛡️ Selecionar Party
        </h3>
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {heroes.map((hero) => {
            const isSelected = selectedHeroes.includes(hero.id);
            return (
              <div
                key={hero.id}
                className={`p-4 rounded-lg border-2 transition-all duration-300 cursor-pointer ${
                  isSelected
                    ? 'bg-gradient-to-r from-yellow-500/20 to-orange-500/20 border-yellow-500/50 text-yellow-300'
                    : 'bg-gray-800/30 border-gray-600/30 text-gray-300 hover:bg-gray-700/30 hover:border-gray-500/50'
                }`}
                onClick={() => onHeroToggle(hero.id)}
              >
                <div className="flex items-center justify-between mb-2">
                  <h4 className="font-bold text-lg">{hero.name}</h4>
                  <div className="text-sm text-gray-400">Nível {hero.level}</div>
                </div>
                <div className="text-sm space-y-1">
                  <div className="flex justify-between">
                    <span>Força:</span>
                    <span className="font-bold text-red-400">{hero.strength}</span>
                  </div>
                  <div className="flex justify-between">
                    <span>Inteligência:</span>
                    <span className="font-bold text-blue-400">{hero.intelligence}</span>
                  </div>
                  <div className="flex justify-between">
                    <span>Destreza:</span>
                    <span className="font-bold text-green-400">{hero.dexterity}</span>
                  </div>
                  <div className="flex justify-between">
                    <span>Ouro:</span>
                    <span className="font-bold text-yellow-400">{hero.gold}g</span>
                  </div>
                </div>
                {isSelected && (
                  <div className="mt-2 text-center">
                    <span className="text-xs bg-yellow-500/20 px-2 py-1 rounded">SELECIONADO</span>
                  </div>
                )}
              </div>
            );
          })}
        </div>
        
        {selectedHeroes.length === 0 && (
          <div className="text-center py-8 text-gray-400">
            <div className="text-4xl mb-2">⚔️</div>
            <p>Selecione pelo menos um herói para começar a batalha</p>
          </div>
        )}
      </Card>

      {/* Botão de Iniciar Combate */}
      <div className="text-center">
        <Button
          onClick={onStartCombat}
          disabled={selectedHeroes.length === 0 || loading}
          className="bg-gradient-to-r from-red-600 to-red-700 hover:from-red-700 hover:to-red-800 py-4 px-8 text-lg font-bold shadow-lg shadow-red-500/30 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {loading ? 'Iniciando...' : '⚔️ Iniciar Combate'}
        </Button>
      </div>
    </div>
  );
};
