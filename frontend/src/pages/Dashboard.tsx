import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';
import { Card } from '../components/Card';
import { heroService, Hero } from '../services/heroService';
import { questService, Quest } from '../services/questService';
import { itemService, Item } from '../services/itemService';
import { userService } from '../services/userService';
import { Navbar } from '../components/Navbar';
import { Tutorial } from '../components/Tutorial';

export const Dashboard: React.FC = () => {
  const { t } = useTranslation();
  const [heroes, setHeroes] = useState<Hero[]>([]);
  const [quests, setQuests] = useState<Quest[]>([]);
  const [items, setItems] = useState<Item[]>([]);
  const [loading, setLoading] = useState(true);
  const [showTutorial, setShowTutorial] = useState(false);

  useEffect(() => {
    checkTutorial();
    loadData();
  }, []);

  const checkTutorial = async () => {
    try {
      const user = await userService.getCurrentUser();
      if (!user.hasSeenTutorial) {
        setTimeout(() => setShowTutorial(true), 1000);
      }
    } catch (error) {
      console.error('Error checking tutorial:', error);
    }
  };

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

  const handleTutorialComplete = () => {
    setShowTutorial(false);
  };

  const handleTutorialSkip = () => {
    setShowTutorial(false);
  };

  const strongestHeroes = [...heroes].sort((a, b) => b.level - a.level).slice(0, 5);

  if (loading) {
    return (
      <>
        <Navbar />
        <div className="container mx-auto px-6 py-8 flex items-center justify-center min-h-screen">
          <div className="text-center">
            <div className="inline-block p-6 bg-gradient-to-br from-amber-500 to-orange-600 rounded-full shadow-lg shadow-amber-500/50 animate-pulse mb-4">
              <svg className="w-16 h-16 text-white animate-spin" fill="none" viewBox="0 0 24 24">
                <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
            </div>
            <p className="text-gray-400 text-lg">{t('common.loading')}</p>
          </div>
        </div>
      </>
    );
  }

  return (
    <>
      {showTutorial && (
        <Tutorial onComplete={handleTutorialComplete} onSkip={handleTutorialSkip} />
      )}
      <Navbar />
      <div className="container mx-auto px-6 py-8">
        <div className="mb-12 text-center">
          <h1 className="text-6xl font-black mb-4 hero-title animate-float">{t('dashboard.welcome')}</h1>
          <p className="text-gray-400 text-lg">Gerencie seu reino de aventuras épicas</p>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-12">
          <div className="stat-card group">
            <div className="flex items-center justify-between mb-3">
              <div className="p-3 bg-gradient-to-br from-blue-500 to-blue-600 rounded-xl shadow-lg group-hover:scale-110 transition-transform">
                <svg className="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                </svg>
              </div>
              <span className="text-4xl font-black text-gradient">{heroes.length}</span>
            </div>
            <h3 className="text-gray-400 text-sm font-medium">{t('dashboard.total_heroes')}</h3>
          </div>

          <div className="stat-card group">
            <div className="flex items-center justify-between mb-3">
              <div className="p-3 bg-gradient-to-br from-purple-500 to-purple-600 rounded-xl shadow-lg group-hover:scale-110 transition-transform">
                <svg className="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
              </div>
              <span className="text-4xl font-black text-gradient">{quests.length}</span>
            </div>
            <h3 className="text-gray-400 text-sm font-medium">{t('dashboard.total_quests')}</h3>
          </div>

          <div className="stat-card group">
            <div className="flex items-center justify-between mb-3">
              <div className="p-3 bg-gradient-to-br from-green-500 to-green-600 rounded-xl shadow-lg group-hover:scale-110 transition-transform">
                <svg className="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
                </svg>
              </div>
              <span className="text-4xl font-black text-gradient">{items.length}</span>
            </div>
            <h3 className="text-gray-400 text-sm font-medium">{t('dashboard.total_items')}</h3>
          </div>

          <div className="stat-card group">
            <div className="flex items-center justify-between mb-3">
              <div className="p-3 bg-gradient-to-br from-amber-500 to-orange-600 rounded-xl shadow-lg group-hover:scale-110 transition-transform animate-glow">
                <svg className="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 3v4M3 5h4M6 17v4m-2-2h4m5-16l2.286 6.857L21 12l-5.714 2.143L13 21l-2.286-6.857L5 12l5.714-2.143L13 3z" />
                </svg>
              </div>
              <span className="text-4xl font-black text-gradient">-</span>
            </div>
            <h3 className="text-gray-400 text-sm font-medium">{t('dashboard.completed_quests')}</h3>
          </div>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
          <Card variant="hero" className="hover:scale-[1.02] transition-transform">
            <div className="flex items-center justify-between mb-6">
              <h2 className="text-3xl font-bold text-gradient">{t('dashboard.strongest_heroes')}</h2>
              <Link to="/heroes" className="text-amber-400 hover:text-amber-300 text-sm font-semibold hover:underline">
                Ver todos →
              </Link>
            </div>
            {strongestHeroes.length > 0 ? (
              <div className="space-y-3">
                {strongestHeroes.map((hero, index) => (
                  <div
                    key={hero.id}
                    className="group flex items-center gap-4 p-4 bg-gradient-to-r from-gray-800/50 to-gray-900/50 rounded-xl border border-gray-700/30 hover:border-amber-500/50 transition-all hover:scale-105"
                  >
                    <div className="flex-shrink-0 w-12 h-12 bg-gradient-to-br from-amber-500 to-orange-600 rounded-full flex items-center justify-center text-white font-black text-xl shadow-lg shadow-amber-500/30">
                      #{index + 1}
                    </div>
                    <div className="flex-1">
                      <p className="font-bold text-lg text-amber-400 group-hover:text-amber-300">{hero.name}</p>
                      <p className="text-sm text-gray-400">{hero.class}</p>
                    </div>
                    <div className="text-right">
                      <div className="badge badge-primary mb-1">Nv. {hero.level}</div>
                      <p className="text-sm text-gray-400">{hero.gold} 🪙</p>
                    </div>
                  </div>
                ))}
              </div>
            ) : (
              <div className="text-center py-12 text-gray-400">
                <div className="inline-block p-4 bg-gray-800/50 rounded-full mb-4">
                  <svg className="w-12 h-12" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4" />
                  </svg>
                </div>
                <p>{t('heroes.no_heroes')}</p>
              </div>
            )}
          </Card>

          <Card variant="quest" className="hover:scale-[1.02] transition-transform">
            <div className="flex items-center justify-between mb-6">
              <h2 className="text-3xl font-bold text-gradient">{t('dashboard.most_played_quests')}</h2>
              <Link to="/quests" className="text-blue-400 hover:text-blue-300 text-sm font-semibold hover:underline">
                Ver todas →
              </Link>
            </div>
            {quests.length > 0 ? (
              <div className="space-y-3">
                {quests.slice(0, 5).map((quest) => (
                  <div
                    key={quest.id}
                    className="group flex items-center justify-between p-4 bg-gradient-to-r from-gray-800/50 to-gray-900/50 rounded-xl border border-gray-700/30 hover:border-blue-500/50 transition-all hover:scale-105"
                  >
                    <div className="flex-1">
                      <p className="font-bold text-blue-400 group-hover:text-blue-300 mb-1">{quest.name}</p>
                      <span className="badge badge-info text-xs">{quest.difficulty}</span>
                    </div>
                    <div className="text-right">
                      <p className="text-amber-400 font-bold text-lg">{quest.goldReward} 🪙</p>
                      <p className="text-sm text-gray-400">{quest.experienceReward} XP</p>
                    </div>
                  </div>
                ))}
              </div>
            ) : (
              <div className="text-center py-12 text-gray-400">
                <div className="inline-block p-4 bg-gray-800/50 rounded-full mb-4">
                  <svg className="w-12 h-12" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
                  </svg>
                </div>
                <p>{t('quests.no_quests')}</p>
              </div>
            )}
          </Card>
        </div>
      </div>
    </>
  );
};
