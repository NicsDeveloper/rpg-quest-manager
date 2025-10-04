import { api } from './api';

export interface Quest {
  id: number;
  title: string;
  description: string;
  environment: number;
  experienceReward: number;
  requiredLevel?: number;
  difficulty?: string;
  category?: string;
  status?: string;
  progress?: number;
  goldReward?: number;
}

export const questsService = {
  async getAllQuests(): Promise<Quest[]> {
    const { data } = await api.get('/api/quests');
    return data;
  },

  async getRecommendedQuests(level: number): Promise<Quest[]> {
    const { data } = await api.get(`/api/quests/recommended/${level}`);
    return data;
  },

  async getAvailableQuests(characterId: number): Promise<Quest[]> {
    const { data } = await api.get(`/api/quests/available/${characterId}`);
    return data;
  },

  async getCompletedQuests(characterId: number): Promise<Quest[]> {
    const { data } = await api.get(`/api/quests/completed/${characterId}`);
    return data;
  },

  async startQuest(characterId: number, questId: number): Promise<void> {
    await api.post('/api/quests/start', { characterId, questId });
  },

  async completeQuest(characterId: number, questId: number): Promise<void> {
    await api.post('/api/quests/complete', { characterId, questId });
  }
};

// Backward compatibility
export async function getAllQuests() {
  return questsService.getAllQuests();
}

export async function getRecommendedQuests(level: number) {
  return questsService.getRecommendedQuests(level);
}


