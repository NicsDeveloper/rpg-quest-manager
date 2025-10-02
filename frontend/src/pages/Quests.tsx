import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Navbar } from '../components/Navbar';
import { Card } from '../components/Card';
import { Button } from '../components/Button';
import { Modal } from '../components/Modal';
import { Input } from '../components/Input';
import { questService, Quest, CreateQuestRequest } from '../services/questService';
import { heroService, Hero } from '../services/heroService';
import { useAuth } from '../contexts/AuthContext';

export const Quests: React.FC = () => {
  const { t } = useTranslation();
  const { isAdmin } = useAuth();
  const [quests, setQuests] = useState<Quest[]>([]);
  const [heroes, setHeroes] = useState<Hero[]>([]);
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isCompleteModalOpen, setIsCompleteModalOpen] = useState(false);
  const [selectedQuest, setSelectedQuest] = useState<Quest | null>(null);
  const [selectedHeroId, setSelectedHeroId] = useState<number>(0);

  const [formData, setFormData] = useState<CreateQuestRequest>({
    name: '',
    description: '',
    type: 'Main',
    difficulty: 'Easy',
    requiredLevel: 1,
    goldReward: 0,
    experienceReward: 0,
    isRepeatable: false,
  });

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const [questsData, heroesData] = await Promise.all([
        questService.getAll(),
        heroService.getAll(),
      ]);
      setQuests(questsData);
      setHeroes(heroesData);
    } catch (error) {
      console.error('Error loading data:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleOpenModal = (quest?: Quest) => {
    if (quest) {
      setSelectedQuest(quest);
      setFormData({
        name: quest.name,
        description: quest.description,
        type: quest.type,
        difficulty: quest.difficulty,
        requiredLevel: quest.requiredLevel,
        goldReward: quest.goldReward,
        experienceReward: quest.experienceReward,
        isRepeatable: quest.isRepeatable,
      });
    } else {
      setSelectedQuest(null);
      setFormData({
        name: '',
        description: '',
        type: 'Main',
        difficulty: 'Easy',
        requiredLevel: 1,
        goldReward: 0,
        experienceReward: 0,
        isRepeatable: false,
      });
    }
    setIsModalOpen(true);
  };

  const handleCloseModal = () => {
    setIsModalOpen(false);
    setSelectedQuest(null);
  };

  const handleOpenCompleteModal = (quest: Quest) => {
    setSelectedQuest(quest);
    setSelectedHeroId(heroes.length > 0 ? heroes[0].id : 0);
    setIsCompleteModalOpen(true);
  };

  const handleCloseCompleteModal = () => {
    setIsCompleteModalOpen(false);
    setSelectedQuest(null);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      if (selectedQuest) {
        await questService.update(selectedQuest.id, formData);
      } else {
        await questService.create(formData);
      }
      loadData();
      handleCloseModal();
    } catch (error) {
      console.error('Error saving quest:', error);
    }
  };

  const handleComplete = async () => {
    if (!selectedQuest || !selectedHeroId) return;

    try {
      await questService.complete(selectedHeroId);
      alert(t('quests.complete_success'));
      handleCloseCompleteModal();
      loadData();
    } catch (error: any) {
      alert(error.response?.data?.message || 'Erro ao completar missão');
    }
  };

  const handleDelete = async (id: number) => {
    if (window.confirm(t('quests.delete_confirm'))) {
      try {
        await questService.delete(id);
        loadData();
      } catch (error) {
        console.error('Error deleting quest:', error);
      }
    }
  };

  const getDifficultyColor = (difficulty: string) => {
    const colors: Record<string, string> = {
      Easy: 'bg-green-600',
      Medium: 'bg-yellow-600',
      Hard: 'bg-orange-600',
      Epic: 'bg-red-600',
      Legendary: 'bg-purple-600',
    };
    return colors[difficulty] || 'bg-gray-600';
  };

  if (loading) {
    return (
      <>
        <Navbar />
        <div className="container mx-auto px-6 py-8">
          <div className="text-center text-gray-400">{t('common.loading')}</div>
        </div>
      </>
    );
  }

  return (
    <>
      <Navbar />
      <div className="container mx-auto px-6 py-8">
        <div className="flex justify-between items-center mb-8">
          <h1 className="text-4xl font-bold text-amber-500">{t('quests.title')}</h1>
          {isAdmin && (
            <Button onClick={() => handleOpenModal()}>
              {t('quests.create')}
            </Button>
          )}
        </div>

        {quests.length === 0 ? (
          <Card>
            <p className="text-center text-gray-400">{t('quests.no_quests')}</p>
          </Card>
        ) : (
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            {quests.map((quest) => (
              <Card key={quest.id}>
                <div className="flex justify-between items-start mb-4">
                  <div className="flex-1">
                    <div className="flex items-center gap-2 mb-2">
                      <h3 className="text-2xl font-bold text-amber-500">{quest.name}</h3>
                      {quest.isRepeatable && (
                        <span className="px-2 py-1 bg-blue-900 text-blue-300 rounded text-xs font-medium">
                          🔄 Diária
                        </span>
                      )}
                    </div>
                    <p className="text-gray-400 text-sm mb-3">{quest.description}</p>
                  </div>
                  <span className={`${getDifficultyColor(quest.difficulty)} text-white px-3 py-1 rounded-full text-sm font-semibold ml-3`}>
                    {quest.difficulty}
                  </span>
                </div>

                <div className="grid grid-cols-2 gap-4 mb-4">
                  <div>
                    <p className="text-sm text-gray-400">{t('quests.type')}</p>
                    <p className="font-semibold">{quest.type}</p>
                  </div>
                  <div>
                    <p className="text-sm text-gray-400">{t('quests.required_level')}</p>
                    <p className="font-semibold">Nv. {quest.requiredLevel}</p>
                  </div>
                </div>

                <div className="bg-gray-700 rounded p-3 mb-4">
                  <p className="text-sm text-amber-500 font-semibold mb-2">{t('quests.rewards')}:</p>
                  <div className="flex gap-4 text-sm">
                    <span>💰 {quest.goldReward}</span>
                    <span>⭐ {quest.experienceReward} XP</span>
                  </div>
                  {quest.rewards && quest.rewards.length > 0 && quest.rewards[0].item && (
                    <div className="mt-2 text-sm text-gray-300">
                      📦 {quest.rewards[0].item.name} x{quest.rewards[0].itemQuantity}
                    </div>
                  )}
                </div>

                <div className="flex gap-2">
                  {isAdmin && (
                    <>
                      <Button variant="primary" onClick={() => handleOpenCompleteModal(quest)} className="flex-1">
                        {t('quests.complete')}
                      </Button>
                      <Button variant="secondary" onClick={() => handleOpenModal(quest)}>
                        {t('common.edit')}
                      </Button>
                      <Button variant="danger" onClick={() => handleDelete(quest.id)}>
                        {t('common.delete')}
                      </Button>
                    </>
                  )}
                </div>
              </Card>
            ))}
          </div>
        )}

        <Modal
          isOpen={isModalOpen}
          onClose={handleCloseModal}
          title={selectedQuest ? t('quests.edit') : t('quests.create')}
          footer={
            <>
              <Button variant="secondary" onClick={handleCloseModal}>
                {t('common.cancel')}
              </Button>
              <Button onClick={handleSubmit}>
                {t('common.save')}
              </Button>
            </>
          }
        >
          <form onSubmit={handleSubmit}>
            <Input
              label={t('quests.name')}
              value={formData.name}
              onChange={(e) => setFormData({ ...formData, name: e.target.value })}
              required
            />

            <div className="mb-4">
              <label className="label">{t('quests.description')}</label>
              <textarea
                className="input"
                value={formData.description}
                onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                required
                rows={3}
              />
            </div>

            <div className="mb-4">
              <label className="label">{t('quests.type')}</label>
              <select
                className="input"
                value={formData.type}
                onChange={(e) => setFormData({ ...formData, type: e.target.value })}
              >
                <option value="Main">Main</option>
                <option value="Side">Side</option>
                <option value="Daily">Daily</option>
                <option value="Event">Event</option>
              </select>
            </div>

            <div className="mb-4">
              <label className="label">{t('quests.difficulty')}</label>
              <select
                className="input"
                value={formData.difficulty}
                onChange={(e) => setFormData({ ...formData, difficulty: e.target.value })}
              >
                <option value="Easy">Easy</option>
                <option value="Medium">Medium</option>
                <option value="Hard">Hard</option>
                <option value="Epic">Epic</option>
                <option value="Legendary">Legendary</option>
              </select>
            </div>

            <Input
              label={t('quests.required_level')}
              type="number"
              value={formData.requiredLevel}
              onChange={(e) => setFormData({ ...formData, requiredLevel: parseInt(e.target.value) })}
              required
              min="1"
            />

            <Input
              label={t('quests.gold_reward')}
              type="number"
              value={formData.goldReward}
              onChange={(e) => setFormData({ ...formData, goldReward: parseInt(e.target.value) })}
              required
              min="0"
            />

            <Input
              label={t('quests.experience_reward')}
              type="number"
              value={formData.experienceReward}
              onChange={(e) => setFormData({ ...formData, experienceReward: parseInt(e.target.value) })}
              required
              min="0"
            />

            <div className="flex items-center space-x-2">
              <input
                type="checkbox"
                id="isRepeatable"
                checked={formData.isRepeatable}
                onChange={(e) => setFormData({ ...formData, isRepeatable: e.target.checked })}
                className="w-4 h-4 text-blue-600 bg-gray-700 border-gray-600 rounded focus:ring-blue-500"
              />
              <label htmlFor="isRepeatable" className="text-sm font-medium text-gray-300">
                Quest Diária (Repetível)
              </label>
            </div>
          </form>
        </Modal>

        <Modal
          isOpen={isCompleteModalOpen}
          onClose={handleCloseCompleteModal}
          title={t('quests.complete')}
          footer={
            <>
              <Button variant="secondary" onClick={handleCloseCompleteModal}>
                {t('common.cancel')}
              </Button>
              <Button onClick={handleComplete}>
                {t('common.confirm')}
              </Button>
            </>
          }
        >
          <div className="mb-4">
            <label className="label">{t('quests.select_hero')}</label>
            <select
              className="input"
              value={selectedHeroId}
              onChange={(e) => setSelectedHeroId(parseInt(e.target.value))}
            >
              {heroes.map((hero) => (
                <option key={hero.id} value={hero.id}>
                  {hero.name} (Nv. {hero.level} - {hero.class})
                </option>
              ))}
            </select>
          </div>
          {selectedQuest && (
            <div className="bg-gray-700 rounded p-4">
              <p className="text-amber-500 font-semibold mb-2">{t('quests.rewards')}:</p>
              <p>💰 {selectedQuest.goldReward} ouro</p>
              <p>⭐ {selectedQuest.experienceReward} XP</p>
              {selectedQuest.rewards && selectedQuest.rewards.length > 0 && selectedQuest.rewards[0].item && (
                <p>📦 {selectedQuest.rewards[0].item.name} x{selectedQuest.rewards[0].itemQuantity}</p>
              )}
            </div>
          )}
        </Modal>
      </div>
    </>
  );
};

