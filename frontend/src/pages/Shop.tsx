import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Navbar } from '../components/Navbar';
import { Card } from '../components/Card';
import { Button } from '../components/Button';
import { shopService, ShopItem } from '../services/shopService';
import { itemService } from '../services/itemService';
import { useAuth } from '../contexts/AuthContext';

export const Shop: React.FC = () => {
  const { t } = useTranslation();
  const { user } = useAuth();
  const [shopItems, setShopItems] = useState<ShopItem[]>([]);
  const [inventory, setInventory] = useState<ShopItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [buying, setBuying] = useState<number | null>(null);
  const [using, setUsing] = useState<number | null>(null);
  const [activeTab, setActiveTab] = useState<'shop' | 'inventory'>('shop');

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const [shopData, inventoryData] = await Promise.all([
        shopService.getShopItems(),
        shopService.getInventory()
      ]);
      setShopItems(shopData);
      setInventory(inventoryData);
    } catch (error) {
      console.error('Error loading shop data:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleBuyItem = async (item: ShopItem, quantity: number = 1) => {
    try {
      setBuying(item.id);
      const result = await shopService.buyItem(item.id, quantity);
      
      if (result.success) {
        alert(result.message);
        loadData(); // Recarregar dados
      } else {
        alert(result.message);
      }
    } catch (error: any) {
      alert(error.response?.data?.message || 'Erro ao comprar item');
    } finally {
      setBuying(null);
    }
  };

  const handleUseItem = async (item: ShopItem) => {
    try {
      setUsing(item.id);
      const result = await itemService.useItem(item.id, 1);
      
      if (result.success) {
        alert(result.message);
        loadData(); // Recarregar dados
      } else {
        alert(result.message);
      }
    } catch (error: any) {
      alert(error.response?.data?.message || 'Erro ao usar item');
    } finally {
      setUsing(null);
    }
  };

  const getRarityColor = (rarity: string) => {
    switch (rarity) {
      case 'Common': return 'text-gray-400';
      case 'Rare': return 'text-blue-400';
      case 'Epic': return 'text-purple-400';
      case 'Legendary': return 'text-yellow-400';
      default: return 'text-gray-400';
    }
  };

  const getRarityBg = (rarity: string) => {
    switch (rarity) {
      case 'Common': return 'bg-gray-800';
      case 'Rare': return 'bg-blue-900';
      case 'Epic': return 'bg-purple-900';
      case 'Legendary': return 'bg-yellow-900';
      default: return 'bg-gray-800';
    }
  };

  const getTypeIcon = (type: string) => {
    switch (type.toLowerCase()) {
      case 'weapon': return '⚔️';
      case 'armor': return '🛡️';
      case 'potion': return '🧪';
      case 'shield': return '🛡️';
      case 'staff': return '🔮';
      case 'bow': return '🏹';
      default: return '📦';
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-gray-900 via-blue-900 to-purple-900">
        <Navbar />
        <div className="container mx-auto px-4 py-8">
          <div className="flex justify-center items-center h-64">
            <div className="text-white text-xl">Carregando loja...</div>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-900 via-blue-900 to-purple-900">
      <Navbar />
      
      <div className="container mx-auto px-4 py-8">
        <div className="text-center mb-8">
          <h1 className="text-4xl font-bold text-white mb-4">🛒 Loja de Itens</h1>
          <p className="text-gray-300 text-lg">
            Bem-vindo à loja! Compre equipamentos e poções para fortalecer seu herói.
          </p>
          {user && (
            <div className="mt-4 text-yellow-400 text-xl font-bold">
              💰 {user.gold} Gold Disponível
            </div>
          )}
        </div>

        {/* Tabs */}
        <div className="flex justify-center mb-8">
          <div className="bg-gray-800 rounded-lg p-1 flex">
            <button
              onClick={() => setActiveTab('shop')}
              className={`px-6 py-2 rounded-md font-medium transition-colors ${
                activeTab === 'shop'
                  ? 'bg-blue-600 text-white'
                  : 'text-gray-400 hover:text-white'
              }`}
            >
              🛒 Loja
            </button>
            <button
              onClick={() => setActiveTab('inventory')}
              className={`px-6 py-2 rounded-md font-medium transition-colors ${
                activeTab === 'inventory'
                  ? 'bg-blue-600 text-white'
                  : 'text-gray-400 hover:text-white'
              }`}
            >
              🎒 Inventário
            </button>
          </div>
        </div>

        {activeTab === 'shop' ? (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {shopItems.map((item) => (
              <Card key={item.id}>
                <div className={`p-4 rounded-lg ${getRarityBg(item.rarity)}`}>
                  <div className="flex items-center justify-between mb-3">
                    <div className="flex items-center gap-2">
                      <span className="text-2xl">{getTypeIcon(item.type)}</span>
                      <h3 className={`text-lg font-bold ${getRarityColor(item.rarity)}`}>
                        {item.name}
                      </h3>
                    </div>
                    <span className={`px-2 py-1 rounded text-xs font-medium ${getRarityColor(item.rarity)}`}>
                      {item.rarity}
                    </span>
                  </div>

                  <p className="text-gray-300 text-sm mb-4">{item.description}</p>

                  {/* Bônus de Atributos */}
                  {(item.bonusStrength > 0 || item.bonusIntelligence > 0 || item.bonusDexterity > 0) && (
                    <div className="mb-4">
                      <p className="text-xs text-gray-400 mb-2">Bônus de Atributos:</p>
                      <div className="flex gap-2 text-xs">
                        {item.bonusStrength > 0 && (
                          <span className="bg-red-900 text-red-300 px-2 py-1 rounded">
                            +{item.bonusStrength} FOR
                          </span>
                        )}
                        {item.bonusIntelligence > 0 && (
                          <span className="bg-blue-900 text-blue-300 px-2 py-1 rounded">
                            +{item.bonusIntelligence} INT
                          </span>
                        )}
                        {item.bonusDexterity > 0 && (
                          <span className="bg-green-900 text-green-300 px-2 py-1 rounded">
                            +{item.bonusDexterity} DEX
                          </span>
                        )}
                      </div>
                    </div>
                  )}

                  {/* Bônus de XP para Poções */}
                  {item.isConsumable && item.percentageXpBonus && (
                    <div className="mb-4">
                      <span className="bg-purple-900 text-purple-300 px-2 py-1 rounded text-xs">
                        +{(item.percentageXpBonus * 100).toFixed(0)}% XP
                      </span>
                    </div>
                  )}

                  <div className="flex items-center justify-between">
                    <div className="text-yellow-400 font-bold text-lg">
                      💰 {item.value} Gold
                    </div>
                    <Button
                      onClick={() => handleBuyItem(item)}
                      disabled={buying === item.id || (user && user.gold < item.value)}
                      variant="success"
                      className="text-sm"
                    >
                      {buying === item.id ? 'Comprando...' : 'Comprar'}
                    </Button>
                  </div>
                </div>
              </Card>
            ))}
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {inventory.length === 0 ? (
              <Card>
                <div className="text-center py-8">
                  <div className="text-6xl mb-4">🎒</div>
                  <h3 className="text-xl font-bold text-gray-300 mb-2">Inventário Vazio</h3>
                  <p className="text-gray-400">Compre alguns itens na loja para começar!</p>
                </div>
              </Card>
            ) : (
              inventory.map((item) => (
                <Card key={item.id}>
                  <div className={`p-4 rounded-lg ${getRarityBg(item.rarity)}`}>
                    <div className="flex items-center justify-between mb-3">
                      <div className="flex items-center gap-2">
                        <span className="text-2xl">{getTypeIcon(item.type)}</span>
                        <h3 className={`text-lg font-bold ${getRarityColor(item.rarity)}`}>
                          {item.name}
                        </h3>
                      </div>
                      <div className="flex items-center gap-2">
                        {item.isEquipped && (
                          <span className="bg-green-900 text-green-300 px-2 py-1 rounded text-xs">
                            Equipado
                          </span>
                        )}
                        <span className="bg-gray-700 text-gray-300 px-2 py-1 rounded text-xs">
                          x{item.quantity}
                        </span>
                      </div>
                    </div>

                    <p className="text-gray-300 text-sm mb-4">{item.description}</p>

                    {/* Bônus de Atributos */}
                    {(item.bonusStrength > 0 || item.bonusIntelligence > 0 || item.bonusDexterity > 0) && (
                      <div className="mb-4">
                        <p className="text-xs text-gray-400 mb-2">Bônus de Atributos:</p>
                        <div className="flex gap-2 text-xs">
                          {item.bonusStrength > 0 && (
                            <span className="bg-red-900 text-red-300 px-2 py-1 rounded">
                              +{item.bonusStrength} FOR
                            </span>
                          )}
                          {item.bonusIntelligence > 0 && (
                            <span className="bg-blue-900 text-blue-300 px-2 py-1 rounded">
                              +{item.bonusIntelligence} INT
                            </span>
                          )}
                          {item.bonusDexterity > 0 && (
                            <span className="bg-green-900 text-green-300 px-2 py-1 rounded">
                              +{item.bonusDexterity} DEX
                            </span>
                          )}
                        </div>
                      </div>
                    )}

                    {/* Bônus de XP para Poções */}
                    {item.isConsumable && item.percentageXpBonus && (
                      <div className="mb-4">
                        <span className="bg-purple-900 text-purple-300 px-2 py-1 rounded text-xs">
                          +{(item.percentageXpBonus * 100).toFixed(0)}% XP
                        </span>
                      </div>
                    )}

                    <div className="flex items-center justify-between">
                      <div className="text-gray-400 text-sm">
                        Valor: {item.value} Gold
                      </div>
                      {item.isConsumable && (
                        <Button
                          onClick={() => handleUseItem(item)}
                          disabled={using === item.id}
                          variant="primary"
                          className="text-sm"
                        >
                          {using === item.id ? 'Usando...' : 'Usar'}
                        </Button>
                      )}
                    </div>
                  </div>
                </Card>
              ))
            )}
          </div>
        )}
      </div>
    </div>
  );
};
