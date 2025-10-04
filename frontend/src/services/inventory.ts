import axios from 'axios';

const API_BASE_URL = 'http://localhost:5000/api';

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
    const response = await axios.get(`${API_BASE_URL}/inventory/${characterId}`);
    return response.data.items;
  }

  async getEquipment(characterId: number): Promise<CharacterEquipment> {
    const response = await axios.get(`${API_BASE_URL}/inventory/equipment/${characterId}`);
    return response.data;
  }

  async equipItem(characterId: number, inventoryItemId: number, slot: EquipmentSlot): Promise<void> {
    await axios.post(`${API_BASE_URL}/inventory/equip`, {
      characterId,
      inventoryItemId,
      slot
    });
  }

  async unequipItem(characterId: number, slot: EquipmentSlot): Promise<void> {
    await axios.post(`${API_BASE_URL}/inventory/unequip`, {
      characterId,
      slot
    });
  }

  async useItem(characterId: number, inventoryItemId: number): Promise<void> {
    await axios.post(`${API_BASE_URL}/inventory/use`, {
      characterId,
      inventoryItemId
    });
  }

  async addItem(characterId: number, itemId: number, quantity: number = 1): Promise<InventoryItem> {
    const response = await axios.post(`${API_BASE_URL}/inventory/add`, {
      characterId,
      itemId,
      quantity
    });
    return response.data.inventoryItem;
  }

  async removeItem(characterId: number, itemId: number, quantity: number = 1): Promise<void> {
    await axios.post(`${API_BASE_URL}/inventory/remove`, {
      characterId,
      itemId,
      quantity
    });
  }
}

export const inventoryService = new InventoryService();
