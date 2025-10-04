import axios from 'axios';
import { cacheService } from './cache';

const API_BASE_URL = 'http://localhost:5000/api';

export interface ShopItem {
  id: number;
  name: string;
  description: string;
  type: string;
  rarity: string;
  rarityColor: string;
  rarityDescription: string;
  level: number;
  value: number;
  shopPrice: number;
  requiredLevel?: number;
  attackBonus?: number;
  defenseBonus?: number;
  healthBonus?: number;
  moraleBonus?: number;
  isConsumable: boolean;
  stackSize: number;
  statusEffects: string[];
  statusEffectChance?: number;
  statusEffectDuration?: number;
}

export interface ShopType {
  type: string;
  description: string;
}

export interface Rarity {
  rarity: string;
  description: string;
  color: string;
}

class ShopService {
  async getShopItems(shopType: string = 'general'): Promise<{ shopType: string; description: string; items: ShopItem[] }> {
    return await cacheService.cached(
      `shop:items:${shopType}`,
      async () => {
        const response = await axios.get(`${API_BASE_URL}/shop?shopType=${shopType}`);
        return response.data;
      },
      10 * 60 * 1000 // 10 minutos
    );
  }

  async getShopItemsByType(itemType: string, shopType: string = 'general'): Promise<{ itemType: string; shopType: string; description: string; items: ShopItem[] }> {
    const response = await axios.get(`${API_BASE_URL}/shop/by-type/${itemType}?shopType=${shopType}`);
    return response.data;
  }

  async getShopItemsByLevel(characterLevel: number, shopType: string = 'general'): Promise<{ characterLevel: number; shopType: string; description: string; items: ShopItem[] }> {
    const response = await axios.get(`${API_BASE_URL}/shop/by-level/${characterLevel}?shopType=${shopType}`);
    return response.data;
  }

  async buyItem(heroId: number, itemId: number, quantity: number = 1): Promise<{ inventoryItem: any }> {
    const response = await axios.post(`${API_BASE_URL}/shop/buy`, {
      heroId,
      itemId,
      quantity
    });
    return response.data;
  }

  async sellItem(heroId: number, inventoryItemId: number, quantity: number = 1): Promise<void> {
    await axios.post(`${API_BASE_URL}/shop/sell`, {
      heroId,
      inventoryItemId,
      quantity
    });
  }

  async getItemSellPrice(itemId: number): Promise<{ itemId: number; sellPrice: number }> {
    const response = await axios.get(`${API_BASE_URL}/shop/sell-price/${itemId}`);
    return response.data;
  }

  async getShopTypes(): Promise<{ shopTypes: ShopType[] }> {
    const response = await axios.get(`${API_BASE_URL}/shop/types`);
    return response.data;
  }

  async getRarities(): Promise<{ rarities: Rarity[] }> {
    const response = await axios.get(`${API_BASE_URL}/shop/rarities`);
    return response.data;
  }
}

export const shopService = new ShopService();
