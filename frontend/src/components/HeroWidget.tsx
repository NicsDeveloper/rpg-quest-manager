import React, { useState, useEffect } from 'react';
import { profileService, MyHero } from '../services/profileService';
import { Link } from 'react-router-dom';

const getExperienceForNextLevel = (level: number): number => {
  if (level >= 20) return Number.MAX_SAFE_INTEGER;
  return Math.ceil(100 * Math.pow(1.5, level - 1));
};

export const HeroWidget: React.FC = () => {
  const [hero, setHero] = useState<MyHero | null>(null);
  const [isExpanded, setIsExpanded] = useState(false);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadHero();
  }, []);

  const loadHero = async () => {
    try {
      const heroData = await profileService.getMyHero();
      setHero(heroData);
    } catch (error) {
      console.error('Erro ao carregar herói:', error);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="hero-widget-compact">
        <div className="animate-pulse flex items-center gap-3">
          <div className="w-12 h-12 bg-gray-700 rounded-full"></div>
          <div className="flex-1">
            <div className="h-4 bg-gray-700 rounded w-24 mb-2"></div>
            <div className="h-2 bg-gray-700 rounded w-32"></div>
          </div>
        </div>
      </div>
    );
  }

  if (!hero) {
    return (
      <Link to="/profile" className="hero-widget-compact hover:bg-gray-800/50 transition-colors">
        <div className="flex items-center gap-3">
          <div className="w-12 h-12 bg-gradient-to-br from-gray-700 to-gray-800 rounded-full flex items-center justify-center text-2xl">
            ❓
          </div>
          <div className="flex-1">
            <p className="text-sm font-semibold text-gray-300">Sem Herói</p>
            <p className="text-xs text-gray-500">Clique para criar</p>
          </div>
        </div>
      </Link>
    );
  }

  const xpForNextLevel = getExperienceForNextLevel(hero.level);
  const xpProgress = hero.level >= 20 ? 100 : (hero.experience / xpForNextLevel) * 100;
  const isMaxLevel = hero.level >= 20;

  // Ícone da classe
  const classIcon = hero.class === 'Guerreiro' ? '⚔️' : hero.class === 'Mago' ? '🔮' : '🏹';

  return (
    <div 
      className="hero-widget"
      onMouseEnter={() => setIsExpanded(true)}
      onMouseLeave={() => setIsExpanded(false)}
    >
      {/* Versão Compacta */}
      <div className={`hero-widget-compact ${isExpanded ? 'opacity-0' : 'opacity-100'}`}>
        <div className="flex items-center gap-3">
          <div className="relative">
            <div className="w-12 h-12 bg-gradient-to-br from-amber-500 to-orange-600 rounded-full flex items-center justify-center text-2xl shadow-lg">
              {classIcon}
            </div>
            <div className="absolute -bottom-1 -right-1 bg-gray-900 rounded-full px-2 py-0.5 border-2 border-amber-500">
              <span className="text-xs font-bold text-amber-500">{hero.level}</span>
            </div>
          </div>
          <div className="flex-1 min-w-0">
            <p className="text-sm font-bold text-white truncate">{hero.name}</p>
            <div className="flex items-center gap-2">
              <div className="flex-1 bg-gray-700 rounded-full h-2 overflow-hidden">
                <div 
                  className="h-full bg-gradient-to-r from-blue-500 to-purple-500 transition-all duration-300"
                  style={{ width: `${xpProgress}%` }}
                />
              </div>
              {!isMaxLevel && (
                <span className="text-xs text-gray-400">{Math.floor(xpProgress)}%</span>
              )}
            </div>
          </div>
        </div>
      </div>

      {/* Versão Expandida */}
      <div className={`hero-widget-expanded ${isExpanded ? 'opacity-100 scale-100' : 'opacity-0 scale-95 pointer-events-none'}`}>
        <Link to="/profile" className="block">
          <div className="p-4 space-y-3">
            {/* Cabeçalho */}
            <div className="flex items-center gap-3 border-b border-gray-700 pb-3">
              <div className="relative">
                <div className="w-16 h-16 bg-gradient-to-br from-amber-500 to-orange-600 rounded-full flex items-center justify-center text-3xl shadow-lg">
                  {classIcon}
                </div>
                {isMaxLevel && (
                  <div className="absolute -top-1 -right-1 text-2xl animate-bounce">👑</div>
                )}
                <div className="absolute -bottom-1 -right-1 bg-gray-900 rounded-full px-2.5 py-1 border-2 border-amber-500">
                  <span className="text-sm font-bold text-amber-500">Nv.{hero.level}</span>
                </div>
              </div>
              <div className="flex-1">
                <h3 className="text-lg font-black text-white">{hero.name}</h3>
                <p className="text-sm text-gray-400">{hero.class}</p>
              </div>
            </div>

            {/* XP e Progressão */}
            {!isMaxLevel ? (
              <div>
                <div className="flex justify-between text-xs text-gray-400 mb-1">
                  <span>Experiência</span>
                  <span className="font-mono">{hero.experience} / {xpForNextLevel}</span>
                </div>
                <div className="bg-gray-700 rounded-full h-3 overflow-hidden">
                  <div 
                    className="h-full bg-gradient-to-r from-blue-500 via-purple-500 to-pink-500 transition-all duration-300"
                    style={{ width: `${xpProgress}%` }}
                  />
                </div>
              </div>
            ) : (
              <div className="text-center py-2 bg-gradient-to-r from-amber-500/20 to-orange-600/20 rounded-lg border border-amber-500/30">
                <p className="text-sm font-bold text-amber-500">👑 NÍVEL MÁXIMO ALCANÇADO!</p>
              </div>
            )}

            {/* Atributos */}
            <div className="grid grid-cols-3 gap-2">
              <div className="bg-red-500/10 border border-red-500/30 rounded-lg p-2 text-center">
                <p className="text-xs text-red-400">💪 Força</p>
                <p className="text-lg font-bold text-white">{hero.strength}</p>
              </div>
              <div className="bg-blue-500/10 border border-blue-500/30 rounded-lg p-2 text-center">
                <p className="text-xs text-blue-400">🧠 Int.</p>
                <p className="text-lg font-bold text-white">{hero.intelligence}</p>
              </div>
              <div className="bg-green-500/10 border border-green-500/30 rounded-lg p-2 text-center">
                <p className="text-xs text-green-400">⚡ Dest.</p>
                <p className="text-lg font-bold text-white">{hero.dexterity}</p>
              </div>
            </div>

            {/* Ouro */}
            <div className="flex items-center justify-between bg-amber-500/10 border border-amber-500/30 rounded-lg p-2">
              <span className="text-sm text-amber-400">💰 Ouro</span>
              <span className="text-lg font-bold text-amber-500">{hero.gold.toLocaleString()}</span>
            </div>

            {/* Link para perfil */}
            <div className="text-center pt-2 border-t border-gray-700">
              <p className="text-xs text-gray-400">Clique para ver perfil completo →</p>
            </div>
          </div>
        </Link>
      </div>
    </div>
  );
};

