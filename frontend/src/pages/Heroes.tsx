import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Navbar } from '../components/Navbar';
import { Card } from '../components/Card';
import { Button } from '../components/Button';
import { Modal } from '../components/Modal';
import { Input } from '../components/Input';
import { heroService, Hero, CreateHeroRequest } from '../services/heroService';
import { useAuth } from '../contexts/AuthContext';

export const Heroes: React.FC = () => {
  const { t } = useTranslation();
  const { isAdmin } = useAuth();
  const [heroes, setHeroes] = useState<Hero[]>([]);
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [selectedHero, setSelectedHero] = useState<Hero | null>(null);

  const [formData, setFormData] = useState<CreateHeroRequest>({
    name: '',
    class: '',
    strength: 10,
    intelligence: 10,
    agility: 10,
  });

  useEffect(() => {
    loadHeroes();
  }, []);

  const loadHeroes = async () => {
    try {
      const data = await heroService.getAll();
      setHeroes(data);
    } catch (error) {
      console.error('Error loading heroes:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleOpenModal = (hero?: Hero) => {
    if (hero) {
      setSelectedHero(hero);
      setFormData({
        name: hero.name,
        class: hero.class,
        strength: hero.strength,
        intelligence: hero.intelligence,
        agility: hero.agility,
      });
    } else {
      setSelectedHero(null);
      setFormData({
        name: '',
        class: '',
        strength: 10,
        intelligence: 10,
        agility: 10,
      });
    }
    setIsModalOpen(true);
  };

  const handleCloseModal = () => {
    setIsModalOpen(false);
    setSelectedHero(null);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      if (selectedHero) {
        await heroService.update(selectedHero.id, formData);
      } else {
        await heroService.create(formData);
      }
      loadHeroes();
      handleCloseModal();
    } catch (error) {
      console.error('Error saving hero:', error);
    }
  };

  const handleDelete = async (id: number) => {
    if (window.confirm(t('heroes.delete_confirm'))) {
      try {
        await heroService.delete(id);
        loadHeroes();
      } catch (error) {
        console.error('Error deleting hero:', error);
      }
    }
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
          <h1 className="text-4xl font-bold text-amber-500">{t('heroes.title')}</h1>
          {isAdmin && (
            <Button onClick={() => handleOpenModal()}>
              {t('heroes.create')}
            </Button>
          )}
        </div>

        {heroes.length === 0 ? (
          <Card>
            <p className="text-center text-gray-400">{t('heroes.no_heroes')}</p>
          </Card>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {heroes.map((hero) => (
              <Card key={hero.id}>
                <div className="flex justify-between items-start mb-4">
                  <div>
                    <h3 className="text-2xl font-bold text-amber-500">{hero.name}</h3>
                    <p className="text-gray-400">{hero.class}</p>
                  </div>
                  <span className="bg-amber-600 text-white px-3 py-1 rounded-full text-sm font-semibold">
                    Nv. {hero.level}
                  </span>
                </div>

                <div className="grid grid-cols-2 gap-4 mb-4">
                  <div>
                    <p className="text-sm text-gray-400">{t('heroes.health')}</p>
                    <p className="font-semibold">{hero.health}/{hero.maxHealth}</p>
                  </div>
                  <div>
                    <p className="text-sm text-gray-400">{t('heroes.mana')}</p>
                    <p className="font-semibold">{hero.mana}/{hero.maxMana}</p>
                  </div>
                  <div>
                    <p className="text-sm text-gray-400">{t('heroes.strength')}</p>
                    <p className="font-semibold">{hero.strength}</p>
                  </div>
                  <div>
                    <p className="text-sm text-gray-400">{t('heroes.intelligence')}</p>
                    <p className="font-semibold">{hero.intelligence}</p>
                  </div>
                  <div>
                    <p className="text-sm text-gray-400">{t('heroes.agility')}</p>
                    <p className="font-semibold">{hero.agility}</p>
                  </div>
                  <div>
                    <p className="text-sm text-gray-400">{t('heroes.gold')}</p>
                    <p className="font-semibold">{hero.gold} 🪙</p>
                  </div>
                </div>

                <div className="mb-4">
                  <p className="text-sm text-gray-400">{t('heroes.experience')}</p>
                  <div className="bg-gray-700 rounded-full h-2 mt-1">
                    <div
                      className="bg-amber-600 h-2 rounded-full"
                      style={{ width: `${(hero.experience % 100)}%` }}
                    />
                  </div>
                  <p className="text-xs text-gray-400 mt-1">{hero.experience} XP</p>
                </div>

                {isAdmin && (
                  <div className="flex gap-2">
                    <Button variant="secondary" onClick={() => handleOpenModal(hero)} className="flex-1">
                      {t('common.edit')}
                    </Button>
                    <Button variant="danger" onClick={() => handleDelete(hero.id)} className="flex-1">
                      {t('common.delete')}
                    </Button>
                  </div>
                )}
              </Card>
            ))}
          </div>
        )}

        <Modal
          isOpen={isModalOpen}
          onClose={handleCloseModal}
          title={selectedHero ? t('heroes.edit') : t('heroes.create')}
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
              label={t('heroes.name')}
              value={formData.name}
              onChange={(e) => setFormData({ ...formData, name: e.target.value })}
              required
            />

            <Input
              label={t('heroes.class')}
              value={formData.class}
              onChange={(e) => setFormData({ ...formData, class: e.target.value })}
              required
            />

            <Input
              label={t('heroes.strength')}
              type="number"
              value={formData.strength}
              onChange={(e) => setFormData({ ...formData, strength: parseInt(e.target.value) })}
              required
              min="1"
            />

            <Input
              label={t('heroes.intelligence')}
              type="number"
              value={formData.intelligence}
              onChange={(e) => setFormData({ ...formData, intelligence: parseInt(e.target.value) })}
              required
              min="1"
            />

            <Input
              label={t('heroes.agility')}
              type="number"
              value={formData.agility}
              onChange={(e) => setFormData({ ...formData, agility: parseInt(e.target.value) })}
              required
              min="1"
            />
          </form>
        </Modal>
      </div>
    </>
  );
};

