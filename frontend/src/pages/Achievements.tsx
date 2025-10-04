import { useState, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { achievementService, type Achievement, type UserAchievement } from '../services/achievements';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { ProgressBar } from '../components/ui/ProgressBar';
import { 
  Trophy, 
  Star, 
  Award, 
  Crown, 
  Gem, 
  Target,
  CheckCircle,
  Clock,
  Gift
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
  }, [user]);

  const loadAchievements = async () => {
    if (!user) return;
    
    try {
      setLoading(true);
      const [allAchievements, userAchievementsData] = await Promise.all([
        achievementService.getAllAchievements(),
        achievementService.getUserAchievements(user.id)
      ]);
      
      setAchievements(allAchievements);
      setUserAchievements(userAchievementsData);
    } catch (error) {
      console.error('Erro ao carregar conquistas:', error);
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
    }
  };

  const getCategoryIcon = (category: string) => {
    switch (category.toLowerCase()) {
      case 'bronze': return Trophy;
      case 'silver': return Star;
      case 'gold': return Award;
      case 'platinum': return Crown;
      case 'legendary': return Gem;
      case 'mythic': return Target;
      default: return Trophy;
    }
  };

  const getCategoryColor = (category: string) => {
    switch (category.toLowerCase()) {
      case 'bronze': return 'bg-amber-100 text-amber-800';
      case 'silver': return 'bg-gray-100 text-gray-800';
      case 'gold': return 'bg-yellow-100 text-yellow-800';
      case 'platinum': return 'bg-blue-100 text-blue-800';
      case 'legendary': return 'bg-purple-100 text-purple-800';
      case 'mythic': return 'bg-pink-100 text-pink-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  const getTypeIcon = (type: string) => {
    switch (type.toLowerCase()) {
      case 'combat': return Target;
      case 'quest': return Star;
      case 'exploration': return Gem;
      case 'collection': return Trophy;
      case 'social': return Crown;
      case 'progression': return Award;
      case 'special': return Gift;
      default: return Trophy;
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
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Conquistas</h1>
        <p className="text-gray-600 mt-2">Complete objetivos e desbloqueie recompensas</p>
      </div>

      {/* Tabs */}
      <div className="border-b border-gray-200">
        <nav className="-mb-px flex space-x-8">
          {[
            { id: 'all', name: 'Todas', count: achievements.length },
            { id: 'available', name: 'Disponíveis', count: achievements.filter(a => !getUserAchievement(a.id)).length },
            { id: 'completed', name: 'Completadas', count: userAchievements.filter(ua => ua.isCompleted && !ua.isClaimed).length },
            { id: 'claimed', name: 'Reivindicadas', count: userAchievements.filter(ua => ua.isClaimed).length }
          ].map((tab) => (
            <button
              key={tab.id}
              onClick={() => setActiveTab(tab.id as any)}
              className={`py-2 px-1 border-b-2 font-medium text-sm ${
                activeTab === tab.id
                  ? 'border-blue-500 text-blue-600'
                  : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
              }`}
            >
              {tab.name} ({tab.count})
            </button>
          ))}
        </nav>
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

          return (
            <Card key={achievement.id} className="relative">
              <div className="flex items-start justify-between mb-3">
                <div className="flex items-center space-x-2">
                  <CategoryIcon className="h-6 w-6 text-gray-600" />
                  <TypeIcon className="h-5 w-5 text-gray-500" />
                </div>
                <span className={`px-2 py-1 rounded-full text-xs font-medium ${categoryColor}`}>
                  {achievement.category}
                </span>
              </div>

              <h3 className="font-semibold text-gray-900 mb-2">{achievement.name}</h3>
              <p className="text-sm text-gray-600 mb-4">{achievement.description}</p>

              {/* Progress */}
              <div className="mb-4">
                <div className="flex justify-between items-center mb-1">
                  <span className="text-sm font-medium text-gray-700">Progresso</span>
                  <span className="text-sm text-gray-600">
                    {progress}/{achievement.requiredValue}
                  </span>
                </div>
                <ProgressBar 
                  value={progress} 
                  max={achievement.requiredValue} 
                  color={isCompleted ? 'green' : 'blue'}
                  showPercentage={false}
                />
              </div>

              {/* Rewards */}
              <div className="bg-gray-50 p-3 rounded-lg mb-4">
                <h4 className="text-sm font-medium text-gray-900 mb-2">Recompensas</h4>
                <div className="space-y-1 text-sm">
                  {achievement.experienceReward > 0 && (
                    <div className="flex justify-between">
                      <span className="text-gray-600">Experiência:</span>
                      <span className="font-medium text-blue-600">+{achievement.experienceReward}</span>
                    </div>
                  )}
                  {achievement.goldReward > 0 && (
                    <div className="flex justify-between">
                      <span className="text-gray-600">Ouro:</span>
                      <span className="font-medium text-yellow-600">+{achievement.goldReward}</span>
                    </div>
                  )}
                  {achievement.itemRewardId && (
                    <div className="flex justify-between">
                      <span className="text-gray-600">Item:</span>
                      <span className="font-medium text-purple-600">Especial</span>
                    </div>
                  )}
                </div>
              </div>

              {/* Status and Actions */}
              <div className="flex items-center justify-between">
                <div className="flex items-center space-x-2">
                  {isCompleted && (
                    <CheckCircle className="h-5 w-5 text-green-500" />
                  )}
                  {isClaimed && (
                    <Gift className="h-5 w-5 text-purple-500" />
                  )}
                  {!isCompleted && (
                    <Clock className="h-5 w-5 text-gray-400" />
                  )}
                </div>

                {isCompleted && !isClaimed && (
                  <Button
                    size="sm"
                    onClick={() => handleClaimReward(achievement.id)}
                  >
                    Reivindicar
                  </Button>
                )}
              </div>

              {/* Completion Badge */}
              {isCompleted && (
                <div className="absolute top-4 right-4">
                  <div className="bg-green-100 text-green-800 px-2 py-1 rounded-full text-xs font-medium">
                    {isClaimed ? 'Reivindicada' : 'Completa'}
                  </div>
                </div>
              )}
            </Card>
          );
        })}
      </div>

      {filteredAchievements.length === 0 && (
        <div className="text-center py-12">
          <Trophy className="h-12 w-12 text-gray-400 mx-auto mb-4" />
          <p className="text-gray-500">
            {activeTab === 'all' 
              ? 'Nenhuma conquista encontrada'
              : `Nenhuma conquista ${activeTab} encontrada`
            }
          </p>
        </div>
      )}
    </div>
  );
}
