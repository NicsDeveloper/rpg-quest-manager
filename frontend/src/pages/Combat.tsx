import { useState } from 'react';
import { useCharacter } from '../contexts/CharacterContext';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { ProgressBar } from '../components/ui/ProgressBar';
import { FadeIn, SlideIn } from '../components/animations';
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
  XCircle
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
  }
}

export default function Combat() {
  const { character } = useCharacter();
  const [combatState, setCombatState] = useState<CombatState | null>(null);
  const [log, setLog] = useState<string[]>([]);
  const [loading, setLoading] = useState(false);
  const [combatStarted, setCombatStarted] = useState(false);
  const [selectedMonster, setSelectedMonster] = useState<number | null>(null);

  const startCombat = async (monsterId: number) => {
    if (!character) return;
    
    setLoading(true);
    soundService.playClick();
    try {
      const { data } = await api.post('/api/combat/start', { 
        characterId: character.id, 
        monsterId: monsterId 
      });
      setCombatState(data);
      setCombatStarted(true);
      setLog(prev => ['Combate iniciado!', ...prev]);
    } catch (error) {
      console.error('Erro ao iniciar combate:', error);
    } finally {
      setLoading(false);
    }
  };

  const attack = async () => {
    if (!combatState || !character) return;
    
    setLoading(true);
    soundService.playClick();
    try {
      const { data } = await api.post('/api/combat/attack', { 
        characterId: character.id, 
        monsterId: combatState.monster.id 
      });
      setCombatState(data);
      
      const logEntry = data.combat.actionDescription;
      if (data.combat.combatEnded) {
        const result = data.combat.victory ? 'VITÓRIA!' : 'DERROTA!';
        setLog(prev => [`${result} ${logEntry}`, ...prev]);
        setCombatStarted(false);
        if (data.combat.victory) {
          soundService.playSuccess();
        } else {
          soundService.playError();
        }
      } else {
        setLog(prev => [logEntry, ...prev]);
      }
    } catch (error) {
      console.error('Erro ao atacar:', error);
      soundService.playError();
    } finally {
      setLoading(false);
    }
  };

  const tryEscape = async () => {
    if (!combatState || !character) return;
    
    setLoading(true);
    soundService.playClick();
    try {
      const { data } = await api.post('/api/combat/escape', { 
        characterId: character.id, 
        monsterId: combatState.monster.id 
      });
      if (data.success) {
        setLog(prev => ['Fuga bem-sucedida!', ...prev]);
        setCombatStarted(false);
        soundService.playSuccess();
      } else {
        setLog(prev => ['Fuga falhou!', ...prev]);
        soundService.playError();
      }
    } catch (error) {
      console.error('Erro ao tentar fugir:', error);
      soundService.playError();
    } finally {
      setLoading(false);
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

  if (!character) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-center">
          <AlertTriangle className="h-12 w-12 text-yellow-500 mx-auto mb-4" />
          <p className="text-gray-500">Nenhum personagem encontrado</p>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <FadeIn delay={0}>
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Combate</h1>
          <p className="text-gray-600 mt-2">Enfrente monstros e ganhe experiência</p>
        </div>
      </FadeIn>
      
      {!combatStarted ? (
        <SlideIn direction="up" delay={100}>
          <Card title="Selecionar Monstro">
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              {[1, 2, 3, 4, 5].map((monsterId) => (
                <div
                  key={monsterId}
                  className={`p-4 border rounded-lg cursor-pointer transition-all hover:shadow-md ${
                    selectedMonster === monsterId ? 'border-blue-300 bg-blue-50' : 'border-gray-200'
                  }`}
                  onClick={() => setSelectedMonster(monsterId)}
                >
                  <div className="flex items-center space-x-3">
                    <Skull className="h-8 w-8 text-red-600" />
                    <div>
                      <h3 className="font-medium text-gray-900">Monstro {monsterId}</h3>
                      <p className="text-sm text-gray-600">Nível {monsterId}</p>
                    </div>
                  </div>
                </div>
              ))}
            </div>
            
            <div className="mt-6 flex justify-center">
              <Button
                onClick={() => selectedMonster && startCombat(selectedMonster)}
                disabled={loading || !selectedMonster}
                loading={loading}
                size="lg"
              >
                <Play className="h-5 w-5 mr-2" />
                {loading ? 'Iniciando...' : 'Iniciar Combate'}
              </Button>
            </div>
          </Card>
        </SlideIn>
      ) : (
        <div className="space-y-6">
          {combatState && (
            <SlideIn direction="up" delay={100}>
              <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                {/* Herói */}
                <Card className="border-green-200 bg-green-50">
                  <div className="flex items-center space-x-3 mb-4">
                    <Shield className="h-8 w-8 text-green-600" />
                    <div>
                      <h3 className="text-xl font-bold text-gray-900">{combatState.hero.name}</h3>
                      <p className="text-sm text-gray-600">Nível {combatState.hero.level}</p>
                    </div>
                  </div>
                  
                  <div className="space-y-3">
                    <div>
                      <div className="flex justify-between text-sm text-gray-600 mb-1">
                        <span>Vida</span>
                        <span>{combatState.hero.health}/{combatState.hero.maxHealth}</span>
                      </div>
                      <ProgressBar 
                        value={combatState.hero.health} 
                        max={combatState.hero.maxHealth}
                        className="h-3"
                      />
                    </div>
                    
                    <div>
                      <div className="flex justify-between text-sm text-gray-600 mb-1">
                        <span>Moral ({getMoraleLevelName(combatState.hero.moraleLevel)})</span>
                        <span>{combatState.hero.morale}/100</span>
                      </div>
                      <ProgressBar 
                        value={combatState.hero.morale} 
                        max={100}
                        className="h-3"
                      />
                    </div>
                    
                    <div>
                      <div className="flex justify-between text-sm text-gray-600 mb-1">
                        <span>Experiência</span>
                        <span>{combatState.hero.experience}/{combatState.hero.nextLevelExperience}</span>
                      </div>
                      <ProgressBar 
                        value={combatState.hero.experience} 
                        max={combatState.hero.nextLevelExperience}
                        className="h-2"
                      />
                    </div>
                    
                    <div className="grid grid-cols-2 gap-4 text-sm">
                      <div className="flex items-center space-x-2">
                        <Sword className="h-4 w-4 text-red-600" />
                        <span>Ataque: {combatState.hero.attack}</span>
                      </div>
                      <div className="flex items-center space-x-2">
                        <Shield className="h-4 w-4 text-blue-600" />
                        <span>Defesa: {combatState.hero.defense}</span>
                      </div>
                    </div>
                  </div>
                </Card>

                {/* Monstro */}
                <Card className="border-red-200 bg-red-50">
                  <div className="flex items-center space-x-3 mb-4">
                    <Skull className="h-8 w-8 text-red-600" />
                    <div>
                      <h3 className="text-xl font-bold text-gray-900">{combatState.monster.name}</h3>
                      <p className="text-sm text-gray-600">{combatState.monster.type} • {combatState.monster.rank}</p>
                    </div>
                  </div>
                  
                  <div className="space-y-3">
                    <div>
                      <div className="flex justify-between text-sm text-gray-600 mb-1">
                        <span>Vida</span>
                        <span>{combatState.monster.health}/{combatState.monster.maxHealth}</span>
                      </div>
                      <ProgressBar 
                        value={combatState.monster.health} 
                        max={combatState.monster.maxHealth}
                        className="h-3"
                      />
                    </div>
                    
                    <div className="text-sm text-gray-600">
                      <p>Habitat: {combatState.monster.habitat}</p>
                    </div>
                    
                    <div className="grid grid-cols-2 gap-4 text-sm">
                      <div className="flex items-center space-x-2">
                        <Sword className="h-4 w-4 text-red-600" />
                        <span>Ataque: {combatState.monster.attack}</span>
                      </div>
                      <div className="flex items-center space-x-2">
                        <Shield className="h-4 w-4 text-blue-600" />
                        <span>Defesa: {combatState.monster.defense}</span>
                      </div>
                    </div>
                    
                    <div className="flex items-center space-x-2 text-sm">
                      <Target className="h-4 w-4 text-green-600" />
                      <span>XP: {combatState.monster.experienceReward}</span>
                    </div>
                  </div>
                </Card>
              </div>
            </SlideIn>
          )}

          {/* Ações */}
          <SlideIn direction="up" delay={200}>
            <Card title="Ações de Combate">
              <div className="flex flex-wrap gap-4 justify-center">
                <Button
                  onClick={attack}
                  disabled={loading || combatState?.combat.combatEnded}
                  loading={loading}
                  size="lg"
                  className="flex-1 min-w-[150px]"
                >
                  <Sword className="h-5 w-5 mr-2" />
                  {loading ? 'Atacando...' : 'Atacar'}
                </Button>
                
                <Button
                  onClick={tryEscape}
                  disabled={loading || combatState?.combat.combatEnded}
                  loading={loading}
                  variant="secondary"
                  size="lg"
                  className="flex-1 min-w-[150px]"
                >
                  <RotateCcw className="h-5 w-5 mr-2" />
                  {loading ? 'Fugindo...' : 'Fugir'}
                </Button>
              </div>
            </Card>
          </SlideIn>

          {/* Log de Combate */}
          <SlideIn direction="up" delay={300}>
            <Card title="Log de Combate">
              <div className="max-h-64 overflow-y-auto space-y-2">
                {log.map((entry, index) => (
                  <div 
                    key={index} 
                    className={`p-3 rounded-lg text-sm ${
                      index === 0 
                        ? 'bg-blue-50 border border-blue-200' 
                        : 'bg-gray-50'
                    }`}
                  >
                    {entry}
                  </div>
                ))}
                {log.length === 0 && (
                  <div className="text-center py-8 text-gray-500">
                    <Target className="h-8 w-8 mx-auto mb-2" />
                    <p>Nenhuma ação registrada</p>
                  </div>
                )}
              </div>
            </Card>
          </SlideIn>

          {/* Informações do último turno */}
          {combatState && (
            <SlideIn direction="up" delay={400}>
              <Card title="Último Turno">
                <div className="space-y-3">
                  <div className="text-sm text-gray-700">
                    <strong>Ação:</strong> {combatState.combat.actionDescription}
                  </div>
                  
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    {combatState.combat.damageToMonster > 0 && (
                      <div className="flex items-center space-x-2 text-red-600">
                        <Sword className="h-4 w-4" />
                        <span>Dano ao monstro: {combatState.combat.damageToMonster}</span>
                      </div>
                    )}
                    
                    {combatState.combat.damageToHero > 0 && (
                      <div className="flex items-center space-x-2 text-orange-600">
                        <Heart className="h-4 w-4" />
                        <span>Dano recebido: {combatState.combat.damageToHero}</span>
                      </div>
                    )}
                    
                    {combatState.combat.experienceGained > 0 && (
                      <div className="flex items-center space-x-2 text-green-600">
                        <Target className="h-4 w-4" />
                        <span>+{combatState.combat.experienceGained} XP</span>
                      </div>
                    )}
                  </div>
                  
                  <div className="flex flex-wrap gap-2">
                    {combatState.combat.isCritical && (
                      <span className="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-red-100 text-red-800">
                        <Zap className="h-3 w-3 mr-1" />
                        CRÍTICO!
                      </span>
                    )}
                    
                    {combatState.combat.isFumble && (
                      <span className="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-gray-100 text-gray-800">
                        <XCircle className="h-3 w-3 mr-1" />
                        FALHA!
                      </span>
                    )}
                  </div>
                  
                  {combatState.combat.appliedEffects.length > 0 && (
                    <div className="text-sm text-gray-700">
                      <strong>Efeitos aplicados:</strong> {combatState.combat.appliedEffects.join(', ')}
                    </div>
                  )}
                </div>
              </Card>
            </SlideIn>
          )}
        </div>
      )}
    </div>
  );
}


