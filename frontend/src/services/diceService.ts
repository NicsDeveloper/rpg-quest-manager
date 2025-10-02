import api from './api';

export interface DiceInventory {
  heroId: number;
  d6Count: number;
  d8Count: number;
  d12Count: number;
  d20Count: number;
}

export interface DicePrices {
  d6: number;
  d8: number;
  d12: number;
  d20: number;
}

export interface PurchaseDiceRequest {
  heroId: number;
  diceType: string; // "D6", "D8", "D12", "D20"
  quantity: number;
}

export interface PurchaseDiceResult {
  success: boolean;
  message: string;
  remainingGold: number;
  updatedInventory: DiceInventory;
}

export const diceService = {
  getInventory: async (heroId: number): Promise<DiceInventory> => {
    const response = await api.get<DiceInventory>(`/dice/inventory/${heroId}`);
    return response.data;
  },

  getPrices: async (): Promise<DicePrices> => {
    const response = await api.get<DicePrices>('/dice/prices');
    return response.data;
  },

  purchaseDice: async (data: PurchaseDiceRequest): Promise<PurchaseDiceResult> => {
    const response = await api.post<PurchaseDiceResult>('/dice/purchase', data);
    return response.data;
  },
};

