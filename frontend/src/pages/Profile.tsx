import React, { useEffect, useState } from 'react';
import { Navbar } from '../components/Navbar';
import { Card } from '../components/Card';
import { Loading } from '../components/Loading';
import { HeroWidget } from '../components/HeroWidget';
import { profileService, MyHero, MyQuest, MyStats } from '../services/profileService';

const getExperienceForNextLevel = (level: number): number => {
  return level * 100;
};

export const Profile: React.FC = () => {
  const [hero, setHero] = useState<MyHero | null>(null);
  const [quests, setQuests] = useState<MyQuest[]>([]);
  const [stats, setStats] = useState<MyStats | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    loadProfile();
  }, []);

  const loadProfile = async () => {
    try {
      const [heroData, questsData, statsData] = await Promise.all([
        profileService.getMyHero(),
        profileService.getMyQuests(),
        profileService.getMyStats(),
      ]);
      setHero(heroData);
      setQuests(questsData);
      setStats(statsData);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Erro ao carregar perfil');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <>
        <Navbar />
        <Loading />
      </>
    );
  }

  if (error || !hero) {
    return (
      <>
        <Navbar />
        <div className="container mx-auto px-6 py-8 flex items-center justify-center min-h-[80vh]">
          <Card className="max-w-md text-center">
            <div className="inline-block p-6 bg-gradient-to-br from-red-500 to-red-600 rounded-full shadow-lg shadow-red-500/30 mb-6 animate-float">
              <svg className="w-16 h-16 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
              </svg>
            </div>
            <h2 className="text-3xl font-bold text-red-500 mb-4">😢 Você ainda não tem um herói!</h2>
            <p className="text-gray-400">{error || 'Peça para um administrador criar um herói para você.'}</p>
          </Card>
        </div>
      </>
    );
  }

  const xpProgress = (hero.experience / getExperienceForNextLevel(hero.level)) * 100;

  return (
    <>
      <HeroWidget />
      <Navbar />
      <div className="container mx-auto px-6 py-8">
        <div className="mb-8 text-center">
          <h1 className="text-5xl font-black mb-2 hero-title animate-float">👤 Meu Perfil</h1>
          <p className="text-gray-400">Acompanhe sua jornada épica</p>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-8">
          <Card variant="hero" className="lg:col-span-2 hover:scale-[1.01] transition-transform">
            <div className="flex items-start justify-between mb-6">
              <div className="flex items-center gap-4">
                <div className="w-20 h-20 bg-gradient-to-br from-amber-500 to-orange-600 rounded-2xl flex items-center justify-center text-white text-3xl font-black shadow-xl shadow-amber-500/50 animate-glow">
                  {hero.name.charAt(0)}
                </div>
                <div>
                  <h2 className="text-4xl font-black text-gradient mb-1">{hero.name}</h2>
                  <p className="text-2xl text-gray-400 font-semibold">{hero.class}</p>
                </div>
              </div>
              <div className="text-right">
                <div className="inline-flex items-center gap-2 bg-gradient-to-r from-amber-500 to-orange-600 px-6 py-3 rounded-full shadow-lg shadow-amber-500/50 animate-glow">
                  <svg className="w-6 h-6 text-white" fill="currentColor" viewBox="0 0 20 20">
                    <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                  </svg>
                  <span className="text-white text-2xl font-black">Nível {hero.level}</span>
                </div>
              </div>
            </div>

            <div className="mb-6">
              <div className="flex justify-between text-sm text-gray-400 mb-2">
                <span className="font-semibold">Experiência</span>
                <span className="font-mono">{hero.experience} / {getExperienceForNextLevel(hero.level)} XP</span>
              </div>
              <div className="progress-bar">
                <div className="progress-fill" style={{ width: `${xpProgress}%` }} />
              </div>
            </div>

            <div className="grid grid-cols-3 gap-4 mb-6">
              <div className="stat-card bg-gradient-to-br from-red-900/30 to-red-800/30 border-red-700/30">
                <div className="text-center">
                  <div className="text-5xl mb-2">⚔️</div>
                  <p className="text-gray-400 text-sm mb-1">Força</p>
                  <p className="text-3xl font-black text-red-400">{hero.strength}</p>
                </div>
              </div>
              <div className="stat-card bg-gradient-to-br from-blue-900/30 to-blue-800/30 border-blue-700/30">
                <div className="text-center">
                  <div className="text-5xl mb-2">🧠</div>
                  <p className="text-gray-400 text-sm mb-1">Inteligência</p>
                  <p className="text-3xl font-black text-blue-400">{hero.intelligence}</p>
                </div>
              </div>
              <div className="stat-card bg-gradient-to-br from-green-900/30 to-green-800/30 border-green-700/30">
                <div className="text-center">
                  <div className="text-5xl mb-2">⚡</div>
                  <p className="text-gray-400 text-sm mb-1">Destreza</p>
                  <p className="text-3xl font-black text-green-400">{hero.dexterity}</p>
                </div>
              </div>
            </div>

            <div className="flex items-center justify-between bg-gradient-to-r from-amber-900/30 to-orange-900/30 rounded-2xl p-6 border border-amber-700/30">
              <div className="flex items-center gap-3">
                <div className="text-6xl animate-bounce">🪙</div>
                <span className="text-2xl font-bold text-gray-300">Ouro</span>
              </div>
              <span className="text-5xl font-black text-gradient">{hero.gold}</span>
            </div>
          </Card>

          {stats && (
            <Card className="hover:scale-[1.02] transition-transform">
              <h3 className="text-2xl font-bold text-gradient mb-6 flex items-center gap-2">
                <svg className="w-7 h-7" fill="currentColor" viewBox="0 0 20 20">
                  <path d="M2 11a1 1 0 011-1h2a1 1 0 011 1v5a1 1 0 01-1 1H3a1 1 0 01-1-1v-5zM8 7a1 1 0 011-1h2a1 1 0 011 1v9a1 1 0 01-1 1H9a1 1 0 01-1-1V7zM14 4a1 1 0 011-1h2a1 1 0 011 1v12a1 1 0 01-1 1h-2a1 1 0 01-1-1V4z" />
                </svg>
                Estatísticas
              </h3>
              <div className="space-y-4">
                <div className="flex justify-between items-center p-3 bg-gray-800/50 rounded-xl hover:bg-gray-800 transition-colors">
                  <span className="text-gray-400">Missões Totais</span>
                  <span className="font-black text-xl text-purple-400">{stats.totalQuests}</span>
                </div>
                <div className="flex justify-between items-center p-3 bg-gray-800/50 rounded-xl hover:bg-gray-800 transition-colors">
                  <span className="text-gray-400">✅ Completadas</span>
                  <span className="font-black text-xl text-green-400">{stats.completedQuests}</span>
                </div>
                <hr className="border-gray-700" />
                <div className="flex justify-between items-center p-3 bg-gray-800/50 rounded-xl hover:bg-gray-800 transition-colors">
                  <span className="text-gray-400">Itens Únicos</span>
                  <span className="font-black text-xl text-blue-400">{stats.uniqueItems}</span>
                </div>
                <div className="flex justify-between items-center p-3 bg-gray-800/50 rounded-xl hover:bg-gray-800 transition-colors">
                  <span className="text-gray-400">Total de Itens</span>
                  <span className="font-black text-xl">{stats.totalItems}</span>
                </div>
                <div className="flex justify-between items-center p-3 bg-gray-800/50 rounded-xl hover:bg-gray-800 transition-colors">
                  <span className="text-gray-400">⚔️ Equipados</span>
                  <span className="font-black text-xl text-amber-400">{stats.equippedItems}</span>
                </div>
                <hr className="border-gray-700" />
                <div className="flex justify-between items-center p-3 bg-gradient-to-r from-purple-900/30 to-pink-900/30 rounded-xl border border-purple-700/30">
                  <span className="text-purple-300 font-semibold">🔥 Poder Total</span>
                  <span className="font-black text-2xl text-gradient">{stats.powerRating}</span>
                </div>
                <div className="flex justify-between items-center p-3 bg-gray-800/50 rounded-xl hover:bg-gray-800 transition-colors">
                  <span className="text-gray-400">📅 Dias Jogando</span>
                  <span className="font-black text-xl">{stats.playDays}</span>
                </div>
              </div>
            </Card>
          )}
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <Card variant="item" className="hover:scale-[1.01] transition-transform">
            <h3 className="text-3xl font-bold text-gradient mb-6 flex items-center gap-3">
              <span className="text-4xl">📦</span>
              Meu Inventário
            </h3>
            {hero.heroItems && hero.heroItems.length > 0 ? (
              <div className="space-y-3 max-h-96 overflow-y-auto scrollbar-thin pr-2">
                {hero.heroItems.map((heroItem) => (
                  <div
                    key={heroItem.id}
                    className="group flex items-center justify-between p-4 bg-gradient-to-r from-gray-800/50 to-gray-900/50 rounded-xl border border-gray-700/30 hover:border-green-500/50 transition-all hover:scale-[1.02]"
                  >
                    <div className="flex items-center gap-3 flex-1">
                      <div className="text-3xl">📦</div>
                      <div>
                        <p className="font-bold text-green-400 group-hover:text-green-300">{heroItem.item.name}</p>
                        <p className="text-sm text-gray-400">{heroItem.item.type}</p>
                      </div>
                    </div>
                    <div className="flex items-center gap-3">
                      <span className="badge badge-info">x{heroItem.quantity}</span>
                      {heroItem.isEquipped && (
                        <span className="badge badge-success animate-pulse">✓ Equipado</span>
                      )}
                    </div>
                  </div>
                ))}
              </div>
            ) : (
              <div className="text-center py-16 text-gray-400">
                <div className="inline-block p-6 bg-gray-800/50 rounded-full mb-4">
                  <svg className="w-16 h-16" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
                  </svg>
                </div>
                <p className="text-lg">Nenhum item no inventário</p>
              </div>
            )}
          </Card>

          <Card variant="quest" className="hover:scale-[1.01] transition-transform">
            <h3 className="text-3xl font-bold text-gradient mb-6 flex items-center gap-3">
              <span className="text-4xl">🎯</span>
              Minhas Missões
            </h3>
            {quests.length > 0 ? (
              <div className="space-y-3 max-h-96 overflow-y-auto scrollbar-thin pr-2">
                {quests.slice(0, 5).map((heroQuest) => (
                  <div
                    key={heroQuest.id}
                    className="p-4 bg-gradient-to-r from-gray-800/50 to-gray-900/50 rounded-xl border border-gray-700/30 hover:border-blue-500/50 transition-all hover:scale-[1.02]"
                  >
                    <div className="flex justify-between items-start mb-3">
                      <p className="font-bold text-blue-400 flex-1">{heroQuest.quest.name}</p>
                      {heroQuest.isCompleted ? (
                        <span className="badge badge-success">✓ Completa</span>
                      ) : (
                        <span className="badge badge-info animate-pulse">🔄 Em Progresso</span>
                      )}
                    </div>
                    <div className="flex gap-4 text-sm">
                      <span className="flex items-center gap-1 text-amber-400 font-semibold">
                        <span>🪙</span>
                        {heroQuest.quest.goldReward}
                      </span>
                      <span className="flex items-center gap-1 text-purple-400 font-semibold">
                        <span>⭐</span>
                        {heroQuest.quest.experienceReward} XP
                      </span>
                    </div>
                  </div>
                ))}
              </div>
            ) : (
              <div className="text-center py-16 text-gray-400">
                <div className="inline-block p-6 bg-gray-800/50 rounded-full mb-4">
                  <svg className="w-16 h-16" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
                  </svg>
                </div>
                <p className="text-lg">Nenhuma missão iniciada</p>
              </div>
            )}
          </Card>
        </div>
      </div>
    </>
  );
};
