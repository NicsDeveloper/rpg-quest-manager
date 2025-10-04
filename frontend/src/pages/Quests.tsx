import { useState, useEffect } from 'react';
import { useCharacter } from '../contexts/CharacterContext';
import { questsService, type Quest } from '../services/quests';
import { FadeIn, SlideIn } from '../components/animations';
import { 
  Map, 
  Target, 
  Search, 
  Filter,
  Play,
  CheckCircle,
  XCircle,
  Clock,
  Award,
  X
} from 'lucide-react';

export default function Quests() {
  const { character, refreshCharacter } = useCharacter();
  const [quests, setQuests] = useState<Quest[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedQuest, setSelectedQuest] = useState<Quest | null>(null);
  const [showQuestModal, setShowQuestModal] = useState(false);
  const [filter, setFilter] = useState<'all' | 'recommended' | 'available' | 'completed'>('all');
  const [searchTerm, setSearchTerm] = useState('');
  const [startingQuest, setStartingQuest] = useState(false);

  useEffect(() => {
    if (character) {
      loadQuests();
    }
  }, [character, filter]);

  const loadQuests = async () => {
    if (!character) return;
    
    try {
      setLoading(true);
      let questsData: Quest[] = [];
      
      switch (filter) {
        case 'recommended':
          questsData = await questsService.getRecommendedQuests(character.level);
          break;
        case 'available':
          questsData = await questsService.getAvailableQuests(character.id);
          break;
        case 'completed':
          questsData = await questsService.getCompletedQuests(character.id);
          break;
        default:
          questsData = await questsService.getAllQuests();
      }
      
      setQuests(questsData);
    } catch (error) {
      console.error('Erro ao carregar quests:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleStartQuest = async (quest: Quest) => {
    if (!character) return;
    
    try {
      setStartingQuest(true);
      await questsService.startQuest(character.id, quest.id);
      await refreshCharacter();
      setShowQuestModal(false);
      await loadQuests();
    } catch (error: any) {
      alert(error.message || 'Erro ao iniciar quest');
    } finally {
      setStartingQuest(false);
    }
  };

  const handleCompleteQuest = async (quest: Quest) => {
    if (!character) return;
    
    try {
      await questsService.completeQuest(character.id, quest.id);
      await refreshCharacter();
      setShowQuestModal(false);
      await loadQuests();
    } catch (error: any) {
      alert(error.message || 'Erro ao completar quest');
    }
  };

  const getEnvironmentName = (environment: number) => {
    const environments = ['Floresta', 'Caverna', 'Montanha', 'Deserto', 'Cidade', 'Ruínas'];
    return environments[environment] || 'Desconhecido';
  };

  const getDifficultyColor = (difficulty: string) => {
    switch (difficulty.toLowerCase()) {
      case 'easy': return 'bg-green-100 text-green-800';
      case 'medium': return 'bg-yellow-100 text-yellow-800';
      case 'hard': return 'bg-red-100 text-red-800';
      case 'epic': return 'bg-purple-100 text-purple-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  const getStatusColor = (status: string) => {
    switch (status.toLowerCase()) {
      case 'available': return 'bg-blue-100 text-blue-800';
      case 'in_progress': return 'bg-yellow-100 text-yellow-800';
      case 'completed': return 'bg-green-100 text-green-800';
      case 'failed': return 'bg-red-100 text-red-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  const getStatusIcon = (status: string) => {
    switch (status.toLowerCase()) {
      case 'available': return Play;
      case 'in_progress': return Clock;
      case 'completed': return CheckCircle;
      case 'failed': return XCircle;
      default: return Target;
    }
  };

  const filteredQuests = quests.filter(quest => 
    searchTerm === '' || 
    quest.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
    quest.description.toLowerCase().includes(searchTerm.toLowerCase())
  );

  if (loading) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-slate-900 via-purple-900 to-slate-900 flex items-center justify-center relative overflow-hidden">
        <div className="absolute inset-0 overflow-hidden">
          {[...Array(15)].map((_, i) => (
            <div
              key={i}
              className="absolute w-2 h-2 bg-amber-400 rounded-full opacity-30 animate-pulse"
              style={{
                left: `${Math.random() * 100}%`,
                top: `${Math.random() * 100}%`,
                animationDelay: `${Math.random() * 3}s`,
                animationDuration: `${2 + Math.random() * 3}s`
              }}
            />
          ))}
        </div>
        <div className="text-center relative z-10">
          <div className="inline-block p-6 bg-gradient-to-br from-amber-500 to-orange-600 rounded-full shadow-lg shadow-amber-500/50 animate-pulse mb-4">
            <div className="animate-spin rounded-full h-16 w-16 border-4 border-white border-t-transparent"></div>
          </div>
          <h2 className="text-4xl font-black text-transparent bg-clip-text bg-gradient-to-r from-amber-300 via-amber-500 to-orange-600 mb-2">Carregando Missões...</h2>
          <p className="text-gray-400 text-lg">Preparando aventuras épicas</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-purple-900 to-slate-900 p-6">
      <div className="max-w-7xl mx-auto space-y-8">
        <FadeIn delay={0}>
          <div className="text-center">
            <h1 className="hero-title text-6xl font-black mb-4">Missões</h1>
            <p className="text-xl text-gray-300">Explore o mundo e complete aventuras épicas</p>
          </div>
        </FadeIn>

        {/* Filters */}
        <SlideIn direction="left" delay={100}>
          <div className="card backdrop-blur-sm bg-black/20">
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
              {/* Filter Type */}
              <div>
                <label className="label text-amber-400 mb-2">Tipo</label>
                <select
                  value={filter}
                  onChange={(e) => setFilter(e.target.value as any)}
                  className="input w-full"
                >
                  <option value="all">Todas</option>
                  <option value="recommended">Recomendadas</option>
                  <option value="available">Disponíveis</option>
                  <option value="completed">Completadas</option>
                </select>
              </div>

              {/* Search */}
              <div>
                <label className="label text-amber-400 mb-2">Buscar</label>
                <div className="relative">
                  <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-5 w-5 text-amber-400" />
                  <input
                    type="text"
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    placeholder="Nome da missão..."
                    className="input w-full pl-12"
                  />
                </div>
              </div>

              {/* Refresh Button */}
              <div className="flex items-end">
                <button
                  onClick={loadQuests}
                  className="btn btn-secondary w-full"
                >
                  <Filter className="h-5 w-5 mr-2" />
                  Atualizar
                </button>
              </div>
            </div>
          </div>
        </SlideIn>

        {/* Quests Grid */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {filteredQuests.map((quest, index) => {
            const StatusIcon = getStatusIcon(quest.status || 'available');
            const difficultyColor = getDifficultyColor(quest.difficulty || 'medium');
            const statusColor = getStatusColor(quest.status || 'available');
            
            return (
              <SlideIn key={quest.id} direction="up" delay={100 + (index * 50)}>
                <div 
                  className="card backdrop-blur-sm bg-black/20 hover:bg-black/30 transition-all duration-300 cursor-pointer group hover:scale-105"
                  onClick={() => {
                    setSelectedQuest(quest);
                    setShowQuestModal(true);
                  }}
                >
                  <div className="flex items-start justify-between mb-4">
                    <div className="p-3 bg-gradient-to-br from-amber-500 to-orange-600 rounded-xl shadow-lg group-hover:shadow-amber-500/50 transition-shadow">
                      <StatusIcon className="h-6 w-6 text-white" />
                    </div>
                    <div className="flex space-x-2">
                      <span className={`px-3 py-1 rounded-full text-xs font-bold ${difficultyColor}`}>
                        {quest.difficulty || 'Medium'}
                      </span>
                      <span className={`px-3 py-1 rounded-full text-xs font-bold ${statusColor}`}>
                        {quest.status || 'Available'}
                      </span>
                    </div>
                  </div>
                  
                  <h3 className="text-xl font-bold text-gradient mb-3">{quest.title}</h3>
                  <p className="text-gray-300 mb-4 line-clamp-2">{quest.description}</p>
                  
                  <div className="space-y-3 text-sm">
                    <div className="flex items-center justify-between">
                      <span className="text-gray-400">Ambiente:</span>
                      <span className="font-bold text-amber-400">{getEnvironmentName(quest.environment)}</span>
                    </div>
                    
                    <div className="flex items-center justify-between">
                      <span className="text-gray-400">Nível:</span>
                      <span className="font-bold text-blue-400">{quest.requiredLevel || 1}</span>
                    </div>
                    
                    <div className="flex items-center justify-between">
                      <span className="text-gray-400">XP:</span>
                      <span className="font-bold text-green-400">+{quest.experienceReward}</span>
                    </div>
                  </div>
                  
                  {quest.status === 'in_progress' && quest.progress !== undefined && (
                    <div className="mt-4">
                      <div className="flex items-center justify-between text-sm text-gray-300 mb-2">
                        <span>Progresso</span>
                        <span className="font-bold text-amber-400">{quest.progress}%</span>
                      </div>
                      <div className="progress-bar">
                        <div
                          className="progress-fill"
                          style={{ width: `${quest.progress}%` }}
                        />
                      </div>
                    </div>
                  )}
                </div>
              </SlideIn>
            );
          })}
        </div>

        {filteredQuests.length === 0 && (
          <div className="text-center py-16">
            <div className="p-8 bg-gradient-to-br from-amber-500 to-orange-600 rounded-full shadow-lg shadow-amber-500/50 mx-auto w-24 h-24 flex items-center justify-center mb-6">
              <Map className="h-12 w-12 text-white" />
            </div>
            <h3 className="text-2xl font-bold text-gradient mb-2">Nenhuma missão encontrada</h3>
            <p className="text-gray-400">Tente ajustar os filtros ou buscar por outro termo</p>
          </div>
        )}

        {/* Quest Details Modal */}
        {showQuestModal && selectedQuest && (
          <div className="fixed inset-0 bg-black/70 backdrop-blur-sm flex items-center justify-center z-50 p-4">
            <div className="card backdrop-blur-sm bg-black/30 w-full max-w-2xl max-h-[90vh] overflow-y-auto animate-fadeIn">
              <div className="flex items-center justify-between mb-6">
                <div className="flex items-center space-x-4">
                  <div className="p-4 bg-gradient-to-br from-amber-500 to-orange-600 rounded-xl shadow-lg">
                    <Map className="h-8 w-8 text-white" />
                  </div>
                  <div>
                    <h2 className="text-2xl font-bold text-gradient">
                      {selectedQuest.title}
                    </h2>
                    <div className="flex items-center space-x-2 mt-2">
                      <span className={`px-3 py-1 rounded-full text-xs font-bold ${getDifficultyColor(selectedQuest.difficulty || 'medium')}`}>
                        {selectedQuest.difficulty || 'Medium'}
                      </span>
                      <span className={`px-3 py-1 rounded-full text-xs font-bold ${getStatusColor(selectedQuest.status || 'available')}`}>
                        {selectedQuest.status || 'Available'}
                      </span>
                    </div>
                  </div>
                </div>
                <button
                  onClick={() => setShowQuestModal(false)}
                  className="text-gray-400 hover:text-amber-400 transition-colors p-2 rounded-lg hover:bg-gray-800/50"
                >
                  <X className="h-6 w-6" />
                </button>
              </div>

              <div className="space-y-6">
                <div>
                  <p className="text-gray-300 leading-relaxed">{selectedQuest.description}</p>
                </div>

                {/* Quest Details */}
                <div className="card bg-black/20 backdrop-blur-sm">
                  <h4 className="text-lg font-bold text-gradient mb-4">Detalhes da Missão</h4>
                  <div className="grid grid-cols-2 gap-4 text-sm">
                    <div className="flex justify-between">
                      <span className="text-gray-400">Ambiente:</span>
                      <span className="font-bold text-amber-400">{getEnvironmentName(selectedQuest.environment)}</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-400">Nível Requerido:</span>
                      <span className="font-bold text-blue-400">{selectedQuest.requiredLevel || 1}</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-400">Experiência:</span>
                      <span className="font-bold text-green-400">+{selectedQuest.experienceReward}</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-400">Categoria:</span>
                      <span className="font-bold text-purple-400">{selectedQuest.category || 'Geral'}</span>
                    </div>
                  </div>
                </div>

                {/* Progress */}
                {selectedQuest.status === 'in_progress' && selectedQuest.progress !== undefined && (
                  <div className="card bg-blue-900/20 backdrop-blur-sm">
                    <h4 className="text-lg font-bold text-gradient mb-3">Progresso</h4>
                    <div className="flex items-center justify-between text-sm text-gray-300 mb-3">
                      <span>Completado</span>
                      <span className="font-bold text-amber-400">{selectedQuest.progress}%</span>
                    </div>
                    <div className="progress-bar">
                      <div
                        className="progress-fill"
                        style={{ width: `${selectedQuest.progress}%` }}
                      />
                    </div>
                  </div>
                )}

                {/* Rewards */}
                {selectedQuest.goldReward && (
                  <div className="card bg-yellow-900/20 backdrop-blur-sm">
                    <h4 className="text-lg font-bold text-gradient mb-3">Recompensas</h4>
                    <div className="flex items-center space-x-3">
                      <div className="p-2 bg-gradient-to-br from-yellow-500 to-orange-600 rounded-lg">
                        <Award className="h-6 w-6 text-white" />
                      </div>
                      <span className="text-gray-300 font-bold">
                        {selectedQuest.goldReward} de ouro
                      </span>
                    </div>
                  </div>
                )}

                {/* Actions */}
                <div className="flex space-x-3">
                  {selectedQuest.status === 'available' && (
                    <button
                      onClick={() => handleStartQuest(selectedQuest)}
                      disabled={startingQuest}
                      className="btn btn-primary flex-1"
                    >
                      {startingQuest ? (
                        <div className="animate-spin rounded-full h-5 w-5 border-2 border-white border-t-transparent mr-2"></div>
                      ) : (
                        <Play className="h-5 w-5 mr-2" />
                      )}
                      Iniciar Missão
                    </button>
                  )}
                  
                  {selectedQuest.status === 'in_progress' && selectedQuest.progress === 100 && (
                    <button
                      onClick={() => handleCompleteQuest(selectedQuest)}
                      className="btn btn-primary flex-1"
                    >
                      <CheckCircle className="h-5 w-5 mr-2" />
                      Completar
                    </button>
                  )}
                </div>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}


