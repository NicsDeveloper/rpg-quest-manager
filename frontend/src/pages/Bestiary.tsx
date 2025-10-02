import React, { useEffect, useState } from 'react';
import api from '../services/api';

interface Discovery {
  id: number;
  enemyId: number;
  enemyName: string;
  comboId: number;
  comboName: string;
  comboDescription: string;
  comboIcon: string;
  discoveredAt: string;
  timesUsed: number;
  timesWon: number;
}

interface Weakness {
  comboName: string;
  comboDescription: string;
  comboIcon: string;
  rollReduction: number;
  dropMultiplier: number;
  expMultiplier: number;
  flavorText: string;
}

interface Boss {
  id: number;
  name: string;
  type: string;
  power: number;
  health: number;
  requiredDiceType: string;
  minimumRoll: number;
  discoveredWeaknesses: Weakness[];
  hasDiscoveries: boolean;
  totalWeaknesses: number;
  discoveredCount: number;
}

interface Combo {
  id: number;
  name: string;
  requiredClass1: string;
  requiredClass2: string;
  requiredClass3?: string;
  description: string;
  effect: string;
  icon: string;
}

export const Bestiary: React.FC = () => {
  const [discoveries, setDiscoveries] = useState<Discovery[]>([]);
  const [bosses, setBosses] = useState<Boss[]>([]);
  const [combos, setCombos] = useState<Combo[]>([]);
  const [loading, setLoading] = useState(true);
  const [activeTab, setActiveTab] = useState<'bosses' | 'combos' | 'discoveries'>('bosses');

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      setLoading(true);
      const [discoveriesRes, bossesRes, combosRes] = await Promise.all([
        api.get('/bestiary/discoveries'),
        api.get('/bestiary/bosses'),
        api.get('/bestiary/combos')
      ]);
      setDiscoveries(discoveriesRes.data);
      setBosses(bossesRes.data);
      setCombos(combosRes.data);
    } catch (err) {
      console.error('Erro ao carregar bestiário:', err);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-screen">
        <div className="text-xl">Carregando bestiário...</div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8 max-w-7xl">
      <h1 className="text-4xl font-bold mb-8 flex items-center gap-2">
        📖 Bestiário
      </h1>

      {/* Tabs */}
      <div className="flex gap-2 mb-8">
        <button
          onClick={() => setActiveTab('bosses')}
          className={`px-6 py-3 rounded-lg font-semibold transition ${
            activeTab === 'bosses'
              ? 'bg-purple-600 text-white'
              : 'bg-gray-800 text-gray-400 hover:bg-gray-700'
          }`}
        >
          🐉 Bosses ({bosses.length})
        </button>
        <button
          onClick={() => setActiveTab('combos')}
          className={`px-6 py-3 rounded-lg font-semibold transition ${
            activeTab === 'combos'
              ? 'bg-purple-600 text-white'
              : 'bg-gray-800 text-gray-400 hover:bg-gray-700'
          }`}
        >
          ✨ Combos ({combos.length})
        </button>
        <button
          onClick={() => setActiveTab('discoveries')}
          className={`px-6 py-3 rounded-lg font-semibold transition ${
            activeTab === 'discoveries'
              ? 'bg-purple-600 text-white'
              : 'bg-gray-800 text-gray-400 hover:bg-gray-700'
          }`}
        >
          🔍 Descobertas ({discoveries.length})
        </button>
      </div>

      {/* Bosses Tab */}
      {activeTab === 'bosses' && (
        <div className="space-y-6">
          {bosses.map((boss) => (
            <div
              key={boss.id}
              className={`rounded-lg p-6 border-2 ${
                boss.hasDiscoveries
                  ? 'bg-gradient-to-br from-purple-900/30 to-purple-800/20 border-purple-600'
                  : 'bg-gray-800/50 border-gray-700'
              }`}
            >
              <div className="flex justify-between items-start mb-4">
                <div>
                  <h3 className="text-2xl font-bold">{boss.name}</h3>
                  <p className="text-gray-400">{boss.type}</p>
                </div>
                <div className="text-right">
                  <div className="bg-red-600 text-white px-4 py-2 rounded-lg font-bold">
                    🎲 {boss.requiredDiceType}
                  </div>
                  <div className="text-sm text-gray-400 mt-1">
                    Mínimo: {boss.minimumRoll}
                  </div>
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4 mb-4">
                <div className="bg-gray-900/50 rounded-lg p-3">
                  <div className="text-gray-400 text-sm">⚔️ Poder</div>
                  <div className="text-xl font-bold">{boss.power}</div>
                </div>
                <div className="bg-gray-900/50 rounded-lg p-3">
                  <div className="text-gray-400 text-sm">❤️ Vida</div>
                  <div className="text-xl font-bold">{boss.health}</div>
                </div>
              </div>

              {boss.hasDiscoveries && (
                <div className="mt-6">
                  <h4 className="text-lg font-bold mb-3 text-purple-300">
                    🔓 Fraquezas Descobertas ({boss.discoveredCount}/{boss.totalWeaknesses})
                  </h4>
                  <div className="space-y-3">
                    {boss.discoveredWeaknesses.map((weakness, idx) => (
                      <div
                        key={idx}
                        className="bg-purple-900/30 border border-purple-600 rounded-lg p-4"
                      >
                        <div className="flex items-center gap-2 mb-2">
                          <span className="text-2xl">{weakness.comboIcon}</span>
                          <span className="font-bold text-purple-300">{weakness.comboName}</span>
                        </div>
                        <p className="text-sm text-gray-300 mb-2">{weakness.comboDescription}</p>
                        <p className="text-sm italic text-purple-200 mb-3">"{weakness.flavorText}"</p>
                        <div className="flex gap-3 text-sm">
                          <span className="bg-green-900/50 text-green-300 px-3 py-1 rounded">
                            📉 Roll -{weakness.rollReduction}
                          </span>
                          {weakness.dropMultiplier > 1 && (
                            <span className="bg-yellow-900/50 text-yellow-300 px-3 py-1 rounded">
                              💎 Drop +{((weakness.dropMultiplier - 1) * 100).toFixed(0)}%
                            </span>
                          )}
                          {weakness.expMultiplier > 1 && (
                            <span className="bg-blue-900/50 text-blue-300 px-3 py-1 rounded">
                              ⭐ XP +{((weakness.expMultiplier - 1) * 100).toFixed(0)}%
                            </span>
                          )}
                        </div>
                      </div>
                    ))}
                  </div>
                </div>
              )}

              {!boss.hasDiscoveries && (
                <div className="mt-4 bg-gray-900/50 border border-gray-700 rounded-lg p-4 text-center text-gray-400">
                  🔒 Nenhuma fraqueza descoberta ainda. Experimente diferentes combos de heróis para descobrir!
                </div>
              )}
            </div>
          ))}
        </div>
      )}

      {/* Combos Tab */}
      {activeTab === 'combos' && (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          {combos.map((combo) => (
            <div
              key={combo.id}
              className="bg-gradient-to-br from-purple-900/30 to-purple-800/20 border-2 border-purple-600 rounded-lg p-6"
            >
              <div className="flex items-center gap-3 mb-4">
                <span className="text-4xl">{combo.icon}</span>
                <div>
                  <h3 className="text-xl font-bold">{combo.name}</h3>
                  <p className="text-sm text-gray-400">
                    {combo.requiredClass1} + {combo.requiredClass2}
                    {combo.requiredClass3 && ` + ${combo.requiredClass3}`}
                  </p>
                </div>
              </div>

              <p className="text-gray-300 mb-3">{combo.description}</p>
              <div className="bg-purple-900/40 border border-purple-700 rounded-lg p-3">
                <div className="text-sm text-purple-300 font-semibold mb-1">✨ Efeito:</div>
                <p className="text-sm">{combo.effect}</p>
              </div>
            </div>
          ))}
        </div>
      )}

      {/* Discoveries Tab */}
      {activeTab === 'discoveries' && (
        <div>
          {discoveries.length === 0 ? (
            <div className="bg-gray-800/50 rounded-lg p-8 text-center text-gray-400">
              🔒 Você ainda não descobriu nenhuma fraqueza de boss.
              <br />
              Experimente diferentes combinações de heróis em combate!
            </div>
          ) : (
            <div className="space-y-4">
              {discoveries.map((discovery) => (
                <div
                  key={discovery.id}
                  className="bg-gradient-to-br from-green-900/30 to-green-800/20 border-2 border-green-600 rounded-lg p-6"
                >
                  <div className="flex justify-between items-start mb-4">
                    <div className="flex items-center gap-3">
                      <span className="text-3xl">{discovery.comboIcon}</span>
                      <div>
                        <h3 className="text-xl font-bold text-green-300">{discovery.comboName}</h3>
                        <p className="text-gray-400">vs {discovery.enemyName}</p>
                      </div>
                    </div>
                    <div className="text-right text-sm">
                      <div className="text-gray-400">
                        Descoberto em{' '}
                        {new Date(discovery.discoveredAt).toLocaleDateString('pt-BR')}
                      </div>
                      <div className="text-green-400 font-semibold">
                        Usado {discovery.timesUsed}x | Vencido {discovery.timesWon}x
                      </div>
                    </div>
                  </div>

                  <p className="text-gray-300">{discovery.comboDescription}</p>
                </div>
              ))}
            </div>
          )}
        </div>
      )}
    </div>
  );
};

