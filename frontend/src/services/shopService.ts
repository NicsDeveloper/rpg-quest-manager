import api from './api';

export interface ShopItem {
  id: number;
  name: string;
  description: string;
  type: string;
  rarity: string;
  value: number;
  bonusStrength: number;
  bonusIntelligence: number;
  bonusDexterity: number;
  quantity: number;
  isEquipped: boolean;
  isConsumable: boolean;
  percentageXpBonus?: number;
}

export interface ShopPurchaseResult {
  success: boolean;
  message: string;
  itemName: string;
  quantity: number;
  totalCost: number;
  remainingGold: number;
}

export const shopService = {
  getShopItems: async (): Promise<ShopItem[]> => {
    const response = await api.get<ShopItem[]>('/shop/items');
    return response.data;
  },

  buyItem: async (itemId: number, quantity: number = 1): Promise<ShopPurchaseResult> => {
    const response = await api.post<ShopPurchaseResult>(`/shop/buy/${itemId}?quantity=${quantity}`);
    return response.data;
  },

  getInventory: async (): Promise<ShopItem[]> => {
    const response = await api.get<ShopItem[]>('/shop/inventory');
    return response.data;
  }
};
