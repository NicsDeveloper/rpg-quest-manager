import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Card } from '../components/Card';
import { Button } from '../components/Button';
import { combatService, CombatDetail, CombatStatus, DiceType } from '../services/combatService';
import { heroService } from '../services/heroService';
import { diceService } from '../services/diceService';

type CombatArc = 'preparation' | 'combat' | 'consequence';

const Combat: React.FC = () => {
  const navigate = useNavigate();
  const [currentArc, setCurrentArc] = useState<CombatArc>('preparation');
  const [combat, setCombat] = useState<CombatDetail | null>(null);
  const [heroes, setHeroes] = useState<any[]>([]);
  const [inventory, setInventory] = useState<any>(null);
  const [loading, setLoading] = useState(false);
  const [rolling, setRolling] = useState(false);
  const [turnTimer, setTurnTimer] = useState(60);
  const [isTimerActive, setIsTimerActive] = useState(false);
  const [heroCooldowns, setHeroCooldowns] = useState<{ [heroId: number]: number }>({});
  const [showRewards, setShowRewards] = useState(false);
  const [combatResult, setCombatResult] = useState<any>(null);

  // Carregar dados iniciais
  useEffect(() => {
    loadInitialData();
  }, []);

  // Timer do turno
  useEffect(() => {
    let interval: number;
    
    if (isTimerActive && turnTimer > 0 && combat?.isHeroTurn && combat?.status === CombatStatus.InProgress) {
      interval = setInterval(() => {
        setTurnTimer(prev => {
          if (prev <= 1) {
            setIsTimerActive(false);
            // Auto ataque do inimigo se tempo esgotar
            if (combat) {
              handleEnemyAttack();
            }
            return 60; // Reset timer
          }
          return prev - 1;
        });
      }, 1000);
    }

    return () => {
      if (interval) clearInterval(interval);
    };
  }, [isTimerActive, turnTimer, combat]);

  // Timer dos cooldowns das habilidades especiais
  useEffect(() => {
    let interval: number;
    const hasActiveCooldowns = Object.values(heroCooldowns).some(cd => cd > 0);
    
    if (hasActiveCooldowns) {
      interval = setInterval(() => {
        setHeroCooldowns(prev => {
          const newCooldowns = { ...prev };
          Object.keys(newCooldowns).forEach(heroId => {
            if (newCooldowns[parseInt(heroId)] > 0) {
              newCooldowns[parseInt(heroId)]--;
            }
          });
          return newCooldowns;
        });
      }, 1000);
    }
    return () => {
      if (interval) clearInterval(interval);
    };
  }, [heroCooldowns]);

  // Detectar fim do combate
  useEffect(() => {
    if (combat && combat.status === CombatStatus.Victory) {
      // Mostrar notificação de vitória
      console.log('🎉 COMBATE FINALIZADO COM VITÓRIA!');
      
      // Mostrar tela de recompensas após 2 segundos
      setTimeout(() => {
        setShowRewards(true);
        setCombatResult(combat);
      }, 2000);
    }
  }, [combat]);

  // Auto ataque do inimigo
  useEffect(() => {
    if (combat && !combat.isHeroTurn && combat.status === CombatStatus.InProgress) {
      const enemyAttackDelay = setTimeout(() => {
        handleEnemyAttack();
      }, 3000); // 3 segundos para inimigo atacar

      return () => clearTimeout(enemyAttackDelay);
    }
  }, [combat?.isHeroTurn, combat?.status]);

  const loadInitialData = async () => {
    try {
      setLoading(true);
      
      // Buscar party ativa
      const activeParty = await heroService.getAll();
      
      if (!activeParty || activeParty.length === 0) {
        navigate('/heroes');
        return;
      }

      setHeroes(activeParty);
      
      // Buscar inventário de dados
      try {
        const inventoryResponse = await diceService.getInventory();
        setInventory(inventoryResponse);
      } catch (error) {
        console.warn('Erro ao carregar inventário, usando dados padrão:', error);
        // Inventário padrão para demonstração
        setInventory({
          userId: 1,
          d6Count: 5,
          d10Count: 3,
          d12Count: 2,
          d20Count: 1
        });
      }
      
    } catch (error) {
      console.error('Erro ao carregar dados:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleStartCombat = async (questId: number) => {
    try {
      setLoading(true);
      
      const heroIds = heroes.map(h => h.id);
      console.log('Iniciando combate:', { questId, heroIds });
      
      const combatDetail = await combatService.startCombat(questId, heroIds);
      console.log('Combate iniciado:', combatDetail);
      
      setCombat(combatDetail);
      setCurrentArc('combat');
      setTurnTimer(60);
      setIsTimerActive(true);
      
    } catch (error: any) {
      console.error('Erro ao iniciar combate:', error);
      
      // Se o erro for 400 e mencionar combate ativo, tentar limpar
      if (error.response?.status === 400 && error.message?.includes('combate ativo')) {
        try {
          console.log('Tentando limpar combate ativo...');
          await combatService.clearActiveCombat();
          // Tentar iniciar combate novamente
          const heroIds = heroes.map(h => h.id);
          const combatDetail = await combatService.startCombat(questId, heroIds);
          
          setCombat(combatDetail);
          setCurrentArc('combat');
          setTurnTimer(60);
          setIsTimerActive(true);
          return;
        } catch (clearError) {
          console.error('Erro ao limpar combate ativo:', clearError);
        }
      }
      
      alert(error.message || 'Erro ao iniciar combate');
    } finally {
      setLoading(false);
    }
  };

  const handleRollDice = async (diceType: DiceType) => {
    if (!combat || rolling) return;

    console.log('Tentando rolar dados:', { combatId: combat.id, diceType });

    try {
      setRolling(true);

      const result = await combatService.rollDice({
        combatSessionId: combat.id,
        diceType
      });
      
      setCombat(result.updatedCombatSession);
      
      // Atualizar timer baseado no resultado
      if (result.updatedCombatSession.isHeroTurn) {
        setTurnTimer(60);
        setIsTimerActive(true);
      } else {
        setIsTimerActive(false);
      }
      
    } catch (error: any) {
      console.error('Erro ao rolar dados:', error);
      alert(error.message || 'Erro ao rolar dados');
    } finally {
      setRolling(false);
    }
  };

  const handleEnemyAttack = async () => {
    if (!combat) return;

    try {
      const result = await combatService.enemyAttack({
        combatSessionId: combat.id
      });
      
      setCombat(result.updatedCombatSession);
      
      // Verificar se combate terminou
      if (result.updatedCombatSession.status === CombatStatus.Victory || 
          result.updatedCombatSession.status === CombatStatus.Defeat) {
        setCurrentArc('consequence');
        setIsTimerActive(false);
      } else if (result.updatedCombatSession.isHeroTurn) {
        setTurnTimer(60);
        setIsTimerActive(true);
      }
      
    } catch (error: any) {
      console.error('Erro no ataque do inimigo:', error);
    }
  };

  const handleCompleteCombat = async () => {
    if (!combat) return;

    try {
      const result = await combatService.completeCombat(combat.id);
      setCombat(result);
      setCurrentArc('consequence');
      setIsTimerActive(false);
    } catch (error: any) {
      console.error('Erro ao finalizar combate:', error);
    }
  };

  const handleCancelCombat = async () => {
    if (!combat) return;

    try {
      await combatService.cancelCombat(combat.id);
      setCombat(null);
      setCurrentArc('preparation');
      setIsTimerActive(false);
    } catch (error: any) {
      console.error('Erro ao cancelar combate:', error);
    }
  };

  const handleUseSpecialAbility = async (heroId: number) => {
    if (!combat) return;
    
    // Verificar se está em cooldown
    if (heroCooldowns[heroId] && heroCooldowns[heroId] > 0) {
      return;
    }
    
    try {
      setRolling(true);
      const result = await combatService.useSpecialAbility({
        combatSessionId: combat.id,
        heroId: heroId
      });
      setCombat(result.updatedCombatSession);
      
      // Definir cooldown de 3 turnos (180 segundos)
      setHeroCooldowns(prev => ({
        ...prev,
        [heroId]: 180
      }));
      
    } catch (error) {
      console.error('Erro ao usar habilidade especial:', error);
    } finally {
      setRolling(false);
    }
  };

  // Verificar se há combate ativo
  useEffect(() => {
    const checkActiveCombat = async () => {
      try {
        const activeCombat = await combatService.getActiveCombat(1); // TODO: Get from auth context
        if (activeCombat) {
          setCombat(activeCombat);
          setCurrentArc('combat');
          if (activeCombat.isHeroTurn && activeCombat.status === CombatStatus.InProgress) {
            setTurnTimer(60);
            setIsTimerActive(true);
          }
        }
      } catch (error) {
        console.error('Erro ao verificar combate ativo:', error);
      }
    };

    checkActiveCombat();
  }, []);

  if (loading) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-purple-900 via-blue-900 to-indigo-900 flex items-center justify-center">
        <div className="text-white text-xl">Carregando...</div>
        </div>
    );
  }

    return (
    <div className="min-h-screen bg-gradient-to-br from-purple-900 via-blue-900 to-indigo-900 p-6">
      <div className="max-w-6xl mx-auto">
        {/* Arcos da Batalha */}
        <Card className="mb-6 bg-gradient-to-r from-gray-800/50 to-gray-900/50 border-gray-700/50">
          <div className="flex items-center justify-between">
            <h2 className="text-2xl font-bold text-white mb-4">Arcos da Batalha</h2>
            {combat && (
              <div className="text-sm text-gray-400">
                {combat.status === CombatStatus.InProgress ? 'Combate Ativo' : 
                 combat.status === CombatStatus.Victory ? 'Batalha Finalizada' : 
                 combat.status === CombatStatus.Defeat ? 'Batalha Perdida' : 'Preparação'}
                </div>
              )}
            </div>

          <div className="flex items-center space-x-4">
            <div className={`flex items-center space-x-2 px-4 py-2 rounded-lg ${
              currentArc === 'preparation' ? 'bg-blue-500/20 border border-blue-400/50' : 'bg-gray-700/30'
            }`}>
              <span className="text-2xl">⚔️</span>
              <div>
                <div className="font-bold text-white">Preparação</div>
                <div className="text-sm text-gray-400">Configure sua party e equipamentos</div>
                      </div>
                        </div>
            
            <div className="text-gray-500">→</div>
            
            <div className={`flex items-center space-x-2 px-4 py-2 rounded-lg ${
              currentArc === 'combat' ? 'bg-orange-500/20 border border-orange-400/50' : 'bg-gray-700/30'
            }`}>
              <span className="text-2xl">⚡</span>
              <div>
                <div className="font-bold text-white">Combate</div>
                <div className="text-sm text-gray-400">Batalhe contra os inimigos</div>
                {currentArc === 'combat' && (
                  <div className="text-xs text-orange-400 font-bold mt-1">ATIVO</div>
                      )}
                    </div>
                </div>
            
            <div className="text-gray-500">→</div>
            
            <div className={`flex items-center space-x-2 px-4 py-2 rounded-lg ${
              currentArc === 'consequence' ? 'bg-green-500/20 border border-green-400/50' : 'bg-gray-700/30'
            }`}>
              <span className="text-2xl">🏆</span>
              <div>
                <div className="font-bold text-white">Consequência</div>
                <div className="text-sm text-gray-400">Veja os resultados e recompensas</div>
              </div>
            </div>
            </div>
          </Card>

        {/* Arco de Preparação */}
        {currentArc === 'preparation' && (
          <div className="space-y-6">
            <Card className="bg-gradient-to-r from-blue-900/30 to-indigo-900/30 border-blue-500/50">
              <h3 className="text-xl font-bold text-white mb-4">Selecionar Missão</h3>
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                {/* Lista de missões aqui */}
                <div className="bg-gray-700/30 rounded-lg p-4 border border-gray-600/30">
                  <h4 className="font-bold text-white mb-2">Missão de Exemplo</h4>
                  <p className="text-sm text-gray-400 mb-3">Uma missão de teste</p>
                  <Button 
                    onClick={() => handleStartCombat(1)}
                    className="w-full bg-blue-600 hover:bg-blue-700"
                  >
                    Iniciar Combate
                  </Button>
                </div>
        </div>
          </Card>
        </div>
        )}

        {/* Arco de Combate */}
        {currentArc === 'combat' && combat && (
          <div className="space-y-4">
            {/* Status do Combate */}
            <Card className={`${
              combat.status === CombatStatus.Victory
                ? 'bg-gradient-to-r from-yellow-900/30 to-orange-900/30 border-yellow-500/50'
                : combat.isHeroTurn 
                  ? 'bg-gradient-to-r from-green-900/30 to-emerald-900/30 border-green-500/50' 
                  : 'bg-gradient-to-r from-red-900/30 to-orange-900/30 border-red-500/30'
            }`}>
              <div className="text-center">
                <h3 className="text-lg font-bold mb-2">
                  {combat.status === CombatStatus.Victory ? (
                    <span className="text-yellow-400 flex items-center justify-center gap-2 animate-bounce">
                      <span>🏆</span>
                      VITÓRIA! - {combat.questName}
                      <span>🏆</span>
                    </span>
                  ) : combat.isHeroTurn ? (
                    <span className="text-green-400 flex items-center justify-center gap-2">
                      <span className="animate-pulse">🛡️</span>
                      SUA VEZ - {combat.questName}
                      <span className="animate-pulse">🛡️</span>
                    </span>
                  ) : (
                    <span className="text-red-400 flex items-center justify-center gap-2">
                      <span className="animate-pulse">👹</span>
                      VEZ DO INIMIGO - {combat.questName}
                      <span className="animate-pulse">👹</span>
                    </span>
                  )}
                </h3>
                
                <div className={`inline-flex items-center gap-2 px-3 py-1 rounded-full text-sm font-bold ${
                  combat.isHeroTurn 
                    ? 'bg-green-500/20 text-green-300 border border-green-400/50' 
                    : 'bg-red-500/20 text-red-300 border border-red-400/50'
                }`}>
                  {combat.isHeroTurn ? (
                    <>
                      <span className="animate-bounce">⚡</span>
                      SUA VEZ DE AGIR!
                      <span className="animate-bounce">⚡</span>
                    </>
                  ) : (
                    <>
                      <span className="animate-pulse">⏳</span>
                      AGUARDE O INIMIGO...
                      <span className="animate-pulse">⏳</span>
                    </>
                  )}
              </div>
              
                {/* Timer */}
                {combat.isHeroTurn && (
                  <div className="mt-3">
                    <div className={`text-2xl font-bold ${
                      turnTimer <= 10 ? 'text-red-400 animate-pulse' : 
                      turnTimer <= 20 ? 'text-yellow-400' : 'text-green-400'
                    }`}>
                      {turnTimer}s
                    </div>
                    <div className="text-xs text-gray-400 mt-1">Tempo para sua ação</div>
                    <div className="w-full bg-gray-700 rounded-full h-2 mt-2">
                      <div 
                        className={`h-2 rounded-full transition-all duration-1000 ${
                          turnTimer <= 15 ? 'bg-red-500' : 
                          turnTimer <= 30 ? 'bg-yellow-500' : 'bg-green-500'
                        }`}
                        style={{ width: `${(turnTimer / 60) * 100}%` }}
                      ></div>
                    </div>
                  </div>
                )}
              </div>
            </Card>

            {/* Condições Ambientais */}
            {combat.environmentalCondition && (
              <Card className="bg-gradient-to-r from-purple-900/30 to-indigo-900/30 border-purple-500/50">
                <div className="text-center">
                  <div className="text-lg font-bold text-purple-400 mb-2 flex items-center justify-center gap-2">
                    <span className="text-2xl">{combat.environmentalCondition.icon}</span>
                    {combat.environmentalCondition.description}
                  </div>
                  <div className="text-sm text-purple-300">
                    Intensidade: {combat.environmentalCondition.intensity}
                  </div>
                </div>
              </Card>
            )}

            {/* Informações do Inimigo */}
            <Card className="bg-gradient-to-r from-red-900/30 to-orange-900/30 border-red-500/50">
              <div className="text-center">
                <h3 className="text-lg font-bold text-white mb-2 flex items-center justify-center gap-2">
                  <span className="text-red-400">👹</span>
                  {combat.currentEnemy.name}
                  <span className="text-red-400">👹</span>
                </h3>
                
                <div className="grid grid-cols-2 md:grid-cols-4 gap-4 text-sm">
                  <div className="bg-gray-700/30 rounded p-2">
                    <div className="text-red-400 font-bold">{combat.currentEnemy.power}</div>
                    <div className="text-gray-400">Poder</div>
                  </div>
                  <div className="bg-gray-700/30 rounded p-2">
                    <div className="text-blue-400 font-bold">{combat.currentEnemy.requiredDiceType}</div>
                    <div className="text-gray-400">Dado Necessário</div>
                  </div>
                  <div className="bg-gray-700/30 rounded p-2">
                    <div className="text-yellow-400 font-bold">{combat.currentEnemy.minimumRoll}+</div>
                    <div className="text-gray-400">Roll Mínimo</div>
                  </div>
                  <div className="bg-gray-700/30 rounded p-2">
                    <div className="text-green-400 font-bold">{combat.currentEnemy.type}</div>
                    <div className="text-gray-400">Tipo</div>
                  </div>
                </div>

                {/* Vida do Inimigo */}
                <div className="mt-4">
                  <div className="flex justify-between text-sm mb-1">
                    <span className="text-gray-300">Vida</span>
                    <span className="text-white font-bold">
                      {combat.currentEnemyHealth || 50}/{combat.maxEnemyHealth || 50}
                    </span>
                  </div>
                  <div className="w-full bg-gray-700 rounded-full h-3">
                    <div 
                      className="h-3 bg-red-500 rounded-full transition-all duration-500"
                      style={{ 
                        width: `${Math.max(0, Math.min(100, 
                          ((combat.currentEnemyHealth || 50) / (combat.maxEnemyHealth || 50)) * 100
                        ))}%` 
                      }}
                    ></div>
                  </div>
                  <div className="text-center text-sm text-gray-400 mt-1">
                    {Math.round(Math.max(0, Math.min(100, 
                      ((combat.currentEnemyHealth || 50) / (combat.maxEnemyHealth || 50)) * 100
                    )))}% de vida
                  </div>
                </div>

                {/* Morale do Inimigo */}
                <div className="mt-3">
                  <div className="text-xs text-gray-400 mb-2 text-center">Morale do Inimigo:</div>
                  {combat.enemyMoraleState && (
                    <div className="flex items-center gap-2 justify-center">
                      <span className="text-lg">{combat.enemyMoraleState.icon}</span>
                      <div className="flex-1 bg-gray-700 rounded-full h-2 max-w-32">
                        <div 
                          className={`h-2 rounded-full transition-all duration-300 ${
                            combat.enemyMoraleState.moralePoints <= 10 ? 'bg-red-500' :
                            combat.enemyMoraleState.moralePoints <= 30 ? 'bg-orange-500' :
                            combat.enemyMoraleState.moralePoints <= 70 ? 'bg-yellow-500' :
                            combat.enemyMoraleState.moralePoints <= 90 ? 'bg-green-500' : 'bg-emerald-500'
                          }`}
                          style={{ width: `${Math.max(0, Math.min(100, combat.enemyMoraleState.moralePoints))}%` }}
                        ></div>
                      </div>
                      <span className="text-xs text-gray-300">{combat.enemyMoraleState.level}</span>
                    </div>
                  )}
                </div>

                {/* Status Effects do Inimigo */}
                <div className="mt-3">
                  <div className="text-xs text-gray-400 mb-2 text-center">Efeitos Ativos:</div>
                  <div className="flex flex-wrap gap-1 justify-center">
                    {/* Status effects reais baseados no combate */}
                    {combat.enemyStatusEffects && combat.enemyStatusEffects.length > 0 ? (
                      combat.enemyStatusEffects
                        .filter(effect => effect.isActive)
                        .map((effect, index) => (
                          <span 
                            key={index}
                            className={`text-xs px-2 py-1 rounded-full border ${
                              effect.type === 'Poisoned' ? 'bg-red-600/30 text-red-300 border-red-500/50' :
                              effect.type === 'Burning' ? 'bg-orange-600/30 text-orange-300 border-orange-500/50' :
                              effect.type === 'Frozen' ? 'bg-blue-600/30 text-blue-300 border-blue-500/50' :
                              effect.type === 'Bleeding' ? 'bg-red-700/30 text-red-200 border-red-600/50' :
                              effect.type === 'Blessed' ? 'bg-green-600/30 text-green-300 border-green-500/50' :
                              effect.type === 'Berserker' ? 'bg-purple-600/30 text-purple-300 border-purple-500/50' :
                              effect.type === 'Shielded' ? 'bg-blue-700/30 text-blue-200 border-blue-600/50' :
                              'bg-gray-600/30 text-gray-300 border-gray-500/50'
                            }`}
                          >
                            {effect.type === 'Poisoned' ? '☠️' :
                             effect.type === 'Burning' ? '🔥' :
                             effect.type === 'Frozen' ? '❄️' :
                             effect.type === 'Bleeding' ? '🩸' :
                             effect.type === 'Blessed' ? '✨' :
                             effect.type === 'Berserker' ? '😡' :
                             effect.type === 'Shielded' ? '🛡️' : '⚡'} {effect.type}
                            {effect.duration > 0 && ` (${effect.duration})`}
                          </span>
                        ))
                    ) : (
                      <span className="text-xs text-gray-500">Nenhum efeito ativo</span>
                    )}
                  </div>
                </div>
              </div>
            </Card>

            {/* Informações dos Heróis */}
            <Card className="bg-gradient-to-r from-blue-900/30 to-indigo-900/30 border-blue-500/50">
              <div className="mb-3">
                <h3 className="text-lg font-bold text-white flex items-center gap-2">
                  <span className="text-blue-400">🛡️</span>
                  Seus Heróis
                </h3>
              </div>
              
              <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                {combat.heroes.map((hero) => {
                  const heroHealth = combat.heroHealths[hero.id] || 0;
                  const maxHeroHealth = combat.maxHeroHealths[hero.id] || 100;
                  const healthPercentage = maxHeroHealth > 0 ? (heroHealth / maxHeroHealth) * 100 : 0;
                  
                  return (
                    <div key={hero.id} className="bg-gray-700/30 rounded-lg p-3 border border-gray-600/30">
                      <div className="flex items-center justify-between mb-2">
                        <div className="flex items-center gap-2">
                          <span className="text-xl">⚔️</span>
                        <div>
                            <h4 className="font-bold text-white text-base">{hero.name}</h4>
                            <p className="text-xs text-gray-400">{hero.class}</p>
                          </div>
                        </div>
                        <div className="text-right">
                          <div className="text-xs text-gray-400">Nível {hero.level}</div>
                          <div className="text-xs text-gray-500">XP: {hero.experience}</div>
                        </div>
                      </div>
                      
                      {/* Vida do Herói */}
                      <div className="mb-2">
                        <div className="flex justify-between text-xs mb-1">
                          <span className="text-gray-300">Vida</span>
                          <span className="text-white font-bold">{heroHealth}/{maxHeroHealth}</span>
                        </div>
                        <div className="w-full bg-gray-700 rounded-full h-2">
                          <div 
                            className={`h-2 rounded-full transition-all duration-500 ${
                              healthPercentage <= 25 ? 'bg-red-500' : 
                              healthPercentage <= 50 ? 'bg-yellow-500' : 'bg-green-500'
                            }`}
                            style={{ width: `${Math.max(0, healthPercentage)}%` }}
                          ></div>
                        </div>
                        <div className="text-center text-xs text-gray-400 mt-1">
                          {Math.round(healthPercentage)}% de vida
                        </div>
                      </div>
                      
                      {/* Atributos */}
                      <div className="grid grid-cols-3 gap-1 text-xs mb-2">
                        <div className="bg-gray-600/30 rounded p-1 text-center">
                          <div className="text-red-400 font-bold">{hero.strength}</div>
                          <div className="text-gray-400">Força</div>
                        </div>
                        <div className="bg-gray-600/30 rounded p-1 text-center">
                          <div className="text-blue-400 font-bold">{hero.intelligence}</div>
                          <div className="text-gray-400">Inteligência</div>
                        </div>
                        <div className="bg-gray-600/30 rounded p-1 text-center">
                          <div className="text-green-400 font-bold">{hero.dexterity}</div>
                          <div className="text-gray-400">Destreza</div>
                        </div>
                      </div>
                      
                      {/* Morale do Herói */}
                      <div className="mb-2">
                        <div className="text-xs text-gray-400 mb-1">Morale:</div>
                        {(() => {
                          const heroMorale = combat.heroMoraleStates.find(m => m.heroId === hero.id);
                          const moralePoints = heroMorale?.moralePoints || 50;
                          const moraleLevel = heroMorale?.level || 'Normal';
                          const moraleIcon = heroMorale?.icon || '😐';
                          
                          return (
                            <div className="flex items-center gap-2">
                              <span className="text-lg">{moraleIcon}</span>
                              <div className="flex-1 bg-gray-700 rounded-full h-2">
                                <div 
                                  className={`h-2 rounded-full transition-all duration-300 ${
                                    moralePoints <= 10 ? 'bg-red-500' :
                                    moralePoints <= 30 ? 'bg-orange-500' :
                                    moralePoints <= 70 ? 'bg-yellow-500' :
                                    moralePoints <= 90 ? 'bg-green-500' : 'bg-emerald-500'
                                  }`}
                                  style={{ width: `${Math.max(0, Math.min(100, moralePoints))}%` }}
                                ></div>
                              </div>
                              <span className="text-xs text-gray-300">{moraleLevel}</span>
                            </div>
                          );
                        })()}
                      </div>

                      {/* Status Effects do Herói */}
                      <div className="mb-2">
                        <div className="text-xs text-gray-400 mb-1">Efeitos Ativos:</div>
                        <div className="flex flex-wrap gap-1">
                          {/* Status effects reais baseados no combate */}
                          {combat.heroStatusEffects && combat.heroStatusEffects.length > 0 ? (
                            combat.heroStatusEffects
                              .filter(effect => effect.heroId === hero.id && effect.isActive)
                              .map((effect, index) => (
                                <span 
                                  key={index}
                                  className={`text-xs px-2 py-1 rounded-full border ${
                                    effect.type === 'Poisoned' ? 'bg-red-600/30 text-red-300 border-red-500/50' :
                                    effect.type === 'Burning' ? 'bg-orange-600/30 text-orange-300 border-orange-500/50' :
                                    effect.type === 'Frozen' ? 'bg-blue-600/30 text-blue-300 border-blue-500/50' :
                                    effect.type === 'Bleeding' ? 'bg-red-700/30 text-red-200 border-red-600/50' :
                                    effect.type === 'Blessed' ? 'bg-green-600/30 text-green-300 border-green-500/50' :
                                    effect.type === 'Berserker' ? 'bg-purple-600/30 text-purple-300 border-purple-500/50' :
                                    effect.type === 'Shielded' ? 'bg-blue-700/30 text-blue-200 border-blue-600/50' :
                                    'bg-gray-600/30 text-gray-300 border-gray-500/50'
                                  }`}
                                >
                                  {effect.type === 'Poisoned' ? '☠️' :
                                   effect.type === 'Burning' ? '🔥' :
                                   effect.type === 'Frozen' ? '❄️' :
                                   effect.type === 'Bleeding' ? '🩸' :
                                   effect.type === 'Blessed' ? '✨' :
                                   effect.type === 'Berserker' ? '😡' :
                                   effect.type === 'Shielded' ? '🛡️' : '⚡'} {effect.type}
                                  {effect.duration > 0 && ` (${effect.duration})`}
                                </span>
                              ))
                          ) : (
                            <span className="text-xs text-gray-500">Nenhum efeito ativo</span>
                          )}
                        </div>
                      </div>

                      {/* Botão de Habilidade Especial */}
                      {combat.isHeroTurn && (
                        <button
                          onClick={() => handleUseSpecialAbility(hero.id)}
                          disabled={rolling || (heroCooldowns[hero.id] ?? 0) > 0}
                          className={`w-full text-white text-xs font-bold py-2 px-3 rounded-lg transition-all duration-200 transform hover:scale-105 disabled:scale-100 disabled:cursor-not-allowed ${
                            (heroCooldowns[hero.id] ?? 0) > 0
                              ? 'bg-gradient-to-r from-gray-600 to-gray-700' 
                              : 'bg-gradient-to-r from-purple-600 to-pink-600 hover:from-purple-700 hover:to-pink-700'
                          }`}
                        >
                          <div className="flex items-center justify-center gap-1">
                            <span className="text-sm">⚡</span>
                            <span>
                              {(heroCooldowns[hero.id] ?? 0) > 0
                                ? `ULT (${Math.ceil((heroCooldowns[hero.id] ?? 0) / 60)}T)`
                                : 'ULT'
                              }
                            </span>
                          </div>
                        </button>
                      )}
                    </div>
                  );
                })}
              </div>
            </Card>

            {/* Painel de Ação - Vez do Jogador */}
            {combat.isHeroTurn && inventory && (
              <Card className="bg-gradient-to-r from-green-900/30 to-emerald-900/30 border-green-500/50">
                <div className="text-center mb-3">
                  <div className="text-lg font-bold text-green-400 mb-2 flex items-center justify-center gap-2">
                    <span className="animate-bounce">🎲</span>
                    SUA VEZ - USE SEUS DADOS
                    <span className="animate-bounce">🎲</span>
                  </div>
                  <div className="text-xs text-green-300 bg-green-500/20 px-2 py-1 rounded-full inline-block">
                    ⚡ Clique em um dado para atacar! ⚡
                  </div>
                  
                  {/* Indicador de Combo */}
                  {combat.comboMultiplier > 1 && (
                    <div className="mt-2 p-2 bg-gradient-to-r from-purple-600/30 to-pink-600/30 border border-purple-400/50 rounded-lg">
                      <div className="text-sm font-bold text-purple-300">
                        🔥 COMBO ATIVO! x{combat.comboMultiplier} 🔥
                      </div>
                      <div className="text-xs text-purple-200">
                        {combat.consecutiveSuccesses} sucessos consecutivos!
                      </div>
                    </div>
                  )}
                </div>
                
                <div className="grid grid-cols-2 md:grid-cols-4 gap-3">
                  {inventory && Object.entries(inventory).map(([diceType, count]) => {
                    if (diceType === 'userId' || diceType === 'id') return null;
                    
                    const diceTypeEnum = parseInt(diceType.replace('d', '')) as DiceType;
                    const isDisabled = (count as number) === 0 || rolling || !combat.isHeroTurn;
                    
                    return (
                      <Button
                        key={diceType}
                        onClick={() => handleRollDice(diceTypeEnum)}
                        disabled={isDisabled}
                        className="bg-gradient-to-r from-blue-600 to-blue-700 hover:from-blue-700 hover:to-blue-800 py-2 px-3 font-bold disabled:opacity-50"
                      >
                        <div className="text-base">🎲</div>
                        <div className="text-xs">{diceTypeEnum}</div>
                        <div className="text-xs">({count as number})</div>
                      </Button>
                    );
                  })}
                </div>
              </Card>
            )}

            {/* Histórico de Combate */}
            {combat.combatLogs && combat.combatLogs.length > 0 && (
              <Card className="bg-gradient-to-r from-gray-800/50 to-gray-900/50 border-gray-700/50">
                <div className="mb-4">
                  <h3 className="text-xl font-bold text-white flex items-center gap-2">
                    <span className="text-yellow-400">📜</span>
                    Histórico da Batalha
                  </h3>
                  </div>

                <div className="space-y-2 max-h-60 overflow-y-auto">
                  {combat.combatLogs
                    .sort((a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime())
                    .map((log, index) => (
                    <div key={log.id || index} className="bg-gray-700/30 rounded-lg p-3 border border-gray-600/30">
                      <div className="flex items-start justify-between">
                        <div className="flex-1">
                          <div className="flex items-center gap-2 mb-1">
                            <span className="text-lg">
                              {log.action === 'HERO_ATTACK' ? '🎲' : 
                               log.action === 'ENEMY_ATTACK' ? '⚔️' : 
                               '📝'}
                            </span>
                            <span className="font-bold text-white">
                              {log.action === 'HERO_ATTACK' ? 'Herói atacou' :
                               log.action === 'ENEMY_ATTACK' ? 'Inimigo atacou' :
                               log.action}
                            </span>
                            {log.success !== undefined && (
                              <span className={`text-xs px-2 py-1 rounded ${
                                log.success ? 'bg-green-500/20 text-green-300' : 'bg-red-500/20 text-red-300'
                              }`}>
                                {log.success ? 'SUCESSO' : 'FALHOU'}
                              </span>
                            )}
                          </div>
                          
                          <div className="text-sm text-gray-300 mb-1">
                            {log.details}
                          </div>
                          
                          {log.diceUsed && (
                            <div className="text-xs text-gray-400">
                              Dado: {log.diceUsed} | Resultado: {log.diceResult}
                              {log.requiredRoll && log.requiredRoll > 0 && ` | Necessário: ${log.requiredRoll}`}
                            </div>
                          )}
                  </div>

                        <div className="text-xs text-gray-500 ml-2">
                          {new Date(log.timestamp).toLocaleTimeString('pt-BR')}
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              </Card>
            )}

            {/* Botões de Ação */}
            <div className="flex gap-4 justify-center">
                    <Button
                      onClick={handleCompleteCombat}
                className="bg-green-600 hover:bg-green-700"
                disabled={combat.status !== CombatStatus.Victory}
                    >
                Finalizar Combate
                    </Button>
                    <Button
                onClick={handleCancelCombat}
                className="bg-red-600 hover:bg-red-700"
              >
                Cancelar Combate
                    </Button>
                  </div>
                </div>
              )}

        {/* Arco de Consequência */}
        {currentArc === 'consequence' && combat && (
          <div className="space-y-6">
            <Card className="bg-gradient-to-r from-green-900/30 to-emerald-900/30 border-green-500/50">
              <div className="text-center">
                <h3 className="text-2xl font-bold text-white mb-4">
                  {combat.status === CombatStatus.Victory ? '🏆 VITÓRIA! 🏆' : '💀 DERROTA! 💀'}
                </h3>
                <p className="text-lg text-gray-300 mb-6">
                  {combat.status === CombatStatus.Victory 
                    ? 'Parabéns! Você derrotou todos os inimigos!'
                    : 'Você foi derrotado, mas não desista!'
                  }
                </p>
                <Button 
                  onClick={() => {
                    setCombat(null);
                    setCurrentArc('preparation');
                    setIsTimerActive(false);
                  }}
                  className="bg-blue-600 hover:bg-blue-700"
                >
                  Voltar ao Menu
                </Button>
              </div>
            </Card>
          </div>
        )}

        {/* Tela de Recompensas */}
        {showRewards && combatResult && (
          <div className="fixed inset-0 bg-black/80 flex items-center justify-center z-50">
            <Card className="bg-gradient-to-r from-yellow-900/90 to-orange-900/90 border-yellow-500/50 max-w-md w-full mx-4">
              <div className="text-center p-6">
                <div className="text-6xl mb-4">🏆</div>
                <h2 className="text-2xl font-bold text-yellow-400 mb-4">
                  VITÓRIA ÉPICA!
                </h2>
                
                <div className="space-y-3 mb-6">
                  <div className="bg-green-600/30 rounded-lg p-3 border border-green-500/50">
                    <div className="text-green-300 font-bold">✨ Recompensas Obtidas</div>
                    <div className="text-sm text-gray-300 mt-1">
                      {combatResult.questName}
                    </div>
                  </div>
                  
                  <div className="grid grid-cols-2 gap-3">
                    <div className="bg-blue-600/30 rounded-lg p-3 border border-blue-500/50">
                      <div className="text-blue-300 font-bold">💎 Ouro</div>
                      <div className="text-xl text-white">
                        +{combatResult.quest?.goldReward || 100}
                      </div>
                    </div>
                    
                    <div className="bg-purple-600/30 rounded-lg p-3 border border-purple-500/50">
                      <div className="text-purple-300 font-bold">⭐ Experiência</div>
                      <div className="text-xl text-white">
                        +{combatResult.quest?.experienceReward || 50}
                      </div>
                    </div>
                  </div>

                  {/* Informações dos Heróis Atualizadas */}
                  <div className="bg-yellow-600/30 rounded-lg p-3 border border-yellow-500/50">
                    <div className="text-yellow-300 font-bold mb-2">🦸 Heróis Atualizados</div>
                    <div className="space-y-1">
                      {combatResult.heroes?.map((hero: any) => (
                        <div key={hero.id} className="text-sm text-gray-200">
                          <span className="font-bold">{hero.name}</span> - 
                          Nível {hero.level} | 
                          XP: {hero.experience} | 
                          Ouro: {hero.gold} | 
                          STR: {hero.strength} | 
                          INT: {hero.intelligence} | 
                          DEX: {hero.dexterity}
                        </div>
                      ))}
                    </div>
                  </div>
                </div>

                <div className="space-y-2">
                  <Button 
                    onClick={() => {
                      setShowRewards(false);
                      setCombatResult(null);
                      setCombat(null);
                      setCurrentArc('preparation');
                      setIsTimerActive(false);
                      navigate('/quests');
                    }}
                    className="w-full bg-gradient-to-r from-green-600 to-emerald-600 hover:from-green-700 hover:to-emerald-700 text-white font-bold py-3 px-6 rounded-lg"
                  >
                    Continuar Aventura
                  </Button>
                  
                  <Button 
                    onClick={() => {
                      setShowRewards(false);
                      setCombatResult(null);
                    }}
                    className="w-full bg-gray-600 hover:bg-gray-700 text-white font-bold py-2 px-4 rounded-lg"
                  >
                    Ver Detalhes do Combate
                  </Button>
                </div>
              </div>
            </Card>
          </div>
        )}
      </div>
    </div>
  );
};

export default Combat;