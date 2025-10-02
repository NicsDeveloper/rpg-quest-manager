import api from './api';

export interface Item {
  id: number;
  name: string;
  description: string;
  type: string;
  rarity: string;
  value: number;
  bonusHealth: number;
  bonusMana: number;
  bonusStrength: number;
  bonusIntelligence: number;
  bonusAgility: number;
  createdAt: string;
}

export interface CreateItemRequest {
  name: string;
  description: string;
  type: string;
  rarity: string;
  value: number;
  bonusHealth?: number;
  bonusMana?: number;
  bonusStrength?: number;
  bonusIntelligence?: number;
  bonusAgility?: number;
}

export interface ItemUsageResult {
  success: boolean;
  message: string;
  itemName: string;
  quantityUsed: number;
  xpGained: number;
  leveledUp: boolean;
  newLevel?: number;
}

export const itemService = {
  getAll: async (): Promise<Item[]> => {
    const response = await api.get<Item[]>('/items');
    return response.data;
  },

  getById: async (id: number): Promise<Item> => {
    const response = await api.get<Item>(`/items/${id}`);
    return response.data;
  },

  create: async (data: CreateItemRequest): Promise<Item> => {
    const response = await api.post<Item>('/items', data);
    return response.data;
  },

  delete: async (id: number): Promise<void> => {
    await api.delete(`/items/${id}`);
  },

  useItem: async (itemId: number, quantity: number = 1): Promise<ItemUsageResult> => {
    const response = await api.post<ItemUsageResult>(`/items/${itemId}/use?quantity=${quantity}`);
    return response.data;
  },
};

