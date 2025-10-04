import React, { useState, useEffect } from 'react';
import { useCharacter } from '../contexts/CharacterContext';
import { inventoryService, type InventoryItem, type EquipmentSlot } from '../services/inventory';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Modal } from '../components/ui/Modal';
import { 
  Backpack, 
  Sword, 
  Shield, 
  Shirt, 
  Hand, 
  Footprints, 
  Gem,
  Package,
  Trash2
} from 'lucide-react';

export default function Inventory() {
  const { character, refreshCharacter } = useCharacter();
  const [inventory, setInventory] = useState<InventoryItem[]>([]);
  const [, setEquipment] = useState<any>(null);
  const [loading, setLoading] = useState(true);
  const [selectedItem, setSelectedItem] = useState<InventoryItem | null>(null);
  const [showItemModal, setShowItemModal] = useState(false);

  useEffect(() => {
    if (character) {
      loadInventory();
    }
  }, [character]);

  const loadInventory = async () => {
    if (!character) return;
    
    try {
      setLoading(true);
      const [inventoryData, equipmentData] = await Promise.all([
        inventoryService.getInventory(character.id),
        inventoryService.getEquipment(character.id)
      ]);
      
      setInventory(inventoryData);
      setEquipment(equipmentData);
    } catch (error) {
      console.error('Erro ao carregar inventário:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleEquipItem = async (item: InventoryItem, slot: EquipmentSlot) => {
    if (!character) return;
    
    try {
      await inventoryService.equipItem(character.id, item.id, slot);
      await loadInventory();
      await refreshCharacter();
    } catch (error) {
      console.error('Erro ao equipar item:', error);
    }
  };

  const handleUnequipItem = async (slot: EquipmentSlot) => {
    if (!character) return;
    
    try {
      await inventoryService.unequipItem(character.id, slot);
      await loadInventory();
      await refreshCharacter();
    } catch (error) {
      console.error('Erro ao desequipar item:', error);
    }
  };

  const handleUseItem = async (item: InventoryItem) => {
    if (!character) return;
    
    try {
      await inventoryService.useItem(character.id, item.id);
      await loadInventory();
      await refreshCharacter();
      setShowItemModal(false);
    } catch (error) {
      console.error('Erro ao usar item:', error);
    }
  };

  const getSlotIcon = (slot: string) => {
    switch (slot) {
      case 'Weapon': return Sword;
      case 'Shield': return Shield;
      case 'Helmet': return Shield;
      case 'Armor': return Shirt;
      case 'Gloves': return Hand;
      case 'Boots': return Footprints;
      case 'Ring': return Gem;
      case 'Amulet': return Gem;
      default: return Package;
    }
  };

  const getRarityColor = (rarity: string) => {
    switch (rarity.toLowerCase()) {
      case 'common': return 'bg-gray-100 text-gray-800';
      case 'uncommon': return 'bg-green-100 text-green-800';
      case 'rare': return 'bg-blue-100 text-blue-800';
      case 'epic': return 'bg-purple-100 text-purple-800';
      case 'legendary': return 'bg-yellow-100 text-yellow-800';
      case 'mythic': return 'bg-pink-100 text-pink-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  const getItemTypeIcon = (type: string) => {
    switch (type.toLowerCase()) {
      case 'weapon': return Sword;
      case 'armor': return Shield;
      case 'accessory': return Gem;
      case 'potion': return Package;
      default: return Package;
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Inventário</h1>
        <p className="text-gray-600 mt-2">Gerencie seus itens e equipamentos</p>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Equipment Slots */}
        <div className="lg:col-span-1">
          <Card title="Equipamentos">
            <div className="space-y-4">
              {['Weapon', 'Shield', 'Helmet', 'Armor', 'Gloves', 'Boots', 'Ring', 'Amulet'].map((slot) => {
                const Icon = getSlotIcon(slot);
                const equippedItem = inventory.find(item => item.equippedSlot === slot);
                
                return (
                  <div key={slot} className="flex items-center space-x-3 p-3 border border-gray-200 rounded-lg">
                    <Icon className="h-6 w-6 text-gray-500" />
                    <div className="flex-1">
                      <p className="text-sm font-medium text-gray-700">{slot}</p>
                      {equippedItem ? (
                        <div className="flex items-center justify-between">
                          <span className="text-sm text-gray-900">{equippedItem.name}</span>
                          <Button
                            size="sm"
                            variant="secondary"
                            onClick={() => handleUnequipItem(slot as EquipmentSlot)}
                          >
                            <Trash2 className="h-4 w-4" />
                          </Button>
                        </div>
                      ) : (
                        <span className="text-sm text-gray-500">Vazio</span>
                      )}
                    </div>
                  </div>
                );
              })}
            </div>
          </Card>
        </div>

        {/* Inventory Items */}
        <div className="lg:col-span-2">
          <Card title="Itens do Inventário">
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              {inventory.map((item) => {
                const ItemIcon = getItemTypeIcon(item.type);
                const rarityColor = getRarityColor(item.rarity);
                
                return (
                  <div
                    key={item.id}
                    className={`p-4 border rounded-lg cursor-pointer transition-all hover:shadow-md ${
                      item.isEquipped ? 'border-blue-300 bg-blue-50' : 'border-gray-200'
                    }`}
                    onClick={() => {
                      setSelectedItem(item);
                      setShowItemModal(true);
                    }}
                  >
                    <div className="flex items-start justify-between mb-2">
                      <ItemIcon className="h-6 w-6 text-gray-600" />
                      <span className={`px-2 py-1 rounded-full text-xs font-medium ${rarityColor}`}>
                        {item.rarity}
                      </span>
                    </div>
                    
                    <h3 className="font-medium text-gray-900 mb-1">{item.name}</h3>
                    <p className="text-sm text-gray-600 mb-2 line-clamp-2">{item.description}</p>
                    
                    <div className="flex items-center justify-between text-sm">
                      <span className="text-gray-500">Nível {item.level}</span>
                      {item.quantity > 1 && (
                        <span className="bg-gray-100 text-gray-700 px-2 py-1 rounded-full">
                          x{item.quantity}
                        </span>
                      )}
                    </div>
                    
                    {item.isEquipped && (
                      <div className="mt-2 text-xs text-blue-600 font-medium">
                        Equipado
                      </div>
                    )}
                  </div>
                );
              })}
            </div>
            
            {inventory.length === 0 && (
              <div className="text-center py-12">
                <Backpack className="h-12 w-12 text-gray-400 mx-auto mb-4" />
                <p className="text-gray-500">Seu inventário está vazio</p>
              </div>
            )}
          </Card>
        </div>
      </div>

      {/* Item Details Modal */}
      <Modal
        isOpen={showItemModal}
        onClose={() => setShowItemModal(false)}
        title={selectedItem?.name}
        size="md"
      >
        {selectedItem && (
          <div className="space-y-4">
            <div className="flex items-start space-x-4">
              <div className="p-3 bg-gray-100 rounded-lg">
                {React.createElement(getItemTypeIcon(selectedItem.type), { className: "h-8 w-8 text-gray-600" })}
              </div>
              <div className="flex-1">
                <div className="flex items-center space-x-2 mb-2">
                  <span className={`px-2 py-1 rounded-full text-xs font-medium ${getRarityColor(selectedItem.rarity)}`}>
                    {selectedItem.rarity}
                  </span>
                  <span className="text-sm text-gray-600">Nível {selectedItem.level}</span>
                </div>
                <p className="text-gray-700">{selectedItem.description}</p>
              </div>
            </div>

            {/* Item Stats */}
            {(selectedItem.attackBonus || selectedItem.defenseBonus || selectedItem.healthBonus || selectedItem.moraleBonus) && (
              <div className="bg-gray-50 p-4 rounded-lg">
                <h4 className="font-medium text-gray-900 mb-2">Atributos</h4>
                <div className="grid grid-cols-2 gap-2 text-sm">
                  {selectedItem.attackBonus && (
                    <div className="flex justify-between">
                      <span className="text-gray-600">Ataque:</span>
                      <span className="font-medium text-green-600">+{selectedItem.attackBonus}</span>
                    </div>
                  )}
                  {selectedItem.defenseBonus && (
                    <div className="flex justify-between">
                      <span className="text-gray-600">Defesa:</span>
                      <span className="font-medium text-blue-600">+{selectedItem.defenseBonus}</span>
                    </div>
                  )}
                  {selectedItem.healthBonus && (
                    <div className="flex justify-between">
                      <span className="text-gray-600">Vida:</span>
                      <span className="font-medium text-red-600">+{selectedItem.healthBonus}</span>
                    </div>
                  )}
                  {selectedItem.moraleBonus && (
                    <div className="flex justify-between">
                      <span className="text-gray-600">Moral:</span>
                      <span className="font-medium text-purple-600">+{selectedItem.moraleBonus}</span>
                    </div>
                  )}
                </div>
              </div>
            )}

            {/* Actions */}
            <div className="flex space-x-2">
              {selectedItem.isConsumable && !selectedItem.isEquipped && (
                <Button
                  onClick={() => handleUseItem(selectedItem)}
                  className="flex-1"
                >
                  Usar
                </Button>
              )}
              
              {!selectedItem.isEquipped && selectedItem.type === 'Weapon' && (
                <Button
                  onClick={() => handleEquipItem(selectedItem, 'Weapon')}
                  className="flex-1"
                >
                  Equipar
                </Button>
              )}
              
              {!selectedItem.isEquipped && selectedItem.type === 'Armor' && (
                <Button
                  onClick={() => handleEquipItem(selectedItem, 'Armor')}
                  className="flex-1"
                >
                  Equipar
                </Button>
              )}
            </div>
          </div>
        )}
      </Modal>
    </div>
  );
}
