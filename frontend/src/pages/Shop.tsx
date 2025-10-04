import React, { useState, useEffect } from 'react';
import { useCharacter } from '../contexts/CharacterContext';
import { shopService, type ShopItem, type ShopType, type Rarity } from '../services/shop';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Modal } from '../components/ui/Modal';
import { 
  ShoppingBag, 
  Search, 
  Coins, 
  Sword,
  Shield,
  Gem,
  Package,
  ShoppingCart
} from 'lucide-react';

export default function Shop() {
  const { character, refreshCharacter } = useCharacter();
  const [items, setItems] = useState<ShopItem[]>([]);
  const [shopTypes, setShopTypes] = useState<ShopType[]>([]);
  const [rarities, setRarities] = useState<Rarity[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedShopType, setSelectedShopType] = useState('general');
  const [selectedRarity, setSelectedRarity] = useState('');
  const [selectedType, setSelectedType] = useState('');
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedItem, setSelectedItem] = useState<ShopItem | null>(null);
  const [showItemModal, setShowItemModal] = useState(false);
  const [buying, setBuying] = useState(false);

  useEffect(() => {
    loadShopData();
  }, []);

  useEffect(() => {
    if (character) {
      loadItems();
    }
  }, [character, selectedShopType, selectedRarity, selectedType]);

  const loadShopData = async () => {
    try {
      const [shopTypesData, raritiesData] = await Promise.all([
        shopService.getShopTypes(),
        shopService.getRarities()
      ]);
      
      setShopTypes(shopTypesData.shopTypes);
      setRarities(raritiesData.rarities);
    } catch (error) {
      console.error('Erro ao carregar dados da loja:', error);
    }
  };

  const loadItems = async () => {
    if (!character) return;
    
    try {
      setLoading(true);
      const response = await shopService.getShopItemsByLevel(character.level, selectedShopType);
      let filteredItems = response.items;

      // Filter by rarity
      if (selectedRarity) {
        filteredItems = filteredItems.filter(item => item.rarity.toLowerCase() === selectedRarity.toLowerCase());
      }

      // Filter by type
      if (selectedType) {
        filteredItems = filteredItems.filter(item => item.type.toLowerCase() === selectedType.toLowerCase());
      }

      // Filter by search term
      if (searchTerm) {
        filteredItems = filteredItems.filter(item => 
          item.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
          item.description.toLowerCase().includes(searchTerm.toLowerCase())
        );
      }

      setItems(filteredItems);
    } catch (error) {
      console.error('Erro ao carregar itens:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleBuyItem = async (item: ShopItem) => {
    if (!character) return;
    
    if (character.gold < item.shopPrice) {
      alert('Ouro insuficiente!');
      return;
    }

    try {
      setBuying(true);
      await shopService.buyItem(character.id, item.id, 1);
      await refreshCharacter();
      setShowItemModal(false);
      alert('Item comprado com sucesso!');
    } catch (error: any) {
      alert(error.message || 'Erro ao comprar item');
    } finally {
      setBuying(false);
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

  const canAfford = (price: number) => {
    return character ? character.gold >= price : false;
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
        <h1 className="text-3xl font-bold text-gray-900">Loja</h1>
        <p className="text-gray-600 mt-2">Compre equipamentos e itens</p>
      </div>

      {/* Character Gold */}
      {character && (
        <Card>
          <div className="flex items-center space-x-3">
            <Coins className="h-6 w-6 text-yellow-500" />
            <div>
              <p className="text-sm text-gray-600">Ouro Disponível</p>
              <p className="text-2xl font-bold text-gray-900">{character.gold}</p>
            </div>
          </div>
        </Card>
      )}

      {/* Filters */}
      <Card title="Filtros">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          {/* Shop Type */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Tipo de Loja</label>
            <select
              value={selectedShopType}
              onChange={(e) => setSelectedShopType(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              {shopTypes.map((type) => (
                <option key={type.type} value={type.type}>
                  {type.description}
                </option>
              ))}
            </select>
          </div>

          {/* Item Type */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Tipo de Item</label>
            <select
              value={selectedType}
              onChange={(e) => setSelectedType(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <option value="">Todos</option>
              <option value="weapon">Armas</option>
              <option value="armor">Armaduras</option>
              <option value="accessory">Acessórios</option>
              <option value="potion">Poções</option>
            </select>
          </div>

          {/* Rarity */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Raridade</label>
            <select
              value={selectedRarity}
              onChange={(e) => setSelectedRarity(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <option value="">Todas</option>
              {rarities.map((rarity) => (
                <option key={rarity.rarity} value={rarity.rarity}>
                  {rarity.description}
                </option>
              ))}
            </select>
          </div>

          {/* Search */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Buscar</label>
            <div className="relative">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
              <input
                type="text"
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                placeholder="Nome do item..."
                className="w-full pl-10 pr-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
          </div>
        </div>
      </Card>

      {/* Items Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
        {items.map((item) => {
          const ItemIcon = getItemTypeIcon(item.type);
          const rarityColor = getRarityColor(item.rarity);
          const affordable = canAfford(item.shopPrice);
          
          return (
            <Card key={item.id} className="hover:shadow-lg transition-shadow cursor-pointer">
              <div
                onClick={() => {
                  setSelectedItem(item);
                  setShowItemModal(true);
                }}
              >
                <div className="flex items-start justify-between mb-3">
                  <ItemIcon className="h-8 w-8 text-gray-600" />
                  <span className={`px-2 py-1 rounded-full text-xs font-medium ${rarityColor}`}>
                    {item.rarity}
                  </span>
                </div>
                
                <h3 className="font-semibold text-gray-900 mb-2">{item.name}</h3>
                <p className="text-sm text-gray-600 mb-3 line-clamp-2">{item.description}</p>
                
                <div className="flex items-center justify-between text-sm mb-3">
                  <span className="text-gray-500">Nível {item.level}</span>
                  <div className="flex items-center space-x-1">
                    <Coins className="h-4 w-4 text-yellow-500" />
                    <span className={`font-medium ${affordable ? 'text-gray-900' : 'text-red-600'}`}>
                      {item.shopPrice}
                    </span>
                  </div>
                </div>
                
                <Button
                  className="w-full"
                  disabled={!affordable}
                  variant={affordable ? 'primary' : 'secondary'}
                >
                  <ShoppingCart className="h-4 w-4 mr-2" />
                  {affordable ? 'Comprar' : 'Sem Ouro'}
                </Button>
              </div>
            </Card>
          );
        })}
      </div>

      {items.length === 0 && (
        <div className="text-center py-12">
          <ShoppingBag className="h-12 w-12 text-gray-400 mx-auto mb-4" />
          <p className="text-gray-500">Nenhum item encontrado</p>
        </div>
      )}

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

            {/* Price */}
            <div className="bg-yellow-50 p-4 rounded-lg">
              <div className="flex items-center justify-between">
                <div className="flex items-center space-x-2">
                  <Coins className="h-5 w-5 text-yellow-600" />
                  <span className="font-medium text-gray-900">Preço</span>
                </div>
                <span className="text-2xl font-bold text-gray-900">{selectedItem.shopPrice}</span>
              </div>
            </div>

            {/* Buy Button */}
            <Button
              onClick={() => handleBuyItem(selectedItem)}
              loading={buying}
              disabled={!canAfford(selectedItem.shopPrice)}
              className="w-full"
              size="lg"
            >
              <ShoppingCart className="h-5 w-5 mr-2" />
              {canAfford(selectedItem.shopPrice) ? 'Comprar' : 'Ouro Insuficiente'}
            </Button>
          </div>
        )}
      </Modal>
    </div>
  );
}
