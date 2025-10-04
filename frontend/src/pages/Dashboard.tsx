import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { heroService, type Hero, type UserProfile } from '../services/heroService';
import { soundService } from '../services/sound';
import { Card } from '../components/ui/Card';
import { 
  Sword, 
  Coins, 
  Target,
  ShoppingBag,
  Users,
  Crown,
  TrendingUp,
  Plus,
  Star
} from 'lucide-react';

export default function Dashboard() {
  const navigate = useNavigate();
  const [userProfile, setUserProfile] = useState<UserProfile | null>(null);
  const [heroes, setHeroes] = useState<Hero[]>([]);
  const [activeParty, setActiveParty] = useState<Hero[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      setIsLoading(true);
      const [profileRes, heroesRes, partyRes] = await Promise.all([
        heroService.getUserProfile(),
        heroService.getMyHeroes(),
        heroService.getActiveParty()
      ]);
      setUserProfile(profileRes);
      setHeroes(heroesRes);
      setActiveParty(partyRes);
    } catch (err: any) {
      console.error('Error loading dashboard data:', err);
      setError(err.response?.data?.message || 'Erro ao carregar dados do dashboard.');
    } finally {
      setIsLoading(false);
    }
  };

  const handleNavigation = (path: string) => {
    soundService.playSound('click');
    navigate(path);
  };

  if (isLoading) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-slate-900 via-purple-900 to-slate-900 flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-32 w-32 border-b-2 border-purple-500 mx-auto mb-4"></div>
          <p className="text-gray-300 text-xl">Carregando dashboard...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-slate-900 via-purple-900 to-slate-900 flex items-center justify-center">
        <div className="text-center">
          <div className="text-red-500 text-6xl mb-4">⚠️</div>
          <h2 className="text-2xl font-bold text-white mb-2">Erro ao carregar</h2>
          <p className="text-gray-300 mb-4">{error}</p>
          <button
            onClick={fetchData}
            className="bg-purple-600 hover:bg-purple-700 text-white px-6 py-2 rounded-lg font-semibold transition"
          >
            Tentar novamente
          </button>
        </div>
      </div>
    );
  }

  const strongestHeroes = [...heroes].sort((a, b) => b.level - a.level).slice(0, 3);
  const totalExperience = heroes.reduce((sum, hero) => sum + hero.experience, 0);

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

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-purple-900 to-slate-900 relative overflow-hidden">
      {/* Background Effects */}
      <div className="absolute inset-0 bg-[url('/grid.svg')] bg-center [mask-image:linear-gradient(180deg,white,rgba(255,255,255,0))]"></div>
      <div className="absolute top-0 -left-4 w-72 h-72 bg-purple-300 rounded-full mix-blend-multiply filter blur-xl opacity-20 animate-blob"></div>
      <div className="absolute top-0 -right-4 w-72 h-72 bg-yellow-300 rounded-full mix-blend-multiply filter blur-xl opacity-20 animate-blob animation-delay-2000"></div>
      <div className="absolute -bottom-8 left-20 w-72 h-72 bg-pink-300 rounded-full mix-blend-multiply filter blur-xl opacity-20 animate-blob animation-delay-4000"></div>

      <div className="relative z-10 container mx-auto px-6 py-8">
        <div className="mb-12 text-center">
          <div className="inline-block relative">
            <h1 className="text-7xl font-black mb-4 bg-gradient-to-r from-blue-400 via-purple-500 to-pink-500 bg-clip-text text-transparent animate-pulse">
              Dashboard
            </h1>
            <div className="absolute -inset-1 bg-gradient-to-r from-blue-400 via-purple-500 to-pink-500 rounded-lg blur opacity-30 animate-pulse"></div>
          </div>
          <p className="text-gray-300 text-xl font-medium">
            Bem-vindo de volta, <span className="text-amber-400 font-bold">{userProfile?.username}</span>!
          </p>
          <p className="text-gray-400 text-sm mt-2">Gerencie seus heróis e embarque em aventuras épicas</p>
        </div>

        {/* Stats Cards */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-8 mb-12">
          <div className="relative group">
            <div className="absolute -inset-0.5 bg-gradient-to-r from-blue-500 to-blue-600 rounded-2xl blur opacity-25 group-hover:opacity-75 transition duration-1000 group-hover:duration-200"></div>
            <div className="relative bg-black/40 backdrop-blur-sm border border-blue-500/20 rounded-2xl p-6 group-hover:border-blue-400/40 transition-all duration-300">
              <div className="flex items-center justify-between mb-4">
                <div className="p-4 bg-gradient-to-br from-blue-500 to-blue-600 rounded-2xl shadow-2xl group-hover:scale-110 transition-transform duration-300">
                  <Users className="w-8 h-8 text-white" />
                </div>
                <div className="text-right">
                  <span className="text-4xl font-black bg-gradient-to-r from-blue-400 to-blue-600 bg-clip-text text-transparent">
                    {userProfile?.totalHeroes || 0}
                  </span>
                </div>
              </div>
              <h3 className="text-blue-300 text-sm font-semibold uppercase tracking-wide">Total de Heróis</h3>
              <p className="text-gray-400 text-xs mt-1">Heróis criados</p>
            </div>
          </div>

          <div className="relative group">
            <div className="absolute -inset-0.5 bg-gradient-to-r from-purple-500 to-purple-600 rounded-2xl blur opacity-25 group-hover:opacity-75 transition duration-1000 group-hover:duration-200"></div>
            <div className="relative bg-black/40 backdrop-blur-sm border border-purple-500/20 rounded-2xl p-6 group-hover:border-purple-400/40 transition-all duration-300">
              <div className="flex items-center justify-between mb-4">
                <div className="p-4 bg-gradient-to-br from-purple-500 to-purple-600 rounded-2xl shadow-2xl group-hover:scale-110 transition-transform duration-300">
                  <Crown className="w-8 h-8 text-white" />
                </div>
                <div className="text-right">
                  <span className="text-4xl font-black bg-gradient-to-r from-purple-400 to-purple-600 bg-clip-text text-transparent">
                    {userProfile?.activePartyCount || 0}
                  </span>
                  <span className="text-gray-400 text-lg">/3</span>
                </div>
              </div>
              <h3 className="text-purple-300 text-sm font-semibold uppercase tracking-wide">Party Ativa</h3>
              <p className="text-gray-400 text-xs mt-1">Heróis em ação</p>
            </div>
          </div>

          <div className="relative group">
            <div className="absolute -inset-0.5 bg-gradient-to-r from-amber-500 to-orange-600 rounded-2xl blur opacity-25 group-hover:opacity-75 transition duration-1000 group-hover:duration-200"></div>
            <div className="relative bg-black/40 backdrop-blur-sm border border-amber-500/20 rounded-2xl p-6 group-hover:border-amber-400/40 transition-all duration-300">
              <div className="flex items-center justify-between mb-4">
                <div className="p-4 bg-gradient-to-br from-amber-500 to-orange-600 rounded-2xl shadow-2xl group-hover:scale-110 transition-transform duration-300">
                  <Coins className="w-8 h-8 text-white" />
                </div>
                <div className="text-right">
                  <span className="text-4xl font-black bg-gradient-to-r from-amber-400 to-orange-600 bg-clip-text text-transparent">
                    {userProfile?.gold || 0}
                  </span>
                </div>
              </div>
              <h3 className="text-amber-300 text-sm font-semibold uppercase tracking-wide">Ouro Total</h3>
              <p className="text-gray-400 text-xs mt-1">Moeda do reino</p>
            </div>
          </div>

          <div className="relative group">
            <div className="absolute -inset-0.5 bg-gradient-to-r from-green-500 to-green-600 rounded-2xl blur opacity-25 group-hover:opacity-75 transition duration-1000 group-hover:duration-200"></div>
            <div className="relative bg-black/40 backdrop-blur-sm border border-green-500/20 rounded-2xl p-6 group-hover:border-green-400/40 transition-all duration-300">
              <div className="flex items-center justify-between mb-4">
                <div className="p-4 bg-gradient-to-br from-green-500 to-green-600 rounded-2xl shadow-2xl group-hover:scale-110 transition-transform duration-300">
                  <TrendingUp className="w-8 h-8 text-white" />
                </div>
                <div className="text-right">
                  <span className="text-4xl font-black bg-gradient-to-r from-green-400 to-green-600 bg-clip-text text-transparent">
                    {totalExperience.toLocaleString()}
                  </span>
                </div>
              </div>
              <h3 className="text-green-300 text-sm font-semibold uppercase tracking-wide">XP Total</h3>
              <p className="text-gray-400 text-xs mt-1">Experiência acumulada</p>
            </div>
          </div>
        </div>

        {/* Main Content Sections */}
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-8 mb-12">
          {/* Active Party */}
          <div className="relative group">
            <div className="absolute -inset-0.5 bg-gradient-to-r from-purple-500 to-pink-500 rounded-3xl blur opacity-20 group-hover:opacity-40 transition duration-300"></div>
            <div className="relative bg-black/40 backdrop-blur-sm border border-purple-500/20 rounded-3xl p-8 group-hover:border-purple-400/40 transition-all duration-200">
              <div className="flex items-center gap-4 mb-8">
                <div className="p-4 bg-gradient-to-br from-purple-500 to-purple-600 rounded-2xl shadow-2xl">
                  <Crown className="w-8 h-8 text-white" />
                </div>
                <div>
                  <h2 className="text-3xl font-bold bg-gradient-to-r from-purple-400 to-pink-400 bg-clip-text text-transparent">
                    Party Ativa
                  </h2>
                  <p className="text-gray-300 font-medium">{activeParty.length}/3 heróis</p>
                </div>
              </div>

              {activeParty.length === 0 ? (
                <div className="text-center py-12">
                  <div className="relative mx-auto w-24 h-24 mb-6">
                    <div className="absolute inset-0 bg-gradient-to-br from-purple-500/20 to-pink-500/20 rounded-full blur-xl"></div>
                    <div className="relative p-6 bg-gradient-to-br from-gray-800/50 to-gray-900/50 rounded-full border border-gray-700/30">
                      <Users className="w-12 h-12 text-gray-400" />
                    </div>
                  </div>
                  <h3 className="text-xl font-bold text-gray-200 mb-3">Nenhum herói na party</h3>
                  <p className="text-gray-400 mb-6 max-w-sm mx-auto">Crie e adicione heróis à sua party para começar as aventuras épicas!</p>
                  <button
                    onClick={() => handleNavigation('/heroes')}
                    className="relative group inline-flex items-center px-6 py-3 bg-gradient-to-r from-purple-500 to-pink-500 text-white font-semibold rounded-xl hover:from-purple-600 hover:to-pink-600 transition-all duration-300 transform hover:scale-105"
                  >
                    <div className="absolute inset-0 bg-gradient-to-r from-purple-500 to-pink-500 rounded-xl blur opacity-30 group-hover:opacity-50 transition duration-300"></div>
                    <Plus className="w-5 h-5 mr-2 relative z-10" />
                    <span className="relative z-10">Gerenciar Heróis</span>
                  </button>
                </div>
              ) : (
                <div className="space-y-4">
                  {activeParty.map((hero) => (
                    <div key={hero.id} className="group relative">
                      <div className="absolute -inset-0.5 bg-gradient-to-r from-blue-500 to-purple-500 rounded-2xl blur opacity-0 group-hover:opacity-20 transition duration-300"></div>
                      <div className="relative flex items-center gap-4 p-5 bg-gray-800/50 rounded-2xl border border-gray-700/30 group-hover:border-gray-600/50 transition-all duration-300">
                        <div className="relative">
                          <div className={`p-4 bg-gradient-to-br ${getClassGradient(hero.class)} rounded-2xl shadow-lg`}>
                            <span className="text-2xl">{getClassIcon(hero.class)}</span>
                          </div>
                          <div className="absolute -top-2 -right-2 w-6 h-6 bg-gradient-to-r from-purple-500 to-pink-500 rounded-full flex items-center justify-center text-xs font-bold text-white">
                            {hero.partySlot}
                          </div>
                        </div>
                        <div className="flex-1">
                          <h3 className="font-bold text-xl text-white mb-1">{hero.name}</h3>
                          <p className="text-gray-300 font-medium">{hero.class} • Nível {hero.level}</p>
                          <div className="flex gap-4 mt-2 text-sm">
                            <span className="text-red-400">❤️ {hero.currentHealth}/{hero.maxHealth}</span>
                            <span className="text-blue-400">⚔️ {hero.strength}</span>
                            <span className="text-purple-400">🧠 {hero.intelligence}</span>
                          </div>
                        </div>
                      </div>
                    </div>
                  ))}
                  <button
                    onClick={() => handleNavigation('/heroes')}
                    className="w-full mt-6 relative group inline-flex items-center justify-center px-6 py-4 bg-gradient-to-r from-gray-700 to-gray-600 text-white font-semibold rounded-xl hover:from-gray-600 hover:to-gray-500 transition-all duration-300 transform hover:scale-105"
                  >
                    <div className="absolute inset-0 bg-gradient-to-r from-gray-700 to-gray-600 rounded-xl blur opacity-30 group-hover:opacity-50 transition duration-300"></div>
                    <span className="relative z-10">Gerenciar Party</span>
                  </button>
                </div>
              )}
            </div>
          </div>

          {/* Strongest Heroes */}
          <div className="relative group">
            <div className="absolute -inset-0.5 bg-gradient-to-r from-yellow-500 to-orange-500 rounded-3xl blur opacity-20 group-hover:opacity-40 transition duration-1000 group-hover:duration-200"></div>
            <div className="relative bg-black/40 backdrop-blur-sm border border-yellow-500/20 rounded-3xl p-8 group-hover:border-yellow-400/40 transition-all duration-300">
              <div className="flex items-center gap-4 mb-8">
                <div className="p-4 bg-gradient-to-br from-yellow-500 to-yellow-600 rounded-2xl shadow-2xl">
                  <Star className="w-8 h-8 text-white" />
                </div>
                <div>
                  <h2 className="text-3xl font-bold bg-gradient-to-r from-yellow-400 to-orange-400 bg-clip-text text-transparent">
                    Heróis Mais Fortes
                  </h2>
                  <p className="text-gray-300 font-medium">Top 3 por nível</p>
                </div>
              </div>

              {strongestHeroes.length === 0 ? (
                <div className="text-center py-12">
                  <div className="relative mx-auto w-24 h-24 mb-6">
                    <div className="absolute inset-0 bg-gradient-to-br from-yellow-500/20 to-orange-500/20 rounded-full blur-xl"></div>
                    <div className="relative p-6 bg-gradient-to-br from-gray-800/50 to-gray-900/50 rounded-full border border-gray-700/30">
                      <Users className="w-12 h-12 text-gray-400" />
                    </div>
                  </div>
                  <h3 className="text-xl font-bold text-gray-200 mb-3">Nenhum herói criado</h3>
                  <p className="text-gray-400 mb-6 max-w-sm mx-auto">Crie seu primeiro herói para começar a aventura épica!</p>
                  <button
                    onClick={() => handleNavigation('/heroes')}
                    className="relative group inline-flex items-center px-6 py-3 bg-gradient-to-r from-yellow-500 to-orange-500 text-white font-semibold rounded-xl hover:from-yellow-600 hover:to-orange-600 transition-all duration-300 transform hover:scale-105"
                  >
                    <div className="absolute inset-0 bg-gradient-to-r from-yellow-500 to-orange-500 rounded-xl blur opacity-30 group-hover:opacity-50 transition duration-300"></div>
                    <Plus className="w-5 h-5 mr-2 relative z-10" />
                    <span className="relative z-10">Criar Herói</span>
                  </button>
                </div>
              ) : (
                <div className="space-y-4">
                  {strongestHeroes.map((hero, index) => (
                    <div key={hero.id} className="group relative">
                      <div className="absolute -inset-0.5 bg-gradient-to-r from-yellow-500 to-orange-500 rounded-2xl blur opacity-0 group-hover:opacity-20 transition duration-300"></div>
                      <div className="relative flex items-center gap-4 p-5 bg-gray-800/50 rounded-2xl border border-gray-700/30 group-hover:border-gray-600/50 transition-all duration-300">
                        <div className="flex items-center justify-center w-10 h-10 bg-gradient-to-br from-yellow-500 to-yellow-600 rounded-full text-white font-bold text-lg shadow-lg">
                          {index + 1}
                        </div>
                        <div className={`p-3 bg-gradient-to-br ${getClassGradient(hero.class)} rounded-xl shadow-lg`}>
                          <span className="text-2xl">{getClassIcon(hero.class)}</span>
                        </div>
                        <div className="flex-1">
                          <h3 className="font-bold text-xl text-white mb-1">{hero.name}</h3>
                          <p className="text-gray-300 font-medium">{hero.class} • Nível {hero.level}</p>
                          <div className="flex gap-4 mt-2 text-sm">
                            <span className="text-yellow-400">⭐ {hero.experience} XP</span>
                            <span className="text-red-400">❤️ {hero.currentHealth}/{hero.maxHealth}</span>
                            <span className="text-blue-400">⚔️ {hero.strength}</span>
                          </div>
                        </div>
                      </div>
                    </div>
                  ))}
                  
                  {/* Preencher espaço se houver menos de 3 heróis */}
                  {Array.from({ length: Math.max(0, 3 - strongestHeroes.length) }).map((_, index) => (
                    <div key={`empty-${index}`} className="flex items-center gap-4 p-5 bg-gray-800/20 rounded-2xl border border-gray-700/20 border-dashed">
                      <div className="flex items-center justify-center w-10 h-10 bg-gray-700/50 rounded-full text-gray-500 font-bold text-lg">
                        {strongestHeroes.length + index + 1}
                      </div>
                      <div className="p-3 bg-gray-700/50 rounded-xl">
                        <span className="text-2xl text-gray-500">❓</span>
                      </div>
                      <div className="flex-1">
                        <h3 className="font-bold text-xl text-gray-500 mb-1">Vago</h3>
                        <p className="text-gray-500 font-medium">Crie mais heróis</p>
                        <div className="flex gap-4 mt-2 text-sm">
                          <span className="text-gray-500">⭐ 0 XP</span>
                          <span className="text-gray-500">❤️ 0/0</span>
                          <span className="text-gray-500">⚔️ 0</span>
                        </div>
                      </div>
                    </div>
                  ))}
                  
                  <button
                    onClick={() => handleNavigation('/heroes')}
                    className="w-full mt-6 relative group inline-flex items-center justify-center px-6 py-4 bg-gradient-to-r from-gray-700 to-gray-600 text-white font-semibold rounded-xl hover:from-gray-600 hover:to-gray-500 transition-all duration-300 transform hover:scale-105"
                  >
                    <div className="absolute inset-0 bg-gradient-to-r from-gray-700 to-gray-600 rounded-xl blur opacity-30 group-hover:opacity-50 transition duration-300"></div>
                    <span className="relative z-10">Ver Todos os Heróis</span>
                  </button>
                </div>
              )}
            </div>
          </div>
        </div>

        {/* Action Buttons */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          <button
            onClick={() => handleNavigation('/heroes')}
            className="action-card group"
          >
            <div className="p-4 bg-gradient-to-br from-blue-500 to-blue-600 rounded-xl shadow-lg group-hover:scale-110 transition-transform">
              <Users className="w-8 h-8 text-white" />
            </div>
            <h3 className="text-gray-400 text-sm font-medium mt-3">Gerenciar Heróis</h3>
          </button>

          <button
            onClick={() => handleNavigation('/combat')}
            className="action-card group"
          >
            <div className="p-4 bg-gradient-to-br from-red-500 to-red-600 rounded-xl shadow-lg group-hover:scale-110 transition-transform">
              <Sword className="w-8 h-8 text-white" />
            </div>
            <h3 className="text-gray-400 text-sm font-medium mt-3">Combate</h3>
          </button>

          <button
            onClick={() => handleNavigation('/quests')}
            className="action-card group"
          >
            <div className="p-4 bg-gradient-to-br from-green-500 to-green-600 rounded-xl shadow-lg group-hover:scale-110 transition-transform">
              <Target className="w-8 h-8 text-white" />
            </div>
            <h3 className="text-gray-400 text-sm font-medium mt-3">Missões</h3>
          </button>

          <button
            onClick={() => handleNavigation('/shop')}
            className="action-card group"
          >
            <div className="p-4 bg-gradient-to-br from-amber-500 to-orange-600 rounded-xl shadow-lg group-hover:scale-110 transition-transform">
              <ShoppingBag className="w-8 h-8 text-white" />
            </div>
            <h3 className="text-gray-400 text-sm font-medium mt-3">Loja</h3>
          </button>
        </div>
      </div>
    </div>
  );
}