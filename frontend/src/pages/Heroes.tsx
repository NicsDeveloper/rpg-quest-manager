import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';
import { Navbar } from '../components/Navbar';
import { HeroWidget } from '../components/HeroWidget';

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
  const navigate = useNavigate();
  const [heroes, setHeroes] = useState<Hero[]>([]);
  const [activeParty, setActiveParty] = useState<Hero[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [newHero, setNewHero] = useState({
    name: '',
    class: 'Guerreiro'
  });
  
  // Atributos base por classe (apenas para exibição)
  const getClassAttributes = (heroClass: string) => {
    switch (heroClass) {
      case 'Guerreiro': return { strength: 18, intelligence: 12, dexterity: 14 };
      case 'Mago': return { strength: 12, intelligence: 18, dexterity: 14 };
      case 'Arqueiro': return { strength: 14, intelligence: 14, dexterity: 16 };
      case 'Paladino': return { strength: 16, intelligence: 14, dexterity: 14 };
      case 'Ladrão': return { strength: 13, intelligence: 13, dexterity: 18 };
      case 'Clérigo': return { strength: 14, intelligence: 16, dexterity: 14 };
      case 'Bárbaro': return { strength: 20, intelligence: 11, dexterity: 13 };
      case 'Bruxo': return { strength: 11, intelligence: 19, dexterity: 14 };
      case 'Druida': return { strength: 13, intelligence: 15, dexterity: 16 };
      case 'Monge': return { strength: 14, intelligence: 14, dexterity: 16 };
      default: return { strength: 13, intelligence: 13, dexterity: 13 };
    }
  };
  
  const classAttributes = getClassAttributes(newHero.class);
  const canCreate = newHero.name.trim().length > 0;

  // Listas para geração aleatória
  const heroNames = [
    'Aragorn', 'Legolas', 'Gimli', 'Gandalf', 'Frodo', 'Samwise', 'Merry', 'Pippin',
    'Boromir', 'Faramir', 'Eowyn', 'Arwen', 'Galadriel', 'Elrond', 'Thranduil',
    'Thorin', 'Bilbo', 'Smaug', 'Saruman', 'Gollum', 'Sauron', 'Nazgul',
    'Conan', 'Kull', 'Red Sonja', 'Thulsa Doom', 'Valeria', 'Subotai',
    'Drizzt', 'Bruenor', 'Wulfgar', 'Catti-brie', 'Regis', 'Artemis',
    'Raistlin', 'Caramon', 'Tanis', 'Sturm', 'Flint', 'Tasslehoff',
    'Kvothe', 'Denna', 'Auri', 'Elodin', 'Simmon', 'Wilem',
    'Geralt', 'Yennefer', 'Triss', 'Ciri', 'Dandelion', 'Zoltan',
    'Kaladin', 'Shallan', 'Dalinar', 'Adolin', 'Jasnah', 'Navani',
    'Vin', 'Kelsier', 'Elend', 'Sazed', 'Breeze', 'Ham',
    'Rand', 'Mat', 'Perrin', 'Egwene', 'Nynaeve', 'Moiraine',
    'Tyrion', 'Jon', 'Daenerys', 'Arya', 'Sansa', 'Bran',
    'Kvothe', 'Denna', 'Auri', 'Elodin', 'Simmon', 'Wilem'
  ];

  const heroClasses = [
    'Guerreiro', 'Mago', 'Arqueiro', 'Paladino', 'Ladrão', 
    'Clérigo', 'Bárbaro', 'Bruxo', 'Druida', 'Monge'
  ];

  // Funções para gerar aleatoriamente
  const generateRandomName = () => {
    const randomName = heroNames[Math.floor(Math.random() * heroNames.length)];
    setNewHero({ ...newHero, name: randomName });
  };

  const generateRandomClass = () => {
    const randomClass = heroClasses[Math.floor(Math.random() * heroClasses.length)];
    setNewHero({ ...newHero, class: randomClass });
  };

  const generateRandomHero = () => {
    const randomName = heroNames[Math.floor(Math.random() * heroNames.length)];
    const randomClass = heroClasses[Math.floor(Math.random() * heroClasses.length)];
    setNewHero({ name: randomName, class: randomClass });
  };

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
        class: 'Guerreiro'
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
      <>
        <HeroWidget />
        <Navbar />
        <div className="flex items-center justify-center h-screen">
          <div className="text-xl">Carregando...</div>
        </div>
      </>
    );
  }

  return (
    <>
      <HeroWidget />
      <Navbar />
      <div className="container mx-auto px-4 py-8 max-w-7xl">
        <div className="flex justify-between items-center mb-8">
          <div>
            <button
              onClick={() => navigate(-1)}
              className="mb-4 text-gray-400 hover:text-white flex items-center gap-2 transition"
            >
              ← Voltar
            </button>
            <h1 className="text-4xl font-bold">Meus Heróis</h1>
          </div>
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
                    <span className="text-gray-400">💰 Ouro:</span>
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
                    <span className="text-gray-400">💰 Ouro:</span>
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
                <div className="flex justify-between items-center mb-2">
                  <label className="block text-sm font-semibold">Nome do Herói</label>
                  <button
                    type="button"
                    onClick={generateRandomName}
                    className="px-3 py-1 bg-blue-600 hover:bg-blue-700 text-white text-xs rounded-lg transition flex items-center gap-1"
                  >
                    🎲 Nome Aleatório
                  </button>
                </div>
                <input
                  type="text"
                  value={newHero.name}
                  onChange={(e) => setNewHero({ ...newHero, name: e.target.value })}
                  className="w-full px-4 py-2 rounded-lg bg-gray-800 border border-gray-700 focus:border-purple-600 focus:outline-none"
                  placeholder="Digite o nome do herói"
                  required
                />
              </div>

              <div>
                <div className="flex justify-between items-center mb-2">
                  <label className="block text-sm font-semibold">Classe</label>
                  <button
                    type="button"
                    onClick={generateRandomClass}
                    className="px-3 py-1 bg-green-600 hover:bg-green-700 text-white text-xs rounded-lg transition flex items-center gap-1"
                  >
                    🎲 Classe Aleatória
                  </button>
                </div>
                <select
                  value={newHero.class}
                  onChange={(e) => setNewHero({ ...newHero, class: e.target.value })}
                  className="w-full px-4 py-2 rounded-lg bg-gray-800 border border-gray-700 focus:border-purple-600 focus:outline-none"
                >
                  <option value="Guerreiro">⚔️ Guerreiro - Força +8, Inteligência +2, Destreza +4</option>
                  <option value="Mago">🔮 Mago - Força +2, Inteligência +8, Destreza +4</option>
                  <option value="Arqueiro">🏹 Arqueiro - Força +4, Inteligência +4, Destreza +6</option>
                  <option value="Paladino">🛡️ Paladino - Força +6, Inteligência +4, Destreza +4</option>
                  <option value="Ladrão">🗡️ Ladrão - Força +3, Inteligência +3, Destreza +8</option>
                  <option value="Clérigo">⛪ Clérigo - Força +4, Inteligência +6, Destreza +4</option>
                  <option value="Bárbaro">🪓 Bárbaro - Força +10, Inteligência +1, Destreza +3</option>
                  <option value="Bruxo">🔮 Bruxo - Força +1, Inteligência +9, Destreza +4</option>
                  <option value="Druida">🌿 Druida - Força +3, Inteligência +5, Destreza +6</option>
                  <option value="Monge">🥋 Monge - Força +4, Inteligência +4, Destreza +6</option>
                </select>
              </div>

              {/* Botão para Herói Completamente Aleatório */}
              <div className="flex justify-center">
                <button
                  type="button"
                  onClick={generateRandomHero}
                  className="px-6 py-3 bg-gradient-to-r from-purple-600 to-pink-600 hover:from-purple-700 hover:to-pink-700 text-white font-bold rounded-lg transition flex items-center gap-2 shadow-lg"
                >
                  🎲✨ Criar Herói Aleatório ✨🎲
                </button>
              </div>

              {/* Atributos Base da Classe */}
              <div className="bg-gradient-to-r from-amber-900/30 to-orange-900/30 rounded-lg p-4 border border-amber-700/30 mb-4">
                <div className="flex justify-between items-center mb-2">
                  <span className="text-sm font-semibold text-amber-400">⭐ Atributos Base da Classe</span>
                  <span className="text-amber-400 text-sm">🔒 Fixos</span>
                </div>
                <p className="text-xs text-gray-400">
                  Os atributos base são definidos automaticamente pela classe escolhida. 
                  Você poderá distribuir pontos adicionais ao subir de nível na página "Atributos".
                </p>
              </div>

              {/* Attribute Sliders - BLOQUEADOS */}
              <div className="space-y-4">
                {/* Strength */}
                <div className="bg-gray-800/50 rounded-lg p-4">
                  <div className="flex justify-between items-center mb-2">
                    <label className="text-sm font-semibold">💪 Força</label>
                    <div className="flex items-center gap-2">
                      <span className="w-12 text-center font-bold text-xl text-amber-400">{classAttributes.strength}</span>
                      <span className="text-xs text-gray-500">🔒</span>
                    </div>
                  </div>
                  <input
                    type="range"
                    min="1"
                    max="25"
                    value={classAttributes.strength}
                    disabled
                    className="w-full accent-red-600 opacity-50 cursor-not-allowed"
                  />
                  <p className="text-xs text-gray-500 mt-1">Dano físico e resistência</p>
                </div>

                {/* Intelligence */}
                <div className="bg-gray-800/50 rounded-lg p-4">
                  <div className="flex justify-between items-center mb-2">
                    <label className="text-sm font-semibold">🧠 Inteligência</label>
                    <div className="flex items-center gap-2">
                      <span className="w-12 text-center font-bold text-xl text-amber-400">{classAttributes.intelligence}</span>
                      <span className="text-xs text-gray-500">🔒</span>
                    </div>
                  </div>
                  <input
                    type="range"
                    min="1"
                    max="25"
                    value={classAttributes.intelligence}
                    disabled
                    className="w-full accent-blue-600 opacity-50 cursor-not-allowed"
                  />
                  <p className="text-xs text-gray-500 mt-1">Poder mágico e descobertas</p>
                </div>

                {/* Dexterity */}
                <div className="bg-gray-800/50 rounded-lg p-4">
                  <div className="flex justify-between items-center mb-2">
                    <label className="text-sm font-semibold">🎯 Destreza</label>
                    <div className="flex items-center gap-2">
                      <span className="w-12 text-center font-bold text-xl text-amber-400">{classAttributes.dexterity}</span>
                      <span className="text-xs text-gray-500">🔒</span>
                    </div>
                  </div>
                  <input
                    type="range"
                    min="1"
                    max="25"
                    value={classAttributes.dexterity}
                    disabled
                    className="w-full accent-green-600 opacity-50 cursor-not-allowed"
                  />
                  <p className="text-xs text-gray-500 mt-1">Precisão e esquiva</p>
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
                  disabled={!canCreate}
                  className={`flex-1 py-3 rounded-lg transition font-bold ${
                    !canCreate
                      ? 'bg-gray-600 text-gray-400 cursor-not-allowed'
                      : 'bg-purple-600 hover:bg-purple-700 text-white'
                  }`}
                >
                  {!canCreate ? '⚠️ Digite um nome' : '✨ Criar Herói'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
      </div>
    </>
  );
};
