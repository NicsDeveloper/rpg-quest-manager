import api from './api';

export interface HeroAttributes {
  heroId: number;
  heroName: string;
  class: string;
  level: number;
  strength: number;
  intelligence: number;
  dexterity: number;
  baseStrength: number;
  baseIntelligence: number;
  baseDexterity: number;
  bonusStrength: number;
  bonusIntelligence: number;
  bonusDexterity: number;
  unallocatedPoints: number;
  totalAttack: number;
  totalDefense: number;
  totalMagic: number;
}

export interface ClassInfo {
  name: string;
  description: string;
  baseStrength: number;
  baseIntelligence: number;
  baseDexterity: number;
  combatFocus: string;
  recommendedFor: string;
}

export interface AllocateAttributesRequest {
  strengthPoints: number;
  intelligencePoints: number;
  dexterityPoints: number;
}

export interface AttributeAllocationResult {
  success: boolean;
  message: string;
  heroId: number;
  newStrength: number;
  newIntelligence: number;
  newDexterity: number;
  remainingPoints: number;
  newTotalAttack: number;
  newTotalDefense: number;
  newTotalMagic: number;
}

export const attributesService = {
  getHeroAttributes: async (heroId: number): Promise<HeroAttributes> => {
    const response = await api.get<HeroAttributes>(`/attributes/hero/${heroId}`);
    return response.data;
  },

  allocateAttributes: async (
    heroId: number, 
    request: AllocateAttributesRequest
  ): Promise<AttributeAllocationResult> => {
    const response = await api.post<AttributeAllocationResult>(
      `/attributes/hero/${heroId}/allocate`, 
      request
    );
    return response.data;
  },

  getAvailableClasses: async (): Promise<ClassInfo[]> => {
    const response = await api.get<ClassInfo[]>('/attributes/classes');
    return response.data;
  }
};
