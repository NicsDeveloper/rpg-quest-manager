import { api } from './api';

export interface Item {
  id: number;
  name: string;
  description: string;
  type: string;
  rarity: string;
  level: number;
  value: number;
  stackSize: number;
  isConsumable: boolean;
  isTradeable: boolean;
  isSellable: boolean;
  attackBonus?: number;
  defenseBonus?: number;
  healthBonus?: number;
  moraleBonus?: number;
  statusEffects: string[];
  statusEffectChance?: number;
  statusEffectDuration?: number;
  requiredLevel?: number;
  droppedBy: string[];
  dropChance: number;
  foundIn: string[];
  availableInShop: boolean;
  shopPrice: number;
  shopTypes: string[];
}

export interface InventoryItem {
  id: number;
  itemId: number;
  name: string;
  description: string;
  type: string;
  rarity: string;
  level: number;
  quantity: number;
  isEquipped: boolean;
  equippedSlot?: string;
  acquiredAt: string;
  attackBonus?: number;
  defenseBonus?: number;
  healthBonus?: number;
  moraleBonus?: number;
  isConsumable: boolean;
  stackSize: number;
}

export interface CharacterEquipment {
  characterId: number;
  equipment: {
    weaponId?: number;
    shieldId?: number;
    helmetId?: number;
    armorId?: number;
    glovesId?: number;
    bootsId?: number;
    ringId?: number;
    amuletId?: number;
  };
}

export type EquipmentSlot = 'Weapon' | 'Shield' | 'Helmet' | 'Armor' | 'Gloves' | 'Boots' | 'Ring' | 'Amulet';

class InventoryService {
  async getInventory(characterId: number): Promise<InventoryItem[]> {
    const response = await api.get(`/inventory/${characterId}`);
    return response.data.items;
  }

  async getEquipment(characterId: number): Promise<CharacterEquipment> {
    const response = await api.get(`/inventory/equipment/${characterId}`);
    return response.data;
  }

  async equipItem(heroId: number, inventoryItemId: number, slot: EquipmentSlot): Promise<void> {
    const response = await api.post('/inventory/equip', {
      heroId,
      inventoryItemId,
      slot
    });
    return response.data;
  }

  async unequipItem(heroId: number, slot: EquipmentSlot): Promise<void> {
    await api.post('/inventory/unequip', {
      heroId,
      slot
    });
  }

  async useItem(heroId: number, inventoryItemId: number): Promise<void> {
    await api.post('/inventory/use', {
      heroId,
      inventoryItemId
    });
  }

  async addItem(heroId: number, itemId: number, quantity: number = 1): Promise<InventoryItem> {
    const response = await api.post('/inventory/add', {
      heroId,
      itemId,
      quantity
    });
    return response.data.inventoryItem;
  }

  async removeItem(heroId: number, itemId: number, quantity: number = 1): Promise<void> {
    await api.post('/inventory/remove', {
      heroId,
      itemId,
      quantity
    });
  }
}

export const inventoryService = new InventoryService();
