import React, { useState, useEffect } from 'react';
import { profileService, MyHero } from '../services/profileService';
import { userService, UserInfo } from '../services/userService';
import { Link } from 'react-router-dom';

const getExperienceForNextLevel = (level: number): number => {
  if (level >= 20) return Number.MAX_SAFE_INTEGER;
  return Math.ceil(100 * Math.pow(1.5, level - 1));
};

export const HeroWidget: React.FC = () => {
  const [party, setParty] = useState<MyHero[]>([]);
  const [user, setUser] = useState<UserInfo | null>(null);
  const [isExpanded, setIsExpanded] = useState(false);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const [partyData, userData] = await Promise.all([
        profileService.getActiveParty(),
        userService.getCurrentUser()
      ]);
      setParty(partyData);
      setUser(userData);
    } catch (error) {
      console.error('Erro ao carregar dados:', error);
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

  if (!party || party.length === 0) {
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

  // Pega o herói principal (primeiro da party ou maior nível)
  const mainHero = party[0];
  const xpForNextLevel = getExperienceForNextLevel(mainHero.level);
  const xpProgress = mainHero.level >= 20 ? 100 : (mainHero.experience / xpForNextLevel) * 100;
  const isMaxLevel = mainHero.level >= 20;

  // Ícone da classe
  const getClassIcon = (heroClass: string) => 
    heroClass === 'Guerreiro' ? '⚔️' : heroClass === 'Mago' ? '🔮' : '🏹';

  return (
    <div 
      className="hero-widget"
      onMouseEnter={() => setIsExpanded(true)}
      onMouseLeave={() => setIsExpanded(false)}
    >
      {/* Versão Compacta - Mostra herói principal + badge de quantidade */}
      <div className={`hero-widget-compact ${isExpanded ? 'opacity-0' : 'opacity-100'}`}>
        <div className="flex items-center gap-3">
          <div className="relative">
            <div className="w-12 h-12 bg-gradient-to-br from-amber-500 to-orange-600 rounded-full flex items-center justify-center text-2xl shadow-lg">
              {getClassIcon(mainHero.class)}
            </div>
            <div className="absolute -bottom-1 -right-1 bg-gray-900 rounded-full px-2 py-0.5 border-2 border-amber-500">
              <span className="text-xs font-bold text-amber-500">{mainHero.level}</span>
            </div>
            {party.length > 1 && (
              <div className="absolute -top-1 -left-1 bg-purple-600 rounded-full w-5 h-5 flex items-center justify-center border-2 border-gray-900">
                <span className="text-xs font-bold text-white">{party.length}</span>
              </div>
            )}
          </div>
          <div className="flex-1 min-w-0">
            <p className="text-sm font-bold text-white truncate">
              {mainHero.name}
              {party.length > 1 && <span className="text-purple-400 ml-1">+{party.length - 1}</span>}
            </p>
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

      {/* Versão Expandida - Mostra todos os heróis da party */}
      <div className={`hero-widget-expanded ${isExpanded ? 'opacity-100 scale-100' : 'opacity-0 scale-95 pointer-events-none'}`}>
        <Link to="/profile" className="block">
          <div className="p-4 space-y-3 max-h-[600px] overflow-y-auto">
            {/* Ouro do Player - SEMPRE NO TOPO */}
            {user && (
              <div className="flex items-center justify-between bg-gradient-to-r from-amber-500/20 to-orange-600/20 border-2 border-amber-500/50 rounded-lg p-3 mb-3 shadow-lg">
                <div className="flex items-center gap-2">
                  <span className="text-2xl">💰</span>
                  <span className="text-sm font-semibold text-amber-300">Ouro do Player</span>
                </div>
                <span className="text-2xl font-black text-amber-500">{user.gold.toLocaleString()}</span>
              </div>
            )}

            {/* Título da Party */}
            <div className="text-center pb-2 border-b border-gray-700">
              <h3 className="text-lg font-black text-transparent bg-clip-text bg-gradient-to-r from-purple-400 to-pink-500">
                ⚔️ PARTY ATIVA ⚔️
              </h3>
              <p className="text-xs text-gray-400">{party.length} herói{party.length > 1 ? 's' : ''} na party</p>
            </div>

            {/* Lista de Heróis */}
            <div className="space-y-3">
              {party.map((hero, index) => {
                const heroXpForNextLevel = getExperienceForNextLevel(hero.level);
                const heroXpProgress = hero.level >= 20 ? 100 : (hero.experience / heroXpForNextLevel) * 100;
                const heroIsMaxLevel = hero.level >= 20;

                return (
                  <div 
                    key={hero.id} 
                    className={`p-3 rounded-xl border-2 ${
                      index === 0 
                        ? 'border-amber-500/50 bg-gradient-to-br from-amber-500/10 to-orange-600/10' 
                        : 'border-gray-700 bg-gray-800/50'
                    } hover:border-purple-500/50 transition-all`}
                  >
                    {/* Cabeçalho do Herói */}
                    <div className="flex items-center gap-3 mb-2">
                      <div className="relative">
                        <div className={`w-14 h-14 bg-gradient-to-br ${
                          index === 0 
                            ? 'from-amber-500 to-orange-600' 
                            : 'from-gray-600 to-gray-700'
                        } rounded-full flex items-center justify-center text-2xl shadow-lg`}>
                          {getClassIcon(hero.class)}
                        </div>
                        {heroIsMaxLevel && (
                          <div className="absolute -top-1 -right-1 text-xl animate-bounce">👑</div>
                        )}
                        <div className="absolute -bottom-1 -right-1 bg-gray-900 rounded-full px-2 py-0.5 border-2 border-amber-500">
                          <span className="text-xs font-bold text-amber-500">Nv.{hero.level}</span>
                        </div>
                        {index === 0 && (
                          <div className="absolute -top-1 -left-1 bg-purple-600 rounded-full px-1.5 py-0.5 border-2 border-gray-900">
                            <span className="text-xs font-bold text-white">★</span>
                          </div>
                        )}
                      </div>
                      <div className="flex-1">
                        <h4 className="text-base font-bold text-white">{hero.name}</h4>
                        <p className="text-xs text-gray-400">{hero.class}</p>
                      </div>
                    </div>

                    {/* XP e Progressão */}
                    {!heroIsMaxLevel ? (
                      <div className="mb-2">
                        <div className="flex justify-between text-xs text-gray-400 mb-1">
                          <span>XP</span>
                          <span className="font-mono">{hero.experience} / {heroXpForNextLevel}</span>
                        </div>
                        <div className="bg-gray-700 rounded-full h-2 overflow-hidden">
                          <div 
                            className="h-full bg-gradient-to-r from-blue-500 via-purple-500 to-pink-500 transition-all duration-300"
                            style={{ width: `${heroXpProgress}%` }}
                          />
                        </div>
                      </div>
                    ) : (
                      <div className="text-center py-1 bg-gradient-to-r from-amber-500/20 to-orange-600/20 rounded border border-amber-500/30 mb-2">
                        <p className="text-xs font-bold text-amber-500">👑 NÍVEL MÁXIMO!</p>
                      </div>
                    )}

                    {/* Atributos */}
                    <div className="grid grid-cols-3 gap-1.5">
                      <div className="bg-red-500/10 border border-red-500/30 rounded p-1.5 text-center">
                        <p className="text-xs text-red-400">💪</p>
                        <p className="text-sm font-bold text-white">{hero.strength}</p>
                      </div>
                      <div className="bg-blue-500/10 border border-blue-500/30 rounded p-1.5 text-center">
                        <p className="text-xs text-blue-400">🧠</p>
                        <p className="text-sm font-bold text-white">{hero.intelligence}</p>
                      </div>
                      <div className="bg-green-500/10 border border-green-500/30 rounded p-1.5 text-center">
                        <p className="text-xs text-green-400">⚡</p>
                        <p className="text-sm font-bold text-white">{hero.dexterity}</p>
                      </div>
                    </div>
                  </div>
                );
              })}
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
