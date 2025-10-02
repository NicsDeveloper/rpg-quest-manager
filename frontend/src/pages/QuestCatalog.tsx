import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Navbar } from '../components/Navbar';
import { Card } from '../components/Card';
import { Button } from '../components/Button';
import { questService, Quest } from '../services/questService';
import { useAuth } from '../contexts/AuthContext';

export const QuestCatalog: React.FC = () => {
  const { t } = useTranslation();
  const { isAdmin } = useAuth();
  const [activeTab, setActiveTab] = useState<'catalog' | 'my-quests'>('catalog');
  const [catalogQuests, setCatalogQuests] = useState<Quest[]>([]);
  const [myQuests, setMyQuests] = useState<Quest[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadQuests();
  }, [activeTab]);

  const loadQuests = async () => {
    setLoading(true);
    try {
      if (activeTab === 'catalog') {
        const data = await questService.getCatalog();
        setCatalogQuests(data);
      } else {
        const data = await questService.getMyQuests();
        setMyQuests(data);
      }
    } catch (error) {
      console.error('Error loading quests:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleAcceptQuest = async (questId: number) => {
    try {
      await questService.accept(questId);
      alert('✅ Missão aceita com sucesso!');
      loadQuests();
    } catch (error: any) {
      alert(error.response?.data?.message || 'Erro ao aceitar missão');
    }
  };

  const getDifficultyColor = (difficulty: string) => {
    const colors: Record<string, string> = {
      Easy: 'from-green-500 to-green-600',
      Medium: 'from-yellow-500 to-yellow-600',
      Hard: 'from-orange-500 to-orange-600',
      Epic: 'from-red-500 to-red-600',
      Legendary: 'from-purple-500 to-purple-600',
    };
    return colors[difficulty] || 'from-gray-500 to-gray-600';
  };

  const getQuestStatus = (quest: any) => {
    if (isAdmin) return null;
    
    if (quest.isAccepted) {
      return (
        <span className="badge badge-success">
          ✓ Aceita
        </span>
      );
    }
    
    if (quest.canAccept) {
      return (
        <span className="badge bg-gradient-to-r from-blue-500 to-blue-600 text-white animate-pulse">
          ⭐ Disponível
        </span>
      );
    }
    
    return (
      <span className="badge bg-gradient-to-r from-gray-600 to-gray-700 text-gray-300">
        🔒 Bloqueada
      </span>
    );
  };

  const renderQuests = () => {
    const quests = activeTab === 'catalog' ? catalogQuests : myQuests;

    if (loading) {
      return (
        <div className="flex items-center justify-center py-20">
          <div className="text-center">
            <div className="inline-block animate-spin rounded-full h-16 w-16 border-t-4 border-b-4 border-amber-500 mb-4"></div>
            <p className="text-gray-400">{t('common.loading')}</p>
          </div>
        </div>
      );
    }

    if (quests.length === 0) {
      return (
        <Card className="text-center py-16">
          <div className="text-6xl mb-4">📜</div>
          <p className="text-xl text-gray-400">
            {activeTab === 'catalog' ? 'Nenhuma missão disponível' : 'Você não tem missões aceitas'}
          </p>
        </Card>
      );
    }

    return (
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {quests.map((quest) => (
          <Card key={quest.id} variant="quest" className="hover:scale-[1.02] transition-all">
            <div className="flex justify-between items-start mb-4">
              <div className="flex-1">
                <h3 className="text-2xl font-bold text-blue-400 mb-2">{quest.name}</h3>
                {getQuestStatus(quest)}
              </div>
              <span className={`ml-3 px-3 py-1 rounded-full text-white text-sm font-bold bg-gradient-to-r ${getDifficultyColor(quest.difficulty)} shadow-lg`}>
                {quest.difficulty}
              </span>
            </div>

            <p className="text-gray-300 text-sm mb-4">{quest.description}</p>

            <div className="grid grid-cols-2 gap-3 mb-4">
              <div className="bg-gray-900/50 rounded-lg p-3">
                <p className="text-xs text-gray-400 mb-1">Tipo</p>
                <p className="font-semibold">{quest.type}</p>
              </div>
              <div className="bg-gray-900/50 rounded-lg p-3">
                <p className="text-xs text-gray-400 mb-1">Nível Mínimo</p>
                <p className="font-semibold text-amber-400">Nv. {quest.requiredLevel}</p>
              </div>
              {quest.requiredClass && quest.requiredClass !== 'Any' && (
                <div className="col-span-2 bg-purple-900/30 rounded-lg p-3 border border-purple-700/30">
                  <p className="text-xs text-purple-400 mb-1">Classe Requerida</p>
                  <p className="font-bold text-purple-300">⚔️ {quest.requiredClass}</p>
                </div>
              )}
            </div>

            <div className="bg-gradient-to-r from-amber-900/30 to-orange-900/30 rounded-lg p-4 mb-4 border border-amber-700/30">
              <p className="text-xs text-amber-500 font-semibold mb-2">🎁 Recompensas</p>
              <div className="flex gap-4 text-sm">
                <span className="flex items-center gap-1 text-amber-400 font-bold">
                  <span>🪙</span> {quest.goldReward}
                </span>
                <span className="flex items-center gap-1 text-purple-400 font-bold">
                  <span>⭐</span> {quest.experienceReward} XP
                </span>
              </div>
            </div>

            {activeTab === 'catalog' && !isAdmin && (
              <div>
                {quest.isAccepted ? (
                  <Button variant="secondary" className="w-full" disabled>
                    ✓ Já Aceita
                  </Button>
                ) : quest.canAccept ? (
                  <Button 
                    variant="primary" 
                    className="w-full animate-pulse"
                    onClick={() => handleAcceptQuest(quest.id)}
                  >
                    ⭐ Aceitar Missão
                  </Button>
                ) : (
                  <Button variant="secondary" className="w-full" disabled>
                    🔒 Requisitos Não Atendidos
                  </Button>
                )}
              </div>
            )}
          </Card>
        ))}
      </div>
    );
  };

  return (
    <>
      <Navbar />
      <div className="container mx-auto px-6 py-8">
        <div className="mb-8 text-center">
          <h1 className="text-5xl font-black mb-3 hero-title animate-float">
            {activeTab === 'catalog' ? '📚 Catálogo de Missões' : '🎯 Minhas Missões'}
          </h1>
          <p className="text-gray-400">
            {activeTab === 'catalog' 
              ? 'Explore e aceite missões épicas para seu herói' 
              : 'Missões que você já aceitou'
            }
          </p>
        </div>

        <div className="flex justify-center gap-4 mb-8">
          <button
            onClick={() => setActiveTab('catalog')}
            className={`px-8 py-4 rounded-xl font-bold text-lg transition-all ${
              activeTab === 'catalog'
                ? 'bg-gradient-to-r from-blue-600 to-blue-700 text-white shadow-lg shadow-blue-500/50 scale-105'
                : 'bg-gray-800 text-gray-400 hover:bg-gray-700'
            }`}
          >
            📚 Catálogo ({catalogQuests.length || '...'})
          </button>
          
          {!isAdmin && (
            <button
              onClick={() => setActiveTab('my-quests')}
              className={`px-8 py-4 rounded-xl font-bold text-lg transition-all ${
                activeTab === 'my-quests'
                  ? 'bg-gradient-to-r from-amber-600 to-orange-600 text-white shadow-lg shadow-amber-500/50 scale-105'
                  : 'bg-gray-800 text-gray-400 hover:bg-gray-700'
              }`}
            >
              🎯 Minhas Missões ({myQuests.length || '...'})
            </button>
          )}
        </div>

        {renderQuests()}
      </div>
    </>
  );
};

