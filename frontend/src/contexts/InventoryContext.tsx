import React, { createContext, useContext, useState, useEffect, useCallback, useRef } from 'react';
import { inventoryService, type InventoryItem, type CharacterEquipment } from '../services/inventory';
import { useCharacter } from './CharacterContext';

interface InventoryContextType {
  inventory: InventoryItem[];
  equipment: CharacterEquipment | null;
  isLoading: boolean;
  error: string | null;
  refreshInventory: () => Promise<void>;
  addItemToInventory: (item: InventoryItem) => void;
  removeItemFromInventory: (itemId: number) => void;
  updateItemQuantity: (itemId: number, newQuantity: number) => void;
  equipItem: (item: InventoryItem, slot: string) => void;
  unequipItem: (slot: string) => void;
}

const InventoryContext = createContext<InventoryContextType | undefined>(undefined);

export function InventoryProvider({ children }: { children: React.ReactNode }) {
  const [inventory, setInventory] = useState<InventoryItem[]>([]);
  const [equipment, setEquipment] = useState<CharacterEquipment | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const { character } = useCharacter();
  const loadingRef = useRef(false);

  const refreshInventory = useCallback(async () => {
    if (!character || loadingRef.current) return;
    
    try {
      loadingRef.current = true;
      setIsLoading(true);
      setError(null);
      
      const [inventoryData, equipmentData] = await Promise.all([
        inventoryService.getInventory(character.id),
        inventoryService.getEquipment(character.id)
      ]);
      
      setInventory(inventoryData);
      setEquipment(equipmentData);
    } catch (err: any) {
      setError(err.message || 'Erro ao carregar inventário');
    } finally {
      setIsLoading(false);
      loadingRef.current = false;
    }
  }, [character]);

  const addItemToInventory = (item: InventoryItem) => {
    setInventory(prev => {
      const existingItem = prev.find(i => i.itemId === item.itemId);
      if (existingItem) {
        return prev.map(i => 
          i.itemId === item.itemId 
            ? { ...i, quantity: i.quantity + item.quantity }
            : i
        );
      }
      return [...prev, item];
    });
  };

  const removeItemFromInventory = (itemId: number) => {
    setInventory(prev => prev.filter(item => item.id !== itemId));
  };

  const updateItemQuantity = (itemId: number, newQuantity: number) => {
    setInventory(prev => 
      prev.map(item => 
        item.id === itemId 
          ? { ...item, quantity: newQuantity }
          : item
      ).filter(item => item.quantity > 0)
    );
  };

  const equipItem = (item: InventoryItem, slot: string) => {
    // Atualizar inventário
    setInventory(prev => 
      prev.map(i => 
        i.id === item.id 
          ? { ...i, isEquipped: true, equippedSlot: slot }
          : i.equippedSlot === slot 
            ? { ...i, isEquipped: false, equippedSlot: undefined }
            : i
      )
    );

    // Atualizar equipamento
    setEquipment(prev => {
      if (!prev) return null;
      return {
        ...prev,
        equipment: {
          ...prev.equipment,
          [slot.toLowerCase() + 'Id']: item.id
        }
      };
    });
  };

  const unequipItem = (slot: string) => {
    // Atualizar inventário
    setInventory(prev => 
      prev.map(item => 
        item.equippedSlot === slot 
          ? { ...item, isEquipped: false, equippedSlot: undefined }
          : item
      )
    );

    // Atualizar equipamento
    setEquipment(prev => {
      if (!prev) return null;
      return {
        ...prev,
        equipment: {
          ...prev.equipment,
          [slot.toLowerCase() + 'Id']: undefined
        }
      };
    });
  };

  useEffect(() => {
    if (character) {
      refreshInventory();
    }
  }, [character, refreshInventory]);

  const value: InventoryContextType = {
    inventory,
    equipment,
    isLoading,
    error,
    refreshInventory,
    addItemToInventory,
    removeItemFromInventory,
    updateItemQuantity,
    equipItem,
    unequipItem
  };

  return (
    <InventoryContext.Provider value={value}>
      {children}
    </InventoryContext.Provider>
  );
}

export function useInventory() {
  const context = useContext(InventoryContext);
  if (context === undefined) {
    throw new Error('useInventory must be used within an InventoryProvider');
  }
  return context;
}
