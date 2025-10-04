import React, { useState, useEffect } from 'react';
import { heroService, type Hero } from '../services/heroService';
import { useInventory } from '../contexts/InventoryContext';
import { inventoryService, type InventoryItem, type EquipmentSlot } from '../services/inventory';
import { FadeIn, SlideIn } from '../components/animations';
import { useToast } from '../components/Toast';
import { 
  Backpack, 
  Sword, 
  Shield, 
  Shirt, 
  Hand, 
  Footprints, 
  Gem,
  Package,
  X,
  Plus,
  Minus
} from 'lucide-react';

export default function Inventory() {
  const [currentHero, setCurrentHero] = useState<Hero | null>(null);
  const { inventory, refreshInventory, equipItem } = useInventory();
  const { showToast } = useToast();
  const [selectedItem, setSelectedItem] = useState<InventoryItem | null>(null);
  const [showItemModal, setShowItemModal] = useState(false);
  const [draggedItem, setDraggedItem] = useState<InventoryItem | null>(null);

  // Carregar herói atual
  useEffect(() => {
    const loadCurrentHero = async () => {
      try {
        const heroes = await heroService.getActiveParty();
        if (heroes.length > 0) {
          setCurrentHero(heroes[0]); // Usar o primeiro herói da party ativa
        }
      } catch (error) {
        console.error('Erro ao carregar herói:', error);
      }
    };

    loadCurrentHero();
  }, []);

  useEffect(() => {
    if (currentHero) {
      refreshInventory();
    }
  }, [currentHero, refreshInventory]);

  const handleEquipItem = async (item: InventoryItem, slot: EquipmentSlot) => {
    if (!currentHero) return;
    
    try {
      await inventoryService.equipItem(currentHero.id, item.id, slot);
      
      // Atualizar estado local em tempo real
      equipItem(item, slot);
      showToast({
        type: 'success',
        title: 'Item equipado!',
        message: `${item.name} foi equipado com sucesso.`
      });
    } catch (error: any) {
      console.error('Erro ao equipar item:', error);
      showToast({
        type: 'error',
        title: 'Erro ao equipar item',
        message: error.response?.data?.message || 'Não foi possível equipar o item.'
      });
    }
  };

  const handleUnequipItem = async (slot: EquipmentSlot) => {
    if (!currentHero) return;
    
    try {
      await inventoryService.unequipItem(currentHero.id, slot);
      await refreshInventory();
      showToast({
        type: 'success',
        title: 'Item removido!',
        message: 'Item foi removido do equipamento.'
      });
    } catch (error: any) {
      console.error('Erro ao desequipar item:', error);
      showToast({
        type: 'error',
        title: 'Erro ao remover item',
        message: error.response?.data?.message || 'Não foi possível remover o item.'
      });
    }
  };

  const handleUseItem = async (item: InventoryItem) => {
    if (!currentHero) return;
    
    try {
      await inventoryService.useItem(currentHero.id, item.id);
      await refreshInventory();
      setShowItemModal(false);
      showToast({
        type: 'success',
        title: 'Item usado!',
        message: `${item.name} foi usado com sucesso.`
      });
    } catch (error: any) {
      console.error('Erro ao usar item:', error);
      showToast({
        type: 'error',
        title: 'Erro ao usar item',
        message: error.response?.data?.message || 'Não foi possível usar o item.'
      });
    }
  };

  const handleDragStart = (e: React.DragEvent, item: InventoryItem) => {
    setDraggedItem(item);
    e.dataTransfer.effectAllowed = 'move';
    e.dataTransfer.setData('text/plain', JSON.stringify(item));
    
    // Criar um elemento fantasma personalizado
    const dragImage = e.currentTarget.cloneNode(true) as HTMLElement;
    dragImage.style.transform = 'rotate(5deg)';
    dragImage.style.opacity = '0.8';
    document.body.appendChild(dragImage);
    e.dataTransfer.setDragImage(dragImage, 0, 0);
    
    // Remover o elemento fantasma após um pequeno delay
    setTimeout(() => {
      if (document.body.contains(dragImage)) {
        document.body.removeChild(dragImage);
      }
    }, 0);
  };

  const handleDragOver = (e: React.DragEvent) => {
    e.preventDefault();
    e.dataTransfer.dropEffect = 'move';
  };

  const handleDrop = async (e: React.DragEvent, slot: EquipmentSlot) => {
    e.preventDefault();
    
    if (!draggedItem || !currentHero) return;

    // Verificar se o item pode ser equipado neste slot
    const canEquip = canItemBeEquippedInSlot(draggedItem, slot);
    
    if (!canEquip) {
      showToast({
        type: 'warning',
        title: 'Slot incompatível',
        message: `${draggedItem.name} não pode ser equipado neste slot.`
      });
      return;
    }

    await handleEquipItem(draggedItem, slot);
    setDraggedItem(null);
  };

  const canItemBeEquippedInSlot = (item: InventoryItem, slot: EquipmentSlot): boolean => {
    const itemType = item.type.toLowerCase();
    
    switch (slot) {
      case 'Weapon':
        return itemType === 'weapon';
      case 'Shield':
        return itemType === 'shield';
      case 'Helmet':
        return itemType === 'helmet';
      case 'Armor':
        return itemType === 'armor';
      case 'Gloves':
        return itemType === 'gloves';
      case 'Boots':
        return itemType === 'boots';
      case 'Ring':
        return itemType === 'ring';
      case 'Amulet':
        return itemType === 'amulet';
      default:
        return false;
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
      case 'common': return 'bg-gray-900/30 text-gray-400 border-gray-700/30';
      case 'uncommon': return 'bg-green-900/30 text-green-400 border-green-700/30';
      case 'rare': return 'bg-blue-900/30 text-blue-400 border-blue-700/30';
      case 'epic': return 'bg-purple-900/30 text-purple-400 border-purple-700/30';
      case 'legendary': return 'bg-yellow-900/30 text-yellow-400 border-yellow-700/30';
      case 'mythic': return 'bg-pink-900/30 text-pink-400 border-pink-700/30';
      default: return 'bg-gray-900/30 text-gray-400 border-gray-700/30';
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

  if (!inventory) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-slate-900 via-purple-900 to-slate-900 flex items-center justify-center relative overflow-hidden">
        <div className="absolute inset-0 overflow-hidden">
          {[...Array(15)].map((_, i) => (
            <div
              key={i}
              className="absolute w-2 h-2 bg-amber-400 rounded-full opacity-30 animate-pulse"
              style={{
                left: `${Math.random() * 100}%`,
                top: `${Math.random() * 100}%`,
                animationDelay: `${Math.random() * 3}s`,
                animationDuration: `${2 + Math.random() * 3}s`
              }}
            />
          ))}
        </div>
        <div className="text-center relative z-10">
          <div className="inline-block p-6 bg-gradient-to-br from-amber-500 to-orange-600 rounded-full shadow-lg shadow-amber-500/50 animate-pulse mb-4">
            <div className="animate-spin rounded-full h-16 w-16 border-4 border-white border-t-transparent"></div>
          </div>
          <h2 className="text-4xl font-black text-transparent bg-clip-text bg-gradient-to-r from-amber-300 via-amber-500 to-orange-600 mb-2">Carregando Inventário...</h2>
          <p className="text-gray-400 text-lg">Organizando seus tesouros</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-purple-900 to-slate-900 p-6">
      <div className="max-w-7xl mx-auto">
        <FadeIn delay={0}>
          <div className="text-center mb-8">
            <h1 className="hero-title text-6xl font-black mb-4">Inventário</h1>
            <p className="text-xl text-gray-300">Gerencie seus itens e equipamentos</p>
          </div>
        </FadeIn>

        {/* Layout Horizontal - Stats Cards */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
          <SlideIn direction="up" delay={100}>
            <div className="stat-card group">
              <div className="flex items-center justify-between mb-3">
                <div className="p-3 bg-gradient-to-br from-blue-500 to-blue-600 rounded-xl shadow-lg group-hover:scale-110 transition-transform">
                  <Backpack className="h-8 w-8 text-white" />
                </div>
                <span className="text-4xl font-black text-gradient">{inventory.length}</span>
              </div>
              <h3 className="text-gray-400 text-sm font-medium">Total de Itens</h3>
            </div>
          </SlideIn>

          <SlideIn direction="up" delay={200}>
            <div className="stat-card group">
              <div className="flex items-center justify-between mb-3">
                <div className="p-3 bg-gradient-to-br from-purple-500 to-purple-600 rounded-xl shadow-lg group-hover:scale-110 transition-transform">
                  <Sword className="h-8 w-8 text-white" />
                </div>
                <span className="text-4xl font-black text-gradient">
                  {inventory.filter(item => item.isEquipped).length}
                </span>
              </div>
              <h3 className="text-gray-400 text-sm font-medium">Itens Equipados</h3>
            </div>
          </SlideIn>

          <SlideIn direction="up" delay={300}>
            <div className="stat-card group">
              <div className="flex items-center justify-between mb-3">
                <div className="p-3 bg-gradient-to-br from-green-500 to-green-600 rounded-xl shadow-lg group-hover:scale-110 transition-transform">
                  <Package className="h-8 w-8 text-white" />
                </div>
                <span className="text-4xl font-black text-gradient">
                  {inventory.filter(item => item.type === 'potion').length}
                </span>
              </div>
              <h3 className="text-gray-400 text-sm font-medium">Poções</h3>
            </div>
          </SlideIn>

          <SlideIn direction="up" delay={400}>
            <div className="stat-card group">
              <div className="flex items-center justify-between mb-3">
                <div className="p-3 bg-gradient-to-br from-amber-500 to-orange-600 rounded-xl shadow-lg group-hover:scale-110 transition-transform animate-glow">
                  <Gem className="h-8 w-8 text-white" />
                </div>
                <span className="text-4xl font-black text-gradient">
                  {inventory.filter(item => item.rarity === 'rare' || item.rarity === 'epic' || item.rarity === 'legendary').length}
                </span>
              </div>
              <h3 className="text-gray-400 text-sm font-medium">Itens Raros</h3>
            </div>
          </SlideIn>
        </div>

        {/* Layout Horizontal - Equipment e Inventory */}
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
          {/* Equipment Slots */}
          <SlideIn direction="left" delay={500}>
            <div className="card backdrop-blur-sm bg-black/20">
              <h3 className="text-xl font-bold text-gradient mb-6">Equipamentos</h3>
              <div className="grid grid-cols-2 gap-4">
                {['Weapon', 'Shield', 'Helmet', 'Armor', 'Gloves', 'Boots', 'Ring', 'Amulet'].map((slot) => {
                  const Icon = getSlotIcon(slot);
                  const equippedItem = inventory.find(item => item.equippedSlot === slot);
                  
                  return (
                    <div key={slot} className="group">
                      <div 
                        className={`p-4 rounded-xl border-2 transition-all duration-300 ${
                          equippedItem 
                            ? 'border-amber-500/50 bg-amber-900/20 group-hover:border-amber-400/70' 
                            : 'border-gray-700/50 bg-gray-900/20 group-hover:border-gray-600/70'
                        } ${draggedItem && canItemBeEquippedInSlot(draggedItem, slot as EquipmentSlot) ? 'border-blue-500/50 bg-blue-900/20 ring-2 ring-blue-400/50' : ''}`}
                        onDragOver={handleDragOver}
                        onDrop={(e) => handleDrop(e, slot as EquipmentSlot)}
                        onDragEnter={(e) => {
                          if (draggedItem && canItemBeEquippedInSlot(draggedItem, slot as EquipmentSlot)) {
                            e.currentTarget.classList.add('ring-2', 'ring-blue-400/50');
                          }
                        }}
                        onDragLeave={(e) => {
                          e.currentTarget.classList.remove('ring-2', 'ring-blue-400/50');
                        }}
                      >
                        <div className="flex flex-col items-center text-center space-y-3">
                          <div className={`p-3 rounded-lg transition-transform group-hover:scale-110 ${
                            equippedItem 
                              ? 'bg-gradient-to-br from-amber-500 to-orange-600' 
                              : 'bg-gray-700/50'
                          }`}>
                            <Icon className="h-6 w-6 text-white" />
                          </div>
                          <div className="w-full">
                            <p className="text-sm font-medium text-gray-300 mb-1">{slot}</p>
                            {equippedItem ? (
                              <div className="space-y-2">
                                <p className="text-xs text-amber-400 font-medium truncate">{equippedItem.name}</p>
                                <button
                                  onClick={() => handleUnequipItem(slot as EquipmentSlot)}
                                  className="btn btn-secondary w-full text-xs py-1"
                                >
                                  <Minus className="h-3 w-3 mr-1" />
                                  Remover
                                </button>
                              </div>
                            ) : (
                              <p className="text-xs text-gray-500">Vazio</p>
                            )}
                          </div>
                        </div>
                      </div>
                    </div>
                  );
                })}
              </div>
            </div>
          </SlideIn>

          {/* Inventory Items */}
          <SlideIn direction="right" delay={600}>
            <div className="card backdrop-blur-sm bg-black/20">
              <h3 className="text-xl font-bold text-gradient mb-6">Itens do Inventário</h3>
              <div className="max-h-96 overflow-y-auto space-y-3 scrollbar-hide">
                {inventory.map((item) => {
                  const ItemIcon = getItemTypeIcon(item.type);
                  const rarityColor = getRarityColor(item.rarity);
                  
                  return (
                    <div
                      key={item.id}
                      className={`p-4 rounded-xl border transition-all duration-300 cursor-grab group hover:scale-[1.02] ${
                        item.isEquipped 
                          ? 'border-amber-500/50 bg-amber-900/20 hover:border-amber-400/70' 
                          : 'border-gray-700/50 bg-gray-900/20 hover:border-gray-600/70'
                      } ${draggedItem?.id === item.id ? 'opacity-50' : ''}`}
                      draggable
                      onDragStart={(e) => handleDragStart(e, item)}
                      onDragEnd={() => setDraggedItem(null)}
                      onClick={() => {
                        // Só abre o modal se não estiver arrastando
                        if (!draggedItem) {
                          setSelectedItem(item);
                          setShowItemModal(true);
                        }
                      }}
                    >
                      <div className="flex items-center space-x-4">
                        <div className={`p-2 rounded-lg transition-transform group-hover:scale-110 ${
                          item.isEquipped 
                            ? 'bg-gradient-to-br from-amber-500 to-orange-600' 
                            : 'bg-gray-700/50'
                        }`}>
                          <ItemIcon className="h-5 w-5 text-white" />
                        </div>
                        
                        <div className="flex-1 min-w-0">
                          <div className="flex items-center justify-between mb-1">
                            <h4 className="text-sm font-bold text-gradient truncate">{item.name}</h4>
                            <span className={`px-2 py-1 rounded-full text-xs font-bold ${rarityColor}`}>
                              {item.rarity}
                            </span>
                          </div>
                          <p className="text-xs text-gray-400 truncate mb-1">{item.description}</p>
                          <div className="flex items-center justify-between text-xs">
                            <span className="text-gray-500">Nível {item.level}</span>
                            {item.quantity > 1 && (
                              <span className="bg-gray-700/50 text-gray-300 px-2 py-1 rounded-full">
                                x{item.quantity}
                              </span>
                            )}
                            {item.isEquipped && (
                              <span className="text-amber-400 font-medium">Equipado</span>
                            )}
                          </div>
                        </div>
                      </div>
                    </div>
                  );
                })}
              </div>
              
              {inventory.length === 0 && (
                <div className="text-center py-12">
                  <div className="p-8 bg-gradient-to-br from-amber-500 to-orange-600 rounded-full shadow-lg shadow-amber-500/50 mx-auto w-24 h-24 flex items-center justify-center mb-6">
                    <Backpack className="h-12 w-12 text-white" />
                  </div>
                  <h3 className="text-2xl font-bold text-gradient mb-2">Inventário Vazio</h3>
                  <p className="text-gray-400">Vá à loja para comprar itens</p>
                </div>
              )}
            </div>
          </SlideIn>
        </div>

        {/* Item Details Modal */}
        {showItemModal && selectedItem && (
          <div className="fixed inset-0 bg-black/70 backdrop-blur-sm flex items-center justify-center z-50 p-4">
            <div className="card backdrop-blur-sm bg-black/30 w-full max-w-2xl max-h-[90vh] overflow-y-auto animate-fadeIn">
              <div className="flex items-center justify-between mb-6">
                <div className="flex items-center space-x-4">
                  <div className="p-4 bg-gradient-to-br from-amber-500 to-orange-600 rounded-xl shadow-lg">
                    {React.createElement(getItemTypeIcon(selectedItem.type), { className: "h-8 w-8 text-white" })}
                  </div>
                  <div>
                    <h2 className="text-2xl font-bold text-gradient">
                      {selectedItem.name}
                    </h2>
                    <div className="flex items-center space-x-2 mt-2">
                      <span className={`px-3 py-1 rounded-full text-xs font-bold ${getRarityColor(selectedItem.rarity)}`}>
                        {selectedItem.rarity}
                      </span>
                      <span className="text-sm text-gray-400">Nível {selectedItem.level}</span>
                      {selectedItem.isEquipped && (
                        <span className="text-amber-400 font-medium text-sm">Equipado</span>
                      )}
                    </div>
                  </div>
                </div>
                <button
                  onClick={() => setShowItemModal(false)}
                  className="text-gray-400 hover:text-amber-400 transition-colors p-2 rounded-lg hover:bg-gray-800/50"
                >
                  <X className="h-6 w-6" />
                </button>
              </div>

              <div className="space-y-6">
                <div>
                  <p className="text-gray-300 leading-relaxed">{selectedItem.description}</p>
                </div>

                {/* Item Stats */}
                {(selectedItem.attackBonus || selectedItem.defenseBonus || selectedItem.healthBonus || selectedItem.moraleBonus) && (
                  <div className="card bg-black/20 backdrop-blur-sm">
                    <h4 className="text-lg font-bold text-gradient mb-4">Atributos</h4>
                    <div className="grid grid-cols-2 gap-4 text-sm">
                      {selectedItem.attackBonus && (
                        <div className="flex justify-between">
                          <span className="text-gray-400">Ataque:</span>
                          <span className="font-bold text-green-400">+{selectedItem.attackBonus}</span>
                        </div>
                      )}
                      {selectedItem.defenseBonus && (
                        <div className="flex justify-between">
                          <span className="text-gray-400">Defesa:</span>
                          <span className="font-bold text-blue-400">+{selectedItem.defenseBonus}</span>
                        </div>
                      )}
                      {selectedItem.healthBonus && (
                        <div className="flex justify-between">
                          <span className="text-gray-400">Vida:</span>
                          <span className="font-bold text-red-400">+{selectedItem.healthBonus}</span>
                        </div>
                      )}
                      {selectedItem.moraleBonus && (
                        <div className="flex justify-between">
                          <span className="text-gray-400">Moral:</span>
                          <span className="font-bold text-purple-400">+{selectedItem.moraleBonus}</span>
                        </div>
                      )}
                    </div>
                  </div>
                )}

                {/* Actions */}
                <div className="flex space-x-3">
                  {selectedItem.isConsumable && !selectedItem.isEquipped && (
                    <button
                      onClick={() => handleUseItem(selectedItem)}
                      className="btn btn-primary flex-1"
                    >
                      <Package className="h-5 w-5 mr-2" />
                      Usar
                    </button>
                  )}
                  
                  {!selectedItem.isEquipped && selectedItem.type === 'Weapon' && (
                    <button
                      onClick={() => handleEquipItem(selectedItem, 'Weapon')}
                      className="btn btn-primary flex-1"
                    >
                      <Plus className="h-5 w-5 mr-2" />
                      Equipar
                    </button>
                  )}
                  
                  {!selectedItem.isEquipped && selectedItem.type === 'Armor' && (
                    <button
                      onClick={() => handleEquipItem(selectedItem, 'Armor')}
                      className="btn btn-primary flex-1"
                    >
                      <Plus className="h-5 w-5 mr-2" />
                      Equipar
                    </button>
                  )}
                </div>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
