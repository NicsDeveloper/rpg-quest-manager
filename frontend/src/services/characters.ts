import { api } from './api';
import { cacheService } from './cache';

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
    console.log(`[CharacterService] Getting character with ID: ${characterId}`);
    return await cacheService.cached(
      `character:${characterId}`,
      async () => {
        console.log(`[CharacterService] Fetching character ${characterId} from API`);
        const response = await api.get(`/characters/${characterId}`);
        return response.data;
      },
      2 * 60 * 1000 // 2 minutos
    );
  }

  async getCharacterStats(characterId: number): Promise<CharacterStats> {
    const response = await api.get(`/inventory/bonuses/${characterId}`);
    return response.data.bonuses;
  }

  async updateCharacter(characterId: number, updates: Partial<Character>): Promise<Character> {
    const response = await api.put(`/characters/${characterId}`, updates);
    return response.data;
  }
}

export const characterService = new CharacterService();
