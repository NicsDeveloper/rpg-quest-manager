import React, { useEffect, useState } from 'react';
import api from '../services/api';

interface FreeDiceGrant {
  diceType: string;
  lastClaimedAt: string;
  nextAvailableAt: string;
  isAvailable: boolean;
  cooldownHours: number;
  timeUntilAvailable: string;
}

export const FreeDice: React.FC = () => {
  const [grants, setGrants] = useState<FreeDiceGrant[]>([]);
  const [loading, setLoading] = useState(true);
  const [success, setSuccess] = useState('');
  const [error, setError] = useState('');

  useEffect(() => {
    fetchGrants();
    const interval = setInterval(fetchGrants, 60000); // Atualiza a cada minuto
    return () => clearInterval(interval);
  }, []);

  const fetchGrants = async () => {
    try {
      setLoading(true);
      const response = await api.get('/freedice');
      setGrants(response.data);
    } catch (err) {
      console.error('Erro ao carregar dados gratuitos:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleClaim = async (diceType: string) => {
    try {
      setError('');
      setSuccess('');
      await api.post(`/freedice/claim/${diceType}`);
      setSuccess(`✅ Dado ${diceType} resgatado com sucesso!`);
      fetchGrants();
    } catch (err: any) {
      setError(err.response?.data?.message || err.response?.data || 'Erro ao resgatar dado');
    }
  };

  const formatTimeRemaining = (timeString: string) => {
    const [hours, minutes] = timeString.match(/(\d+):(\d+):(\d+)/)?.slice(1, 3) || ['0', '0'];
    const h = parseInt(hours);
    const m = parseInt(minutes);

    if (h > 0) {
      return `${h}h ${m}m`;
    }
    return `${m}m`;
  };

  const getDiceIcon = (diceType: string) => {
    switch (diceType) {
      case 'D6':
        return '🎲';
      case 'D8':
        return '🎯';
      case 'D12':
        return '⭐';
      case 'D20':
        return '👑';
      default:
        return '🎲';
    }
  };

  const getDiceColor = (diceType: string) => {
    switch (diceType) {
      case 'D6':
        return 'from-gray-800 to-gray-700 border-gray-600';
      case 'D8':
        return 'from-green-900 to-green-800 border-green-600';
      case 'D12':
        return 'from-blue-900 to-blue-800 border-blue-600';
      case 'D20':
        return 'from-purple-900 to-purple-800 border-purple-600';
      default:
        return 'from-gray-800 to-gray-700 border-gray-600';
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-screen">
        <div className="text-xl">Carregando...</div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8 max-w-4xl">
      <h1 className="text-4xl font-bold mb-2 flex items-center gap-2">
        🎁 Dados Gratuitos
      </h1>
      <p className="text-gray-400 mb-8">
        Resgate dados gratuitos periodicamente! Cada tipo de dado tem seu próprio cooldown.
      </p>

      {error && (
        <div className="bg-red-500/20 border border-red-500 text-red-200 px-4 py-3 rounded-lg mb-6">
          {error}
        </div>
      )}

      {success && (
        <div className="bg-green-500/20 border border-green-500 text-green-200 px-4 py-3 rounded-lg mb-6">
          {success}
        </div>
      )}

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        {grants.map((grant) => (
          <div
            key={grant.diceType}
            className={`bg-gradient-to-br ${getDiceColor(grant.diceType)} border-2 rounded-lg p-6`}
          >
            <div className="flex items-center justify-between mb-4">
              <div className="flex items-center gap-3">
                <span className="text-5xl">{getDiceIcon(grant.diceType)}</span>
                <div>
                  <h3 className="text-2xl font-bold">{grant.diceType}</h3>
                  <p className="text-sm text-gray-400">
                    Cooldown: {grant.cooldownHours}h
                  </p>
                </div>
              </div>
            </div>

            {grant.isAvailable ? (
              <button
                onClick={() => handleClaim(grant.diceType)}
                className="w-full bg-green-600 hover:bg-green-700 text-white py-3 rounded-lg font-bold transition"
              >
                ✅ Resgatar Agora
              </button>
            ) : (
              <div className="space-y-2">
                <div className="bg-black/30 rounded-lg p-3 text-center">
                  <div className="text-sm text-gray-400">Próximo resgate em:</div>
                  <div className="text-xl font-bold text-yellow-400">
                    ⏳ {formatTimeRemaining(grant.timeUntilAvailable)}
                  </div>
                </div>
                <button
                  disabled
                  className="w-full bg-gray-700 text-gray-500 py-3 rounded-lg font-bold cursor-not-allowed"
                >
                  🔒 Indisponível
                </button>
              </div>
            )}

            <div className="mt-4 text-xs text-gray-500 text-center">
              Último resgate:{' '}
              {new Date(grant.lastClaimedAt).toLocaleString('pt-BR')}
            </div>
          </div>
        ))}
      </div>

      <div className="mt-8 bg-yellow-900/20 border border-yellow-600 rounded-lg p-6">
        <h3 className="text-lg font-bold mb-2 text-yellow-300">💡 Dica</h3>
        <p className="text-gray-300">
          Se você ficar sem dados e sem ouro, não se preocupe! Os dados gratuitos
          estão aqui para você nunca ficar impedido de jogar. Cada tipo de dado
          tem seu próprio cooldown independente.
        </p>
      </div>
    </div>
  );
};

