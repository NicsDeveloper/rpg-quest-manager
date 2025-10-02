import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Card } from '../components/Card';
import { heroService, Hero } from '../services/heroService';
import { questService, Quest } from '../services/questService';
import { itemService, Item } from '../services/itemService';
import { Navbar } from '../components/Navbar';

export const Dashboard: React.FC = () => {
  const { t } = useTranslation();
  const [heroes, setHeroes] = useState<Hero[]>([]);
  const [quests, setQuests] = useState<Quest[]>([]);
  const [items, setItems] = useState<Item[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const [heroesData, questsData, itemsData] = await Promise.all([
        heroService.getAll(),
        questService.getAll(),
        itemService.getAll(),
      ]);
      setHeroes(heroesData);
      setQuests(questsData);
      setItems(itemsData);
    } catch (error) {
      console.error('Error loading dashboard data:', error);
    } finally {
      setLoading(false);
    }
  };

  const strongestHeroes = [...heroes].sort((a, b) => b.level - a.level).slice(0, 5);

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
        <h1 className="text-4xl font-bold mb-8 text-amber-500">{t('dashboard.welcome')}</h1>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
          <Card>
            <h3 className="text-lg font-semibold text-gray-400 mb-2">{t('dashboard.total_heroes')}</h3>
            <p className="text-4xl font-bold text-amber-500">{heroes.length}</p>
          </Card>

          <Card>
            <h3 className="text-lg font-semibold text-gray-400 mb-2">{t('dashboard.total_quests')}</h3>
            <p className="text-4xl font-bold text-amber-500">{quests.length}</p>
          </Card>

          <Card>
            <h3 className="text-lg font-semibold text-gray-400 mb-2">{t('dashboard.total_items')}</h3>
            <p className="text-4xl font-bold text-amber-500">{items.length}</p>
          </Card>

          <Card>
            <h3 className="text-lg font-semibold text-gray-400 mb-2">{t('dashboard.completed_quests')}</h3>
            <p className="text-4xl font-bold text-amber-500">-</p>
          </Card>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <Card>
            <h2 className="text-2xl font-bold mb-4 text-amber-500">{t('dashboard.strongest_heroes')}</h2>
            {strongestHeroes.length > 0 ? (
              <div className="space-y-3">
                {strongestHeroes.map((hero) => (
                  <div key={hero.id} className="flex justify-between items-center p-3 bg-gray-700 rounded">
                    <div>
                      <p className="font-semibold">{hero.name}</p>
                      <p className="text-sm text-gray-400">{hero.class}</p>
                    </div>
                    <div className="text-right">
                      <p className="text-amber-500 font-bold">Nv. {hero.level}</p>
                      <p className="text-sm text-gray-400">{hero.gold} 🪙</p>
                    </div>
                  </div>
                ))}
              </div>
            ) : (
              <p className="text-gray-400">{t('heroes.no_heroes')}</p>
            )}
          </Card>

          <Card>
            <h2 className="text-2xl font-bold mb-4 text-amber-500">{t('dashboard.most_played_quests')}</h2>
            {quests.length > 0 ? (
              <div className="space-y-3">
                {quests.slice(0, 5).map((quest) => (
                  <div key={quest.id} className="flex justify-between items-center p-3 bg-gray-700 rounded">
                    <div>
                      <p className="font-semibold">{quest.name}</p>
                      <p className="text-sm text-gray-400">{quest.difficulty}</p>
                    </div>
                    <div className="text-right">
                      <p className="text-amber-500 font-bold">{quest.goldReward} 🪙</p>
                      <p className="text-sm text-gray-400">{quest.experienceReward} XP</p>
                    </div>
                  </div>
                ))}
              </div>
            ) : (
              <p className="text-gray-400">{t('quests.no_quests')}</p>
            )}
          </Card>
        </div>
      </div>
    </>
  );
};

