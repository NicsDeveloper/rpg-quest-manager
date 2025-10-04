import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { heroService, type Hero } from '../services/heroService';
import { 
  Plus, 
  Users, 
  UserPlus, 
  UserMinus, 
  Crown,
  Sword,
  Shield,
  Zap,
  Calendar,
  ArrowLeft,
  Star,
  Heart,
  Coins,
  Sparkles,
  X
} from 'lucide-react';

export const Heroes = () => {
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
    return heroService.getClassAttributes(heroClass);
  };
  
  const classAttributes = getClassAttributes(newHero.class);
  const canCreate = newHero.name.trim().length > 0;

  // Listas para geração aleatória
  const heroNames = [
    'Aragorn', 'Legolas', 'Gimli', 'Gandalf', 'Frodo', 'Sam', 'Boromir', 'Merry', 'Pippin',
    'Conan', 'Red Sonja', 'Elric', 'Fafhrd', 'Gray Mouser', 'Tyrion', 'Daenerys', 'Jon Snow',
    'Arya', 'Sansa', 'Bran', 'Jaime', 'Cersei', 'Tyrion', 'Sandor', 'Brienne', 'Bronn'
  ];

  const heroClasses = heroService.getAvailableClasses();

  const generateRandomName = () => {
    const randomName = heroNames[Math.floor(Math.random() * heroNames.length)];
    setNewHero(prev => ({ ...prev, name: randomName }));
  };

  const generateRandomClass = () => {
    const randomClass = heroClasses[Math.floor(Math.random() * heroClasses.length)];
    setNewHero(prev => ({ ...prev, class: randomClass }));
  };

  useEffect(() => {
    loadHeroes();
  }, []);

  const loadHeroes = async () => {
    try {
      setLoading(true);
      const [heroesData, partyData] = await Promise.all([
        heroService.getMyHeroes(),
        heroService.getActiveParty()
      ]);
      setHeroes(heroesData);
      setActiveParty(partyData);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Erro ao carregar heróis');
    } finally {
      setLoading(false);
    }
  };

  const handleCreateHero = async () => {
    if (!canCreate) return;

    try {
      await heroService.createHero(newHero);
      setShowCreateModal(false);
      setNewHero({ name: '', class: 'Guerreiro' });
      setSuccess('Herói criado com sucesso!');
      await loadHeroes();
    } catch (err: any) {
      setError(err.response?.data?.message || 'Erro ao criar herói');
    }
  };

  const handleAddToParty = async (heroId: number) => {
    try {
      await heroService.addToParty(heroId);
      setSuccess('Herói adicionado à party!');
      await loadHeroes();
    } catch (err: any) {
      setError(err.response?.data?.message || 'Erro ao adicionar à party');
    }
  };

  const handleRemoveFromParty = async (heroId: number) => {
    try {
      await heroService.removeFromParty(heroId);
      setSuccess('Herói removido da party!');
      await loadHeroes();
    } catch (err: any) {
      setError(err.response?.data?.message || 'Erro ao remover da party');
    }
  };

  const isInParty = (heroId: number) => {
    return activeParty.some(hero => hero.id === heroId);
  };

  const canAddToParty = activeParty.length < 3;

  const getClassIcon = (className: string) => {
    const icons: { [key: string]: string } = {
      'Guerreiro': '⚔️',
      'Mago': '🔮',
      'Arqueiro': '🏹',
      'Ladino': '🗡️',
      'Paladino': '🛡️',
      'Clérigo': '✨',
      'Bárbaro': '🔥',
      'Bruxo': '👹',
      'Druida': '🌿',
      'Monge': '🥋'
    };
    return icons[className] || '⚔️';
  };

  const getClassGradient = (className: string) => {
    const gradients: { [key: string]: string } = {
      'Guerreiro': 'from-red-500 to-red-600',
      'Mago': 'from-blue-500 to-blue-600',
      'Arqueiro': 'from-green-500 to-green-600',
      'Ladino': 'from-purple-500 to-purple-600',
      'Paladino': 'from-yellow-500 to-yellow-600',
      'Clérigo': 'from-pink-500 to-pink-600',
      'Bárbaro': 'from-orange-500 to-orange-600',
      'Bruxo': 'from-indigo-500 to-indigo-600',
      'Druida': 'from-emerald-500 to-emerald-600',
      'Monge': 'from-teal-500 to-teal-600'
    };
    return gradients[className] || 'from-gray-500 to-gray-600';
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-slate-900 via-purple-900 to-slate-900 flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-32 w-32 border-b-2 border-purple-500 mx-auto mb-4"></div>
          <p className="text-gray-300 text-xl">Carregando heróis...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-purple-900 to-slate-900 relative overflow-hidden">
      {/* Background Effects */}
      <div className="absolute inset-0 bg-[url('/grid.svg')] bg-center [mask-image:linear-gradient(180deg,white,rgba(255,255,255,0))]"></div>
      <div className="absolute top-0 -left-4 w-72 h-72 bg-purple-300 rounded-full mix-blend-multiply filter blur-xl opacity-20 animate-blob"></div>
      <div className="absolute top-0 -right-4 w-72 h-72 bg-yellow-300 rounded-full mix-blend-multiply filter blur-xl opacity-20 animate-blob animation-delay-2000"></div>
      <div className="absolute -bottom-8 left-20 w-72 h-72 bg-pink-300 rounded-full mix-blend-multiply filter blur-xl opacity-20 animate-blob animation-delay-4000"></div>

      <div className="relative z-10 container mx-auto px-6 py-8">
        {/* Header */}
        <div className="mb-12">
          <div className="flex items-center gap-4 mb-6">
            <button
              onClick={() => navigate('/')}
              className="p-3 bg-gray-800/50 hover:bg-gray-700/50 rounded-xl border border-gray-700/30 transition-all duration-300 hover:scale-105"
            >
              <ArrowLeft className="w-6 h-6 text-gray-300" />
            </button>
            <div className="flex-1">
              <div className="inline-block relative">
                <h1 className="text-6xl font-black mb-2 bg-gradient-to-r from-blue-400 via-purple-500 to-pink-500 bg-clip-text text-transparent animate-pulse">
                  Heróis
                </h1>
                <div className="absolute -inset-1 bg-gradient-to-r from-blue-400 via-purple-500 to-pink-500 rounded-lg blur opacity-30 animate-pulse"></div>
              </div>
              <p className="text-gray-300 text-xl font-medium">
                Gerencie seus heróis e monte sua party épica
              </p>
            </div>
            <button
              onClick={() => setShowCreateModal(true)}
              className="relative group inline-flex items-center px-6 py-3 bg-gradient-to-r from-purple-500 to-pink-500 text-white font-semibold rounded-xl hover:from-purple-600 hover:to-pink-600 transition-all duration-300 transform hover:scale-105"
            >
              <div className="absolute inset-0 bg-gradient-to-r from-purple-500 to-pink-500 rounded-xl blur opacity-30 group-hover:opacity-50 transition duration-300"></div>
              <Plus className="w-5 h-5 mr-2 relative z-10" />
              <span className="relative z-10">Criar Herói</span>
            </button>
          </div>

          {/* Stats Summary */}
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div className="relative group">
              <div className="absolute -inset-0.5 bg-gradient-to-r from-blue-500 to-blue-600 rounded-2xl blur opacity-25 group-hover:opacity-75 transition duration-1000 group-hover:duration-200"></div>
              <div className="relative bg-black/40 backdrop-blur-sm border border-blue-500/20 rounded-2xl p-6 group-hover:border-blue-400/40 transition-all duration-300">
                <div className="flex items-center justify-between">
                  <div className="p-4 bg-gradient-to-br from-blue-500 to-blue-600 rounded-2xl shadow-2xl">
                    <Users className="w-8 h-8 text-white" />
                  </div>
                  <div className="text-right">
                    <span className="text-4xl font-black bg-gradient-to-r from-blue-400 to-blue-600 bg-clip-text text-transparent">
                      {heroes.length}
                    </span>
                    <p className="text-blue-300 text-sm font-semibold uppercase tracking-wide">Total de Heróis</p>
                  </div>
                </div>
              </div>
            </div>

            <div className="relative group">
              <div className="absolute -inset-0.5 bg-gradient-to-r from-purple-500 to-purple-600 rounded-2xl blur opacity-25 group-hover:opacity-75 transition duration-1000 group-hover:duration-200"></div>
              <div className="relative bg-black/40 backdrop-blur-sm border border-purple-500/20 rounded-2xl p-6 group-hover:border-purple-400/40 transition-all duration-300">
                <div className="flex items-center justify-between">
                  <div className="p-4 bg-gradient-to-br from-purple-500 to-purple-600 rounded-2xl shadow-2xl">
                    <Crown className="w-8 h-8 text-white" />
                  </div>
                  <div className="text-right">
                    <span className="text-4xl font-black bg-gradient-to-r from-purple-400 to-purple-600 bg-clip-text text-transparent">
                      {activeParty.length}
                    </span>
                    <span className="text-gray-400 text-lg">/3</span>
                    <p className="text-purple-300 text-sm font-semibold uppercase tracking-wide">Party Ativa</p>
                  </div>
                </div>
              </div>
            </div>

            <div className="relative group">
              <div className="absolute -inset-0.5 bg-gradient-to-r from-amber-500 to-orange-600 rounded-2xl blur opacity-25 group-hover:opacity-75 transition duration-1000 group-hover:duration-200"></div>
              <div className="relative bg-black/40 backdrop-blur-sm border border-amber-500/20 rounded-2xl p-6 group-hover:border-amber-400/40 transition-all duration-300">
                <div className="flex items-center justify-between">
                  <div className="p-4 bg-gradient-to-br from-amber-500 to-orange-600 rounded-2xl shadow-2xl">
                    <Star className="w-8 h-8 text-white" />
                  </div>
                  <div className="text-right">
                    <span className="text-4xl font-black bg-gradient-to-r from-amber-400 to-orange-600 bg-clip-text text-transparent">
                      {heroes.reduce((sum, hero) => sum + hero.level, 0)}
                    </span>
                    <p className="text-amber-300 text-sm font-semibold uppercase tracking-wide">Níveis Totais</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        {/* Messages */}
        {error && (
          <div className="mb-6 p-4 bg-red-900/20 border border-red-700/30 rounded-xl text-red-300">
            {error}
          </div>
        )}
        {success && (
          <div className="mb-6 p-4 bg-green-900/20 border border-green-700/30 rounded-xl text-green-300">
            {success}
          </div>
        )}

        {/* Heroes Grid */}
        {heroes.length === 0 ? (
          <div className="text-center py-20">
            <div className="relative mx-auto w-32 h-32 mb-8">
              <div className="absolute inset-0 bg-gradient-to-br from-purple-500/20 to-pink-500/20 rounded-full blur-xl"></div>
              <div className="relative p-8 bg-gradient-to-br from-gray-800/50 to-gray-900/50 rounded-full border border-gray-700/30">
                <Users className="w-16 h-16 text-gray-400" />
              </div>
            </div>
            <h3 className="text-3xl font-bold text-gray-200 mb-4">Nenhum herói criado</h3>
            <p className="text-gray-400 mb-8 max-w-md mx-auto">
              Crie seu primeiro herói para começar sua jornada épica! Escolha uma classe e embarque em aventuras incríveis.
            </p>
            <button
              onClick={() => setShowCreateModal(true)}
              className="relative group inline-flex items-center px-8 py-4 bg-gradient-to-r from-purple-500 to-pink-500 text-white font-semibold rounded-xl hover:from-purple-600 hover:to-pink-600 transition-all duration-300 transform hover:scale-105"
            >
              <div className="absolute inset-0 bg-gradient-to-r from-purple-500 to-pink-500 rounded-xl blur opacity-30 group-hover:opacity-50 transition duration-300"></div>
              <Sparkles className="w-6 h-6 mr-3 relative z-10" />
              <span className="relative z-10">Criar Primeiro Herói</span>
            </button>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-8">
            {heroes.map((hero) => (
              <div key={hero.id} className="relative group">
                <div className="absolute -inset-0.5 bg-gradient-to-r from-blue-500 to-purple-500 rounded-3xl blur opacity-0 group-hover:opacity-20 transition duration-300"></div>
                <div className="relative bg-black/40 backdrop-blur-sm border border-gray-700/30 rounded-3xl p-6 group-hover:border-gray-600/50 transition-all duration-300">
                  {hero.isInActiveParty && (
                    <div className="absolute -top-3 -right-3 w-8 h-8 bg-gradient-to-r from-purple-500 to-pink-500 rounded-full flex items-center justify-center text-white font-bold text-sm shadow-lg">
                      <Crown className="w-4 h-4" />
                    </div>
                  )}
                  
                  {/* Hero Avatar */}
                  <div className="text-center mb-6">
                    <div className={`relative mx-auto w-20 h-20 mb-4 bg-gradient-to-br ${getClassGradient(hero.class)} rounded-2xl flex items-center justify-center shadow-2xl group-hover:scale-110 transition-transform duration-300`}>
                      <span className="text-4xl">{getClassIcon(hero.class)}</span>
                      {hero.isInActiveParty && (
                        <div className="absolute -bottom-2 -right-2 w-6 h-6 bg-gradient-to-r from-purple-500 to-pink-500 rounded-full flex items-center justify-center text-white font-bold text-xs">
                          {hero.partySlot}
                        </div>
                      )}
                    </div>
                    <h3 className="text-2xl font-bold text-white mb-1">{hero.name}</h3>
                    <p className="text-gray-300 font-medium">{hero.class}</p>
                  </div>

                  {/* Hero Stats */}
                  <div className="space-y-3 mb-6">
                    <div className="flex items-center justify-between">
                      <div className="flex items-center gap-2">
                        <Crown className="w-4 h-4 text-yellow-400" />
                        <span className="text-gray-300 text-sm">Nível</span>
                      </div>
                      <span className="text-white font-semibold">{hero.level}</span>
                    </div>
                    
                    <div className="flex items-center justify-between">
                      <div className="flex items-center gap-2">
                        <Heart className="w-4 h-4 text-red-400" />
                        <span className="text-gray-300 text-sm">Vida</span>
                      </div>
                      <span className="text-white font-semibold">{hero.currentHealth}/{hero.maxHealth}</span>
                    </div>

                    <div className="flex items-center justify-between">
                      <div className="flex items-center gap-2">
                        <Sword className="w-4 h-4 text-blue-400" />
                        <span className="text-gray-300 text-sm">Força</span>
                      </div>
                      <span className="text-white font-semibold">{hero.strength}</span>
                    </div>

                    <div className="flex items-center justify-between">
                      <div className="flex items-center gap-2">
                        <Zap className="w-4 h-4 text-purple-400" />
                        <span className="text-gray-300 text-sm">Inteligência</span>
                      </div>
                      <span className="text-white font-semibold">{hero.intelligence}</span>
                    </div>

                    <div className="flex items-center justify-between">
                      <div className="flex items-center gap-2">
                        <Shield className="w-4 h-4 text-green-400" />
                        <span className="text-gray-300 text-sm">Destreza</span>
                      </div>
                      <span className="text-white font-semibold">{hero.dexterity}</span>
                    </div>
                  </div>

                  {/* Party Management */}
                  <div className="space-y-2">
                    {isInParty(hero.id) ? (
                      <button
                        onClick={() => handleRemoveFromParty(hero.id)}
                        className="w-full relative group inline-flex items-center justify-center px-4 py-3 bg-gradient-to-r from-red-600 to-red-700 text-white font-semibold rounded-xl hover:from-red-700 hover:to-red-800 transition-all duration-300 transform hover:scale-105"
                      >
                        <div className="absolute inset-0 bg-gradient-to-r from-red-600 to-red-700 rounded-xl blur opacity-30 group-hover:opacity-50 transition duration-300"></div>
                        <UserMinus className="w-5 h-5 mr-2 relative z-10" />
                        <span className="relative z-10">Remover da Party</span>
                      </button>
                    ) : (
                      <button
                        onClick={() => handleAddToParty(hero.id)}
                        disabled={!canAddToParty}
                        className="w-full relative group inline-flex items-center justify-center px-4 py-3 bg-gradient-to-r from-green-600 to-green-700 text-white font-semibold rounded-xl hover:from-green-700 hover:to-green-800 transition-all duration-300 transform hover:scale-105 disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:scale-100"
                      >
                        <div className="absolute inset-0 bg-gradient-to-r from-green-600 to-green-700 rounded-xl blur opacity-30 group-hover:opacity-50 transition duration-300"></div>
                        <UserPlus className="w-5 h-5 mr-2 relative z-10" />
                        <span className="relative z-10">
                          {canAddToParty ? 'Adicionar à Party' : 'Party Cheia'}
                        </span>
                      </button>
                    )}
                  </div>

                  {/* Hero Info */}
                  <div className="mt-4 pt-4 border-t border-gray-700/30">
                    <div className="flex items-center justify-between text-xs text-gray-400">
                      <div className="flex items-center gap-1">
                        <Coins className="w-3 h-3" />
                        <span>{hero.gold} ouro</span>
                      </div>
                      <div className="flex items-center gap-1">
                        <Calendar className="w-3 h-3" />
                        <span>{new Date(hero.createdAt).toLocaleDateString()}</span>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}

        {/* Create Hero Modal */}
        {showCreateModal && (
          <div className="fixed inset-0 bg-black/50 backdrop-blur-sm flex items-center justify-center z-50 p-4">
            <div className="relative bg-gray-900/95 backdrop-blur-sm border border-gray-700/50 rounded-3xl p-8 max-w-md w-full">
              <button
                onClick={() => setShowCreateModal(false)}
                className="absolute top-4 right-4 p-2 hover:bg-gray-800/50 rounded-xl transition-colors"
              >
                <X className="w-5 h-5 text-gray-400" />
              </button>

              <div className="text-center mb-8">
                <div className="mx-auto w-16 h-16 bg-gradient-to-br from-purple-500 to-pink-500 rounded-2xl flex items-center justify-center mb-4">
                  <Sparkles className="w-8 h-8 text-white" />
                </div>
                <h2 className="text-2xl font-bold text-white mb-2">Criar Novo Herói</h2>
                <p className="text-gray-400">Escolha um nome e classe para seu herói</p>
              </div>

              <div className="space-y-6">
                {/* Name Input */}
                <div>
                  <label className="block text-sm font-medium text-gray-300 mb-2">
                    Nome do Herói
                  </label>
                  <div className="flex gap-2">
                    <input
                      type="text"
                      value={newHero.name}
                      onChange={(e) => setNewHero(prev => ({ ...prev, name: e.target.value }))}
                      className="flex-1 px-4 py-3 bg-gray-800/50 border border-gray-700/50 rounded-xl text-white placeholder-gray-400 focus:outline-none focus:border-purple-500 focus:ring-1 focus:ring-purple-500"
                      placeholder="Digite o nome..."
                    />
                    <button
                      onClick={generateRandomName}
                      className="px-4 py-3 bg-gray-700/50 hover:bg-gray-600/50 border border-gray-700/50 rounded-xl transition-colors"
                    >
                      <Sparkles className="w-4 h-4 text-gray-300" />
                    </button>
                  </div>
                </div>

                {/* Class Selection */}
                <div>
                  <label className="block text-sm font-medium text-gray-300 mb-2">
                    Classe
                  </label>
                  <div className="flex gap-2">
                    <select
                      value={newHero.class}
                      onChange={(e) => setNewHero(prev => ({ ...prev, class: e.target.value }))}
                      className="flex-1 px-4 py-3 bg-gray-800/50 border border-gray-700/50 rounded-xl text-white focus:outline-none focus:border-purple-500 focus:ring-1 focus:ring-purple-500"
                    >
                      {heroClasses.map(heroClass => (
                        <option key={heroClass} value={heroClass} className="bg-gray-800">
                          {heroClass}
                        </option>
                      ))}
                    </select>
                    <button
                      onClick={generateRandomClass}
                      className="px-4 py-3 bg-gray-700/50 hover:bg-gray-600/50 border border-gray-700/50 rounded-xl transition-colors"
                    >
                      <Sparkles className="w-4 h-4 text-gray-300" />
                    </button>
                  </div>
                </div>

                {/* Class Preview */}
                <div className="p-4 bg-gray-800/30 rounded-xl border border-gray-700/30">
                  <div className="flex items-center gap-3 mb-3">
                    <div className={`w-10 h-10 bg-gradient-to-br ${getClassGradient(newHero.class)} rounded-lg flex items-center justify-center`}>
                      <span className="text-lg">{getClassIcon(newHero.class)}</span>
                    </div>
                    <div>
                      <h4 className="font-semibold text-white">{newHero.class}</h4>
                      <p className="text-xs text-gray-400">{heroService.getClassDescription(newHero.class)}</p>
                    </div>
                  </div>
                  <div className="grid grid-cols-3 gap-2 text-xs">
                    <div className="text-center">
                      <div className="text-blue-400 font-semibold">{classAttributes.strength}</div>
                      <div className="text-gray-400">Força</div>
                    </div>
                    <div className="text-center">
                      <div className="text-purple-400 font-semibold">{classAttributes.intelligence}</div>
                      <div className="text-gray-400">Inteligência</div>
                    </div>
                    <div className="text-center">
                      <div className="text-green-400 font-semibold">{classAttributes.dexterity}</div>
                      <div className="text-gray-400">Destreza</div>
                    </div>
                  </div>
                </div>

                {/* Create Button */}
                <button
                  onClick={handleCreateHero}
                  disabled={!canCreate}
                  className="w-full relative group inline-flex items-center justify-center px-6 py-4 bg-gradient-to-r from-purple-500 to-pink-500 text-white font-semibold rounded-xl hover:from-purple-600 hover:to-pink-600 transition-all duration-300 transform hover:scale-105 disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:scale-100"
                >
                  <div className="absolute inset-0 bg-gradient-to-r from-purple-500 to-pink-500 rounded-xl blur opacity-30 group-hover:opacity-50 transition duration-300"></div>
                  <Plus className="w-5 h-5 mr-2 relative z-10" />
                  <span className="relative z-10">Criar Herói</span>
                </button>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};