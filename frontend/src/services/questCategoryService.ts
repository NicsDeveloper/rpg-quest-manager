import api from './api';

export interface QuestCategory {
  id: number;
  name: string;
  description: string;
  difficulty: string;
  type: string;
  environment: string;
  minLevel: number;
  maxLevel: number;
  icon: string;
  color: string;
  isActive: boolean;
  createdAt: string;
}

export interface Quest {
  id: number;
  name: string;
  description: string;
  type: string;
  difficulty: string;
  requiredClass: string;
  requiredLevel: number;
  experienceReward: number;
  goldReward: number;
  isRepeatable: boolean;
  createdAt: string;
  categoryId?: number;
  category?: QuestCategory;
  environment: string;
  environmentalCondition?: string;
  environmentalIntensity: number;
  immuneClasses: string[];
  immuneEnemyTypes: string[];
  specialRewards: string[];
  bossId: number;
  isBossQuest: boolean;
  estimatedDuration: number;
  storyOrder: number;
  prerequisites: number[];
  isUnlocked: boolean;
}

export interface Monster {
  id: number;
  name: string;
  description: string;
  type: string;
  size: string;
  level: number;
  power: number;
  health: number;
  armor: number;
  speed: number;
  strength: number;
  dexterity: number;
  constitution: number;
  intelligence: number;
  wisdom: number;
  charisma: number;
  attackDice: string;
  attackBonus: number;
  damageBonus: number;
  criticalChance: number;
  resistances: string[];
  immunities: string[];
  vulnerabilities: string[];
  statusEffects: string[];
  statusImmunities: string[];
  specialAbilities: string[];
  specialAbilityCooldown: number;
  preferredEnvironment: string;
  environmentalBonuses: string[];
  experienceReward: number;
  goldReward: number;
  lootTable: string[];
  baseMorale: number;
  moraleRange: number;
  isBoss: boolean;
  bossPhase: number;
  bossHealthThreshold: number;
  bossPhases: string[];
  isElite: boolean;
  minGroupSize: number;
  maxGroupSize: number;
  spawnChance: number;
  icon: string;
  color: string;
  model: string;
  lore: string;
  origin: string;
  weakness: string;
  createdAt: string;
  isActive: boolean;
}

export const questCategoryService = {
  async getCategories(): Promise<QuestCategory[]> {
    const response = await api.get('/questcategories');
    return response.data;
  },

  async getQuestsByCategory(categoryId: number): Promise<Quest[]> {
    const response = await api.get(`/questcategories/${categoryId}/quests`);
    return response.data;
  },

  async getAllQuests(): Promise<Quest[]> {
    const response = await api.get('/quests');
    return response.data;
  },

  async getMonsters(): Promise<Monster[]> {
    const response = await api.get('/monsters');
    return response.data;
  },

  async getMonsterById(id: number): Promise<Monster> {
    const response = await api.get(`/monsters/${id}`);
    return response.data;
  },

  getDifficultyColor(difficulty: string): string {
    switch (difficulty) {
      case 'Easy': return '#10b981';
      case 'Medium': return '#f59e0b';
      case 'Hard': return '#ef4444';
      case 'Epic': return '#8b5cf6';
      case 'Legendary': return '#f97316';
      default: return '#6366f1';
    }
  },

  getDifficultyIcon(difficulty: string): string {
    switch (difficulty) {
      case 'Easy': return '🟢';
      case 'Medium': return '🟡';
      case 'Hard': return '🔴';
      case 'Epic': return '🟣';
      case 'Legendary': return '🟠';
      default: return '🔵';
    }
  },

  getTypeIcon(type: string): string {
    switch (type) {
      case 'Main': return '📖';
      case 'Side': return '📋';
      case 'Boss': return '👹';
      case 'Event': return '🎉';
      case 'Dungeon': return '🏰';
      case 'Raid': return '⚔️';
      case 'PvP': return '⚡';
      case 'Daily': return '📅';
      case 'Weekly': return '📆';
      case 'Seasonal': return '🎄';
      default: return '❓';
    }
  },

  getEnvironmentIcon(environment: string): string {
    switch (environment) {
      case 'Forest': return '🌲';
      case 'Desert': return '🏜️';
      case 'Mountain': return '⛰️';
      case 'Cave': return '🕳️';
      case 'Ruins': return '🏛️';
      case 'Swamp': return '🐸';
      case 'Tundra': return '🧊';
      case 'Volcano': return '🌋';
      case 'Sky': return '☁️';
      case 'Underwater': return '🌊';
      case 'Void': return '🌌';
      case 'Castle': return '🏰';
      case 'Temple': return '⛩️';
      case 'Crypt': return '⚰️';
      case 'Laboratory': return '🧪';
      default: return '🌍';
    }
  }
};
