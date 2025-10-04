import { useState, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { achievementService, type Achievement, type UserAchievement, AchievementType, AchievementCategory } from '../services/achievements';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { 
  Trophy, 
  Award, 
  Crown, 
  Gem, 
  Gift,
  Medal,
  Zap,
  Sword,
  Book,
  Compass,
  Users,
  TrendingUp
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

  const handleClaimReward = async (achievementId: number) => {
    if (!user) return;
    
    try {
      await achievementService.claimAchievementReward(user.id, achievementId);
      await loadAchievements();
    } catch (error) {
      console.error('Erro ao reivindicar recompensa:', error);
      alert('Erro ao reivindicar recompensa. Tente novamente.');
    }
  };

  const getCategoryIcon = (category: number) => {
    switch (category) {
      case AchievementCategory.Bronze: return Medal;
      case AchievementCategory.Silver: return Award;
      case AchievementCategory.Gold: return Trophy;
      case AchievementCategory.Platinum: return Crown;
      case AchievementCategory.Legendary: return Gem;
      case AchievementCategory.Mythic: return Zap;
      default: return Trophy;
    }
  };

  const getCategoryColor = (category: number) => {
    switch (category) {
      case AchievementCategory.Bronze: return 'from-amber-500 to-orange-600';
      case AchievementCategory.Silver: return 'from-gray-400 to-gray-600';
      case AchievementCategory.Gold: return 'from-yellow-500 to-amber-600';
      case AchievementCategory.Platinum: return 'from-blue-500 to-indigo-600';
      case AchievementCategory.Legendary: return 'from-purple-500 to-violet-600';
      case AchievementCategory.Mythic: return 'from-pink-500 to-rose-600';
      default: return 'from-gray-500 to-gray-600';
    }
  };

  const getTypeIcon = (type: number) => {
    switch (type) {
      case AchievementType.Combat: return Sword;
      case AchievementType.Quest: return Book;
      case AchievementType.Exploration: return Compass;
      case AchievementType.Collection: return Trophy;
      case AchievementType.Social: return Users;
      case AchievementType.Progression: return TrendingUp;
      case AchievementType.Special: return Gift;
      default: return Trophy;
    }
  };

  const getCategoryName = (category: number): string => {
    switch (category) {
      case AchievementCategory.Bronze: return 'Bronze';
      case AchievementCategory.Silver: return 'Silver';
      case AchievementCategory.Gold: return 'Gold';
      case AchievementCategory.Platinum: return 'Platinum';
      case AchievementCategory.Legendary: return 'Legendary';
      case AchievementCategory.Mythic: return 'Mythic';
      default: return 'Unknown';
    }
  };

  const getTypeName = (type: number): string => {
    switch (type) {
      case AchievementType.Combat: return 'Combate';
      case AchievementType.Quest: return 'Missão';
      case AchievementType.Exploration: return 'Exploração';
      case AchievementType.Collection: return 'Coleção';
      case AchievementType.Social: return 'Social';
      case AchievementType.Progression: return 'Progressão';
      case AchievementType.Special: return 'Especial';
      default: return 'Geral';
    }
  };

  const getUserAchievement = (achievementId: number): UserAchievement | undefined => {
    return userAchievements.find(ua => ua.achievementId === achievementId);
  };

  const filteredAchievements = achievements.filter(achievement => {
    if (achievement.isHidden) return false;
    
    const userAchievement = getUserAchievement(achievement.id);
    
    switch (activeTab) {
      case 'available':
        return !userAchievement;
      case 'completed':
        return userAchievement?.isCompleted && !userAchievement.isClaimed;
      case 'claimed':
        return userAchievement?.isClaimed;
      default:
        return true;
    }
  });

  if (loading) {
    return (
      <div className="flex items-center justify-center py-20">
        <div className="text-center">
          <div className="inline-block animate-spin rounded-full h-16 w-16 border-t-4 border-b-4 border-amber-500 mb-4"></div>
          <p className="text-gray-400">Carregando conquistas...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-6 py-8">
      <div className="mb-8 text-center">
        <h1 className="text-5xl font-black mb-2 hero-title animate-float">🏆 Conquistas</h1>
        <p className="text-gray-400 text-xl">Complete objetivos épicos e desbloqueie recompensas lendárias</p>
      </div>

      {/* Tabs */}
      <div className="mb-8">
        <div className="flex flex-wrap justify-center gap-4">
          {[
            { id: 'all', name: 'Todas', count: achievements.length, icon: '🏆' },
            { id: 'available', name: 'Disponíveis', count: achievements.filter(a => !getUserAchievement(a.id)).length, icon: '⭐' },
            { id: 'completed', name: 'Completadas', count: userAchievements.filter(ua => ua.isCompleted && !ua.isClaimed).length, icon: '✅' },
            { id: 'claimed', name: 'Reivindicadas', count: userAchievements.filter(ua => ua.isClaimed).length, icon: '🎁' }
          ].map((tab) => (
            <button
              key={tab.id}
              onClick={() => setActiveTab(tab.id as any)}
              className={`flex items-center gap-3 px-6 py-4 rounded-2xl font-bold text-lg transition-all duration-300 hover:scale-105 ${
                activeTab === tab.id
                  ? 'bg-gradient-to-r from-amber-500 to-orange-600 text-white shadow-lg shadow-amber-500/50 animate-glow'
                  : 'bg-gradient-to-r from-gray-800/50 to-gray-900/50 text-gray-300 border border-gray-700/50 hover:border-amber-500/30'
              }`}
            >
              <span className="text-2xl">{tab.icon}</span>
              <span>{tab.name}</span>
              <span className="bg-white/20 px-3 py-1 rounded-full text-sm">{tab.count}</span>
            </button>
          ))}
        </div>
      </div>

      {/* Achievements Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {filteredAchievements.map((achievement) => {
          const userAchievement = getUserAchievement(achievement.id);
          const CategoryIcon = getCategoryIcon(achievement.category);
          const TypeIcon = getTypeIcon(achievement.type);
          const categoryColor = getCategoryColor(achievement.category);
          const progress = userAchievement?.progress || 0;
          const isCompleted = userAchievement?.isCompleted || false;
          const isClaimed = userAchievement?.isClaimed || false;
          const progressPercentage = (progress / achievement.requiredValue) * 100;

          return (
            <Card key={achievement.id} variant="epic" className="hover:scale-[1.02] transition-all relative overflow-hidden">
              {/* Header com ícones e categoria */}
              <div className="flex justify-between items-start mb-4">
                <div className="flex items-center gap-3">
                  <div className="flex items-center justify-center w-12 h-12 bg-gradient-to-br from-gray-800/80 to-gray-900/80 rounded-xl border border-gray-700/50">
                    <CategoryIcon className="h-6 w-6 text-amber-400" />
                  </div>
                  <div className="flex items-center justify-center w-10 h-10 bg-gradient-to-br from-gray-700/80 to-gray-800/80 rounded-lg">
                    <TypeIcon className="h-5 w-5 text-blue-400" />
                  </div>
                </div>
                <span className={`px-4 py-2 rounded-full text-white text-sm font-bold bg-gradient-to-r ${categoryColor} shadow-lg`}>
                  {getCategoryName(achievement.category)}
                </span>
              </div>

              {/* Título e Descrição */}
              <div className="mb-4">
                <h3 className="text-2xl font-bold text-blue-400 mb-2">{achievement.name}</h3>
                <p className="text-gray-300 text-sm leading-relaxed">{achievement.description}</p>
              </div>

              {/* Tipo */}
              <div className="mb-4">
                <div className="bg-gray-900/50 rounded-lg p-3 border border-gray-700/30">
                  <p className="text-xs text-gray-400 mb-1">Tipo</p>
                  <p className="font-semibold text-purple-400">{getTypeName(achievement.type)}</p>
                </div>
              </div>

              {/* Progress */}
              <div className="mb-4">
                <div className="flex justify-between items-center mb-2">
                  <span className="text-sm font-medium text-gray-400">Progresso</span>
                  <span className="text-sm text-gray-300 font-bold">
                    {progress}/{achievement.requiredValue}
                  </span>
                </div>
                <div className="progress-bar">
                  <div className="progress-fill" style={{ width: `${progressPercentage}%` }} />
                </div>
              </div>

              {/* Rewards */}
              <div className="bg-gradient-to-r from-amber-900/30 to-orange-900/30 rounded-lg p-4 mb-4 border border-amber-700/30">
                <p className="text-xs text-amber-500 font-semibold mb-2">🎁 Recompensas</p>
                <div className="flex gap-4 text-sm">
                  {achievement.experienceReward > 0 && (
                    <span className="flex items-center gap-1 text-purple-400 font-bold">
                      <span>⭐</span> {achievement.experienceReward} XP
                    </span>
                  )}
                  {achievement.goldReward > 0 && (
                    <span className="flex items-center gap-1 text-amber-400 font-bold">
                      <span>💰</span> {achievement.goldReward}
                    </span>
                  )}
                  {achievement.itemRewardId && (
                    <span className="flex items-center gap-1 text-green-400 font-bold">
                      <span>🎁</span> Item Especial
                    </span>
                  )}
                </div>
              </div>

              {/* Status and Actions */}
              <div className="space-y-2">
                {isCompleted && !isClaimed && (
                  <Button 
                    variant="primary" 
                    className="w-full bg-gradient-to-r from-green-600 to-emerald-600 hover:from-green-700 hover:to-emerald-700 font-bold shadow-lg shadow-green-500/30 animate-pulse"
                    onClick={() => handleClaimReward(achievement.id)}
                  >
                    🎁 Reivindicar Recompensa
                  </Button>
                )}
                
                {isCompleted && isClaimed && (
                  <div className="w-full bg-gradient-to-r from-purple-600 to-violet-600 text-white text-center py-3 rounded-xl font-bold shadow-lg shadow-purple-500/30">
                    ✅ Recompensa Reivindicada
                  </div>
                )}

                {!isCompleted && (
                  <div className="text-center py-2">
                    <p className="text-xs text-gray-400">
                      {progress === 0 ? '🔒 Ainda não iniciada' : '⏳ Em progresso'}
                    </p>
                  </div>
                )}
              </div>

              {/* Completion Badge */}
              {isCompleted && (
                <div className="absolute top-4 right-4">
                  <div className="bg-gradient-to-r from-green-500 to-emerald-600 text-white px-3 py-1 rounded-full text-xs font-bold shadow-lg shadow-green-500/30 animate-glow">
                    {isClaimed ? '🎁 Reivindicada' : '✅ Completa'}
                  </div>
                </div>
              )}
            </Card>
          );
        })}
      </div>

      {filteredAchievements.length === 0 && (
        <Card className="text-center py-16">
          <div className="text-6xl mb-4">🏆</div>
          <h3 className="text-2xl font-bold text-gray-300 mb-2">
            {activeTab === 'all' 
              ? 'Nenhuma conquista encontrada'
              : `Nenhuma conquista ${activeTab} encontrada`
            }
          </h3>
          <p className="text-gray-400">
            {activeTab === 'all' 
              ? 'As conquistas aparecerão aqui quando estiverem disponíveis.'
              : 'Continue jogando para desbloquear mais conquistas!'
            }
          </p>
        </Card>
      )}
    </div>
  );
}
