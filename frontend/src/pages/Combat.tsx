import React, { useState, useEffect } from 'react';
import { heroService, type Hero } from '../services/heroService';
import { FadeIn, SlideIn } from '../components/animations';
import { useToast } from '../components/Toast';
import { soundService } from '../services/sound';
import { api } from '../services/api';
import { 
  Sword, 
  Shield, 
  Heart, 
  Zap, 
  Target, 
  Skull, 
  Play, 
  RotateCcw,
  AlertTriangle,
  XCircle,
  Clock,
  Dice1,
  Dice2,
  Dice3,
  Dice4,
  Dice5,
  Dice6,
  Crown,
  Timer,
  Map as MapIcon
} from 'lucide-react';

type CombatState = {
  hero: {
    id: number
    name: string
    level: number
    experience: number
    nextLevelExperience: number
    health: number
    maxHealth: number
    attack: number
    defense: number
    morale: number
    moraleLevel: string
    gold: number
  }
  monster: {
    id: number
    name: string
    type: string
    rank: string
    habitat: string
    health: number
    maxHealth: number
    attack: number
    defense: number
    experienceReward: number
  }
  combat: {
    damageToMonster: number
    damageToHero: number
    isCritical: boolean
    isFumble: boolean
    combatEnded: boolean
    victory: boolean
    experienceGained: number
    actionDescription: string
    appliedEffects: string[]
    goldReward: number
  }
}

type CombatPhase = 'preparation' | 'combat' | 'consequence';

type DiceType = 'd4' | 'd6' | 'd8' | 'd10' | 'd12' | 'd20';


