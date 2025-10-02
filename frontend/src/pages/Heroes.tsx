import React, { useEffect, useState } from 'react';
import api from '../services/api';

interface Hero {
  id: number;
  name: string;
  class: string;
  level: number;
  experience: number;
  strength: number;
  intelligence: number;
  dexterity: number;
  gold: number;
  createdAt: string;
}

export const Heroes: React.FC = () => {
  const [heroes, setHeroes] = useState<Hero[]>([]);
  const [activeParty, setActiveParty] = useState<Hero[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [newHero, setNewHero] = useState({
    name: '',
    class: 'Guerreiro',
    strength: 15,
    intelligence: 10,
    dexterity: 10
  });

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      setLoading(true);
      const [heroesRes, partyRes] = await Promise.all([
        api.get('/profile/my-heroes'),
        api.get('/profile/active-party')
      ]);
      setHeroes(heroesRes.data);
      setActiveParty(partyRes.data);
    } catch (err: any) {
      console.error('Erro ao carregar dados:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleAddToParty = async (heroId: number) => {
    try {
      setError('');
      setSuccess('');
      await api.post(`/profile/add-to-party/${heroId}`);
      setSuccess('Herói adicionado à party!');
      fetchData();
    } catch (err: any) {
      setError(err.response?.data?.message || err.response?.data || 'Erro ao adicionar à party');
    }
  };

  const handleRemoveFromParty = async (heroId: number) => {
    try {
      setError('');
      setSuccess('');
      await api.post(`/profile/remove-from-party/${heroId}`);
      setSuccess('Herói removido da party!');
      fetchData();
    } catch (err: any) {
      setError(err.response?.data?.message || err.response?.data || 'Erro ao remover da party');
    }
  };

  const handleCreateHero = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      setError('');
      setSuccess('');
      await api.post('/profile/create-hero', newHero);
      setSuccess('Herói criado com sucesso!');
      setShowCreateModal(false);
      setNewHero({
        name: '',
        class: 'Guerreiro',
        strength: 15,
        intelligence: 10,
        dexterity: 10
      });
      fetchData();
    } catch (err: any) {
      setError(err.response?.data?.message || err.response?.data || 'Erro ao criar herói');
    }
  };

  const isInParty = (heroId: number) => {
    return activeParty.some(h => h.id === heroId);
  };

  const canAddToParty = activeParty.length < 3;

  if (loading) {
    return (
      <div className="flex items-center justify-center h-screen">
        <div className="text-xl">Carregando...</div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8 max-w-7xl">
      <div className="flex justify-between items-center mb-8">
        <h1 className="text-4xl font-bold">Meus Heróis</h1>
        <button
          onClick={() => setShowCreateModal(true)}
          className="bg-green-600 hover:bg-green-700 text-white px-6 py-3 rounded-lg font-semibold transition"
        >
          ➕ Criar Novo Herói
        </button>
      </div>

      {error && (
        <div className="bg-red-500/20 border border-red-500 text-red-200 px-4 py-3 rounded-lg mb-6">
          {error}
        </div>
      )}

      {success && (
        <div className="bg-green-500/20 border border-green-500 text-green-200 px-4 py-3 rounded-lg mb-6">
          {success}
        </div>
      )}

      {/* Active Party */}
      <div className="mb-12">
        <h2 className="text-2xl font-bold mb-4 flex items-center gap-2">
          ⚔️ Party Ativa ({activeParty.length}/3)
        </h2>
        {activeParty.length === 0 ? (
          <div className="bg-gray-800/50 rounded-lg p-8 text-center text-gray-400">
            Nenhum herói na party. Adicione heróis abaixo!
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            {activeParty.map((hero) => (
              <div
                key={hero.id}
                className="bg-gradient-to-br from-yellow-900/30 to-yellow-800/20 border-2 border-yellow-600 rounded-lg p-6"
              >
                <div className="flex justify-between items-start mb-4">
                  <div>
                    <h3 className="text-xl font-bold text-yellow-300">{hero.name}</h3>
                    <p className="text-yellow-500">{hero.class}</p>
                  </div>
                  <span className="bg-yellow-600 text-white px-3 py-1 rounded-full text-sm font-bold">
                    Nv. {hero.level}
                  </span>
                </div>

                <div className="space-y-2 text-sm mb-4">
                  <div className="flex justify-between">
                    <span className="text-gray-400">💪 Força:</span>
                    <span className="font-bold text-red-400">{hero.strength}</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-gray-400">🧠 Inteligência:</span>
                    <span className="font-bold text-blue-400">{hero.intelligence}</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-gray-400">🎯 Destreza:</span>
                    <span className="font-bold text-green-400">{hero.dexterity}</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-gray-400">🪙 Ouro:</span>
                    <span className="font-bold text-yellow-400">{hero.gold}</span>
                  </div>
                </div>

                <button
                  onClick={() => handleRemoveFromParty(hero.id)}
                  className="w-full bg-red-600 hover:bg-red-700 text-white py-2 rounded-lg transition"
                >
                  ❌ Remover da Party
                </button>
              </div>
            ))}
          </div>
        )}
      </div>

      {/* All Heroes */}
      <div>
        <h2 className="text-2xl font-bold mb-4">📋 Todos os Heróis ({heroes.length})</h2>
        {heroes.length === 0 ? (
          <div className="bg-gray-800/50 rounded-lg p-8 text-center text-gray-400">
            Você ainda não tem heróis. Crie um novo herói para começar!
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            {heroes.map((hero) => (
              <div
                key={hero.id}
                className={`rounded-lg p-6 border-2 ${
                  isInParty(hero.id)
                    ? 'bg-gradient-to-br from-yellow-900/20 to-yellow-800/10 border-yellow-600'
                    : 'bg-gray-800/50 border-gray-700'
                }`}
              >
                <div className="flex justify-between items-start mb-4">
                  <div>
                    <h3 className="text-xl font-bold">{hero.name}</h3>
                    <p className="text-gray-400">{hero.class}</p>
                  </div>
                  <span className="bg-purple-600 text-white px-3 py-1 rounded-full text-sm font-bold">
                    Nv. {hero.level}
                  </span>
                </div>

                <div className="space-y-2 text-sm mb-4">
                  <div className="flex justify-between">
                    <span className="text-gray-400">💪 Força:</span>
                    <span className="font-bold">{hero.strength}</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-gray-400">🧠 Inteligência:</span>
                    <span className="font-bold">{hero.intelligence}</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-gray-400">🎯 Destreza:</span>
                    <span className="font-bold">{hero.dexterity}</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-gray-400">🪙 Ouro:</span>
                    <span className="font-bold">{hero.gold}</span>
                  </div>
                </div>

                {isInParty(hero.id) ? (
                  <button
                    onClick={() => handleRemoveFromParty(hero.id)}
                    className="w-full bg-red-600 hover:bg-red-700 text-white py-2 rounded-lg transition"
                  >
                    ❌ Remover
                  </button>
                ) : (
                  <button
                    onClick={() => handleAddToParty(hero.id)}
                    disabled={!canAddToParty}
                    className={`w-full py-2 rounded-lg transition ${
                      canAddToParty
                        ? 'bg-green-600 hover:bg-green-700 text-white'
                        : 'bg-gray-600 text-gray-400 cursor-not-allowed'
                    }`}
                  >
                    {canAddToParty ? '✅ Adicionar à Party' : '⚠️ Party Cheia'}
                  </button>
                )}
              </div>
            ))}
          </div>
        )}
      </div>

      {/* Create Hero Modal */}
      {showCreateModal && (
        <div className="fixed inset-0 bg-black/80 flex items-center justify-center z-50 p-4">
          <div className="bg-gray-900 rounded-lg p-8 max-w-md w-full border-2 border-purple-600">
            <h2 className="text-2xl font-bold mb-6">➕ Criar Novo Herói</h2>

            {error && (
              <div className="bg-red-500/20 border border-red-500 text-red-200 px-4 py-3 rounded-lg mb-4">
                {error}
              </div>
            )}

            <form onSubmit={handleCreateHero} className="space-y-4">
              <div>
                <label className="block text-sm font-semibold mb-2">Nome do Herói</label>
                <input
                  type="text"
                  value={newHero.name}
                  onChange={(e) => setNewHero({ ...newHero, name: e.target.value })}
                  className="w-full px-4 py-2 rounded-lg bg-gray-800 border border-gray-700 focus:border-purple-600 focus:outline-none"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-semibold mb-2">Classe</label>
                <select
                  value={newHero.class}
                  onChange={(e) => setNewHero({ ...newHero, class: e.target.value })}
                  className="w-full px-4 py-2 rounded-lg bg-gray-800 border border-gray-700 focus:border-purple-600 focus:outline-none"
                >
                  <option value="Guerreiro">⚔️ Guerreiro</option>
                  <option value="Mago">🔮 Mago</option>
                  <option value="Arqueiro">🏹 Arqueiro</option>
                  <option value="Paladino">🛡️ Paladino</option>
                  <option value="Assassino">🗡️ Assassino</option>
                </select>
              </div>

              <div className="grid grid-cols-3 gap-4">
                <div>
                  <label className="block text-sm font-semibold mb-2">💪 Força</label>
                  <input
                    type="number"
                    min="5"
                    max="20"
                    value={newHero.strength}
                    onChange={(e) => setNewHero({ ...newHero, strength: parseInt(e.target.value) })}
                    className="w-full px-4 py-2 rounded-lg bg-gray-800 border border-gray-700 focus:border-purple-600 focus:outline-none"
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-semibold mb-2">🧠 Int</label>
                  <input
                    type="number"
                    min="5"
                    max="20"
                    value={newHero.intelligence}
                    onChange={(e) => setNewHero({ ...newHero, intelligence: parseInt(e.target.value) })}
                    className="w-full px-4 py-2 rounded-lg bg-gray-800 border border-gray-700 focus:border-purple-600 focus:outline-none"
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-semibold mb-2">🎯 Dex</label>
                  <input
                    type="number"
                    min="5"
                    max="20"
                    value={newHero.dexterity}
                    onChange={(e) => setNewHero({ ...newHero, dexterity: parseInt(e.target.value) })}
                    className="w-full px-4 py-2 rounded-lg bg-gray-800 border border-gray-700 focus:border-purple-600 focus:outline-none"
                    required
                  />
                </div>
              </div>

              <div className="flex gap-4 pt-4">
                <button
                  type="button"
                  onClick={() => setShowCreateModal(false)}
                  className="flex-1 bg-gray-700 hover:bg-gray-600 text-white py-3 rounded-lg transition"
                >
                  Cancelar
                </button>
                <button
                  type="submit"
                  className="flex-1 bg-purple-600 hover:bg-purple-700 text-white py-3 rounded-lg transition font-bold"
                >
                  Criar Herói
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};
