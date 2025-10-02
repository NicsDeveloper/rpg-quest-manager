import api from './api';

export interface Quest {
  id: number;
  name: string;
  description: string;
  type: string;
  difficulty: string;
  requiredLevel: number;
  goldReward: number;
  experienceReward: number;
  createdAt: string;
  rewards?: Reward[];
}

export interface Reward {
  id: number;
  questId: number;
  gold: number;
  experience: number;
  itemId?: number;
  itemQuantity: number;
  item?: {
    id: number;
    name: string;
    description: string;
    type: string;
    rarity: string;
  };
}

export interface CreateQuestRequest {
  name: string;
  description: string;
  type: string;
  difficulty: string;
  requiredLevel: number;
  goldReward: number;
  experienceReward: number;
}

export interface UpdateQuestRequest {
  name?: string;
  description?: string;
  type?: string;
  difficulty?: string;
  requiredLevel?: number;
  goldReward?: number;
  experienceReward?: number;
}

export interface CompleteQuestRequest {
  heroId: number;
}

export const questService = {
  getAll: async (): Promise<Quest[]> => {
    const response = await api.get<Quest[]>('/quests');
    return response.data;
  },

  getById: async (id: number): Promise<Quest> => {
    const response = await api.get<Quest>(`/quests/${id}`);
    return response.data;
  },

  create: async (data: CreateQuestRequest): Promise<Quest> => {
    const response = await api.post<Quest>('/quests', data);
    return response.data;
  },

  update: async (id: number, data: UpdateQuestRequest): Promise<Quest> => {
    const response = await api.put<Quest>(`/quests/${id}`, data);
    return response.data;
  },

  delete: async (id: number): Promise<void> => {
    await api.delete(`/quests/${id}`);
  },

  complete: async (questId: number, heroId: number): Promise<void> => {
    const data: CompleteQuestRequest = { heroId };
    await api.post(`/quests/${questId}/complete`, data);
  },
};

