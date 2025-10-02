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
import api from '../services/api';

interface Hero {
  id: number;
  name: string;
  class: string;
  level: number;
  strength: number;
  intelligence: number;
  dexterity: number;
}

export const Combat: React.FC = () => {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const questId = parseInt(searchParams.get('questId') || '0');

  const [combat, setCombat] = useState<CombatSessionDetail | null>(null);
  const [inventory, setInventory] = useState<DiceInventory | null>(null);
  const [activeParty, setActiveParty] = useState<Hero[]>([]);
  const [loading, setLoading] = useState(true);
  const [rolling, setRolling] = useState(false);
  const [selectedEnemy, setSelectedEnemy] = useState<EnemyInfo | null>(null);
  const [lastRoll, setLastRoll] = useState<RollDiceResult | null>(null);
  const [completionResult, setCompletionResult] = useState<CompleteCombatResult | null>(null);
  const [showDiscoveries, setShowDiscoveries] = useState(false);

  useEffect(() => {
    initCombat();
  }, []);

  const initCombat = async () => {
    try {
      setLoading(true);
      const hero = await profileService.getMyHero();

      // Carrega party ativa
      const party = await api.get('/profile/active-party');
      setActiveParty(party.data);

      // Verifica se já existe combate ativo
      let activeCombat = await combatService.getActiveCombat(hero.id);

      // Se não existe e foi passado um questId, inicia um novo
      if (!activeCombat && questId > 0) {
        await combatService.startCombat(hero.id, questId);
        activeCombat = await combatService.getActiveCombat(hero.id);
      }

      if (!activeCombat) {
        alert('Nenhum combate ativo encontrado. Aceite uma quest primeiro!');
        navigate('/quests/catalog');
        return;
      }

      setCombat(activeCombat);
      setSelectedEnemy(activeCombat.enemies?.[0] || null);

      const inv = await diceService.getInventory();
      setInventory(inv);
    } catch (error: any) {
      console.error('Erro ao iniciar combate:', error);
      alert(error.response?.data?.message || error.message || 'Erro ao carregar combate');
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
        diceType,
      });

      setLastRoll(result);

      // Atualiza inventário
      const updatedInventory = await diceService.getInventory();
      setInventory(updatedInventory);

      // Usa o combate atualizado que já vem na resposta
      if (result.updatedCombatSession) {
        setCombat(result.updatedCombatSession);
      }
    } catch (error: any) {
      const errorMsg = error.response?.data?.message || error.message || 'Erro ao rolar dado';
      alert(errorMsg);
      
      // Se o combate já terminou, tenta recarregar a página do catálogo
      if (errorMsg.includes('concluído com VITÓRIA')) {
        alert('🎉 Combate já foi concluído! Redirecionando...');
        navigate('/quests/catalog');
      }
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
      alert(error.response?.data?.message || error.message || 'Erro ao completar combate');
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
      alert(error.response?.data?.message || error.message || 'Erro ao fugir');
    }
  };

  const getDiceCount = (diceType: string): number => {
    if (!inventory) return 0;
    switch (diceType) {
      case 'D6':
        return inventory.d6Count;
      case 'D10':
        return inventory.d10Count;
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
      case 'D10':
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

  const getPartyPower = (): number => {
    return activeParty.reduce((sum, hero) => sum + hero.strength + hero.intelligence + hero.dexterity, 0);
  };

  const getPartyBonus = (): string => {
    const power = getPartyPower();
    if (power >= 150) return '+3 em todas as rolagens';
    if (power >= 100) return '+2 em todas as rolagens';
    if (power >= 50) return '+1 em todas as rolagens';
    return 'Sem bônus';
  };

  const getRewardPenalty = (): string => {
    if (activeParty.length === 3) return '-30% de recompensas';
    if (activeParty.length === 2) return '-15% de recompensas';
    return 'Sem penalidade';
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
              <div className="text-8xl mb-4 animate-bounce">
                {completionResult.status === 'Victory' ? '🎉' : '💀'}
              </div>
              <h1 className="text-5xl font-bold text-gradient mb-4 animate-float">
                {completionResult.status === 'Victory' ? '⚔️ VITÓRIA ÉPICA! ⚔️' : '💀 DERROTA 💀'}
              </h1>
              <p className="text-2xl text-gray-300 mb-4">{completionResult.message}</p>
              
              {activeParty.length > 1 && (
                <div className="bg-amber-900/30 border border-amber-700/50 rounded-lg p-4 mb-4">
                  <p className="text-amber-300">
                    🛡️ Sua party de <strong>{activeParty.length} heróis</strong> lutou bravamente!
                  </p>
                  <p className="text-sm text-amber-400">{getRewardPenalty()}</p>
                </div>
              )}
            </div>

            {completionResult.droppedItems.length > 0 && (
              <div className="mb-6">
                <h2 className="text-3xl font-bold text-yellow-400 mb-4 flex items-center justify-center gap-2">
                  <span className="animate-bounce">🎁</span>
                  Tesouros Obtidos
                  <span className="animate-bounce">🎁</span>
                </h2>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  {completionResult.droppedItems.map((item) => (
                    <div
                      key={item.id}
                      className="bg-gradient-to-br from-gray-800/50 to-gray-900/70 rounded-lg p-5 border-2 border-yellow-500/30 hover:border-yellow-500 hover:scale-105 transition-all shadow-lg shadow-yellow-500/20"
                    >
                      <h3 className="text-2xl font-bold text-yellow-300 mb-2 flex items-center gap-2">
                        ✨ {item.name}
                      </h3>
                      <p className="text-sm text-gray-400 mb-3">{item.description}</p>
                      <div className="flex justify-between text-sm mb-3">
                        <span className={`font-bold ${
                          item.rarity === 'Legendary' ? 'text-purple-400' :
                          item.rarity === 'Epic' ? 'text-orange-400' :
                          item.rarity === 'Rare' ? 'text-blue-400' : 'text-gray-400'
                        }`}>
                          {item.rarity === 'Legendary' && '👑 '}
                          {item.rarity === 'Epic' && '💎 '}
                          {item.rarity === 'Rare' && '⭐ '}
                          {item.rarity}
                        </span>
                        <span className="text-blue-400">{item.type}</span>
                      </div>
                      {(item.bonusStrength > 0 ||
                        item.bonusIntelligence > 0 ||
                        item.bonusDexterity > 0) && (
                        <div className="bg-green-900/30 rounded-lg p-2 text-center">
                          <p className="text-sm font-bold text-green-400">
                            {item.bonusStrength > 0 && `⚔️ +${item.bonusStrength} FOR `}
                            {item.bonusIntelligence > 0 && `🧠 +${item.bonusIntelligence} INT `}
                            {item.bonusDexterity > 0 && `🎯 +${item.bonusDexterity} DEX`}
                          </p>
                        </div>
                      )}
                    </div>
                  ))}
                </div>
              </div>
            )}

            <div className="flex gap-4">
              <Button variant="primary" onClick={() => navigate('/profile')} className="flex-1">
                📊 Ver Perfil
              </Button>
              <Button variant="secondary" onClick={() => navigate('/quests/catalog')} className="flex-1">
                🎯 Próximas Missões
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
        {/* Header Épico */}
        <div className="mb-8 text-center">
          <h1 className="text-6xl font-black mb-3 text-gradient animate-float">
            ⚔️ {combat.questName} ⚔️
          </h1>
          <p className="text-xl text-gray-400 mb-4">Prepare-se para a batalha!</p>
          
          {/* Party Composition */}
          {activeParty.length > 0 && (
            <Card className="bg-gradient-to-r from-purple-900/30 to-indigo-900/30 border-purple-500/30 max-w-4xl mx-auto">
              <div className="flex items-center justify-between mb-4">
                <h3 className="text-2xl font-bold text-purple-400 flex items-center gap-2">
                  🛡️ Sua Party ({activeParty.length}/3)
                </h3>
                <div className="text-right">
                  <p className="text-sm text-purple-300">Poder Total: <span className="font-bold text-2xl">{getPartyPower()}</span></p>
                  <p className="text-xs text-green-400">{getPartyBonus()}</p>
                  {activeParty.length > 1 && (
                    <p className="text-xs text-amber-400">{getRewardPenalty()}</p>
                  )}
                </div>
              </div>
              
              <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                {activeParty.map((hero) => (
                  <div key={hero.id} className="bg-gray-800/50 rounded-lg p-4 border-2 border-purple-500/50 hover:border-purple-400 hover:scale-105 transition-all">
                    <div className="flex items-center gap-3 mb-3">
                      <div className="w-12 h-12 bg-gradient-to-br from-purple-500 to-purple-700 rounded-xl flex items-center justify-center text-white text-xl font-bold shadow-lg">
                        {hero.name.charAt(0)}
                      </div>
                      <div className="flex-1">
                        <h4 className="font-bold text-purple-300 text-lg">{hero.name}</h4>
                        <p className="text-xs text-gray-400">{hero.class} - Nv. {hero.level}</p>
                      </div>
                    </div>
                    <div className="grid grid-cols-3 gap-2 text-center text-xs">
                      <div className="bg-red-900/30 rounded py-1">
                        <p className="text-red-400 font-bold">{hero.strength}</p>
                        <p className="text-gray-500">FOR</p>
                        {combat.heroes && combat.heroes.find(h => h.id === hero.id)?.totalAttack !== hero.strength && (
                          <p className="text-green-400 text-xs">+{combat.heroes.find(h => h.id === hero.id)?.totalAttack - hero.strength}</p>
                        )}
                      </div>
                      <div className="bg-blue-900/30 rounded py-1">
                        <p className="text-blue-400 font-bold">{hero.intelligence}</p>
                        <p className="text-gray-500">INT</p>
                        {combat.heroes && combat.heroes.find(h => h.id === hero.id)?.totalMagic !== hero.intelligence && (
                          <p className="text-green-400 text-xs">+{combat.heroes.find(h => h.id === hero.id)?.totalMagic - hero.intelligence}</p>
                        )}
                      </div>
                      <div className="bg-green-900/30 rounded py-1">
                        <p className="text-green-400 font-bold">{hero.dexterity}</p>
                        <p className="text-gray-500">DEX</p>
                        {combat.heroes && combat.heroes.find(h => h.id === hero.id)?.totalDefense !== hero.dexterity && (
                          <p className="text-green-400 text-xs">+{combat.heroes.find(h => h.id === hero.id)?.totalDefense - hero.dexterity}</p>
                        )}
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            </Card>
          )}
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          {/* Área de Inimigos e Descobertas */}
          <div className="lg:col-span-2 space-y-6">
            {/* Botão de Descobertas */}
            <div 
              className="bg-gradient-to-r from-blue-900/30 to-cyan-900/30 border-2 border-blue-500/30 rounded-2xl p-6 text-center cursor-pointer hover:scale-105 transition-all shadow-lg"
              onClick={() => setShowDiscoveries(!showDiscoveries)}
            >
              <div className="flex items-center justify-center gap-3">
                <span className="text-4xl">🔍</span>
                <div>
                  <h3 className="text-2xl font-bold text-blue-400">Explorar Área</h3>
                  <p className="text-sm text-gray-400">Clique para descobrir segredos e tesouros</p>
                </div>
                <span className="text-4xl">🗺️</span>
              </div>
            </div>

            {showDiscoveries && (
              <Card className="bg-gradient-to-br from-green-900/20 to-emerald-900/20 border-green-500/30 animate-fadeIn">
                <h3 className="text-2xl font-bold text-green-400 mb-4">🌟 Descobertas</h3>
                <div className="space-y-3">
                  <div className="bg-green-900/30 border border-green-500/50 rounded-lg p-4">
                    <p className="text-green-300">✨ Você encontrou um baú antigo com poções e artefatos mágicos!</p>
                  </div>
                  <div className="bg-blue-900/30 border border-blue-500/50 rounded-lg p-4">
                    <p className="text-blue-300">📜 Inscrições antigas revelam a fraqueza do boss...</p>
                  </div>
                  <div className="bg-purple-900/30 border border-purple-500/50 rounded-lg p-4">
                    <p className="text-purple-300">🔮 Uma aura mágica fortalece sua party (+1 bônus temporário)</p>
                  </div>
                </div>
              </Card>
            )}

            {/* Inimigos */}
            <Card className="bg-gradient-to-br from-red-900/20 to-orange-900/20 border-red-500/30">
              <h2 className="text-3xl font-bold text-red-400 mb-4 flex items-center gap-2">
                <span className="animate-pulse">👹</span>
                Inimigos na Área
                <span className="animate-pulse">👹</span>
              </h2>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                {combat.enemies?.map((enemy) => {
                  const defeated = getEnemyDefeated(enemy.id);
                  return (
                    <div
                      key={enemy.id}
                      className={`p-5 rounded-xl border-2 cursor-pointer transition-all transform hover:scale-105 ${
                        defeated
                          ? 'bg-gray-800/30 border-gray-600 opacity-40 grayscale'
                          : selectedEnemy?.id === enemy.id
                          ? 'bg-gradient-to-br from-red-900/50 to-red-800/50 border-red-500 shadow-lg shadow-red-500/50 scale-105'
                          : 'bg-gradient-to-br from-gray-800/50 to-gray-900/50 border-gray-700 hover:border-red-500/70 shadow-lg'
                      }`}
                      onClick={() => !defeated && setSelectedEnemy(enemy)}
                    >
                      <div className="flex justify-between items-start mb-3">
                        <div>
                          <h3 className="text-2xl font-bold text-red-300 flex items-center gap-2">
                            {defeated && '✅ '}
                            {enemy.name}
                          </h3>
                          {enemy.isBoss && (
                            <span className="inline-flex items-center gap-1 mt-2 px-3 py-1 bg-yellow-500/20 border border-yellow-500/50 rounded-full text-yellow-400 text-sm font-bold animate-pulse">
                              👑 BOSS
                            </span>
                          )}
                        </div>
                      </div>
                      <p className="text-sm text-gray-400 mb-4">{enemy.type}</p>
                      
                      <div className="bg-gray-900/50 rounded-lg p-3 space-y-2">
                        <div className="flex justify-between items-center">
                          <span className="text-sm text-gray-400">Dado Necessário:</span>
                          <span className="text-xl font-bold flex items-center gap-1">
                            {getDiceIcon(enemy.requiredDiceType)} {enemy.requiredDiceType}
                          </span>
                        </div>
                        <div className="flex justify-between items-center">
                          <span className="text-sm text-gray-400">Tipo de Combate:</span>
                          <span className="text-sm font-bold text-blue-400">
                            {enemy.combatType === 'Physical' && '⚔️ Físico'}
                            {enemy.combatType === 'Magical' && '🔮 Mágico'}
                            {enemy.combatType === 'Agile' && '🏃 Ágil'}
                          </span>
                        </div>
                        <div className="flex justify-between items-center">
                          <span className="text-sm text-gray-400">Rolagem Mínima:</span>
                          <span className="text-2xl font-bold text-yellow-400">{enemy.minimumRoll}+</span>
                        </div>
                        <div className="flex justify-between items-center">
                          <span className="text-sm text-gray-400">Poder:</span>
                          <span className="text-sm font-bold text-red-400">{enemy.power}</span>
                        </div>
                        <div className="flex justify-between items-center">
                          <span className="text-sm text-gray-400">Vida:</span>
                          <span className="text-sm font-bold text-green-400">{enemy.health}</span>
                        </div>
                      </div>
                      
                      {selectedEnemy?.id === enemy.id && !defeated && (
                        <div className="mt-3 text-center">
                          <span className="inline-flex items-center gap-1 px-3 py-1 bg-red-500/20 border border-red-500/50 rounded-full text-red-400 text-sm font-bold animate-pulse">
                            🎯 ALVO SELECIONADO
                          </span>
                        </div>
                      )}
                    </div>
                  );
                })}
              </div>
            </Card>

            {/* Log de Combate */}
            {combat.combatLogs.length > 0 && (
              <Card className="bg-gradient-to-br from-gray-900/80 to-gray-800/80 border-gray-700">
                <h2 className="text-2xl font-bold text-blue-400 mb-4 flex items-center gap-2">
                  📜 Histórico de Batalha
                </h2>
                <div className="space-y-2 max-h-80 overflow-y-auto scrollbar-thin pr-2">
                  {combat.combatLogs
                    .slice()
                    .reverse()
                    .map((log, index) => (
                      <div
                        key={log.id}
                        className={`p-4 rounded-lg border-2 transform transition-all hover:scale-102 ${
                          log.success === true
                            ? 'bg-gradient-to-r from-green-900/30 to-green-800/30 border-green-500/50 shadow-lg shadow-green-500/20'
                            : log.success === false
                            ? 'bg-gradient-to-r from-red-900/30 to-red-800/30 border-red-500/50 shadow-lg shadow-red-500/20'
                            : 'bg-gray-800/50 border-gray-700'
                        } ${index === 0 ? 'animate-fadeIn' : ''}`}
                      >
                        <p className="text-sm text-gray-200 font-semibold">{log.details}</p>
                        <p className="text-xs text-gray-500 mt-2">
                          🕐 {new Date(log.timestamp).toLocaleTimeString('pt-BR')}
                        </p>
                      </div>
                    ))}
                </div>
              </Card>
            )}
          </div>

          {/* Painel de Ação - Dados */}
          <div>
            <Card className="sticky top-6 bg-gradient-to-br from-yellow-900/30 to-amber-900/30 border-yellow-500/30 shadow-2xl">
              <h2 className="text-3xl font-bold text-yellow-400 mb-4 flex items-center gap-2 justify-center">
                <span className="animate-bounce">🎲</span>
                Seus Dados
                <span className="animate-bounce">🎲</span>
              </h2>

              {selectedEnemy ? (
                <>
                  <div className="bg-red-900/30 border-2 border-red-500/50 rounded-xl p-4 mb-4 shadow-lg shadow-red-500/30">
                    <p className="text-sm text-gray-400 mb-1">🎯 Alvo Selecionado:</p>
                    <p className="text-2xl font-bold text-red-300 mb-3">{selectedEnemy.name}</p>
                    <div className="bg-gray-900/50 rounded-lg p-3">
                      <p className="text-sm text-gray-400 text-center mb-2">
                        Você precisa rolar <span className="text-yellow-400 font-bold text-lg">{combat.requiredRoll}+</span>
                      </p>
                      <p className="text-sm text-gray-400 text-center mb-2">
                        no dado <span className="text-yellow-400 font-bold">{selectedEnemy.requiredDiceType}</span>
                      </p>
                      <p className="text-xs text-blue-400 text-center">
                        {combat.combatTypeDescription}
                      </p>
                    </div>
                  </div>

                  {lastRoll && (
                    <div
                      className={`mb-4 p-5 rounded-xl border-2 shadow-2xl animate-fadeIn ${
                        lastRoll.success
                          ? 'bg-gradient-to-br from-green-900/50 to-green-800/50 border-green-500 shadow-green-500/50'
                          : 'bg-gradient-to-br from-red-900/50 to-red-800/50 border-red-500 shadow-red-500/50'
                      }`}
                    >
                      <p className="text-sm text-gray-300 text-center mb-2">Resultado:</p>
                      <p className="text-6xl font-black text-center my-4 animate-bounce">{lastRoll.roll}</p>
                      <p
                        className={`text-lg font-bold text-center ${
                          lastRoll.success ? 'text-green-300' : 'text-red-300'
                        }`}
                      >
                        {lastRoll.message}
                      </p>
                    </div>
                  )}

                  {/* Bônus de Atributos */}
                  {combat.heroBonuses && combat.heroBonuses.length > 0 && (
                    <div className="mb-4 p-4 bg-gradient-to-r from-purple-900/30 to-indigo-900/30 border border-purple-500/30 rounded-lg">
                      <h3 className="text-lg font-bold text-purple-400 mb-3 flex items-center gap-2">
                        ⚡ Bônus de Atributos
                      </h3>
                      <div className="space-y-2">
                        {combat.heroBonuses.map((bonus) => (
                          <div key={bonus.heroId} className="flex justify-between items-center text-sm">
                            <span className="text-gray-300">{bonus.heroName}:</span>
                            <div className="flex gap-2">
                              <span className={`px-2 py-1 rounded text-xs ${
                                bonus.combatBonus < 0 ? 'bg-green-900/50 text-green-400' : 'bg-gray-700 text-gray-400'
                              }`}>
                                {bonus.combatBonus < 0 ? `+${Math.abs(bonus.combatBonus)}` : bonus.combatBonus} {bonus.relevantStat}
                              </span>
                            </div>
                          </div>
                        ))}
                      </div>
                    </div>
                  )}

                  <div className="space-y-3 mb-6">
                    {['D6', 'D10', 'D12', 'D20'].map((diceType) => {
                      const count = getDiceCount(diceType);
                      const isRecommended = diceType === selectedEnemy.requiredDiceType;
                      return (
                        <button
                          key={diceType}
                          onClick={() => handleRollDice(diceType)}
                          disabled={rolling || count === 0}
                          className={`w-full py-4 px-6 rounded-xl font-bold text-lg transition-all transform hover:scale-105 disabled:opacity-40 disabled:cursor-not-allowed shadow-lg ${
                            isRecommended && count > 0
                              ? 'bg-gradient-to-r from-yellow-600 to-orange-600 text-white border-2 border-yellow-400 animate-pulse shadow-yellow-500/50'
                              : count > 0
                              ? 'bg-gradient-to-r from-blue-600 to-blue-700 text-white border-2 border-blue-400 hover:from-blue-700 hover:to-blue-800 shadow-blue-500/30'
                              : 'bg-gray-700 text-gray-500 border-2 border-gray-600'
                          }`}
                        >
                          <div className="flex items-center justify-between">
                            <span className="text-3xl">{getDiceIcon(diceType)}</span>
                            <span>{diceType}</span>
                            <span className="px-3 py-1 bg-black/30 rounded-full">{count}x</span>
                          </div>
                          {isRecommended && count > 0 && (
                            <p className="text-xs mt-2 text-yellow-200">⚡ Recomendado para este inimigo!</p>
                          )}
                        </button>
                      );
                    })}
                  </div>

                  <div className="border-t-2 border-gray-700 pt-4 space-y-3">
                    <Button
                      variant="success"
                      onClick={handleCompleteCombat}
                      disabled={rolling}
                      className="w-full bg-gradient-to-r from-green-600 to-emerald-600 hover:from-green-700 hover:to-emerald-700 py-4 text-lg font-bold shadow-lg shadow-green-500/30"
                    >
                      ✅ Finalizar Combate
                    </Button>
                    <Button
                      variant="danger"
                      onClick={handleFlee}
                      disabled={rolling}
                      className="w-full bg-gradient-to-r from-red-600 to-red-700 hover:from-red-700 hover:to-red-800 py-4 text-lg font-bold shadow-lg shadow-red-500/30"
                    >
                      🏃 Fugir da Batalha
                    </Button>
                  </div>
                </>
              ) : (
                <div className="text-center py-12">
                  <div className="text-6xl mb-4 animate-bounce">⚔️</div>
                  <p className="text-gray-400 text-lg">Selecione um inimigo para começar a batalha!</p>
                </div>
              )}
            </Card>
          </div>
        </div>
      </div>
    </>
  );
};
