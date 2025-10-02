import React from 'react';
import { Card } from './Card';
import { Button } from './Button';
import { CombatDetail, CombatStatus } from '../services/combatService';

interface CombatConsequenceProps {
  combat: CombatDetail;
  onCompleteCombat: () => void;
  onReturnToQuests: () => void;
  loading: boolean;
}

export const CombatConsequence: React.FC<CombatConsequenceProps> = ({
  combat,
  onCompleteCombat,
  onReturnToQuests,
  loading
}) => {
  const isVictory = combat.status === CombatStatus.Victory;

  return (
    <div className="space-y-6">
      {/* Resultado Principal */}
        <Card className={`${
        isVictory 
          ? 'bg-gradient-to-r from-green-900/30 to-emerald-900/30 border-green-500/30' 
          : 'bg-gradient-to-r from-red-900/30 to-rose-900/30 border-red-500/30'
      }`}>
        <div className="text-center">
          <div className="text-6xl mb-4">
            {isVictory ? '🏆' : '💀'}
          </div>
          <h2 className={`text-3xl font-bold mb-2 ${
            isVictory ? 'text-green-400' : 'text-red-400'
          }`}>
            {isVictory ? 'VITÓRIA!' : 'DERROTA!'}
          </h2>
          <p className="text-gray-300 text-lg">
            {isVictory 
              ? 'Parabéns! Você derrotou todos os inimigos!'
              : 'Seus heróis foram derrotados...'
            }
          </p>
        </div>
      </Card>

      {/* Estatísticas da Batalha */}
      <Card className="bg-gradient-to-r from-purple-900/30 to-indigo-900/30 border-purple-500/30">
        <h3 className="text-xl font-bold text-purple-400 mb-4 flex items-center gap-2">
          📊 Estatísticas da Batalha
        </h3>
        <div className="grid grid-cols-2 md:grid-cols-4 gap-4 text-sm">
          <div className="text-center">
            <div className="text-2xl font-bold text-blue-400">{combat.combatLogs?.length || 0}</div>
            <div className="text-gray-400">Ações Realizadas</div>
          </div>
          <div className="text-center">
            <div className="text-2xl font-bold text-green-400">
              {combat.combatLogs?.filter((log: any) => log.success).length || 0}
            </div>
            <div className="text-gray-400">Ataques Bem-sucedidos</div>
          </div>
          <div className="text-center">
            <div className="text-2xl font-bold text-red-400">
              {combat.combatLogs?.filter((log: any) => !log.success).length || 0}
            </div>
            <div className="text-gray-400">Ataques Falharam</div>
          </div>
          <div className="text-center">
            <div className="text-2xl font-bold text-yellow-400">
              0
            </div>
            <div className="text-gray-400">Inimigos Restantes</div>
          </div>
        </div>
      </Card>

      {/* Log de Combate */}
      {combat.combatLogs && combat.combatLogs.length > 0 && (
        <Card className="bg-gradient-to-r from-gray-900/30 to-slate-900/30 border-gray-500/30">
          <h3 className="text-xl font-bold text-gray-400 mb-4 flex items-center gap-2">
            📜 Histórico de Combate
          </h3>
          <div className="space-y-2 max-h-64 overflow-y-auto">
            {combat.combatLogs.map((log: any, index: number) => (
              <div key={index} className="flex items-center gap-3 p-2 bg-gray-800/30 rounded">
                <div className={`w-2 h-2 rounded-full ${
                  log.success ? 'bg-green-400' : 'bg-red-400'
                }`}></div>
                <div className="flex-1">
                  <div className="text-sm text-gray-300">{log.details}</div>
                  <div className="text-xs text-gray-500">
                    {new Date(log.timestamp).toLocaleTimeString()}
                  </div>
                </div>
              </div>
            ))}
          </div>
        </Card>
      )}

      {/* Ações */}
      <div className="flex flex-col sm:flex-row gap-4 justify-center">
        {isVictory && (
          <Button
            onClick={onCompleteCombat}
            disabled={loading}
            className="bg-gradient-to-r from-green-600 to-green-700 hover:from-green-700 hover:to-green-800 py-3 px-6 font-bold shadow-lg shadow-green-500/30 disabled:opacity-50"
          >
            {loading ? 'Processando...' : '🏆 Finalizar Combate e Receber Recompensas'}
          </Button>
        )}
        
        <Button
          onClick={onReturnToQuests}
          className="bg-gradient-to-r from-blue-600 to-blue-700 hover:from-blue-700 hover:to-blue-800 py-3 px-6 font-bold shadow-lg shadow-blue-500/30"
        >
          📜 Voltar para Missões
        </Button>
      </div>
    </div>
  );
};
