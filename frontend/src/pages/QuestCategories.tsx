import React, { useState, useEffect } from 'react';
import { Card } from '../components/Card';
import { questCategoryService, QuestCategory, Quest, Monster } from '../services/questCategoryService';

export const QuestCategories: React.FC = () => {
  const [categories, setCategories] = useState<QuestCategory[]>([]);
  const [selectedCategory, setSelectedCategory] = useState<QuestCategory | null>(null);
  const [quests, setQuests] = useState<Quest[]>([]);
  const [monsters, setMonsters] = useState<Monster[]>([]);
  const [loading, setLoading] = useState(true);
  const [activeTab, setActiveTab] = useState<'quests' | 'monsters'>('quests');

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const [categoriesData, questsData, monstersData] = await Promise.all([
        questCategoryService.getCategories(),
        questCategoryService.getAllQuests(),
        questCategoryService.getMonsters()
      ]);
      
      setCategories(categoriesData);
      setQuests(questsData);
      setMonsters(monstersData);
    } catch (error) {
      console.error('Erro ao carregar dados:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleCategorySelect = (category: QuestCategory) => {
    setSelectedCategory(category);
    const categoryQuests = quests.filter(q => q.categoryId === category.id);
    setQuests(categoryQuests);
  };

  const getQuestDifficultyColor = (difficulty: string) => {
    return questCategoryService.getDifficultyColor(difficulty);
  };

  const getQuestDifficultyIcon = (difficulty: string) => {
    return questCategoryService.getDifficultyIcon(difficulty);
  };

  const getQuestTypeIcon = (type: string) => {
    return questCategoryService.getTypeIcon(type);
  };

  const getEnvironmentIcon = (environment: string) => {
    return questCategoryService.getEnvironmentIcon(environment);
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-gray-900 via-blue-900 to-purple-900 flex items-center justify-center">
        <div className="text-white text-xl">Carregando...</div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-900 via-blue-900 to-purple-900 p-6">
      <div className="max-w-7xl mx-auto">
        {/* Header */}
        <div className="text-center mb-8">
          <h1 className="text-4xl font-bold text-white mb-4">
            🎮 Sistema de Missões Épico
          </h1>
          <p className="text-xl text-gray-300">
            Explore missões, monstros e bosses redesenhados com sistema completo de classificação
          </p>
        </div>

        {/* Tabs */}
        <div className="flex justify-center mb-8">
          <div className="bg-gray-800/50 rounded-lg p-1 flex gap-2">
            <button
              onClick={() => setActiveTab('quests')}
              className={`px-6 py-3 rounded-lg font-semibold transition-all ${
                activeTab === 'quests'
                  ? 'bg-blue-600 text-white'
                  : 'text-gray-300 hover:text-white'
              }`}
            >
              📖 Missões
            </button>
            <button
              onClick={() => setActiveTab('monsters')}
              className={`px-6 py-3 rounded-lg font-semibold transition-all ${
                activeTab === 'monsters'
                  ? 'bg-red-600 text-white'
                  : 'text-gray-300 hover:text-white'
              }`}
            >
              👹 Monstros
            </button>
          </div>
        </div>

        {activeTab === 'quests' && (
          <>
            {/* Categories */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-5 gap-6 mb-8">
              {categories.map((category) => (
                <Card
                  key={category.id}
                  className={`cursor-pointer transition-all hover:scale-105 ${
                    selectedCategory?.id === category.id
                      ? 'ring-2 ring-blue-500'
                      : ''
                  }`}
                  onClick={() => handleCategorySelect(category)}
                >
                  <div className="text-center">
                    <div className="text-4xl mb-3">{category.icon}</div>
                    <h3 className="text-lg font-bold text-white mb-2">{category.name}</h3>
                    <p className="text-sm text-gray-300 mb-3">{category.description}</p>
                    
                    <div className="flex items-center justify-center gap-2 mb-2">
                      <span className="text-lg">{getQuestDifficultyIcon(category.difficulty)}</span>
                      <span className="text-sm font-semibold" style={{ color: getQuestDifficultyColor(category.difficulty) }}>
                        {category.difficulty}
                      </span>
                    </div>
                    
                    <div className="flex items-center justify-center gap-2 mb-2">
                      <span className="text-lg">{getQuestTypeIcon(category.type)}</span>
                      <span className="text-sm text-gray-300">{category.type}</span>
                    </div>
                    
                    <div className="flex items-center justify-center gap-2">
                      <span className="text-lg">{getEnvironmentIcon(category.environment)}</span>
                      <span className="text-sm text-gray-300">{category.environment}</span>
                    </div>
                    
                    <div className="mt-3 text-xs text-gray-400">
                      Nível {category.minLevel}-{category.maxLevel}
                    </div>
                  </div>
                </Card>
              ))}
            </div>

            {/* Quests */}
            {selectedCategory && (
              <div className="space-y-6">
                <h2 className="text-2xl font-bold text-white text-center mb-6">
                  {selectedCategory.name} - Missões Disponíveis
                </h2>
                
                <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                  {quests.map((quest) => (
                    <Card key={quest.id} className="bg-gradient-to-r from-blue-900/30 to-purple-900/30 border-blue-500/50">
                      <div className="p-6">
                        <div className="flex items-start justify-between mb-4">
                          <div className="flex items-center gap-3">
                            <span className="text-3xl">{getQuestTypeIcon(quest.type)}</span>
                            <div>
                              <h3 className="text-xl font-bold text-white">{quest.name}</h3>
                              <div className="flex items-center gap-2 mt-1">
                                <span className="text-lg">{getQuestDifficultyIcon(quest.difficulty)}</span>
                                <span 
                                  className="text-sm font-semibold"
                                  style={{ color: getQuestDifficultyColor(quest.difficulty) }}
                                >
                                  {quest.difficulty}
                                </span>
                                <span className="text-sm text-gray-400">•</span>
                                <span className="text-sm text-gray-300">Nível {quest.requiredLevel}</span>
                              </div>
                            </div>
                          </div>
                        </div>

                        <p className="text-gray-300 mb-4">{quest.description}</p>

                        <div className="grid grid-cols-2 gap-4 mb-4">
                          <div className="bg-gray-800/50 rounded-lg p-3">
                            <div className="text-sm text-gray-400 mb-1">Ambiente</div>
                            <div className="flex items-center gap-2">
                              <span className="text-lg">{getEnvironmentIcon(quest.environment)}</span>
                              <span className="text-white font-semibold">{quest.environment}</span>
                            </div>
                          </div>
                          
                          <div className="bg-gray-800/50 rounded-lg p-3">
                            <div className="text-sm text-gray-400 mb-1">Duração</div>
                            <div className="text-white font-semibold">{quest.estimatedDuration} min</div>
                          </div>
                        </div>

                        {quest.environmentalCondition && (
                          <div className="bg-yellow-900/30 border border-yellow-500/50 rounded-lg p-3 mb-4">
                            <div className="text-sm text-yellow-400 mb-1">Condição Ambiental</div>
                            <div className="text-yellow-300 font-semibold">
                              {quest.environmentalCondition} (Intensidade: {quest.environmentalIntensity})
                            </div>
                          </div>
                        )}

                        <div className="flex items-center justify-between">
                          <div className="flex items-center gap-4">
                            <div className="text-center">
                              <div className="text-green-400 font-bold">{quest.experienceReward}</div>
                              <div className="text-xs text-gray-400">XP</div>
                            </div>
                            <div className="text-center">
                              <div className="text-yellow-400 font-bold">{quest.goldReward}</div>
                              <div className="text-xs text-gray-400">Ouro</div>
                            </div>
                          </div>
                          
                          <div className="flex items-center gap-2">
                            {quest.isBossQuest && (
                              <span className="bg-red-600/30 text-red-300 px-2 py-1 rounded-full text-xs font-semibold">
                                👑 Boss
                              </span>
                            )}
                            {quest.isRepeatable && (
                              <span className="bg-blue-600/30 text-blue-300 px-2 py-1 rounded-full text-xs font-semibold">
                                🔄 Repetível
                              </span>
                            )}
                          </div>
                        </div>
                      </div>
                    </Card>
                  ))}
                </div>
              </div>
            )}
          </>
        )}

        {activeTab === 'monsters' && (
          <div className="space-y-6">
            <h2 className="text-2xl font-bold text-white text-center mb-6">
              👹 Monstros e Bosses
            </h2>
            
            <div className="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-6">
              {monsters.map((monster) => (
                <Card key={monster.id} className="bg-gradient-to-r from-red-900/30 to-orange-900/30 border-red-500/50">
                  <div className="p-6">
                    <div className="flex items-start justify-between mb-4">
                      <div className="flex items-center gap-3">
                        <span className="text-4xl">{monster.icon}</span>
                        <div>
                          <h3 className="text-xl font-bold text-white">{monster.name}</h3>
                          <div className="flex items-center gap-2 mt-1">
                            <span className="text-sm text-gray-300">Nível {monster.level}</span>
                            <span className="text-sm text-gray-400">•</span>
                            <span className="text-sm text-gray-300">{monster.type}</span>
                            {monster.isBoss && (
                              <span className="bg-red-600/30 text-red-300 px-2 py-1 rounded-full text-xs font-semibold">
                                👑 Boss
                              </span>
                            )}
                          </div>
                        </div>
                      </div>
                    </div>

                    <p className="text-gray-300 mb-4">{monster.description}</p>

                    <div className="grid grid-cols-2 gap-4 mb-4">
                      <div className="bg-gray-800/50 rounded-lg p-3">
                        <div className="text-sm text-gray-400 mb-1">Poder</div>
                        <div className="text-red-400 font-bold">{monster.power}</div>
                      </div>
                      
                      <div className="bg-gray-800/50 rounded-lg p-3">
                        <div className="text-sm text-gray-400 mb-1">Vida</div>
                        <div className="text-green-400 font-bold">{monster.health}</div>
                      </div>
                      
                      <div className="bg-gray-800/50 rounded-lg p-3">
                        <div className="text-sm text-gray-400 mb-1">Armadura</div>
                        <div className="text-blue-400 font-bold">{monster.armor}</div>
                      </div>
                      
                      <div className="bg-gray-800/50 rounded-lg p-3">
                        <div className="text-sm text-gray-400 mb-1">Velocidade</div>
                        <div className="text-yellow-400 font-bold">{monster.speed}</div>
                      </div>
                    </div>

                    <div className="flex items-center justify-between">
                      <div className="flex items-center gap-4">
                        <div className="text-center">
                          <div className="text-green-400 font-bold">{monster.experienceReward}</div>
                          <div className="text-xs text-gray-400">XP</div>
                        </div>
                        <div className="text-center">
                          <div className="text-yellow-400 font-bold">{monster.goldReward}</div>
                          <div className="text-xs text-gray-400">Ouro</div>
                        </div>
                      </div>
                      
                      <div className="text-right">
                        <div className="text-sm text-gray-400">Dado de Ataque</div>
                        <div className="text-white font-semibold">{monster.attackDice}</div>
                      </div>
                    </div>
                  </div>
                </Card>
              ))}
            </div>
          </div>
        )}
      </div>
    </div>
  );
};
