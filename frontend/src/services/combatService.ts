import api from './api';

export interface CombatSession {
  id: number;
  heroId: number;
  questId: number;
  status: string;
  startedAt: string;
  message?: string;
}

export interface EnemyInfo {
  id: number;
  name: string;
  type: string;
  power: number;
  health: number;
  requiredDiceType: string;
  minimumRoll: number;
  combatType: string;
  isBoss: boolean;
  createdAt: string;
}

export interface CombatLog {
  id: number;
  action: string;
  enemyId?: number;
  diceUsed?: string;
  diceResult?: number;
  requiredRoll?: number;
  success?: boolean;
  details: string;
  timestamp: string;
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
  gold: number;
  createdAt: string;
  totalAttack: number;
  totalDefense: number;
  totalMagic: number;
}

export interface CombatBonus {
  heroId: number;
  heroName: string;
  attackBonus: number;
  defenseBonus: number;
  magicBonus: number;
  combatBonus: number;
  relevantStat: string;
}

export interface CombatSessionDetail {
  id: number;
  heroId: number;
  heroIds: number[];
  heroes: HeroCombatInfo[];
  questId: number;
  questName: string;
  status: string;
  startedAt: string;
  enemies?: EnemyInfo[]; // Legacy - mapeado de remainingEnemies
  remainingEnemies: EnemyInfo[];
  combatLogs: CombatLog[];
  requiredRoll: number;
  combatTypeDescription: string;
  heroBonuses: CombatBonus[];
}

export interface RollDiceRequest {
  combatSessionId: number;
  diceType: string; // "D6", "D10", "D12", "D20"
}

export interface RollDiceResult {
  roll: number;
  requiredRoll: number;
  success: boolean;
  message: string;
  updatedCombatSession: CombatSessionDetail;
}

export interface DroppedItem {
  id: number;
  name: string;
  description: string;
  rarity: string;
  type: string;
  bonusStrength: number;
  bonusIntelligence: number;
  bonusDexterity: number;
}

export interface CompleteCombatResult {
  status: string; // "Victory", "Fled", "Defeated"
  droppedItems: DroppedItem[];
  message: string;
}

export const combatService = {
  startCombat: async (_heroId: number, questId: number): Promise<CombatSession> => {
    // Busca a party ativa (usa todos os heróis da party, não apenas um)
    const partyResponse = await api.get('/profile/active-party');
    const activeParty = partyResponse.data;
    
    if (!activeParty || activeParty.length === 0) {
      throw new Error('Você precisa ter pelo menos um herói na party ativa para combater!');
    }
    
    const heroIds = activeParty.map((h: any) => h.id);
    const response = await api.post<CombatSession>('/combat/start', { heroIds, questId });
    return response.data;
  },

  getActiveCombat: async (heroId: number): Promise<CombatSessionDetail | null> => {
    try {
      const response = await api.get<CombatSessionDetail>(`/combat/active/${heroId}`);
      const data = response.data;
      // Mapeia remainingEnemies para enemies (compatibilidade)
      if (!data.enemies && data.remainingEnemies) {
        data.enemies = data.remainingEnemies;
      }
      return data;
    } catch (error: any) {
      if (error.response?.status === 404) {
        return null;
      }
      throw error;
    }
  },

  rollDice: async (data: RollDiceRequest): Promise<RollDiceResult> => {
    const response = await api.post<RollDiceResult>('/combat/roll-dice', data);
    return response.data;
  },

  completeCombat: async (combatSessionId: number): Promise<CompleteCombatResult> => {
    const response = await api.post<CompleteCombatResult>('/combat/complete', { combatSessionId });
    return response.data;
  },

  flee: async (combatSessionId: number): Promise<void> => {
    await api.post('/combat/flee', { combatSessionId });
  },
};