export default function Combat() {
  const [currentHero, setCurrentHero] = useState<Hero | null>(null);
  const { showToast } = useToast();
  const [combatState, setCombatState] = useState<CombatState | null>(null);
  const [log, setLog] = useState<string[]>([]);
  const [loading, setLoading] = useState(false);
  const [combatStarted, setCombatStarted] = useState(false);
  const [selectedMonster, setSelectedMonster] = useState<number | null>(null);
  const [currentPhase, setCurrentPhase] = useState<CombatPhase>('preparation');
  const [activeQuest, setActiveQuest] = useState<any>(null);
  const [questMonster, setQuestMonster] = useState<any>(null);
  const [hasActiveQuest, setHasActiveQuest] = useState(false);
  const [turnTimer, setTurnTimer] = useState(60);
  const [isTimerActive, setIsTimerActive] = useState(false);
  const [isHeroTurn, setIsHeroTurn] = useState(true);
  const [diceInventory, setDiceInventory] = useState<Record<DiceType, number>>({
    d4: 3,
    d6: 5,
    d8: 2,
    d10: 3,
    d12: 2,
    d20: 1
  });
  const [lastDiceRoll, setLastDiceRoll] = useState<{ type: DiceType; result: number } | null>(null);
  const [showDiceModal, setShowDiceModal] = useState(false);

  // Carregar herói atual
  useEffect(() => {
    const loadCurrentHero = async () => {
      try {
        const heroes = await heroService.getActiveParty();
        if (heroes.length > 0) {
          setCurrentHero(heroes[0]); // Usar o primeiro herói da party ativa
        }
      } catch (error) {
        console.error('Erro ao carregar herói:', error);
      }
    };

    loadCurrentHero();
  }, []);

  // Timer do turno
  useEffect(() => {
    let interval: number;
    
    if (isTimerActive && turnTimer > 0 && isHeroTurn && combatStarted) {
      interval = setInterval(() => {
        setTurnTimer(prev => {
          if (prev <= 1) {
            setIsTimerActive(false);
            // Auto ataque do inimigo se tempo esgotar
            handleEnemyTurn();
            return 60; // Reset timer
          }
          return prev - 1;
        });
      }, 1000);
    }

    return () => {
      if (interval) clearInterval(interval);
    };
  }, [isTimerActive, turnTimer, isHeroTurn, combatStarted]);

  // Auto ataque do inimigo
  useEffect(() => {
    if (combatStarted && !isHeroTurn && combatState && !combatState.combat.combatEnded) {
      const enemyAttackDelay = setTimeout(() => {
        handleEnemyTurn();
      }, 3000); // 3 segundos para inimigo atacar

      return () => clearTimeout(enemyAttackDelay);
    }
  }, [isHeroTurn, combatStarted, combatState]);

  // Carregar missão ativa
  const loadActiveQuest = async () => {
    if (!currentHero) return;
    
    try {
      const { data } = await api.get(`/combat/active-quest/${currentHero.id}`);
      setHasActiveQuest(data.hasActiveQuest);
      setActiveQuest(data.quest);
      setQuestMonster(data.monster);
      
      if (data.monster) {
        setSelectedMonster(data.monster.id);
        // Auto-iniciar combate se há missão ativa
        setTimeout(() => {
          startCombat(data.monster.id);
        }, 500);
      }
    } catch (error) {
      console.error('Erro ao carregar missão ativa:', error);
      setHasActiveQuest(false);
      setActiveQuest(null);
      setQuestMonster(null);
    }
  };

  useEffect(() => {
    if (currentHero) {
      loadActiveQuest();
    }
  }, [currentHero]);

  const startCombat = async (monsterId: number) => {
    if (!currentHero) return;
    
    setLoading(true);
    soundService.playClick();
    try {
      const { data } = await api.post('/combat/start', { 
        heroId: currentHero.id, 
        monsterId: monsterId 
      });
      setCombatState(data);
      setCombatStarted(true);
      setCurrentPhase('combat');
      setIsHeroTurn(true);
      setTurnTimer(60);
      setIsTimerActive(true);
      setLog(prev => ['Combate iniciado!', ...prev]);
      showToast({
        type: 'success',
        title: 'Combate iniciado!',
        message: 'Prepare-se para a batalha!'
      });
    } catch (error: any) {
      console.error('Erro ao iniciar combate:', error);
      showToast({
        type: 'error',
        title: 'Erro ao iniciar combate',
        message: error.message || 'Tente novamente mais tarde.'
      });
    } finally {
      setLoading(false);
    }
  };

  const rollDice = (diceType: DiceType) => {
    if (diceInventory[diceType] <= 0 || !isHeroTurn) return;
    
    const maxValue = parseInt(diceType.replace('d', ''));
    const result = Math.floor(Math.random() * maxValue) + 1;
    
    setLastDiceRoll({ type: diceType, result });
    setShowDiceModal(true);
    
    // Consumir o dado
    setDiceInventory(prev => ({
      ...prev,
      [diceType]: prev[diceType] - 1
    }));
    
    // Processar ataque com o resultado do dado
    setTimeout(() => {
      handleAttackWithDice(diceType, result);
    }, 2000);
  };

  const handleAttackWithDice = async (diceType: DiceType, diceResult: number) => {
    if (!combatState || !currentHero) return;
    
    setLoading(true);
    soundService.playClick();
    try {
      const { data } = await api.post('/combat/attack', { 
        heroId: currentHero.id, 
        monsterId: combatState.monster.id,
        diceType: diceType,
        diceResult: diceResult
      });
      setCombatState(data);
      
      const logEntry = data.combat.actionDescription;
      if (data.combat.combatEnded) {
        const result = data.combat.victory ? 'VITÓRIA!' : 'DERROTA!';
        setLog(prev => [`${result} ${logEntry}`, ...prev]);
        setCurrentPhase('consequence');
        setIsTimerActive(false);
        if (data.combat.victory) {
          soundService.playSuccess();
          showToast({
            type: 'success',
            title: 'Vitória!',
            message: 'Você derrotou o monstro e completou a missão!'
          });
          
          // Recarregar missão ativa para atualizar o status
          setTimeout(() => {
            loadActiveQuest();
          }, 1000);
        } else {
          soundService.playError();
          showToast({
            type: 'error',
            title: 'Derrota!',
            message: 'Você foi derrotado!'
          });
        }
      } else {
        setLog(prev => [logEntry, ...prev]);
        // Alternar turno
        setIsHeroTurn(false);
        setIsTimerActive(false);
      }
    } catch (error: any) {
      console.error('Erro ao atacar:', error);
      soundService.playError();
      showToast({
        type: 'error',
        title: 'Erro no ataque',
        message: error.message || 'Tente novamente.'
      });
    } finally {
      setLoading(false);
      setShowDiceModal(false);
    }
  };

  const handleEnemyTurn = async () => {
    if (!combatState || !currentHero) return;
    
    setLoading(true);
    try {
      // O backend já processa o contra-ataque do monstro automaticamente
      // quando o herói ataca. Para simular o turno do inimigo, vamos
      // fazer um ataque "automático" do herói (sem dados) que acionará
      // o sistema de contra-ataque
      const { data } = await api.post('/combat/attack', { 
        heroId: currentHero.id, 
        monsterId: combatState.monster.id 
      });
      setCombatState(data);
      
      const logEntry = data.combat.actionDescription;
      if (data.combat.combatEnded) {
        const result = data.combat.victory ? 'VITÓRIA!' : 'DERROTA!';
        setLog(prev => [`${result} ${logEntry}`, ...prev]);
        setCurrentPhase('consequence');
        setIsTimerActive(false);
        if (data.combat.victory) {
          soundService.playSuccess();
          showToast({
            type: 'success',
            title: 'Vitória!',
            message: 'Você derrotou o monstro e completou a missão!'
          });
          // Recarregar missão ativa para atualizar o status
          setTimeout(() => {
            loadActiveQuest();
          }, 1000);
        } else {
          soundService.playError();
          showToast({
            type: 'error',
            title: 'Derrota!',
            message: 'Você foi derrotado!'
          });
        }
      } else {
        setLog(prev => [logEntry, ...prev]);
        // Voltar para o turno do herói
        setIsHeroTurn(true);
        setTurnTimer(60);
        setIsTimerActive(true);
      }
    } catch (error: any) {
      console.error('Erro no turno do inimigo:', error);
      showToast({
        type: 'error',
        title: 'Erro no combate',
        message: error.message || 'Tente novamente.'
      });
    } finally {
      setLoading(false);
    }
  };

  const tryEscape = async () => {
    if (!combatState || !currentHero) return;
    
    setLoading(true);
    soundService.playClick();
    try {
      const { data } = await api.post('/combat/escape', { 
        heroId: currentHero.id, 
        monsterId: combatState.monster.id 
      });
      if (data.success) {
        setLog(prev => ['Fuga bem-sucedida!', ...prev]);
        setCombatStarted(false);
        setCurrentPhase('preparation');
        setIsTimerActive(false);
        soundService.playSuccess();
        showToast({
          type: 'success',
          title: 'Fuga bem-sucedida!',
          message: 'Você escapou do combate!'
        });
      } else {
        setLog(prev => ['Fuga falhou!', ...prev]);
        soundService.playError();
        showToast({
          type: 'warning',
          title: 'Fuga falhou!',
          message: 'O monstro te impediu de fugir!'
        });
      }
    } catch (error: any) {
      console.error('Erro ao tentar fugir:', error);
      soundService.playError();
      showToast({
        type: 'error',
        title: 'Erro na fuga',
        message: error.message || 'Tente novamente.'
      });
    } finally {
      setLoading(false);
    }
  };

  const getDiceIcon = (diceType: DiceType) => {
    switch (diceType) {
      case 'd4': return Dice1;
      case 'd6': return Dice2;
      case 'd8': return Dice3;
      case 'd10': return Dice4;
      case 'd12': return Dice5;
      case 'd20': return Dice6;
      default: return Dice1;
    }
  };



  const getMoraleLevelName = (level: string) => {
    switch (level) {
      case 'Despair': return 'Desespero'
      case 'Low': return 'Baixo'
      case 'Normal': return 'Normal'
      case 'High': return 'Alto'
      case 'Inspired': return 'Inspirado'
      default: return level
    }
  }

  if (!currentHero) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-slate-900 via-purple-900 to-slate-900 flex items-center justify-center relative overflow-hidden">
        <div className="absolute inset-0 overflow-hidden">
          {[...Array(15)].map((_, i) => (
            <div
              key={i}
              className="absolute w-2 h-2 bg-amber-400 rounded-full opacity-30 animate-pulse"
              style={{
                left: `${Math.random() * 100}%`,
                top: `${Math.random() * 100}%`,
                animationDelay: `${Math.random() * 3}s`,
                animationDuration: `${2 + Math.random() * 3}s`
              }}
            />
          ))}
        </div>
        <div className="text-center relative z-10">
          <div className="inline-block p-6 bg-gradient-to-br from-amber-500 to-orange-600 rounded-full shadow-lg shadow-amber-500/50 animate-pulse mb-4">
            <AlertTriangle className="h-16 w-16 text-white" />
          </div>
          <h2 className="text-4xl font-black text-transparent bg-clip-text bg-gradient-to-r from-amber-300 via-amber-500 to-orange-600 mb-2">Nenhum Personagem</h2>
          <p className="text-gray-400 text-lg">Crie um personagem para começar a combater</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-purple-900 to-slate-900 p-6">
      <div className="max-w-7xl mx-auto">
      <FadeIn delay={0}>
          <div className="text-center mb-8">
            <h1 className="hero-title text-6xl font-black mb-4">Combate</h1>
            <p className="text-xl text-gray-300">Enfrente monstros e ganhe experiência</p>
        </div>
      </FadeIn>
      
        {/* Arcos da Batalha */}
        <div className="card backdrop-blur-sm bg-black/20 mb-8">
          <div className="flex items-center justify-between mb-6">
            <h2 className="text-2xl font-bold text-gradient">Arcos da Batalha</h2>
            {combatState && (
              <div className="text-sm text-gray-400">
                {combatState.combat.combatEnded 
                  ? (combatState.combat.victory ? 'Batalha Finalizada' : 'Batalha Perdida')
                  : 'Combate Ativo'
                }
              </div>
            )}
          </div>

          <div className="flex items-center space-x-4">
            <div className={`flex items-center space-x-2 px-4 py-2 rounded-lg ${
              currentPhase === 'preparation' ? 'bg-blue-500/20 border border-blue-400/50' : 'bg-gray-700/30'
            }`}>
              <span className="text-2xl">⚔️</span>
              <div>
                <div className="font-bold text-white">Preparação</div>
                <div className="text-sm text-gray-400">Configure sua party e equipamentos</div>
              </div>
            </div>
            
            <div className="text-gray-500">→</div>
            
            <div className={`flex items-center space-x-2 px-4 py-2 rounded-lg ${
              currentPhase === 'combat' ? 'bg-orange-500/20 border border-orange-400/50' : 'bg-gray-700/30'
            }`}>
              <span className="text-2xl">⚡</span>
              <div>
                <div className="font-bold text-white">Combate</div>
                <div className="text-sm text-gray-400">Batalhe contra os inimigos</div>
                {currentPhase === 'combat' && (
                  <div className="text-xs text-orange-400 font-bold mt-1">ATIVO</div>
                )}
              </div>
            </div>
            
            <div className="text-gray-500">→</div>
            
            <div className={`flex items-center space-x-2 px-4 py-2 rounded-lg ${
              currentPhase === 'consequence' ? 'bg-green-500/20 border border-green-400/50' : 'bg-gray-700/30'
            }`}>
              <span className="text-2xl">🏆</span>
              <div>
                <div className="font-bold text-white">Consequência</div>
                <div className="text-sm text-gray-400">Veja os resultados e recompensas</div>
              </div>
            </div>
          </div>
        </div>

               {/* Arco de Preparação */}
               {currentPhase === 'preparation' && (
                 <SlideIn direction="up" delay={100}>
                   <div className="card backdrop-blur-sm bg-black/20">
                     {!hasActiveQuest ? (
                       <div className="text-center py-8">
                         <div className="p-6 bg-gradient-to-br from-amber-500 to-orange-600 rounded-xl shadow-lg mx-auto w-24 h-24 flex items-center justify-center mb-6">
                           <MapIcon className="h-12 w-12 text-white" />
                         </div>
                         <h3 className="text-2xl font-bold text-gradient mb-4">Nenhuma Missão Ativa</h3>
                         <p className="text-gray-300 mb-6">
                           Você precisa iniciar uma missão para poder combater. 
                           Vá até a aba "Missões" e escolha uma missão para começar sua aventura!
                         </p>
                         <div className="flex justify-center">
                           <button
                             onClick={() => window.location.href = '/quests'}
                             className="btn btn-primary"
                           >
                             <MapIcon className="h-5 w-5 mr-2" />
                             Ir para Missões
                           </button>
                         </div>
                       </div>
                     ) : (
                       <>
                         <h3 className="text-xl font-bold text-gradient mb-6">Missão Ativa</h3>
                         
                         {/* Informações da Missão */}
                         <div className="mb-6 p-4 bg-gradient-to-r from-blue-900/30 to-purple-900/30 rounded-xl border border-blue-500/50">
                           <h4 className="text-lg font-bold text-blue-400 mb-2">{activeQuest?.title}</h4>
                           <p className="text-gray-300 text-sm mb-3">{activeQuest?.description}</p>
                           <div className="flex items-center space-x-4 text-xs">
                             <span className="text-gray-400">Alvo: <span className="text-amber-400 font-bold">{activeQuest?.targetMonsterName}</span></span>
                             <span className="text-gray-400">Ambiente: <span className="text-green-400 font-bold">{activeQuest?.environment}</span></span>
                             <span className="text-gray-400">Dificuldade: <span className="text-red-400 font-bold">{activeQuest?.difficulty}</span></span>
                           </div>
                         </div>

                         {/* Monstro da Missão */}
                         {questMonster && (
                           <div className="mb-6">
                             <h4 className="text-lg font-bold text-gradient mb-4">Inimigo da Missão</h4>
                             <div
                               className={`p-4 rounded-xl border-2 transition-all duration-300 cursor-pointer hover:scale-105 ${
                                 selectedMonster === questMonster.id 
                                   ? 'border-amber-500/50 bg-amber-900/20' 
                                   : 'border-gray-700/50 bg-gray-900/20 hover:border-gray-600/70'
                               }`}
                               onClick={() => setSelectedMonster(questMonster.id)}
                             >
                               <div className="flex items-center space-x-3">
                                 <div className="p-3 bg-gradient-to-br from-red-500 to-red-600 rounded-xl shadow-lg">
                                   <Skull className="h-8 w-8 text-white" />
                                 </div>
                                 <div className="flex-1">
                                   <h3 className="font-bold text-gradient">{questMonster.name}</h3>
                                   <p className="text-sm text-gray-400">
                                     {questMonster.type} • Nível {questMonster.level || 1}
                                   </p>
                                   <div className="flex items-center space-x-4 mt-2 text-xs">
                                     <span className="text-red-400">❤️ {questMonster.health}/{questMonster.maxHealth}</span>
                                     <span className="text-orange-400">⚔️ {questMonster.attack}</span>
                                     <span className="text-blue-400">🛡️ {questMonster.defense}</span>
                                   </div>
                                 </div>
                               </div>
                             </div>
                           </div>
                         )}
                         
                         <div className="mt-6 flex justify-center">
                           <button
                             onClick={() => selectedMonster && startCombat(selectedMonster)}
                             disabled={loading || !selectedMonster}
                             className={`btn ${selectedMonster ? 'btn-primary' : 'btn-secondary'}`}
                           >
                             {loading ? (
                               <div className="animate-spin rounded-full h-5 w-5 border-2 border-white border-t-transparent mr-2"></div>
                             ) : (
                               <Play className="h-5 w-5 mr-2" />
                             )}
                             {loading ? 'Iniciando...' : 'Iniciar Combate'}
                           </button>
                         </div>
                       </>
                     )}
                   </div>
                 </SlideIn>
               )}

        {/* Arco de Combate */}
        {currentPhase === 'combat' && combatState && (
          <div className="space-y-6">
            {/* Status do Combate */}
            <div className={`card backdrop-blur-sm ${
              combatState.combat.victory
                ? 'bg-yellow-900/30 border-yellow-500/50'
                : isHeroTurn 
                  ? 'bg-green-900/30 border-green-500/50' 
                  : 'bg-red-900/30 border-red-500/30'
            }`}>
              <div className="text-center">
                <h3 className="text-lg font-bold mb-2">
                  {combatState.combat.victory ? (
                    <span className="text-yellow-400 flex items-center justify-center gap-2 animate-bounce">
                      <span>🏆</span>
                      VITÓRIA! - {combatState.monster.name}
                      <span>🏆</span>
                    </span>
                  ) : isHeroTurn ? (
                    <span className="text-green-400 flex items-center justify-center gap-2">
                      <span className="animate-pulse">🛡️</span>
                      SUA VEZ - {combatState.monster.name}
                      <span className="animate-pulse">🛡️</span>
                    </span>
                  ) : (
                    <span className="text-red-400 flex items-center justify-center gap-2">
                      <span className="animate-pulse">👹</span>
                      VEZ DO INIMIGO - {combatState.monster.name}
                      <span className="animate-pulse">👹</span>
                    </span>
                  )}
                </h3>
                
                <div className={`inline-flex items-center gap-2 px-3 py-1 rounded-full text-sm font-bold ${
                  isHeroTurn 
                    ? 'bg-green-500/20 text-green-300 border border-green-400/50' 
                    : 'bg-red-500/20 text-red-300 border border-red-400/50'
                }`}>
                  {isHeroTurn ? (
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
                {isHeroTurn && (
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
            </div>

            {/* Layout Horizontal - Baseado no Esboço */}
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 min-h-[500px]">
              {/* Coluna Esquerda - Monstro */}
              <div className="flex flex-col space-y-3">
                {/* Imagem do Monstro */}
                <div className="card backdrop-blur-sm bg-red-900/20 border-red-500/50 flex-1">
                  <div className="text-center h-full flex flex-col justify-center">
                    <div className="p-4 bg-gradient-to-br from-red-500 to-red-600 rounded-xl shadow-lg mx-auto w-20 h-20 flex items-center justify-center mb-3">
                      <Skull className="h-10 w-10 text-white" />
                    </div>
                    <h3 className="text-base font-bold text-gradient mb-1">{combatState.monster.name}</h3>
                    <p className="text-xs text-gray-400">{combatState.monster.type} • Nível {combatState.monster.rank || 1}</p>
                  </div>
                </div>

                {/* Caixa de Habilidades/Itens do Monstro */}
                <div className="card backdrop-blur-sm bg-red-900/20 border-red-500/50 h-20">
                  <h4 className="text-xs font-bold text-gradient mb-1">Habilidades do Monstro</h4>
                  <div className="flex justify-between text-xs">
                    <div className="flex items-center space-x-1">
                      <span className="text-gray-300">Ataque</span>
                      <span className="text-red-400 font-bold">{combatState.monster.attack}</span>
                    </div>
                    <div className="flex items-center space-x-1">
                      <span className="text-gray-300">Defesa</span>
                      <span className="text-blue-400 font-bold">{combatState.monster.defense}</span>
                    </div>
                  </div>
                </div>

                {/* Contagem de Tempo do Inimigo */}
                <div className="card backdrop-blur-sm bg-red-900/20 border-red-500/50 h-20">
                  <div className="text-center h-full flex flex-col justify-center">
                    <div className="p-2 bg-gradient-to-br from-red-500 to-red-600 rounded-lg mx-auto w-8 h-8 flex items-center justify-center mb-2">
                      <Timer className="h-4 w-4 text-white" />
                    </div>
                    <p className="text-xs text-gray-400 mb-1">Inimigo</p>
                    {!isHeroTurn && (
                      <div className="text-lg font-bold text-red-400">3s</div>
                    )}
                    {isHeroTurn && (
                      <div className="text-sm text-gray-500">Aguardando...</div>
                    )}
                  </div>
                </div>
              </div>

              {/* Coluna Central - Monitor da Partida */}
              <div className="flex flex-col space-y-3">
                {/* Modal de Rolagem de Dados */}
                {showDiceModal && lastDiceRoll && (
                  <div className="card backdrop-blur-sm bg-yellow-900/30 border-yellow-500/50 h-24">
                    <h4 className="text-xs font-bold text-gradient mb-1">Rolagem de Dados</h4>
                    <div className="text-center h-full flex flex-col justify-center">
                      <div className="p-2 bg-gradient-to-br from-yellow-500 to-orange-600 rounded mx-auto w-10 h-10 flex items-center justify-center mb-1">
                        {React.createElement(getDiceIcon(lastDiceRoll.type), { className: "h-5 w-5 text-white" })}
                      </div>
                      <p className="text-sm font-bold text-gradient">
                        {lastDiceRoll.type}: {lastDiceRoll.result}
                      </p>
                    </div>
                  </div>
                )}

                {/* Monitor da Partida */}
                <div className="card backdrop-blur-sm bg-black/30 border-gray-700/50 flex-1">
                  <h4 className="text-xs font-bold text-gradient mb-2">Monitor da Partida</h4>
                  <div className="space-y-1 max-h-full overflow-y-auto">
                    {log.map((entry, index) => (
                      <div 
                        key={index} 
                        className={`p-2 rounded text-xs ${
                          index === 0 
                            ? 'bg-blue-900/30 border border-blue-500/50' 
                            : 'bg-gray-800/30'
                        }`}
                      >
                        {entry}
                      </div>
                    ))}
                    {log.length === 0 && (
                      <div className="text-center py-2 text-gray-500">
                        <Target className="h-4 w-4 mx-auto mb-1" />
                        <p className="text-xs">Nenhuma ação registrada</p>
                      </div>
                    )}
                  </div>
                </div>
              </div>

              {/* Coluna Direita - Herói */}
              <div className="flex flex-col space-y-3">
                {/* Imagem do Herói */}
                <div className="card backdrop-blur-sm bg-blue-900/20 border-blue-500/50 flex-1">
                  <div className="text-center h-full flex flex-col justify-center">
                    <div className="p-4 bg-gradient-to-br from-blue-500 to-blue-600 rounded-xl shadow-lg mx-auto w-20 h-20 flex items-center justify-center mb-3">
                      <Crown className="h-10 w-10 text-white" />
                    </div>
                    <h3 className="text-base font-bold text-gradient mb-1">{combatState.hero.name}</h3>
                    <p className="text-xs text-gray-400">Nível {combatState.hero.level}</p>
                  </div>
                </div>

                {/* Caixa de Habilidades/Itens do Herói */}
                <div className="card backdrop-blur-sm bg-blue-900/20 border-blue-500/50 h-20">
                  <h4 className="text-xs font-bold text-gradient mb-1">Dados Disponíveis</h4>
                  <div className="grid grid-cols-3 gap-1">
                    {Object.entries(diceInventory).map(([diceType, count]) => {
                      const DiceIcon = getDiceIcon(diceType as DiceType);
                      const isDisabled = count === 0 || !isHeroTurn || loading;
                      
                      return (
                        <button
                          key={diceType}
                          onClick={() => rollDice(diceType as DiceType)}
                          disabled={isDisabled}
                          className={`p-1 rounded transition-all duration-200 ${
                            isDisabled
                              ? 'bg-gray-700/50 text-gray-500 cursor-not-allowed'
                              : 'bg-gradient-to-br from-amber-500 to-orange-600 hover:from-amber-600 hover:to-orange-700 text-white hover:scale-105'
                          }`}
                        >
                          <DiceIcon className="h-3 w-3 mx-auto mb-1" />
                          <div className="text-xs font-bold">{diceType}</div>
                          <div className="text-xs">({count})</div>
                        </button>
                      );
                    })}
                  </div>
                </div>

                {/* Contagem de Tempo do Herói */}
                <div className="card backdrop-blur-sm bg-blue-900/20 border-blue-500/50 h-20">
                  <div className="text-center h-full flex flex-col justify-center">
                    <div className="p-2 bg-gradient-to-br from-blue-500 to-blue-600 rounded-lg mx-auto w-8 h-8 flex items-center justify-center mb-2">
                      <Clock className="h-4 w-4 text-white" />
                    </div>
                    <p className="text-xs text-gray-400 mb-1">Herói</p>
                    {isHeroTurn && (
                      <div className={`text-lg font-bold ${
                        turnTimer <= 10 ? 'text-red-400 animate-pulse' : 
                        turnTimer <= 20 ? 'text-yellow-400' : 'text-green-400'
                      }`}>
                        {turnTimer}s
                      </div>
                    )}
                    {!isHeroTurn && (
                      <div className="text-sm text-gray-500">Aguardando...</div>
                    )}
                  </div>
                </div>
              </div>
            </div>

            {/* Status do Herói e Monstro - Abaixo do Layout Principal */}
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mt-6">
              {/* Status do Herói */}
              <div className="card backdrop-blur-sm bg-blue-900/20 border-blue-500/50">
                <h4 className="text-sm font-bold text-gradient mb-3">Status do Herói</h4>
                <div className="space-y-3">
                  <div>
                    <div className="flex justify-between text-sm text-gray-300 mb-2">
                      <span>Vida</span>
                      <span>{combatState.hero.health}/{combatState.hero.maxHealth}</span>
                    </div>
                    <div className="w-full bg-gray-700 rounded-full h-3">
                      <div 
                        className="h-3 bg-green-500 rounded-full transition-all duration-500"
                        style={{ width: `${(combatState.hero.health / combatState.hero.maxHealth) * 100}%` }}
                      ></div>
                    </div>
                  </div>
                  
                  <div>
                    <div className="flex justify-between text-sm text-gray-300 mb-2">
                      <span>Moral ({getMoraleLevelName(combatState.hero.moraleLevel)})</span>
                      <span>{combatState.hero.morale}/100</span>
                    </div>
                    <div className="w-full bg-gray-700 rounded-full h-3">
                      <div 
                        className="h-3 bg-purple-500 rounded-full transition-all duration-500"
                        style={{ width: `${combatState.hero.morale}%` }}
                      ></div>
                    </div>
                  </div>
                  
                  <div className="grid grid-cols-2 gap-4 text-sm">
                    <div className="flex items-center space-x-2">
                      <Sword className="h-4 w-4 text-red-400" />
                      <span>Ataque: {combatState.hero.attack}</span>
                    </div>
                    <div className="flex items-center space-x-2">
                      <Shield className="h-4 w-4 text-blue-400" />
                      <span>Defesa: {combatState.hero.defense}</span>
                    </div>
                  </div>
                </div>
              </div>

              {/* Status do Monstro */}
              <div className="card backdrop-blur-sm bg-red-900/20 border-red-500/50">
                <h4 className="text-sm font-bold text-gradient mb-3">Status do Monstro</h4>
                <div className="space-y-3">
                  <div>
                    <div className="flex justify-between text-sm text-gray-300 mb-2">
                      <span>Vida</span>
                      <span>{combatState.monster.health}/{combatState.monster.maxHealth}</span>
                    </div>
                    <div className="w-full bg-gray-700 rounded-full h-3">
                      <div 
                        className="h-3 bg-red-500 rounded-full transition-all duration-500"
                        style={{ width: `${(combatState.monster.health / combatState.monster.maxHealth) * 100}%` }}
                      ></div>
                    </div>
                  </div>
                  
                  <div className="text-sm text-gray-400">
                    <p>Habitat: {combatState.monster.habitat}</p>
                  </div>
                  
                  <div className="grid grid-cols-2 gap-4 text-sm">
                    <div className="flex items-center space-x-2">
                      <Sword className="h-4 w-4 text-red-400" />
                      <span>Ataque: {combatState.monster.attack}</span>
                    </div>
                    <div className="flex items-center space-x-2">
                      <Shield className="h-4 w-4 text-blue-400" />
                      <span>Defesa: {combatState.monster.defense}</span>
                    </div>
                  </div>
                  
                  <div className="flex items-center space-x-2 text-sm">
                    <Target className="h-4 w-4 text-green-400" />
                    <span>XP: {combatState.monster.experienceReward}</span>
                  </div>
                </div>
              </div>
            </div>

            {/* Botões de Ação */}
            <div className="flex gap-4 justify-center">
              <button
                onClick={tryEscape}
                disabled={loading || combatState.combat.combatEnded}
                className="btn btn-secondary"
              >
                <RotateCcw className="h-5 w-5 mr-2" />
                {loading ? 'Fugindo...' : 'Fugir'}
              </button>
            </div>
          </div>
        )}

        {/* Arco de Consequência */}
        {currentPhase === 'consequence' && combatState && (
          <SlideIn direction="up" delay={100}>
            <div className="card backdrop-blur-sm bg-black/20">
              <div className="text-center">
                <div className="p-8 bg-gradient-to-br from-amber-500 to-orange-600 rounded-xl shadow-lg mx-auto w-32 h-32 flex items-center justify-center mb-6">
                  {combatState.combat.victory ? (
                    <Crown className="h-16 w-16 text-white" />
                  ) : (
                    <Skull className="h-16 w-16 text-white" />
                  )}
                </div>
                
                <h2 className="text-3xl font-bold text-gradient mb-4">
                  {combatState.combat.victory ? 'VITÓRIA!' : 'DERROTA!'}
                </h2>
                
                <p className="text-lg text-gray-300 mb-6">
                  {combatState.combat.actionDescription}
                </p>
                
                <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
                    {combatState.combat.damageToMonster > 0 && (
                    <div className="p-4 bg-red-900/30 rounded-lg border border-red-500/30">
                      <div className="flex items-center justify-center space-x-2 text-red-400">
                        <Sword className="h-5 w-5" />
                        <span className="font-bold">Dano ao Monstro</span>
                      </div>
                      <div className="text-2xl font-bold text-white mt-2">
                        {combatState.combat.damageToMonster}
                      </div>
                      </div>
                    )}
                    
                    {combatState.combat.damageToHero > 0 && (
                    <div className="p-4 bg-orange-900/30 rounded-lg border border-orange-500/30">
                      <div className="flex items-center justify-center space-x-2 text-orange-400">
                        <Heart className="h-5 w-5" />
                        <span className="font-bold">Dano Recebido</span>
                      </div>
                      <div className="text-2xl font-bold text-white mt-2">
                        {combatState.combat.damageToHero}
                      </div>
                      </div>
                    )}
                    
                    {combatState.combat.experienceGained > 0 && (
                    <div className="p-4 bg-green-900/30 rounded-lg border border-green-500/30">
                      <div className="flex items-center justify-center space-x-2 text-green-400">
                        <Target className="h-5 w-5" />
                        <span className="font-bold">Experiência</span>
                      </div>
                      <div className="text-2xl font-bold text-white mt-2">
                        +{combatState.combat.experienceGained}
                      </div>
                      </div>
                    )}

                    {combatState.combat.victory && combatState.combat.goldReward > 0 && (
                    <div className="p-4 bg-yellow-900/30 rounded-lg border border-yellow-500/30">
                      <div className="flex items-center justify-center space-x-2 text-yellow-400">
                        <span className="text-lg">💰</span>
                        <span className="font-bold">Ouro Ganho</span>
                      </div>
                      <div className="text-2xl font-bold text-white mt-2">
                        +{combatState.combat.goldReward}
                      </div>
                      </div>
                    )}
                  </div>
                  
                <div className="flex flex-wrap gap-2 justify-center mb-6">
                    {combatState.combat.isCritical && (
                    <span className="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium bg-red-900/30 text-red-300 border border-red-500/50">
                      <Zap className="h-4 w-4 mr-1" />
                        CRÍTICO!
                      </span>
                    )}
                    
                    {combatState.combat.isFumble && (
                    <span className="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium bg-gray-900/30 text-gray-300 border border-gray-500/50">
                      <XCircle className="h-4 w-4 mr-1" />
                        FALHA!
                      </span>
                    )}
                  </div>

                {/* Seção de Recompensas Detalhadas */}
                {combatState.combat.victory && (
                  <div className="bg-gradient-to-r from-green-900/20 to-blue-900/20 rounded-xl p-6 mb-6 border border-green-500/30">
                    <h3 className="text-xl font-bold text-green-400 mb-4 flex items-center gap-2">
                      <span>🎁</span>
                      Recompensas da Vitória
                    </h3>
                    
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                      {/* Experiência e Level Up */}
                      <div className="bg-gray-800/50 rounded-lg p-4 border border-gray-600/30">
                        <h4 className="text-sm font-bold text-purple-400 mb-2 flex items-center gap-2">
                          <span>⭐</span>
                          Progressão
                        </h4>
                        <div className="space-y-2">
                          <div className="flex justify-between items-center">
                            <span className="text-gray-300 text-sm">XP Ganho:</span>
                            <span className="text-green-400 font-bold">+{combatState.combat.experienceGained}</span>
                          </div>
                          <div className="flex justify-between items-center">
                            <span className="text-gray-300 text-sm">XP Total:</span>
                            <span className="text-blue-400 font-bold">{combatState.hero.experience}</span>
                          </div>
                          <div className="flex justify-between items-center">
                            <span className="text-gray-300 text-sm">Próximo Nível:</span>
                            <span className="text-yellow-400 font-bold">{combatState.hero.nextLevelExperience - combatState.hero.experience} XP</span>
                          </div>
                        </div>
                      </div>

                      {/* Ouro e Recursos */}
                      <div className="bg-gray-800/50 rounded-lg p-4 border border-gray-600/30">
                        <h4 className="text-sm font-bold text-yellow-400 mb-2 flex items-center gap-2">
                          <span>💰</span>
                          Recursos
                        </h4>
                        <div className="space-y-2">
                          <div className="flex justify-between items-center">
                            <span className="text-gray-300 text-sm">Ouro Ganho:</span>
                            <span className="text-yellow-400 font-bold">+{combatState.combat.goldReward}</span>
                          </div>
                          <div className="flex justify-between items-center">
                            <span className="text-gray-300 text-sm">Ouro Total:</span>
                            <span className="text-yellow-400 font-bold">{combatState.hero.gold || 0}</span>
                          </div>
                          {combatState.combat.droppedItems && combatState.combat.droppedItems.length > 0 && (
                            <div className="flex justify-between items-center">
                              <span className="text-gray-300 text-sm">Drops:</span>
                              <span className="text-green-400 font-bold">
                                🎁 {combatState.combat.droppedItems.length} {combatState.combat.droppedItems.length === 1 ? 'item' : 'itens'}
                              </span>
                            </div>
                          )}
                        </div>
                      </div>
                    </div>

                    {/* Itens Dropados */}
                    {combatState.combat.droppedItems && combatState.combat.droppedItems.length > 0 && (
                      <div className="mt-4 p-4 bg-gradient-to-r from-green-900/30 to-emerald-900/30 rounded-lg border border-green-500/30">
                        <h4 className="text-sm font-bold text-green-400 mb-3 flex items-center gap-2">
                          <span>🎁</span>
                          Itens Dropados
                        </h4>
                        <div className="grid grid-cols-2 gap-2">
                          {combatState.combat.droppedItems.map((item: any, index: number) => (
                            <div key={index} className="flex items-center gap-2 p-2 bg-black/30 rounded-lg border border-green-500/20">
                              <span className="text-lg">
                                {item.type === 'Weapon' ? '⚔️' : 
                                 item.type === 'Armor' ? '🛡️' : 
                                 item.type === 'Potion' ? '🧪' : 
                                 item.type === 'Material' ? '💎' : '📦'}
                              </span>
                              <div className="flex-1 min-w-0">
                                <p className="text-xs font-semibold text-white truncate">{item.name}</p>
                                <p className={`text-xs ${
                                  item.rarity === 'Legendary' ? 'text-orange-400' :
                                  item.rarity === 'Epic' ? 'text-purple-400' :
                                  item.rarity === 'Rare' ? 'text-blue-400' :
                                  item.rarity === 'Uncommon' ? 'text-green-400' : 'text-gray-400'
                                }`}>
                                  {item.rarity}
                                </p>
                              </div>
                            </div>
                          ))}
                        </div>
                      </div>
                    )}

                  </div>
                )}
                  
                <button
                  onClick={() => {
                    setCombatStarted(false);
                    setCurrentPhase('preparation');
                    setCombatState(null);
                    setLog([]);
                    setSelectedMonster(null);
                    setIsHeroTurn(true);
                    setTurnTimer(60);
                    setIsTimerActive(false);
                    setShowDiceModal(false);
                    setLastDiceRoll(null);
                  }}
                  className="btn btn-primary"
                >
                  <Crown className="h-5 w-5 mr-2" />
                  Resgatar Recompensas
                </button>
                    </div>
                </div>
            </SlideIn>
          )}
        </div>
    </div>
  );
}


