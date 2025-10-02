import React, { useState, useEffect } from 'react';
import { Navbar } from '../components/Navbar';
import { Card } from '../components/Card';
import { Loading } from '../components/Loading';
import { HeroWidget } from '../components/HeroWidget';
import api from '../services/api';

interface DeletedHero {
  id: number;
  name: string;
  class: string;
  level: number;
  experience: number;
  strength: number;
  intelligence: number;
  dexterity: number;
  deletedAt: string;
  daysUntilPermanentDeletion: number;
  canRecover: boolean;
}

export const HeroRecovery: React.FC = () => {
  const [deletedHeroes, setDeletedHeroes] = useState<DeletedHero[]>([]);
  const [loading, setLoading] = useState(true);
  const [actionLoading, setActionLoading] = useState<number | null>(null);

  useEffect(() => {
    loadDeletedHeroes();
  }, []);

  const loadDeletedHeroes = async () => {
    try {
      setLoading(true);
      const response = await api.get('/profile/deleted-heroes');
      setDeletedHeroes(response.data);
    } catch (error) {
      console.error('Erro ao carregar heróis deletados:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleRestore = async (heroId: number) => {
    if (!confirm('Tem certeza que deseja restaurar este herói?')) {
      return;
    }

    try {
      setActionLoading(heroId);
      const response = await api.post(`/profile/restore-hero/${heroId}`);
      alert(response.data.message || 'Herói restaurado com sucesso!');
      await loadDeletedHeroes();
    } catch (error: any) {
      alert(error.response?.data?.message || error.message || 'Erro ao restaurar herói');
    } finally {
      setActionLoading(null);
    }
  };

  const getClassIcon = (heroClass: string) => {
    switch (heroClass) {
      case 'Guerreiro':
        return '⚔️';
      case 'Mago':
        return '🔮';
      case 'Arqueiro':
        return '🏹';
      default:
        return '🛡️';
    }
  };

  const getDaysColor = (days: number) => {
    if (days <= 1) return 'text-red-500';
    if (days <= 3) return 'text-orange-500';
    return 'text-yellow-500';
  };

  if (loading) {
    return (
      <>
        <Navbar />
        <HeroWidget />
        <Loading />
      </>
    );
  }

  return (
    <>
      <Navbar />
      <HeroWidget />
      <div className="container mx-auto px-4 py-8">
        {/* Header */}
        <div className="text-center mb-8">
          <h1 className="text-5xl font-black text-transparent bg-clip-text bg-gradient-to-r from-red-500 via-orange-500 to-yellow-500 mb-4 animate-pulse">
            ♻️ Área de Recuperação
          </h1>
          <p className="text-gray-400 text-lg">
            Heróis deletados ficam aqui por <span className="text-amber-500 font-bold">7 dias</span> antes da deleção permanente
          </p>
        </div>

        {/* Lista de Heróis Deletados */}
        {deletedHeroes.length === 0 ? (
          <Card className="text-center py-12">
            <div className="text-6xl mb-4">✨</div>
            <h2 className="text-2xl font-bold text-gray-300 mb-2">Nenhum herói deletado</h2>
            <p className="text-gray-500">Você não tem heróis na área de recuperação.</p>
          </Card>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {deletedHeroes.map((hero) => (
              <Card
                key={hero.id}
                className={`relative overflow-hidden transition-all duration-300 hover:scale-105 ${
                  !hero.canRecover
                    ? 'opacity-50 border-red-500/50 bg-red-900/10'
                    : 'border-orange-500/30 bg-gradient-to-br from-orange-900/20 to-red-900/20'
                }`}
              >
                {/* Badge de Status */}
                <div className="absolute top-4 right-4">
                  {hero.canRecover ? (
                    <div className="bg-green-600 text-white text-xs px-3 py-1 rounded-full font-bold animate-pulse">
                      RECUPERÁVEL
                    </div>
                  ) : (
                    <div className="bg-red-600 text-white text-xs px-3 py-1 rounded-full font-bold">
                      EXPIRADO
                    </div>
                  )}
                </div>

                {/* Ícone e Nome */}
                <div className="text-center mb-4">
                  <div className="text-6xl mb-3 opacity-70">{getClassIcon(hero.class)}</div>
                  <h3 className="text-2xl font-bold text-white mb-1">{hero.name}</h3>
                  <p className="text-gray-400">{hero.class} - Nível {hero.level}</p>
                </div>

                {/* Atributos */}
                <div className="grid grid-cols-3 gap-2 mb-4">
                  <div className="bg-red-900/30 border border-red-700/50 rounded p-2 text-center">
                    <p className="text-xs text-red-400">💪 FOR</p>
                    <p className="text-lg font-bold text-white">{hero.strength}</p>
                  </div>
                  <div className="bg-blue-900/30 border border-blue-700/50 rounded p-2 text-center">
                    <p className="text-xs text-blue-400">🧠 INT</p>
                    <p className="text-lg font-bold text-white">{hero.intelligence}</p>
                  </div>
                  <div className="bg-green-900/30 border border-green-700/50 rounded p-2 text-center">
                    <p className="text-xs text-green-400">⚡ DES</p>
                    <p className="text-lg font-bold text-white">{hero.dexterity}</p>
                  </div>
                </div>

                {/* Informações de Deleção */}
                <div className="bg-gray-900/50 rounded-lg p-4 mb-4 border border-gray-700">
                  <div className="flex items-center justify-between mb-2">
                    <span className="text-sm text-gray-400">Deletado em:</span>
                    <span className="text-sm text-gray-300">
                      {new Date(hero.deletedAt).toLocaleDateString('pt-BR')}
                    </span>
                  </div>
                  <div className="flex items-center justify-between">
                    <span className="text-sm text-gray-400">Tempo restante:</span>
                    <span className={`text-lg font-bold ${getDaysColor(hero.daysUntilPermanentDeletion)}`}>
                      {hero.daysUntilPermanentDeletion === 0 ? (
                        'EXPIRADO'
                      ) : (
                        <>
                          {hero.daysUntilPermanentDeletion} dia{hero.daysUntilPermanentDeletion !== 1 ? 's' : ''}
                        </>
                      )}
                    </span>
                  </div>
                </div>

                {/* Botão de Restaurar */}
                {hero.canRecover ? (
                  <button
                    onClick={() => handleRestore(hero.id)}
                    disabled={actionLoading === hero.id}
                    className="w-full bg-gradient-to-r from-green-600 to-emerald-700 hover:from-green-500 hover:to-emerald-600 text-white font-bold py-3 px-6 rounded-lg transition-all duration-300 transform hover:scale-105 disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2"
                  >
                    {actionLoading === hero.id ? (
                      <>
                        <div className="animate-spin rounded-full h-5 w-5 border-b-2 border-white"></div>
                        Restaurando...
                      </>
                    ) : (
                      <>
                        ♻️ Restaurar Herói
                      </>
                    )}
                  </button>
                ) : (
                  <div className="w-full bg-red-900/30 border-2 border-red-700/50 text-red-400 font-bold py-3 px-6 rounded-lg text-center">
                    ⚠️ Não pode ser recuperado
                  </div>
                )}

                {/* Aviso de Exclusão Permanente */}
                {hero.daysUntilPermanentDeletion <= 2 && hero.canRecover && (
                  <div className="mt-3 bg-red-900/30 border border-red-700 rounded-lg p-3 text-center animate-pulse">
                    <p className="text-red-400 text-sm font-bold">
                      ⚠️ ATENÇÃO: Este herói será deletado permanentemente em breve!
                    </p>
                  </div>
                )}
              </Card>
            ))}
          </div>
        )}

        {/* Info Box */}
        <Card className="mt-8 bg-gradient-to-r from-blue-900/20 to-purple-900/20 border-blue-500/30">
          <div className="flex items-start gap-4">
            <div className="text-4xl">ℹ️</div>
            <div>
              <h3 className="text-xl font-bold text-blue-400 mb-2">Como funciona a recuperação?</h3>
              <ul className="space-y-2 text-gray-300">
                <li className="flex items-start gap-2">
                  <span className="text-green-500 mt-1">✓</span>
                  <span>Heróis deletados ficam aqui por <strong className="text-amber-500">7 dias</strong></span>
                </li>
                <li className="flex items-start gap-2">
                  <span className="text-green-500 mt-1">✓</span>
                  <span>Durante esse período, você pode restaurá-los a qualquer momento</span>
                </li>
                <li className="flex items-start gap-2">
                  <span className="text-green-500 mt-1">✓</span>
                  <span>Após 7 dias, o herói será <strong className="text-red-500">deletado permanentemente</strong></span>
                </li>
                <li className="flex items-start gap-2">
                  <span className="text-green-500 mt-1">✓</span>
                  <span>Heróis restaurados voltam com todos os seus itens e progresso intactos</span>
                </li>
              </ul>
            </div>
          </div>
        </Card>
      </div>
    </>
  );
};

