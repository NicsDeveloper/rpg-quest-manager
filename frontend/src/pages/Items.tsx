import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Navbar } from '../components/Navbar';
import { Card } from '../components/Card';
import { Button } from '../components/Button';
import { Modal } from '../components/Modal';
import { Input } from '../components/Input';
import { itemService, Item, CreateItemRequest } from '../services/itemService';
import { useAuth } from '../contexts/AuthContext';

export const Items: React.FC = () => {
  const { t } = useTranslation();
  const { isAdmin } = useAuth();
  const [items, setItems] = useState<Item[]>([]);
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);

  const [formData, setFormData] = useState<CreateItemRequest>({
    name: '',
    description: '',
    type: 'Weapon',
    rarity: 'Common',
    value: 0,
    bonusHealth: 0,
    bonusMana: 0,
    bonusStrength: 0,
    bonusIntelligence: 0,
    bonusAgility: 0,
  });

  useEffect(() => {
    loadItems();
  }, []);

  const loadItems = async () => {
    try {
      const data = await itemService.getAll();
      setItems(data);
    } catch (error) {
      console.error('Error loading items:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleOpenModal = () => {
    setFormData({
      name: '',
      description: '',
      type: 'Weapon',
      rarity: 'Common',
      value: 0,
      bonusHealth: 0,
      bonusMana: 0,
      bonusStrength: 0,
      bonusIntelligence: 0,
      bonusAgility: 0,
    });
    setIsModalOpen(true);
  };

  const handleCloseModal = () => {
    setIsModalOpen(false);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await itemService.create(formData);
      loadItems();
      handleCloseModal();
    } catch (error) {
      console.error('Error creating item:', error);
    }
  };

  const handleDelete = async (id: number) => {
    if (window.confirm(t('items.delete_confirm'))) {
      try {
        await itemService.delete(id);
        loadItems();
      } catch (error) {
        console.error('Error deleting item:', error);
      }
    }
  };

  const getRarityColor = (rarity: string) => {
    const colors: Record<string, string> = {
      Common: 'bg-gray-600',
      Uncommon: 'bg-green-600',
      Rare: 'bg-blue-600',
      Epic: 'bg-purple-600',
      Legendary: 'bg-amber-600',
    };
    return colors[rarity] || 'bg-gray-600';
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
          <h1 className="text-4xl font-bold text-amber-500">{t('items.title')}</h1>
          {isAdmin && (
            <Button onClick={handleOpenModal}>
              {t('items.create')}
            </Button>
          )}
        </div>

        {items.length === 0 ? (
          <Card>
            <p className="text-center text-gray-400">{t('items.no_items')}</p>
          </Card>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
            {items.map((item) => (
              <Card key={item.id}>
                <div className="flex justify-between items-start mb-3">
                  <h3 className="text-xl font-bold text-amber-500">{item.name}</h3>
                  <span className={`${getRarityColor(item.rarity)} text-white px-2 py-1 rounded text-xs font-semibold`}>
                    {item.rarity}
                  </span>
                </div>

                <p className="text-sm text-gray-400 mb-3">{item.description}</p>

                <div className="mb-3">
                  <p className="text-sm">
                    <span className="text-gray-400">{t('items.type')}:</span> {item.type}
                  </p>
                  <p className="text-sm">
                    <span className="text-gray-400">{t('items.value')}:</span> {item.value} 🪙
                  </p>
                </div>

                {(item.bonusHealth > 0 || item.bonusMana > 0 || item.bonusStrength > 0 || 
                  item.bonusIntelligence > 0 || item.bonusAgility > 0) && (
                  <div className="bg-gray-700 rounded p-3 mb-3">
                    <p className="text-xs text-amber-500 font-semibold mb-2">Bônus:</p>
                    <div className="text-xs space-y-1">
                      {item.bonusHealth > 0 && <p>❤️ +{item.bonusHealth} Vida</p>}
                      {item.bonusMana > 0 && <p>💙 +{item.bonusMana} Mana</p>}
                      {item.bonusStrength > 0 && <p>💪 +{item.bonusStrength} Força</p>}
                      {item.bonusIntelligence > 0 && <p>🧠 +{item.bonusIntelligence} Inteligência</p>}
                      {item.bonusAgility > 0 && <p>⚡ +{item.bonusAgility} Agilidade</p>}
                    </div>
                  </div>
                )}

                {isAdmin && (
                  <Button variant="danger" onClick={() => handleDelete(item.id)} className="w-full">
                    {t('common.delete')}
                  </Button>
                )}
              </Card>
            ))}
          </div>
        )}

        <Modal
          isOpen={isModalOpen}
          onClose={handleCloseModal}
          title={t('items.create')}
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
              label={t('items.name')}
              value={formData.name}
              onChange={(e) => setFormData({ ...formData, name: e.target.value })}
              required
            />

            <div className="mb-4">
              <label className="label">{t('items.description')}</label>
              <textarea
                className="input"
                value={formData.description}
                onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                required
                rows={3}
              />
            </div>

            <div className="mb-4">
              <label className="label">{t('items.type')}</label>
              <select
                className="input"
                value={formData.type}
                onChange={(e) => setFormData({ ...formData, type: e.target.value })}
              >
                <option value="Weapon">Weapon</option>
                <option value="Armor">Armor</option>
                <option value="Accessory">Accessory</option>
                <option value="Consumable">Consumable</option>
              </select>
            </div>

            <div className="mb-4">
              <label className="label">{t('items.rarity')}</label>
              <select
                className="input"
                value={formData.rarity}
                onChange={(e) => setFormData({ ...formData, rarity: e.target.value })}
              >
                <option value="Common">Common</option>
                <option value="Uncommon">Uncommon</option>
                <option value="Rare">Rare</option>
                <option value="Epic">Epic</option>
                <option value="Legendary">Legendary</option>
              </select>
            </div>

            <Input
              label={t('items.value')}
              type="number"
              value={formData.value}
              onChange={(e) => setFormData({ ...formData, value: parseInt(e.target.value) })}
              required
              min="0"
            />

            <Input
              label={t('items.bonus_health')}
              type="number"
              value={formData.bonusHealth}
              onChange={(e) => setFormData({ ...formData, bonusHealth: parseInt(e.target.value) })}
              min="0"
            />

            <Input
              label={t('items.bonus_mana')}
              type="number"
              value={formData.bonusMana}
              onChange={(e) => setFormData({ ...formData, bonusMana: parseInt(e.target.value) })}
              min="0"
            />

            <Input
              label={t('items.bonus_strength')}
              type="number"
              value={formData.bonusStrength}
              onChange={(e) => setFormData({ ...formData, bonusStrength: parseInt(e.target.value) })}
              min="0"
            />

            <Input
              label={t('items.bonus_intelligence')}
              type="number"
              value={formData.bonusIntelligence}
              onChange={(e) => setFormData({ ...formData, bonusIntelligence: parseInt(e.target.value) })}
              min="0"
            />

            <Input
              label={t('items.bonus_agility')}
              type="number"
              value={formData.bonusAgility}
              onChange={(e) => setFormData({ ...formData, bonusAgility: parseInt(e.target.value) })}
              min="0"
            />
          </form>
        </Modal>
      </div>
    </>
  );
};

