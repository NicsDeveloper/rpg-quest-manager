import React, { useEffect, useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { Navbar } from '../components/Navbar';
import { Card } from '../components/Card';
import { Button } from '../components/Button';
import { Loading } from '../components/Loading';
import { HeroWidget } from '../components/HeroWidget';
import {
  combatService,
  CombatSessionDetail,
  RollDiceResult,
  CompleteCombatResult,
  EnemyInfo,
} from '../services/combatService';
import { diceService, DiceInventory } from '../services/diceService';
import { profileService } from '../services/profileService';

export const Combat: React.FC = () => {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const questId = parseInt(searchParams.get('questId') || '0');

  const [combat, setCombat] = useState<CombatSessionDetail | null>(null);
  const [inventory, setInventory] = useState<DiceInventory | null>(null);
  const [loading, setLoading] = useState(true);
  const [rolling, setRolling] = useState(false);
  const [selectedEnemy, setSelectedEnemy] = useState<EnemyInfo | null>(null);
  const [lastRoll, setLastRoll] = useState<RollDiceResult | null>(null);
  const [completionResult, setCompletionResult] = useState<CompleteCombatResult | null>(null);

  useEffect(() => {
    initCombat();
  }, []);

  const initCombat = async () => {
    try {
      setLoading(true);
      const hero = await profileService.getMyHero();

      // Verifica se já existe combate ativo
      let activeCombat = await combatService.getActiveCombat(hero.id);

      // Se não existe e foi passado um questId, inicia um novo
      if (!activeCombat && questId > 0) {
        const newCombat = await combatService.startCombat(hero.id, questId);
        activeCombat = await combatService.getActiveCombat(hero.id);
      }

      if (!activeCombat) {
        alert('Nenhum combate ativo encontrado. Aceite uma quest primeiro!');
        navigate('/quests/catalog');
        return;
      }

      setCombat(activeCombat);
      setSelectedEnemy(activeCombat.enemies[0] || null);

      const inv = await diceService.getInventory(hero.id);
      setInventory(inv);
    } catch (error: any) {
      console.error('Erro ao iniciar combate:', error);
      alert(error.response?.data || 'Erro ao carregar combate');
      navigate('/quests/catalog');
    } finally {
      setLoading(false);
    }
  };

  const handleRollDice = async (diceType: string) => {
    if (!combat || !selectedEnemy || !inventory) return;

    try {
      setRolling(true);
      setLastRoll(null);

      const result = await combatService.rollDice({
        combatSessionId: combat.id,
        enemyId: selectedEnemy.id,
        diceType,
      });

      setLastRoll(result);

      // Atualiza inventário
      const updatedInventory = await diceService.getInventory(combat.heroId);
      setInventory(updatedInventory);

      // Recarrega a sessão de combate para ver os logs
      const updatedCombat = await combatService.getActiveCombat(combat.heroId);
      if (updatedCombat) {
        setCombat(updatedCombat);
      }
    } catch (error: any) {
      alert(error.response?.data || 'Erro ao rolar dado');
    } finally {
      setRolling(false);
    }
  };

  const handleCompleteCombat = async () => {
    if (!combat) return;

    try {
      const result = await combatService.completeCombat(combat.id);
      setCompletionResult(result);
    } catch (error: any) {
      alert(error.response?.data || 'Erro ao completar combate');
    }
  };

  const handleFlee = async () => {
    if (!combat) return;

    if (!confirm('Tem certeza que deseja fugir do combate? Você não receberá recompensas.')) {
      return;
    }

    try {
      await combatService.flee(combat.id);
      alert('Você fugiu do combate!');
      navigate('/quests/catalog');
    } catch (error: any) {
      alert(error.response?.data || 'Erro ao fugir');
    }
  };

  const getDiceCount = (diceType: string): number => {
    if (!inventory) return 0;
    switch (diceType) {
      case 'D6':
        return inventory.d6Count;
      case 'D8':
        return inventory.d8Count;
      case 'D12':
        return inventory.d12Count;
      case 'D20':
        return inventory.d20Count;
      default:
        return 0;
    }
  };

  const getDiceIcon = (diceType: string): string => {
    switch (diceType) {
      case 'D6':
        return '🎲';
      case 'D8':
        return '🎯';
      case 'D12':
        return '⚡';
      case 'D20':
        return '👑';
      default:
        return '🎲';
    }
  };

  const getEnemyDefeated = (enemyId: number): boolean => {
    if (!combat) return false;
    return combat.combatLogs.some(
      (log) => log.enemyId === enemyId && log.success === true
    );
  };

  if (loading) {
    return (
      <>
        <HeroWidget />
        <Navbar />
        <div className="container mx-auto px-6 py-8">
          <Loading />
        </div>
      </>
    );
  }

  if (completionResult) {
    return (
      <>
        <HeroWidget />
        <Navbar />
        <div className="container mx-auto px-6 py-8 max-w-4xl">
          <Card className="bg-gradient-to-br from-yellow-900/30 to-yellow-700/30 border-yellow-500/50">
            <div className="text-center mb-6">
              <div className="text-8xl mb-4">
                {completionResult.status === 'Victory' ? '🎉' : '💀'}
              </div>
              <h1 className="text-4xl font-bold text-yellow-400 mb-2">
                {completionResult.status === 'Victory' ? 'VITÓRIA!' : 'DERROTA'}
              </h1>
              <p className="text-xl text-gray-300">{completionResult.message}</p>
            </div>

            {completionResult.droppedItems.length > 0 && (
              <div className="mb-6">
                <h2 className="text-2xl font-bold text-yellow-400 mb-4">🎁 Itens Obtidos:</h2>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  {completionResult.droppedItems.map((item) => (
                    <div
                      key={item.id}
                      className="bg-gray-800/50 rounded-lg p-4 border-2 border-yellow-500/30"
                    >
                      <h3 className="text-xl font-bold text-yellow-300 mb-2">{item.name}</h3>
                      <p className="text-sm text-gray-400 mb-2">{item.description}</p>
                      <div className="flex justify-between text-sm">
                        <span className="text-purple-400">{item.rarity}</span>
                        <span className="text-blue-400">{item.type}</span>
                      </div>
                      {(item.bonusStrength > 0 ||
                        item.bonusIntelligence > 0 ||
                        item.bonusDexterity > 0) && (
                        <div className="mt-2 text-sm text-green-400">
                          {item.bonusStrength > 0 && `+${item.bonusStrength} STR `}
                          {item.bonusIntelligence > 0 && `+${item.bonusIntelligence} INT `}
                          {item.bonusDexterity > 0 && `+${item.bonusDexterity} DEX`}
                        </div>
                      )}
                    </div>
                  ))}
                </div>
              </div>
            )}

            <div className="flex gap-4">
              <Button variant="primary" onClick={() => navigate('/profile')} className="flex-1">
                Ver Perfil
              </Button>
              <Button variant="secondary" onClick={() => navigate('/quests/catalog')} className="flex-1">
                Voltar às Missões
              </Button>
            </div>
          </Card>
        </div>
      </>
    );
  }

  if (!combat || !inventory) {
    return (
      <>
        <HeroWidget />
        <Navbar />
        <div className="container mx-auto px-6 py-8">
          <Card>
            <p className="text-red-400">Erro ao carregar combate.</p>
          </Card>
        </div>
      </>
    );
  }

  return (
    <>
      <HeroWidget />
      <Navbar />
      <div className="container mx-auto px-6 py-8">
        <div className="mb-6">
          <h1 className="text-4xl font-bold text-red-400 mb-2">⚔️ Combate: {combat.questName}</h1>
          <p className="text-gray-400">Role os dados para derrotar seus inimigos!</p>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          {/* Inimigos */}
          <div className="lg:col-span-2">
            <Card>
              <h2 className="text-2xl font-bold text-red-400 mb-4">👹 Inimigos</h2>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                {combat.enemies.map((enemy) => {
                  const defeated = getEnemyDefeated(enemy.id);
                  return (
                    <div
                      key={enemy.id}
                      className={`p-4 rounded-lg border-2 cursor-pointer transition-all ${
                        defeated
                          ? 'bg-gray-800/50 border-gray-600 opacity-50'
                          : selectedEnemy?.id === enemy.id
                          ? 'bg-red-900/30 border-red-500'
                          : 'bg-gray-800/30 border-gray-700 hover:border-red-500/50'
                      }`}
                      onClick={() => !defeated && setSelectedEnemy(enemy)}
                    >
                      <div className="flex justify-between items-start mb-2">
                        <h3 className="text-xl font-bold text-red-300">
                          {defeated && '✅ '}
                          {enemy.name}
                        </h3>
                        {enemy.isBoss && <span className="text-yellow-400 text-2xl">👑</span>}
                      </div>
                      <p className="text-sm text-gray-400 mb-2">{enemy.type}</p>
                      <div className="flex justify-between items-center">
                        <span className="text-sm text-gray-400">
                          Necessário: {getDiceIcon(enemy.requiredDiceType)} {enemy.requiredDiceType}
                        </span>
                        <span className="text-sm font-bold text-yellow-400">
                          {enemy.minimumRoll}+
                        </span>
                      </div>
                    </div>
                  );
                })}
              </div>
            </Card>

            {/* Log de Combate */}
            {combat.combatLogs.length > 0 && (
              <Card className="mt-6">
                <h2 className="text-2xl font-bold text-blue-400 mb-4">📜 Histórico</h2>
                <div className="space-y-2 max-h-64 overflow-y-auto">
                  {combat.combatLogs
                    .slice()
                    .reverse()
                    .map((log) => (
                      <div
                        key={log.id}
                        className={`p-3 rounded-lg ${
                          log.success === true
                            ? 'bg-green-900/20 border border-green-500/30'
                            : log.success === false
                            ? 'bg-red-900/20 border border-red-500/30'
                            : 'bg-gray-800/30 border border-gray-700'
                        }`}
                      >
                        <p className="text-sm text-gray-300">{log.details}</p>
                        <p className="text-xs text-gray-500 mt-1">
                          {new Date(log.timestamp).toLocaleTimeString()}
                        </p>
                      </div>
                    ))}
                </div>
              </Card>
            )}
          </div>

          {/* Painel de Dados */}
          <div>
            <Card className="sticky top-6">
              <h2 className="text-2xl font-bold text-yellow-400 mb-4">🎲 Seus Dados</h2>

              {selectedEnemy ? (
                <>
                  <div className="bg-red-900/20 border border-red-500/30 rounded-lg p-4 mb-4">
                    <p className="text-sm text-gray-400">Alvo Selecionado:</p>
                    <p className="text-lg font-bold text-red-300">{selectedEnemy.name}</p>
                    <p className="text-sm text-gray-400 mt-2">
                      Você precisa rolar {selectedEnemy.minimumRoll}+ no {selectedEnemy.requiredDiceType}
                    </p>
                  </div>

                  {lastRoll && (
                    <div
                      className={`mb-4 p-4 rounded-lg border-2 ${
                        lastRoll.success
                          ? 'bg-green-900/20 border-green-500'
                          : 'bg-red-900/20 border-red-500'
                      }`}
                    >
                      <p className="text-sm text-gray-400">Último Roll:</p>
                      <p className="text-3xl font-bold text-center my-2">{lastRoll.rollResult}</p>
                      <p
                        className={`text-sm text-center ${
                          lastRoll.success ? 'text-green-400' : 'text-red-400'
                        }`}
                      >
                        {lastRoll.message}
                      </p>
                    </div>
                  )}

                  <div className="space-y-3 mb-4">
                    {['D6', 'D8', 'D12', 'D20'].map((diceType) => {
                      const count = getDiceCount(diceType);
                      return (
                        <Button
                          key={diceType}
                          variant={count > 0 ? 'primary' : 'secondary'}
                          onClick={() => handleRollDice(diceType)}
                          disabled={rolling || count === 0}
                          className="w-full"
                        >
                          {getDiceIcon(diceType)} {diceType} ({count}x)
                        </Button>
                      );
                    })}
                  </div>

                  <div className="border-t border-gray-700 pt-4 space-y-2">
                    <Button
                      variant="success"
                      onClick={handleCompleteCombat}
                      disabled={rolling}
                      className="w-full"
                    >
                      ✅ Finalizar Combate
                    </Button>
                    <Button variant="danger" onClick={handleFlee} disabled={rolling} className="w-full">
                      🏃 Fugir
                    </Button>
                  </div>
                </>
              ) : (
                <p className="text-gray-400 text-center">Selecione um inimigo para começar!</p>
              )}
            </Card>
          </div>
        </div>
      </div>
    </>
  );
};

