import { api } from './api';

export interface Quest {
  id: number;
  title: string;
  description: string;
  environment: number;
  experienceReward: number;
  requiredLevel?: number;
  difficulty?: number;
  category?: number;
  status?: number;
  progress?: number;
  goldReward?: number;
  introductionText?: string;
  completionText?: string;
  targetMonsterName?: string;
  targetMonsterType?: number;
  prerequisites?: any[];
  rewards?: string[];
  isRepeatable?: boolean;
  estimatedDuration?: number;
}

export const questsService = {
  async getAllQuests(): Promise<Quest[]> {
    const { data } = await api.get('/quests');
    return data;
  },

  async getRecommendedQuests(level: number): Promise<Quest[]> {
    const { data } = await api.get(`/quests/recommended/${level}`);
    return data;
  },

  async getAvailableQuests(characterId: number): Promise<Quest[]> {
    const { data } = await api.get(`/quests/available/${characterId}`);
    return data;
  },

  async getCompletedQuests(characterId: number): Promise<Quest[]> {
    const { data } = await api.get(`/quests/completed/${characterId}`);
    return data;
  },

  async startQuest(questId: number, heroId: number): Promise<void> {
    await api.post(`/quests/${questId}/start`, { heroId });
  },

  async completeQuest(questId: number): Promise<void> {
    await api.post(`/quests/${questId}/complete`);
  }
};

// Backward compatibility
export async function getAllQuests() {
  return questsService.getAllQuests();
}

export async function getRecommendedQuests(level: number) {
  return questsService.getRecommendedQuests(level);
}


