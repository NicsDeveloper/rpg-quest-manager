import api from './api';

export interface Quest {
  id: number;
  name: string;
  description: string;
  type: string;
  difficulty: string;
  requiredLevel: number;
  requiredClass: string;
  goldReward: number;
  experienceReward: number;
  isRepeatable: boolean;
  createdAt: string;
  rewards?: Reward[];
  isAccepted?: boolean;
  canAccept?: boolean;
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
  isRepeatable: boolean;
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

  getCatalog: async (): Promise<Quest[]> => {
    const response = await api.get<Quest[]>('/quests/catalog');
    return response.data;
  },

  getMyQuests: async (): Promise<Quest[]> => {
    const response = await api.get<Quest[]>('/quests/my-quests');
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

  accept: async (questId: number): Promise<void> => {
    await api.post(`/quests/${questId}/accept`);
  },

  complete: async (heroId: number): Promise<void> => {
    const data: CompleteQuestRequest = { heroId };
    await api.post(`/quests/complete`, data);
  },
};

