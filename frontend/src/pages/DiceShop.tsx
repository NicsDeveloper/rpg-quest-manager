import React, { useEffect, useState } from 'react';
import { Navbar } from '../components/Navbar';
import { Card } from '../components/Card';
import { Button } from '../components/Button';
import { Loading } from '../components/Loading';
import { HeroWidget } from '../components/HeroWidget';
import { diceService, DiceInventory, DicePrices } from '../services/diceService';
import { profileService, MyHero } from '../services/profileService';

export const DiceShop: React.FC = () => {
  const [hero, setHero] = useState<MyHero | null>(null);
  const [inventory, setInventory] = useState<DiceInventory | null>(null);
  const [prices, setPrices] = useState<DicePrices | null>(null);
  const [loading, setLoading] = useState(true);
  const [purchasing, setPurchasing] = useState(false);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const heroData = await profileService.getMyHero();
      setHero(heroData);

      const [inventoryData, pricesData] = await Promise.all([
        diceService.getInventory(heroData.id),
        diceService.getPrices(),
      ]);

      setInventory(inventoryData);
      setPrices(pricesData);
    } catch (error) {
      console.error('Erro ao carregar dados:', error);
      alert('Erro ao carregar dados da loja');
    } finally {
      setLoading(false);
    }
  };

  const handlePurchase = async (diceType: string, price: number) => {
    if (!hero || !inventory) return;

    if (hero.gold < price) {
      alert(`Você não tem ouro suficiente! Necessário: ${price} ouro`);
      return;
    }

    const quantity = parseInt(prompt('Quantos dados deseja comprar?', '1') || '0');
    if (quantity <= 0) return;

    const totalPrice = price * quantity;
    if (hero.gold < totalPrice) {
      alert(`Você não tem ouro suficiente! Necessário: ${totalPrice} ouro`);
      return;
    }

    try {
      setPurchasing(true);
      const result = await diceService.purchaseDice({
        heroId: hero.id,
        diceType,
        quantity,
      });

      setInventory(result.updatedInventory);
      setHero({ ...hero, gold: result.remainingGold });
      alert(`✅ ${result.message}\n\n💰 Ouro restante: ${result.remainingGold}`);
    } catch (error: any) {
      alert(error.response?.data || 'Erro ao comprar dados');
    } finally {
      setPurchasing(false);
    }
  };

  if (loading) {
    return (
      <>
        <HeroWidget />
        <Navbar />
        <div className="container mx-auto px-6 py-8">
          <Loading />
        </div>
      </>
    );
  }

  if (!hero || !inventory || !prices) {
    return (
      <>
        <HeroWidget />
        <Navbar />
        <div className="container mx-auto px-6 py-8">
          <Card>
            <p className="text-red-400">Erro ao carregar dados do herói ou inventário.</p>
          </Card>
        </div>
      </>
    );
  }

  const diceOptions = [
    {
      type: 'D6',
      name: 'Dado D6',
      description: 'Dado de 6 lados - Para desafios fáceis',
      icon: '🎲',
      price: prices.d6,
      count: inventory.d6Count,
      gradient: 'from-green-600 to-green-800',
    },
    {
      type: 'D10',
      name: 'Dado D10',
      description: 'Dado de 10 lados - Para desafios médios',
      icon: '🎯',
      price: prices.d10,
      count: inventory.d10Count,
      gradient: 'from-blue-600 to-blue-800',
    },
    {
      type: 'D12',
      name: 'Dado D12',
      description: 'Dado de 12 lados - Para desafios difíceis',
      icon: '⚡',
      price: prices.d12,
      count: inventory.d12Count,
      gradient: 'from-purple-600 to-purple-800',
    },
    {
      type: 'D20',
      name: 'Dado D20',
      description: 'Dado de 20 lados - Para desafios épicos',
      icon: '👑',
      price: prices.d20,
      count: inventory.d20Count,
      gradient: 'from-yellow-600 to-yellow-800',
    },
  ];

  return (
    <>
      <HeroWidget />
      <Navbar />
      <div className="container mx-auto px-6 py-8">
        <div className="mb-8">
          <h1 className="text-4xl font-bold text-yellow-400 mb-2">🎲 Loja de Dados Virtuais</h1>
          <p className="text-gray-400">Compre dados para enfrentar desafios e derrotar bosses</p>
          <div className="mt-4 bg-gradient-to-r from-yellow-600/20 to-yellow-800/20 border border-yellow-500/30 rounded-lg p-4">
            <p className="text-yellow-300 font-bold text-xl">💰 Seu Ouro: {hero.gold}</p>
          </div>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          {diceOptions.map((dice) => (
            <Card key={dice.type} className="hover:scale-105 transition-all">
              <div className={`bg-gradient-to-br ${dice.gradient} rounded-t-lg p-6 -m-6 mb-4`}>
                <div className="text-6xl text-center mb-2">{dice.icon}</div>
                <h3 className="text-2xl font-bold text-white text-center">{dice.name}</h3>
              </div>

              <p className="text-gray-300 text-center mb-4">{dice.description}</p>

              <div className="bg-gray-800 rounded-lg p-4 mb-4 space-y-3">
                <div className="flex justify-between items-center">
                  <span className="text-gray-400">💰 Preço:</span>
                  <span className="text-yellow-400 font-bold text-xl">{dice.price} ouro</span>
                </div>
                <div className="border-t border-gray-700 pt-3">
                  <div className="flex justify-between items-center">
                    <span className="text-gray-400">📦 Você tem:</span>
                    <span className={`font-bold text-2xl ${dice.count > 0 ? 'text-green-400 animate-pulse' : 'text-red-400'}`}>
                      {dice.count}x
                    </span>
                  </div>
                  {dice.count === 0 && (
                    <p className="text-xs text-red-400 text-center mt-1">⚠️ Sem dados deste tipo!</p>
                  )}
                  {dice.count > 0 && dice.count < 3 && (
                    <p className="text-xs text-yellow-400 text-center mt-1">⚡ Estoque baixo!</p>
                  )}
                  {dice.count >= 3 && (
                    <p className="text-xs text-green-400 text-center mt-1">✓ Bem abastecido</p>
                  )}
                </div>
              </div>

              <Button
                variant="primary"
                onClick={() => handlePurchase(dice.type, dice.price)}
                disabled={purchasing || hero.gold < dice.price}
                className="w-full"
              >
                {purchasing ? 'Comprando...' : hero.gold < dice.price ? 'Ouro Insuficiente' : 'Comprar'}
              </Button>
            </Card>
          ))}
        </div>

        <div className="mt-8">
          <Card className="bg-gradient-to-r from-blue-900/30 to-purple-900/30 border-blue-500/30">
            <h3 className="text-2xl font-bold text-blue-400 mb-4">ℹ️ Como Funciona</h3>
            <ul className="space-y-2 text-gray-300">
              <li className="flex items-start">
                <span className="text-green-400 mr-2">✓</span>
                <span>Cada tipo de dado é necessário para diferentes tipos de inimigos</span>
              </li>
              <li className="flex items-start">
                <span className="text-green-400 mr-2">✓</span>
                <span>Dados D6 são para inimigos fracos, D20 para bosses épicos</span>
              </li>
              <li className="flex items-start">
                <span className="text-green-400 mr-2">✓</span>
                <span>Você precisa rolar um valor mínimo para vencer cada inimigo</span>
              </li>
              <li className="flex items-start">
                <span className="text-green-400 mr-2">✓</span>
                <span>Bosses dropam itens raros e lendários ao serem derrotados!</span>
              </li>
              <li className="flex items-start">
                <span className="text-yellow-400 mr-2">⚠</span>
                <span>Dados são consumidos ao serem usados - compre com sabedoria!</span>
              </li>
            </ul>
          </Card>
        </div>
      </div>
    </>
  );
};

