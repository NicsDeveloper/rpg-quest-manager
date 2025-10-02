import api from './api';
import { Hero, HeroItem } from './heroService';

export interface MyHero extends Hero {
  heroItems?: HeroItem[];
  heroQuests?: any[];
}

export interface MyQuest {
  id: number;
  quest: {
    id: number;
    name: string;
    description: string;
    difficulty: string;
    goldReward: number;
    experienceReward: number;
    rewards: any[];
  };
  isCompleted: boolean;
  startedAt: string;
  completedAt?: string;
}

export interface MyStats {
  totalQuests: number;
  completedQuests: number;
  totalItems: number;
  uniqueItems: number;
  equippedItems: number;
  totalGold: number;
  currentLevel: number;
  totalExperience: number;
  experienceForNextLevel: number;
  playDays: number;
  powerRating: number;
}

export interface CreateHeroData {
  name: string;
  class: string;
  strength: number;
  intelligence: number;
  dexterity: number;
}

export const profileService = {
  getMyHero: async (): Promise<MyHero> => {
    const response = await api.get<MyHero>('/profile/my-hero');
    return response.data;
  },

  getMyQuests: async (): Promise<MyQuest[]> => {
    const response = await api.get<MyQuest[]>('/profile/my-quests');
    return response.data;
  },

  getMyStats: async (): Promise<MyStats> => {
    const response = await api.get<MyStats>('/profile/stats');
    return response.data;
  },

  createHero: async (data: CreateHeroData): Promise<MyHero> => {
    const response = await api.post<MyHero>('/profile/create-hero', data);
    return response.data;
  },

  getMyHeroes: async (): Promise<MyHero[]> => {
    const response = await api.get<MyHero[]>('/profile/my-heroes');
    return response.data;
  },

  getActiveParty: async (): Promise<MyHero[]> => {
    const response = await api.get<MyHero[]>('/profile/active-party');
    return response.data;
  },
};

