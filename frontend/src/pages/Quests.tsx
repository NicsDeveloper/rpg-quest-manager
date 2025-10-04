import { useState, useEffect } from 'react';
import { useCharacter } from '../contexts/CharacterContext';
import { questsService, type Quest } from '../services/quests';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Modal } from '../components/ui/Modal';
import { ProgressBar } from '../components/ui/ProgressBar';
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
  Award
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
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <FadeIn delay={0}>
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Missões</h1>
          <p className="text-gray-600 mt-2">Explore o mundo e complete aventuras épicas</p>
        </div>
      </FadeIn>

      {/* Filters */}
      <SlideIn direction="left" delay={100}>
        <Card title="Filtros">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            {/* Filter Type */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Tipo</label>
              <select
                value={filter}
                onChange={(e) => setFilter(e.target.value as any)}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                <option value="all">Todas</option>
                <option value="recommended">Recomendadas</option>
                <option value="available">Disponíveis</option>
                <option value="completed">Completadas</option>
              </select>
            </div>

            {/* Search */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Buscar</label>
              <div className="relative">
                <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
                <input
                  type="text"
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  placeholder="Nome da missão..."
                  className="w-full pl-10 pr-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
              </div>
            </div>

            {/* Refresh Button */}
            <div className="flex items-end">
              <Button
                onClick={loadQuests}
                variant="secondary"
                className="w-full"
              >
                <Filter className="h-4 w-4 mr-2" />
                Atualizar
              </Button>
            </div>
          </div>
        </Card>
      </SlideIn>

      {/* Quests Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {filteredQuests.map((quest, index) => {
          const StatusIcon = getStatusIcon(quest.status || 'available');
          const difficultyColor = getDifficultyColor(quest.difficulty || 'medium');
          const statusColor = getStatusColor(quest.status || 'available');
          
          return (
            <SlideIn key={quest.id} direction="up" delay={100 + (index * 50)}>
              <Card className="hover:shadow-lg transition-shadow cursor-pointer">
                <div
                  onClick={() => {
                    setSelectedQuest(quest);
                    setShowQuestModal(true);
                  }}
                >
                  <div className="flex items-start justify-between mb-3">
                    <StatusIcon className="h-6 w-6 text-gray-600" />
                    <div className="flex space-x-2">
                      <span className={`px-2 py-1 rounded-full text-xs font-medium ${difficultyColor}`}>
                        {quest.difficulty || 'Medium'}
                      </span>
                      <span className={`px-2 py-1 rounded-full text-xs font-medium ${statusColor}`}>
                        {quest.status || 'Available'}
                      </span>
                    </div>
                  </div>
                  
                  <h3 className="font-semibold text-gray-900 mb-2">{quest.title}</h3>
                  <p className="text-sm text-gray-600 mb-3 line-clamp-2">{quest.description}</p>
                  
                  <div className="space-y-2 text-sm">
                    <div className="flex items-center justify-between">
                      <span className="text-gray-500">Ambiente:</span>
                      <span className="font-medium">{getEnvironmentName(quest.environment)}</span>
                    </div>
                    
                    <div className="flex items-center justify-between">
                      <span className="text-gray-500">Nível:</span>
                      <span className="font-medium">{quest.requiredLevel || 1}</span>
                    </div>
                    
                    <div className="flex items-center justify-between">
                      <span className="text-gray-500">XP:</span>
                      <span className="font-medium text-green-600">+{quest.experienceReward}</span>
                    </div>
                  </div>
                  
                  {quest.status === 'in_progress' && quest.progress !== undefined && (
                    <div className="mt-3">
                      <div className="flex items-center justify-between text-xs text-gray-600 mb-1">
                        <span>Progresso</span>
                        <span>{quest.progress}%</span>
                      </div>
                      <ProgressBar value={quest.progress} max={100} />
                    </div>
                  )}
                </div>
              </Card>
            </SlideIn>
          );
        })}
      </div>

      {filteredQuests.length === 0 && (
        <div className="text-center py-12">
          <Map className="h-12 w-12 text-gray-400 mx-auto mb-4" />
          <p className="text-gray-500">Nenhuma missão encontrada</p>
        </div>
      )}

      {/* Quest Details Modal */}
      <Modal
        isOpen={showQuestModal}
        onClose={() => setShowQuestModal(false)}
        title={selectedQuest?.title}
        size="lg"
      >
        {selectedQuest && (
          <div className="space-y-6">
            <div className="flex items-start space-x-4">
              <div className="p-3 bg-gray-100 rounded-lg">
                <Map className="h-8 w-8 text-gray-600" />
              </div>
              <div className="flex-1">
                <div className="flex items-center space-x-2 mb-2">
                  <span className={`px-2 py-1 rounded-full text-xs font-medium ${getDifficultyColor(selectedQuest.difficulty || 'medium')}`}>
                    {selectedQuest.difficulty || 'Medium'}
                  </span>
                  <span className={`px-2 py-1 rounded-full text-xs font-medium ${getStatusColor(selectedQuest.status || 'available')}`}>
                    {selectedQuest.status || 'Available'}
                  </span>
                </div>
                <p className="text-gray-700">{selectedQuest.description}</p>
              </div>
            </div>

            {/* Quest Details */}
            <div className="bg-gray-50 p-4 rounded-lg">
              <h4 className="font-medium text-gray-900 mb-3">Detalhes da Missão</h4>
              <div className="grid grid-cols-2 gap-3 text-sm">
                <div className="flex justify-between">
                  <span className="text-gray-600">Ambiente:</span>
                  <span className="font-medium">{getEnvironmentName(selectedQuest.environment)}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Nível Requerido:</span>
                  <span className="font-medium">{selectedQuest.requiredLevel || 1}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Experiência:</span>
                  <span className="font-medium text-green-600">+{selectedQuest.experienceReward}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Categoria:</span>
                  <span className="font-medium">{selectedQuest.category || 'Geral'}</span>
                </div>
              </div>
            </div>

            {/* Progress */}
            {selectedQuest.status === 'in_progress' && selectedQuest.progress !== undefined && (
              <div className="bg-blue-50 p-4 rounded-lg">
                <h4 className="font-medium text-gray-900 mb-2">Progresso</h4>
                <div className="flex items-center justify-between text-sm text-gray-600 mb-2">
                  <span>Completado</span>
                  <span>{selectedQuest.progress}%</span>
                </div>
                <ProgressBar value={selectedQuest.progress} max={100} />
              </div>
            )}

            {/* Rewards */}
            {selectedQuest.goldReward && (
              <div className="bg-yellow-50 p-4 rounded-lg">
                <h4 className="font-medium text-gray-900 mb-2">Recompensas</h4>
                <div className="flex items-center space-x-2">
                  <Award className="h-5 w-5 text-yellow-600" />
                  <span className="text-sm text-gray-700">
                    {selectedQuest.goldReward} de ouro
                  </span>
                </div>
              </div>
            )}

            {/* Actions */}
            <div className="flex space-x-2">
              {selectedQuest.status === 'available' && (
                <Button
                  onClick={() => handleStartQuest(selectedQuest)}
                  loading={startingQuest}
                  className="flex-1"
                >
                  <Play className="h-4 w-4 mr-2" />
                  Iniciar Missão
                </Button>
              )}
              
              {selectedQuest.status === 'in_progress' && selectedQuest.progress === 100 && (
                <Button
                  onClick={() => handleCompleteQuest(selectedQuest)}
                  className="flex-1"
                >
                  <CheckCircle className="h-4 w-4 mr-2" />
                  Completar
                </Button>
              )}
            </div>
          </div>
        )}
      </Modal>
    </div>
  );
}


