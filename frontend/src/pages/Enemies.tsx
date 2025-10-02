import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Navbar } from '../components/Navbar';
import { Card } from '../components/Card';
import { Button } from '../components/Button';
import { Modal } from '../components/Modal';
import { Input } from '../components/Input';
import { enemyService, Enemy, CreateEnemyRequest } from '../services/enemyService';

export const Enemies: React.FC = () => {
  const { t } = useTranslation();
  const [enemies, setEnemies] = useState<Enemy[]>([]);
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [selectedEnemy, setSelectedEnemy] = useState<Enemy | null>(null);

  const [formData, setFormData] = useState<CreateEnemyRequest>({
    name: '',
    description: '',
    level: 1,
    health: 100,
    strength: 10,
    defense: 5,
    goldDrop: 10,
    experienceDrop: 20,
  });

  useEffect(() => {
    loadEnemies();
  }, []);

  const loadEnemies = async () => {
    try {
      const data = await enemyService.getAll();
      setEnemies(data);
    } catch (error) {
      console.error('Error loading enemies:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleOpenModal = (enemy?: Enemy) => {
    if (enemy) {
      setSelectedEnemy(enemy);
      setFormData({
        name: enemy.name,
        description: enemy.description,
        level: enemy.level,
        health: enemy.health,
        strength: enemy.strength,
        defense: enemy.defense,
        goldDrop: enemy.goldDrop,
        experienceDrop: enemy.experienceDrop,
      });
    } else {
      setSelectedEnemy(null);
      setFormData({
        name: '',
        description: '',
        level: 1,
        health: 100,
        strength: 10,
        defense: 5,
        goldDrop: 10,
        experienceDrop: 20,
      });
    }
    setIsModalOpen(true);
  };

  const handleCloseModal = () => {
    setIsModalOpen(false);
    setSelectedEnemy(null);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      if (selectedEnemy) {
        await enemyService.update(selectedEnemy.id, formData);
      } else {
        await enemyService.create(formData);
      }
      loadEnemies();
      handleCloseModal();
    } catch (error) {
      console.error('Error saving enemy:', error);
    }
  };

  const handleDelete = async (id: number) => {
    if (window.confirm(t('enemies.delete_confirm'))) {
      try {
        await enemyService.delete(id);
        loadEnemies();
      } catch (error) {
        console.error('Error deleting enemy:', error);
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
          <h1 className="text-4xl font-bold text-amber-500">{t('enemies.title')}</h1>
          <Button onClick={() => handleOpenModal()}>
            {t('enemies.create')}
          </Button>
        </div>

        {enemies.length === 0 ? (
          <Card>
            <p className="text-center text-gray-400">{t('enemies.no_enemies')}</p>
          </Card>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {enemies.map((enemy) => (
              <Card key={enemy.id}>
                <div className="flex justify-between items-start mb-3">
                  <h3 className="text-2xl font-bold text-red-500">{enemy.name}</h3>
                  <span className="bg-red-600 text-white px-3 py-1 rounded-full text-sm font-semibold">
                    Nv. {enemy.level}
                  </span>
                </div>

                <p className="text-sm text-gray-400 mb-4">{enemy.description}</p>

                <div className="grid grid-cols-2 gap-3 mb-4">
                  <div>
                    <p className="text-sm text-gray-400">{t('enemies.health')}</p>
                    <p className="font-semibold">❤️ {enemy.health}</p>
                  </div>
                  <div>
                    <p className="text-sm text-gray-400">{t('enemies.strength')}</p>
                    <p className="font-semibold">⚔️ {enemy.strength}</p>
                  </div>
                  <div>
                    <p className="text-sm text-gray-400">{t('enemies.defense')}</p>
                    <p className="font-semibold">🛡️ {enemy.defense}</p>
                  </div>
                </div>

                <div className="bg-gray-700 rounded p-3 mb-4">
                  <p className="text-xs text-amber-500 font-semibold mb-2">Drops:</p>
                  <div className="text-sm space-y-1">
                    <p>🪙 {enemy.goldDrop} ouro</p>
                    <p>⭐ {enemy.experienceDrop} XP</p>
                  </div>
                </div>

                <div className="flex gap-2">
                  <Button variant="secondary" onClick={() => handleOpenModal(enemy)} className="flex-1">
                    {t('common.edit')}
                  </Button>
                  <Button variant="danger" onClick={() => handleDelete(enemy.id)} className="flex-1">
                    {t('common.delete')}
                  </Button>
                </div>
              </Card>
            ))}
          </div>
        )}

        <Modal
          isOpen={isModalOpen}
          onClose={handleCloseModal}
          title={selectedEnemy ? t('enemies.edit') : t('enemies.create')}
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
              label={t('enemies.name')}
              value={formData.name}
              onChange={(e) => setFormData({ ...formData, name: e.target.value })}
              required
            />

            <div className="mb-4">
              <label className="label">{t('enemies.description')}</label>
              <textarea
                className="input"
                value={formData.description}
                onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                required
                rows={3}
              />
            </div>

            <Input
              label={t('enemies.level')}
              type="number"
              value={formData.level}
              onChange={(e) => setFormData({ ...formData, level: parseInt(e.target.value) })}
              required
              min="1"
            />

            <Input
              label={t('enemies.health')}
              type="number"
              value={formData.health}
              onChange={(e) => setFormData({ ...formData, health: parseInt(e.target.value) })}
              required
              min="1"
            />

            <Input
              label={t('enemies.strength')}
              type="number"
              value={formData.strength}
              onChange={(e) => setFormData({ ...formData, strength: parseInt(e.target.value) })}
              required
              min="1"
            />

            <Input
              label={t('enemies.defense')}
              type="number"
              value={formData.defense}
              onChange={(e) => setFormData({ ...formData, defense: parseInt(e.target.value) })}
              required
              min="0"
            />

            <Input
              label={t('enemies.gold_drop')}
              type="number"
              value={formData.goldDrop}
              onChange={(e) => setFormData({ ...formData, goldDrop: parseInt(e.target.value) })}
              required
              min="0"
            />

            <Input
              label={t('enemies.experience_drop')}
              type="number"
              value={formData.experienceDrop}
              onChange={(e) => setFormData({ ...formData, experienceDrop: parseInt(e.target.value) })}
              required
              min="0"
            />
          </form>
        </Modal>
      </div>
    </>
  );
};

