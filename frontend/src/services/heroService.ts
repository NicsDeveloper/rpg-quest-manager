import api from './api';

export interface Hero {
  id: number;
  name: string;
  class: string;
  level: number;
  experience: number;
  health: number;
  maxHealth: number;
  mana: number;
  maxMana: number;
  strength: number;
  intelligence: number;
  agility: number;
  gold: number;
  createdAt: string;
  items?: HeroItem[];
}

export interface HeroItem {
  id: number;
  itemId: number;
  quantity: number;
  isEquipped: boolean;
  item: {
    id: number;
    name: string;
    description: string;
    type: string;
    rarity: string;
  };
}

export interface CreateHeroRequest {
  name: string;
  class: string;
  strength: number;
  intelligence: number;
  agility: number;
}

export interface UpdateHeroRequest {
  name?: string;
  class?: string;
  health?: number;
  mana?: number;
  strength?: number;
  intelligence?: number;
  agility?: number;
  gold?: number;
}

export const heroService = {
  getAll: async (): Promise<Hero[]> => {
    const response = await api.get<Hero[]>('/heroes');
    return response.data;
  },

  getById: async (id: number): Promise<Hero> => {
    const response = await api.get<Hero>(`/heroes/${id}`);
    return response.data;
  },

  create: async (data: CreateHeroRequest): Promise<Hero> => {
    const response = await api.post<Hero>('/heroes', data);
    return response.data;
  },

  update: async (id: number, data: UpdateHeroRequest): Promise<Hero> => {
    const response = await api.put<Hero>(`/heroes/${id}`, data);
    return response.data;
  },

  delete: async (id: number): Promise<void> => {
    await api.delete(`/heroes/${id}`);
  },

  getInventory: async (heroId: number): Promise<HeroItem[]> => {
    const response = await api.get<HeroItem[]>(`/heroes/${heroId}/inventory`);
    return response.data;
  },
};

