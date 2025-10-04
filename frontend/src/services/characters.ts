import axios from 'axios';
import { cacheService } from './cache';

const API_BASE_URL = 'http://localhost:5000/api';

export interface Character {
  id: number;
  userId: number;
  name: string;
  level: number;
  experience: number;
  nextLevelExperience: number;
  health: number;
  maxHealth: number;
  attack: number;
  defense: number;
  morale: number;
  gold: number;
  statusEffects: string[];
  createdAt: string;
  lastPlayedAt: string;
}

export interface CharacterStats {
  attack: number;
  defense: number;
  health: number;
  morale: number;
}

class CharacterService {
  async getCharacter(characterId: number): Promise<Character> {
    return await cacheService.cached(
      `character:${characterId}`,
      async () => {
        const response = await axios.get(`${API_BASE_URL}/characters/${characterId}`);
        return response.data;
      },
      2 * 60 * 1000 // 2 minutos
    );
  }

  async getCharacterStats(characterId: number): Promise<CharacterStats> {
    const response = await axios.get(`${API_BASE_URL}/inventory/bonuses/${characterId}`);
    return response.data.bonuses;
  }

  async updateCharacter(characterId: number, updates: Partial<Character>): Promise<Character> {
    const response = await axios.put(`${API_BASE_URL}/characters/${characterId}`, updates);
    return response.data;
  }
}

export const characterService = new CharacterService();
