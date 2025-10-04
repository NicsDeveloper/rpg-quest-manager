import React, { useState } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { useCharacter } from '../contexts/CharacterContext';
import { SoundSettings } from './SoundSettings';
import { SaveManager } from './SaveManager';
import { 
  Home, 
  Sword, 
  ShoppingBag, 
  Backpack, 
  Map, 
  LogOut,
  User,
  Star,
  Users,
  Volume2,
  Save,
  Crown
} from 'lucide-react';

interface LayoutProps {
  children: React.ReactNode;
}

export function Layout({ children }: LayoutProps) {
  const { user, logout } = useAuth();
  const { character } = useCharacter();
  const location = useLocation();
  const [showSoundSettings, setShowSoundSettings] = useState(false);
  const [showSaveManager, setShowSaveManager] = useState(false);

  const navigation = [
    { name: 'Dashboard', href: '/', icon: Home },
    { name: 'Personagem', href: '/character', icon: User },
    { name: 'Inventário', href: '/inventory', icon: Backpack },
    { name: 'Loja', href: '/shop', icon: ShoppingBag },
    { name: 'Missões', href: '/quests', icon: Map },
    { name: 'Combate', href: '/combat', icon: Sword },
    { name: 'Conquistas', href: '/achievements', icon: Star },
    { name: 'Grupos', href: '/parties', icon: Users },
  ];

  const isActive = (href: string) => {
    return location.pathname === href;
  };

  return (
    <div className="min-h-screen">
      {/* Header */}
      <header className="glass border-b border-gray-700/50 shadow-2xl" data-tutorial="dashboard">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center h-20">
            <div className="flex items-center space-x-4">
              <div className="flex items-center space-x-3">
                <div className="relative">
                  <Crown className="h-10 w-10 text-amber-400 animate-pulse" />
                  <Sword className="h-8 w-8 text-blue-400 absolute -top-1 -right-1" />
                </div>
                <div>
                  <h1 className="text-2xl font-bold text-white">
                    <span className="text-gradient">
                      RPG Quest Manager
                    </span>
                  </h1>
                  <p className="text-sm text-gray-400">Reino de Aventuras</p>
                </div>
              </div>
            </div>


            <div className="flex items-center space-x-4" data-tutorial="settings">
              <span className="text-sm text-gray-400">
                Bem-vindo, <span className="text-amber-400 font-bold">{user?.username}</span>
              </span>
              <button
                onClick={() => setShowSoundSettings(true)}
                className="flex items-center space-x-1 text-gray-400 hover:text-amber-400 transition-colors duration-300 p-2 rounded-lg hover:bg-gray-800/50"
                title="Configurações de áudio"
              >
                <Volume2 className="h-5 w-5" />
              </button>
              <button
                onClick={() => setShowSaveManager(true)}
                className="flex items-center space-x-1 text-gray-400 hover:text-amber-400 transition-colors duration-300 p-2 rounded-lg hover:bg-gray-800/50"
                title="Gerenciador de saves"
              >
                <Save className="h-5 w-5" />
              </button>
              <button
                onClick={logout}
                className="flex items-center space-x-1 text-gray-400 hover:text-red-400 transition-colors duration-300 p-2 rounded-lg hover:bg-red-900/20"
              >
                <LogOut className="h-5 w-5" />
                <span className="hidden sm:inline text-sm">Sair</span>
              </button>
            </div>
          </div>
        </div>
      </header>

      <div className="flex">
        {/* Sidebar */}
        <nav className="w-64 glass border-r border-gray-700/50 shadow-2xl min-h-screen">
          <div className="p-6">
            <h2 className="text-lg font-bold text-gradient mb-6 text-center">
              🏰 Navegação
            </h2>
            <ul className="space-y-3" data-tutorial="navigation">
              {navigation.map((item) => {
                const Icon = item.icon;
                return (
                  <li key={item.name}>
                    <Link
                      to={item.href}
                      className={`flex items-center space-x-3 px-4 py-3 w-full rounded-xl transition-all duration-300 ${
                        isActive(item.href) 
                          ? 'bg-gradient-to-r from-amber-500 to-orange-600 text-white shadow-lg shadow-amber-500/50' 
                          : 'text-gray-400 hover:text-gray-100 hover:bg-gray-800/50'
                      }`}
                      data-tutorial={item.name.toLowerCase()}
                    >
                      <Icon className="h-6 w-6" />
                      <span className="font-bold">{item.name}</span>
                    </Link>
                  </li>
                );
              })}
            </ul>
          </div>
        </nav>

        {/* Main Content */}
        <main className="flex-1 min-h-screen">
          {children}
        </main>
      </div>

      {/* Sound Settings Modal */}
      <SoundSettings 
        isOpen={showSoundSettings} 
        onClose={() => setShowSoundSettings(false)} 
      />

      {/* Save Manager Modal */}
      <SaveManager
        isOpen={showSaveManager}
        onClose={() => setShowSaveManager(false)}
        onLoadGame={(saveData) => {
          // Implementar carregamento do jogo
          console.log('Carregando jogo:', saveData);
        }}
        characterId={character?.id}
        characterData={character}
        gameState={{
          currentQuest: null,
          lastLocation: location.pathname,
          achievements: [],
          unlockedAbilities: [],
          partyId: null
        }}
      />
    </div>
  );
}
