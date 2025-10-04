import { useState, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { achievementService, type Achievement, type UserAchievement, AchievementType, AchievementCategory } from '../services/achievements';
import { 
  Trophy, 
  Award, 
  Crown, 
  Gem, 
  Gift,
  Medal,
  Sword,
  Book,
  Compass,
  Users,
  TrendingUp,
  Star,
  ArrowLeft,
  CheckCircle,
  Clock,
  Target
} from 'lucide-react';

export default function Achievements() {
  const { user } = useAuth();
  const [achievements, setAchievements] = useState<Achievement[]>([]);
  const [userAchievements, setUserAchievements] = useState<UserAchievement[]>([]);
  const [loading, setLoading] = useState(true);
  const [activeTab, setActiveTab] = useState<'all' | 'available' | 'completed' | 'claimed'>('all');

  useEffect(() => {
    if (user) {
      loadAchievements();
    }
  }, [user?.id]);

  const loadAchievements = async () => {
    if (!user) return;
    
    try {
      setLoading(true);
      const [allAchievements, userAchievementsData] = await Promise.all([
        achievementService.getAllAchievements(),
        achievementService.getUserAchievements(user.id)
      ]);
      
      setAchievements(allAchievements || []);
      setUserAchievements(userAchievementsData || []);
    } catch (error) {
      console.error('Erro ao carregar conquistas:', error);
      setAchievements([]);
      setUserAchievements([]);
    } finally {
      setLoading(false);
    }
  };

  const getAchievementIcon = (type: number) => {
    switch (type) {
      case AchievementType.Combat:
        return <Sword className="w-6 h-6" />;
      case AchievementType.Quest:
        return <Compass className="w-6 h-6" />;
      case AchievementType.Progression:
        return <TrendingUp className="w-6 h-6" />;
      case AchievementType.Collection:
        return <Gift className="w-6 h-6" />;
      case AchievementType.Social:
        return <Users className="w-6 h-6" />;
      case AchievementType.Exploration:
        return <Book className="w-6 h-6" />;
      default:
        return <Trophy className="w-6 h-6" />;
    }
  };

  const getAchievementGradient = (type: number) => {
    switch (type) {
      case AchievementType.Combat:
        return 'from-red-500 to-red-600';
      case AchievementType.Quest:
        return 'from-blue-500 to-blue-600';
      case AchievementType.Progression:
        return 'from-green-500 to-green-600';
      case AchievementType.Collection:
        return 'from-purple-500 to-purple-600';
      case AchievementType.Social:
        return 'from-pink-500 to-pink-600';
      case AchievementType.Exploration:
        return 'from-yellow-500 to-yellow-600';
      default:
        return 'from-gray-500 to-gray-600';
    }
  };

  const getCategoryIcon = (category: number) => {
    switch (category) {
      case AchievementCategory.Bronze:
        return <Medal className="w-5 h-5" />;
      case AchievementCategory.Silver:
        return <Award className="w-5 h-5" />;
      case AchievementCategory.Gold:
        return <Trophy className="w-5 h-5" />;
      case AchievementCategory.Platinum:
        return <Crown className="w-5 h-5" />;
      case AchievementCategory.Legendary:
        return <Gem className="w-5 h-5" />;
      default:
        return <Star className="w-5 h-5" />;
    }
  };

  const getCategoryColor = (category: number) => {
    switch (category) {
      case AchievementCategory.Bronze:
        return 'text-amber-600';
      case AchievementCategory.Silver:
        return 'text-gray-400';
      case AchievementCategory.Gold:
        return 'text-yellow-500';
      case AchievementCategory.Platinum:
        return 'text-purple-400';
      case AchievementCategory.Legendary:
        return 'text-blue-400';
      default:
        return 'text-gray-400';
    }
  };


  const isCompleted = (achievementId: number) => {
    return userAchievements.some(ua => ua.achievementId === achievementId && ua.isCompleted);
  };

  const isClaimed = (achievementId: number) => {
    return userAchievements.some(ua => ua.achievementId === achievementId && ua.isClaimed);
  };

  const getProgress = (achievementId: number) => {
    const userAchievement = userAchievements.find(ua => ua.achievementId === achievementId);
    return userAchievement ? userAchievement.progress : 0;
  };

  const getFilteredAchievements = () => {
    switch (activeTab) {
      case 'completed':
        return achievements.filter(a => isCompleted(a.id));
      case 'claimed':
        return achievements.filter(a => isClaimed(a.id));
      case 'available':
        return achievements.filter(a => !isCompleted(a.id));
      default:
        return achievements;
    }
  };

  const getStats = () => {
    const total = achievements.length;
    const completed = achievements.filter(a => isCompleted(a.id)).length;
    const claimed = achievements.filter(a => isClaimed(a.id)).length;
    const available = total - completed;
    const completionRate = total > 0 ? Math.round((completed / total) * 100) : 0;

    return { total, completed, claimed, available, completionRate };
  };

  const stats = getStats();

  if (loading) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-slate-900 via-purple-900 to-slate-900 flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-32 w-32 border-b-2 border-purple-500 mx-auto mb-4"></div>
          <p className="text-gray-300 text-xl">Carregando conquistas...</p>
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
              onClick={() => window.history.back()}
              className="p-3 bg-gray-800/50 hover:bg-gray-700/50 rounded-xl border border-gray-700/30 transition-all duration-300 hover:scale-105"
            >
              <ArrowLeft className="w-6 h-6 text-gray-300" />
            </button>
            <div className="flex-1">
              <div className="inline-block relative">
                <h1 className="text-6xl font-black mb-2 bg-gradient-to-r from-yellow-400 via-orange-500 to-red-500 bg-clip-text text-transparent animate-pulse">
                  Conquistas
                </h1>
                <div className="absolute -inset-1 bg-gradient-to-r from-yellow-400 via-orange-500 to-red-500 rounded-lg blur opacity-30 animate-pulse"></div>
              </div>
              <p className="text-gray-300 text-xl font-medium">
                Complete desafios e desbloqueie recompensas épicas
              </p>
            </div>
          </div>

          {/* Stats Cards */}
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-5 gap-6">
            <div className="relative group">
              <div className="absolute -inset-0.5 bg-gradient-to-r from-blue-500 to-blue-600 rounded-2xl blur opacity-25 group-hover:opacity-75 transition duration-1000 group-hover:duration-200"></div>
              <div className="relative bg-black/40 backdrop-blur-sm border border-blue-500/20 rounded-2xl p-6 group-hover:border-blue-400/40 transition-all duration-300">
                <div className="flex items-center justify-between mb-4">
                  <div className="p-4 bg-gradient-to-br from-blue-500 to-blue-600 rounded-2xl shadow-2xl">
                    <Trophy className="w-8 h-8 text-white" />
                  </div>
                  <div className="text-right">
                    <span className="text-4xl font-black bg-gradient-to-r from-blue-400 to-blue-600 bg-clip-text text-transparent">
                      {stats.total}
                    </span>
                  </div>
                </div>
                <h3 className="text-blue-300 text-sm font-semibold uppercase tracking-wide">Total</h3>
                <p className="text-gray-400 text-xs mt-1">Conquistas disponíveis</p>
              </div>
            </div>

            <div className="relative group">
              <div className="absolute -inset-0.5 bg-gradient-to-r from-green-500 to-green-600 rounded-2xl blur opacity-25 group-hover:opacity-75 transition duration-1000 group-hover:duration-200"></div>
              <div className="relative bg-black/40 backdrop-blur-sm border border-green-500/20 rounded-2xl p-6 group-hover:border-green-400/40 transition-all duration-300">
                <div className="flex items-center justify-between mb-4">
                  <div className="p-4 bg-gradient-to-br from-green-500 to-green-600 rounded-2xl shadow-2xl">
                    <CheckCircle className="w-8 h-8 text-white" />
                  </div>
                  <div className="text-right">
                    <span className="text-4xl font-black bg-gradient-to-r from-green-400 to-green-600 bg-clip-text text-transparent">
                      {stats.completed}
                    </span>
                  </div>
                </div>
                <h3 className="text-green-300 text-sm font-semibold uppercase tracking-wide">Completas</h3>
                <p className="text-gray-400 text-xs mt-1">Desafios vencidos</p>
              </div>
            </div>

            <div className="relative group">
              <div className="absolute -inset-0.5 bg-gradient-to-r from-purple-500 to-purple-600 rounded-2xl blur opacity-25 group-hover:opacity-75 transition duration-1000 group-hover:duration-200"></div>
              <div className="relative bg-black/40 backdrop-blur-sm border border-purple-500/20 rounded-2xl p-6 group-hover:border-purple-400/40 transition-all duration-300">
                <div className="flex items-center justify-between mb-4">
                  <div className="p-4 bg-gradient-to-br from-purple-500 to-purple-600 rounded-2xl shadow-2xl">
                    <Gift className="w-8 h-8 text-white" />
                  </div>
                  <div className="text-right">
                    <span className="text-4xl font-black bg-gradient-to-r from-purple-400 to-purple-600 bg-clip-text text-transparent">
                      {stats.claimed}
                    </span>
                  </div>
                </div>
                <h3 className="text-purple-300 text-sm font-semibold uppercase tracking-wide">Reivindicadas</h3>
                <p className="text-gray-400 text-xs mt-1">Recompensas coletadas</p>
              </div>
            </div>

            <div className="relative group">
              <div className="absolute -inset-0.5 bg-gradient-to-r from-amber-500 to-orange-600 rounded-2xl blur opacity-25 group-hover:opacity-75 transition duration-1000 group-hover:duration-200"></div>
              <div className="relative bg-black/40 backdrop-blur-sm border border-amber-500/20 rounded-2xl p-6 group-hover:border-amber-400/40 transition-all duration-300">
                <div className="flex items-center justify-between mb-4">
                  <div className="p-4 bg-gradient-to-br from-amber-500 to-orange-600 rounded-2xl shadow-2xl">
                    <Target className="w-8 h-8 text-white" />
                  </div>
                  <div className="text-right">
                    <span className="text-4xl font-black bg-gradient-to-r from-amber-400 to-orange-600 bg-clip-text text-transparent">
                      {stats.available}
                    </span>
                  </div>
                </div>
                <h3 className="text-amber-300 text-sm font-semibold uppercase tracking-wide">Disponíveis</h3>
                <p className="text-gray-400 text-xs mt-1">Para completar</p>
              </div>
            </div>

            <div className="relative group">
              <div className="absolute -inset-0.5 bg-gradient-to-r from-red-500 to-pink-600 rounded-2xl blur opacity-25 group-hover:opacity-75 transition duration-1000 group-hover:duration-200"></div>
              <div className="relative bg-black/40 backdrop-blur-sm border border-red-500/20 rounded-2xl p-6 group-hover:border-red-400/40 transition-all duration-300">
                <div className="flex items-center justify-between mb-4">
                  <div className="p-4 bg-gradient-to-br from-red-500 to-pink-600 rounded-2xl shadow-2xl">
                    <TrendingUp className="w-8 h-8 text-white" />
                  </div>
                  <div className="text-right">
                    <span className="text-4xl font-black bg-gradient-to-r from-red-400 to-pink-600 bg-clip-text text-transparent">
                      {stats.completionRate}%
                    </span>
                  </div>
                </div>
                <h3 className="text-red-300 text-sm font-semibold uppercase tracking-wide">Taxa</h3>
                <p className="text-gray-400 text-xs mt-1">De conclusão</p>
              </div>
            </div>
          </div>
        </div>

        {/* Tabs */}
        <div className="mb-8">
          <div className="flex flex-wrap gap-2 p-2 bg-gray-800/30 rounded-2xl border border-gray-700/30">
            {[
              { key: 'all', label: 'Todas', icon: Trophy },
              { key: 'available', label: 'Disponíveis', icon: Target },
              { key: 'completed', label: 'Completas', icon: CheckCircle },
              { key: 'claimed', label: 'Reivindicadas', icon: Gift }
            ].map(({ key, label, icon: Icon }) => (
              <button
                key={key}
                onClick={() => setActiveTab(key as any)}
                className={`flex items-center gap-2 px-4 py-2 rounded-xl font-semibold transition-all duration-300 ${
                  activeTab === key
                    ? 'bg-gradient-to-r from-purple-500 to-pink-500 text-white shadow-lg'
                    : 'text-gray-400 hover:text-white hover:bg-gray-700/50'
                }`}
              >
                <Icon className="w-4 h-4" />
                {label}
              </button>
            ))}
          </div>
        </div>

        {/* Achievements Grid */}
        {getFilteredAchievements().length === 0 ? (
          <div className="text-center py-20">
            <div className="relative mx-auto w-32 h-32 mb-8">
              <div className="absolute inset-0 bg-gradient-to-br from-yellow-500/20 to-orange-500/20 rounded-full blur-xl"></div>
              <div className="relative p-8 bg-gradient-to-br from-gray-800/50 to-gray-900/50 rounded-full border border-gray-700/30">
                <Trophy className="w-16 h-16 text-gray-400" />
              </div>
            </div>
            <h3 className="text-3xl font-bold text-gray-200 mb-4">
              {activeTab === 'all' ? 'Nenhuma conquista disponível' : 
               activeTab === 'available' ? 'Todas as conquistas foram completadas!' :
               activeTab === 'completed' ? 'Nenhuma conquista completada ainda' :
               'Nenhuma conquista reivindicada ainda'}
            </h3>
            <p className="text-gray-400 mb-8 max-w-md mx-auto">
              {activeTab === 'available' ? 
                'Parabéns! Você completou todas as conquistas disponíveis. Continue jogando para descobrir novos desafios!' :
                'Continue explorando o jogo para descobrir e completar novas conquistas épicas!'
              }
            </p>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
            {getFilteredAchievements().map((achievement) => {
              const completed = isCompleted(achievement.id);
              const claimed = isClaimed(achievement.id);
              const progress = getProgress(achievement.id);
              
              return (
                <div key={achievement.id} className="relative group">
                  <div className={`absolute -inset-0.5 bg-gradient-to-r ${getAchievementGradient(achievement.type)} rounded-3xl blur opacity-0 group-hover:opacity-20 transition duration-300 ${completed ? 'opacity-30' : ''}`}></div>
                  <div className={`relative bg-black/40 backdrop-blur-sm border rounded-3xl p-6 transition-all duration-300 ${
                    completed 
                      ? 'border-green-500/40 group-hover:border-green-400/60' 
                      : 'border-gray-700/30 group-hover:border-gray-600/50'
                  }`}>
                    {/* Achievement Header */}
                    <div className="flex items-center gap-4 mb-6">
                      <div className={`p-4 bg-gradient-to-br ${getAchievementGradient(achievement.type)} rounded-2xl shadow-2xl ${
                        completed ? 'ring-4 ring-green-500/50' : ''
                      }`}>
                        {getAchievementIcon(achievement.type)}
                      </div>
                      <div className="flex-1">
                        <div className="flex items-center gap-2 mb-1">
                          <h3 className="text-xl font-bold text-white">{achievement.name}</h3>
                          {completed && <CheckCircle className="w-5 h-5 text-green-500" />}
                          {claimed && <Gift className="w-5 h-5 text-purple-500" />}
                        </div>
                        <div className="flex items-center gap-2">
                          <span className={`text-sm ${getCategoryColor(achievement.category)}`}>
                            {getCategoryIcon(achievement.category)}
                          </span>
                          <span className="text-gray-400 text-sm capitalize">
                            {Object.keys(AchievementCategory)[achievement.category]?.toLowerCase() || 'unknown'}
                          </span>
                        </div>
                      </div>
                    </div>

                    {/* Achievement Description */}
                    <p className="text-gray-300 text-sm mb-4 leading-relaxed">
                      {achievement.description}
                    </p>

                    {/* Progress Bar */}
                    {!completed && achievement.requiredValue > 1 && (
                      <div className="mb-4">
                        <div className="flex justify-between text-xs text-gray-400 mb-2">
                          <span>Progresso</span>
                          <span>{progress}/{achievement.requiredValue}</span>
                        </div>
                        <div className="w-full bg-gray-700/50 rounded-full h-2">
                          <div 
                            className={`bg-gradient-to-r ${getAchievementGradient(achievement.type)} h-2 rounded-full transition-all duration-500`}
                            style={{ width: `${Math.min((progress / achievement.requiredValue) * 100, 100)}%` }}
                          ></div>
                        </div>
                      </div>
                    )}

                    {/* Reward */}
                    {(achievement.goldReward > 0 || achievement.experienceReward > 0 || achievement.itemRewardId) && (
                      <div className="p-3 bg-gray-800/30 rounded-xl border border-gray-700/30 mb-4">
                        <div className="flex items-center gap-2 text-sm">
                          <Gift className="w-4 h-4 text-purple-400" />
                          <span className="text-gray-300">Recompensa:</span>
                          <div className="flex gap-2">
                            {achievement.goldReward > 0 && (
                              <span className="text-yellow-400 font-semibold">{achievement.goldReward} ouro</span>
                            )}
                            {achievement.experienceReward > 0 && (
                              <span className="text-blue-400 font-semibold">{achievement.experienceReward} XP</span>
                            )}
                            {achievement.itemRewardId && (
                              <span className="text-purple-400 font-semibold">Item especial</span>
                            )}
                          </div>
                        </div>
                      </div>
                    )}

                    {/* Status */}
                    <div className="flex items-center justify-between">
                      <div className="flex items-center gap-2">
                        {completed ? (
                          <div className="flex items-center gap-2 text-green-400">
                            <CheckCircle className="w-4 h-4" />
                            <span className="text-sm font-semibold">Completa</span>
                          </div>
                        ) : (
                          <div className="flex items-center gap-2 text-amber-400">
                            <Clock className="w-4 h-4" />
                            <span className="text-sm font-semibold">Em progresso</span>
                          </div>
                        )}
                      </div>
                      
                      {claimed && (
                        <div className="flex items-center gap-2 text-purple-400">
                          <Gift className="w-4 h-4" />
                          <span className="text-sm font-semibold">Reivindicada</span>
                        </div>
                      )}
                    </div>

                    {/* Shine effect for completed achievements */}
                    {completed && (
                      <div className="absolute top-0 right-0 w-20 h-20 bg-gradient-to-br from-yellow-400/20 to-transparent rounded-full blur-xl pointer-events-none"></div>
                    )}
                  </div>
                </div>
              );
            })}
          </div>
        )}
      </div>
    </div>
  );
}