import { api } from './api';
import { cacheService } from './cache';

export interface Character {
  id: number;
  userId: number;
  name: string;
  class: string;
  level: number;
  experience: number;
  strength: number;
  intelligence: number;
  dexterity: number;
  health: number;
  maxHealth: number;
  isInActiveParty: boolean;
  partySlot: number | null;
  gold: number;
  createdAt: string;
}

export interface CharacterStats {
  strength: number;
  intelligence: number;
  dexterity: number;
  health: number;
  maxHealth: number;
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
    const character = await this.getCharacter(characterId);
    return {
      strength: character.strength,
      intelligence: character.intelligence,
      dexterity: character.dexterity,
      health: character.health,
      maxHealth: character.maxHealth
    };
  }

  async updateCharacter(characterId: number, updates: Partial<Character>): Promise<Character> {
    const response = await api.put(`/characters/${characterId}`, updates);
    return response.data;
  }
}

export const characterService = new CharacterService();
