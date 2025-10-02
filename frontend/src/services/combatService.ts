import api from './api';

// Enums
export enum CombatStatus {
  Preparing = 0,
  InProgress = 1,
  Victory = 2,
  Defeat = 3,
  Cancelled = 4
}

export enum DiceType {
  D6 = 6,
  D10 = 10,
  D12 = 12,
  D20 = 20
}

// Interfaces
export interface StartCombatRequest {
  questId: number;
  heroIds: number[];
}

export interface CombatSession {
  id: number;
  userId: number;
  heroIds: number[];
  questId: number;
  questName: string;
  currentEnemyId: number;
  currentEnemyName: string;
  status: CombatStatus;
  isHeroTurn: boolean;
  currentEnemyHealth: number;
  maxEnemyHealth: number;
  heroHealths: { [heroId: number]: number };
  maxHeroHealths: { [heroId: number]: number };
  createdAt: string;
  startedAt?: string;
  completedAt?: string;
}

export interface HeroCombatInfo {
  id: number;
  name: string;
  class: string;
  level: number;
  experience: number;
  strength: number;
  intelligence: number;
  dexterity: number;
  health: number;
  maxHealth: number;
  totalAttack: number;
  totalDefense: number;
  totalMagic: number;
}

export interface EnemyCombatInfo {
  id: number;
  name: string;
  type: string;
  power: number;
  health: number;
  requiredDiceType: DiceType;
  minimumRoll: number;
  combatType: number;
  isBoss: boolean;
}

export interface StatusEffect {
  id: number;
  combatSessionId: number;
  heroId?: number;
  enemyId?: number;
  type: string;
  duration: number;
  intensity: number;
  description: string;
  isActive: boolean;
  expiresAt: string;
}

export interface CombatLog {
  id: number;
  action: string;
  enemyName: string;
  diceUsed?: DiceType;
  diceResult?: number;
  requiredRoll?: number;
  success?: boolean;
  damageDealt?: number;
  enemyHealthAfter?: number;
  details: string;
  timestamp: string;
}

export interface CombatDetail {
  id: number;
  userId: number;
  heroIds: number[];
  heroes: HeroCombatInfo[];
  questId: number;
  questName: string;
  currentEnemy: EnemyCombatInfo;
  status: CombatStatus;
  isHeroTurn: boolean;
  currentEnemyHealth: number;
  maxEnemyHealth: number;
  heroHealths: { [heroId: number]: number };
  maxHeroHealths: { [heroId: number]: number };
  combatLogs: CombatLog[];
  createdAt: string;
  startedAt?: string;
  completedAt?: string;
  // Sistema de Combos
  consecutiveSuccesses: number;
  consecutiveFailures: number;
  comboMultiplier: number;
  lastAction: string;
  
  // Sistema de Condições Ambientais e Morale
  environmentalCondition?: {
    type: string;
    intensity: number;
    description: string;
    icon: string;
  };
  heroMoraleStates: {
    id: number;
    heroId?: number;
    enemyId?: number;
    level: string;
    moralePoints: number;
    icon: string;
    description: string;
  }[];
  enemyMoraleState?: {
    id: number;
    heroId?: number;
    enemyId?: number;
    level: string;
    moralePoints: number;
    icon: string;
    description: string;
  };
  
  // Sistema de Status Effects
  heroStatusEffects?: StatusEffect[];
  enemyStatusEffects?: StatusEffect[];
}

export interface RollDiceRequest {
  combatSessionId: number;
  diceType: DiceType;
}

export interface RollDiceResult {
  roll: number;
  requiredRoll: number;
  success: boolean;
  damageDealt: number;
  enemyHealthAfter: number;
  message: string;
  updatedCombatSession: CombatDetail;
}

export interface EnemyAttackRequest {
  combatSessionId: number;
}

export interface EnemyAttackResult {
  enemyRoll: number;
  enemyPower: number;
  totalDamage: number;
  heroDefense: number;
  finalDamage: number;
  message: string;
  allHeroesDead: boolean;
  updatedCombatSession: CombatDetail;
}

export const combatService = {
  startCombat: async (questId: number, heroIds: number[]): Promise<CombatDetail> => {
    const response = await api.post<CombatDetail>('/combat/start', { questId, heroIds });
    return response.data;
  },

  getActiveCombat: async (userId: number): Promise<CombatDetail | null> => {
    try {
      const response = await api.get<CombatDetail>(`/combat/active/${userId}`);
      return response.data;
    } catch (error: any) {
      if (error.response?.status === 404) {
        return null;
      }
      throw error;
    }
  },

  rollDice: async (request: RollDiceRequest): Promise<RollDiceResult> => {
    const response = await api.post<RollDiceResult>('/combat/roll-dice', request);
    return response.data;
  },

  enemyAttack: async (request: EnemyAttackRequest): Promise<EnemyAttackResult> => {
    const response = await api.post<EnemyAttackResult>('/combat/enemy-attack', request);
    return response.data;
  },

  completeCombat: async (combatSessionId: number): Promise<CombatDetail> => {
    const response = await api.post<CombatDetail>(`/combat/${combatSessionId}/complete`);
    return response.data;
  },

  cancelCombat: async (combatSessionId: number): Promise<void> => {
    await api.post(`/combat/${combatSessionId}/cancel`);
  },

  useSpecialAbility: async (request: UseSpecialAbilityRequest): Promise<UseSpecialAbilityResult> => {
    const response = await api.post<UseSpecialAbilityResult>('/combat/use-special-ability', request);
    return response.data;
  },
};

export interface UseSpecialAbilityRequest {
  combatSessionId: number;
  heroId: number;
}

export interface UseSpecialAbilityResult {
  success: boolean;
  message: string;
  damageDealt: number;
  healingDone: number;
  cooldownRemaining: number;
  updatedCombatSession: CombatDetail;
}