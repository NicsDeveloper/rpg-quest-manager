import React, { useState, useEffect } from 'react';
import { heroService, type Hero } from '../services/heroService';
import { useInventory } from '../contexts/InventoryContext';
import { shopService, type ShopItem, type ShopType, type Rarity } from '../services/shop';
import { FadeIn, SlideIn } from '../components/animations';
import { useToast } from '../components/Toast';
import { 
  ShoppingBag, 
  Search, 
  Coins, 
  Sword,
  Shield,
  Gem,
  Package,
  ShoppingCart,
  X
} from 'lucide-react';

export default function Shop() {
  const { addItemToInventory } = useInventory();
  const { showToast } = useToast();
  const [userProfile, setUserProfile] = useState<any>(null);
  const [selectedHero, setSelectedHero] = useState<Hero | null>(null);
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
    loadUserProfile();
  }, []);

  useEffect(() => {
    if (userProfile && selectedHero) {
      loadItems();
    }
  }, [userProfile, selectedHero, selectedShopType, selectedRarity, selectedType]);

  const loadUserProfile = async () => {
    try {
      const profile = await heroService.getUserProfile();
      setUserProfile(profile);
      
      // Se não há herói selecionado, seleciona o primeiro da party ativa
      if (!selectedHero && profile.activePartyCount > 0) {
        const activeParty = await heroService.getActiveParty();
        if (activeParty.length > 0) {
          setSelectedHero(activeParty[0]);
        }
      }
    } catch (error) {
      console.error('Erro ao carregar perfil do usuário:', error);
    }
  };

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
    if (!selectedHero) return;
    
    try {
      setLoading(true);
      const response = await shopService.getShopItemsByLevel(selectedHero?.level || 1, selectedShopType);
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
    if (!selectedHero || !userProfile) return;
    
    if (userProfile.gold < item.shopPrice) {
      showToast({
        type: 'error',
        title: 'Ouro insuficiente!',
        message: `Você precisa de ${item.shopPrice} de ouro para comprar este item.`
      });
      return;
    }

    try {
      setBuying(true);
      const result = await shopService.buyItem(selectedHero.id, item.id, 1);
      
      // Atualizar ouro em tempo real
      setUserProfile({ ...userProfile, gold: userProfile.gold - item.shopPrice });
      
      // Adicionar item ao inventário em tempo real
      if (result.inventoryItem) {
        addItemToInventory(result.inventoryItem);
      }
      
      setShowItemModal(false);
      showToast({
        type: 'success',
        title: 'Item comprado!',
        message: `${item.name} foi adicionado ao seu inventário.`
      });
    } catch (error: any) {
      showToast({
        type: 'error',
        title: 'Erro ao comprar item',
        message: error.message || 'Tente novamente mais tarde.'
      });
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
      case 'common': return 'bg-gray-900/30 text-gray-400 border-gray-700/30';
      case 'uncommon': return 'bg-green-900/30 text-green-400 border-green-700/30';
      case 'rare': return 'bg-blue-900/30 text-blue-400 border-blue-700/30';
      case 'epic': return 'bg-purple-900/30 text-purple-400 border-purple-700/30';
      case 'legendary': return 'bg-yellow-900/30 text-yellow-400 border-yellow-700/30';
      case 'mythic': return 'bg-pink-900/30 text-pink-400 border-pink-700/30';
      default: return 'bg-gray-900/30 text-gray-400 border-gray-700/30';
    }
  };

  const canAfford = (price: number) => {
    return userProfile ? userProfile.gold >= price : false;
  };

  if (loading) {
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
          <h2 className="text-4xl font-black text-transparent bg-clip-text bg-gradient-to-r from-amber-300 via-amber-500 to-orange-600 mb-2">Carregando Loja...</h2>
          <p className="text-gray-400 text-lg">Preparando tesouros épicos</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-purple-900 to-slate-900 p-6">
      <div className="max-w-7xl mx-auto space-y-8">
        <FadeIn delay={0}>
          <div className="text-center">
            <h1 className="hero-title text-6xl font-black mb-4">Loja</h1>
            <p className="text-xl text-gray-300">Compre equipamentos e itens épicos</p>
          </div>
        </FadeIn>

        {/* Character Gold */}
        {selectedHero && (
          <SlideIn direction="up" delay={100}>
            <div className="card backdrop-blur-sm bg-black/20">
              <div className="flex items-center justify-center space-x-4">
                <div className="p-4 bg-gradient-to-br from-yellow-500 to-orange-600 rounded-xl shadow-lg">
                  <Coins className="h-8 w-8 text-white" />
                </div>
                <div className="text-center">
                  <p className="text-sm text-gray-400 mb-1">Ouro Disponível</p>
                  <p className="text-4xl font-black text-gradient">{userProfile?.gold || 0}</p>
                </div>
              </div>
            </div>
          </SlideIn>
        )}

        {/* Hero Selection */}
        {userProfile && (
          <SlideIn direction="right" delay={150}>
            <div className="card backdrop-blur-sm bg-black/20">
              <h3 className="text-xl font-bold text-gradient mb-4">Herói Selecionado</h3>
              <div className="flex items-center gap-4">
                <div className="flex-1">
                  {selectedHero ? (
                    <div className="flex items-center gap-3">
                      <div className="w-12 h-12 bg-gradient-to-br from-blue-500 to-purple-600 rounded-full flex items-center justify-center">
                        <span className="text-white font-bold text-lg">{selectedHero.name.charAt(0)}</span>
                      </div>
                      <div>
                        <p className="font-semibold text-white">{selectedHero.name}</p>
                        <p className="text-sm text-gray-400">{selectedHero.class} - Nível {selectedHero.level}</p>
                      </div>
                    </div>
                  ) : (
                    <p className="text-gray-400">Nenhum herói selecionado</p>
                  )}
                </div>
                {userProfile.activePartyCount === 0 && (
                  <div className="text-center">
                    <p className="text-sm text-red-400 mb-2">Nenhum herói na party</p>
                    <button
                      onClick={() => window.location.href = '/heroes'}
                      className="btn-primary text-sm"
                    >
                      Criar Herói
                    </button>
                  </div>
                )}
              </div>
            </div>
          </SlideIn>
        )}

        {/* Filters */}
        <SlideIn direction="left" delay={200}>
          <div className="card backdrop-blur-sm bg-black/20">
            <h3 className="text-xl font-bold text-gradient mb-6">Filtros</h3>
            <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
              {/* Shop Type */}
              <div>
                <label className="label text-amber-400 mb-2">Tipo de Loja</label>
                <select
                  value={selectedShopType}
                  onChange={(e) => setSelectedShopType(e.target.value)}
                  className="input w-full"
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
                <label className="label text-amber-400 mb-2">Tipo de Item</label>
                <select
                  value={selectedType}
                  onChange={(e) => setSelectedType(e.target.value)}
                  className="input w-full"
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
                <label className="label text-amber-400 mb-2">Raridade</label>
                <select
                  value={selectedRarity}
                  onChange={(e) => setSelectedRarity(e.target.value)}
                  className="input w-full"
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
                <label className="label text-amber-400 mb-2">Buscar</label>
                <div className="relative">
                  <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-5 w-5 text-amber-400" />
                  <input
                    type="text"
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    placeholder="Nome do item..."
                    className="input w-full pl-12"
                  />
                </div>
              </div>
            </div>
          </div>
        </SlideIn>

        {/* Items Grid */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
          {items.map((item, index) => {
            const ItemIcon = getItemTypeIcon(item.type);
            const rarityColor = getRarityColor(item.rarity);
            const affordable = canAfford(item.shopPrice);
            
            return (
              <SlideIn key={item.id} direction="up" delay={300 + (index * 50)}>
                <div
                  className="card backdrop-blur-sm bg-black/20 hover:bg-black/30 transition-all duration-300 cursor-pointer group hover:scale-105"
                  onClick={() => {
                    setSelectedItem(item);
                    setShowItemModal(true);
                  }}
                >
                  <div className="flex items-start justify-between mb-4">
                    <div className="p-3 bg-gradient-to-br from-amber-500 to-orange-600 rounded-xl shadow-lg group-hover:shadow-amber-500/50 transition-shadow">
                      <ItemIcon className="h-6 w-6 text-white" />
                    </div>
                    <span className={`px-3 py-1 rounded-full text-xs font-bold ${rarityColor}`}>
                      {item.rarity}
                    </span>
                  </div>
                  
                  <h3 className="text-xl font-bold text-gradient mb-3">{item.name}</h3>
                  <p className="text-gray-300 mb-4 line-clamp-2">{item.description}</p>
                  
                  <div className="space-y-3 text-sm">
                    <div className="flex items-center justify-between">
                      <span className="text-gray-400">Nível:</span>
                      <span className="font-bold text-blue-400">{item.level}</span>
                    </div>
                    
                    <div className="flex items-center justify-between">
                      <span className="text-gray-400">Preço:</span>
                      <div className="flex items-center space-x-1">
                        <Coins className="h-4 w-4 text-yellow-400" />
                        <span className={`font-bold ${affordable ? 'text-yellow-400' : 'text-red-400'}`}>
                          {item.shopPrice}
                        </span>
                      </div>
                    </div>
                  </div>
                  
                  <button
                    className={`btn w-full mt-4 ${affordable ? 'btn-primary' : 'btn-secondary'}`}
                    disabled={!affordable}
                    onClick={(e) => {
                      e.stopPropagation();
                      if (affordable) {
                        handleBuyItem(item);
                      }
                    }}
                  >
                    <ShoppingCart className="h-5 w-5 mr-2" />
                    {affordable ? 'Comprar' : 'Sem Ouro'}
                  </button>
                </div>
              </SlideIn>
            );
          })}
        </div>

        {items.length === 0 && (
          <div className="text-center py-16">
            <div className="p-8 bg-gradient-to-br from-amber-500 to-orange-600 rounded-full shadow-lg shadow-amber-500/50 mx-auto w-24 h-24 flex items-center justify-center mb-6">
              <ShoppingBag className="h-12 w-12 text-white" />
            </div>
            <h3 className="text-2xl font-bold text-gradient mb-2">Nenhum item encontrado</h3>
            <p className="text-gray-400">Tente ajustar os filtros ou buscar por outro termo</p>
          </div>
        )}

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

                {/* Price */}
                <div className="card bg-yellow-900/20 backdrop-blur-sm">
                  <h4 className="text-lg font-bold text-gradient mb-3">Preço</h4>
                  <div className="flex items-center space-x-3">
                    <div className="p-2 bg-gradient-to-br from-yellow-500 to-orange-600 rounded-lg">
                      <Coins className="h-6 w-6 text-white" />
                    </div>
                    <span className="text-gray-300 font-bold text-2xl">
                      {selectedItem.shopPrice} de ouro
                    </span>
                  </div>
                </div>

                {/* Buy Button */}
                <button
                  onClick={() => handleBuyItem(selectedItem)}
                  disabled={!canAfford(selectedItem.shopPrice) || buying}
                  className={`btn w-full ${canAfford(selectedItem.shopPrice) ? 'btn-primary' : 'btn-secondary'}`}
                >
                  {buying ? (
                    <div className="animate-spin rounded-full h-5 w-5 border-2 border-white border-t-transparent mr-2"></div>
                  ) : (
                    <ShoppingCart className="h-5 w-5 mr-2" />
                  )}
                  {canAfford(selectedItem.shopPrice) ? 'Comprar' : 'Ouro Insuficiente'}
                </button>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
