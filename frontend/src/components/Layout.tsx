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
  Coins,
  Heart,
  Shield,
  Star,
  Users,
  Volume2,
  Save
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
    <div className="min-h-screen bg-gray-100">
      {/* Header */}
      <header className="bg-white shadow-sm border-b border-gray-200">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center h-16">
            <div className="flex items-center space-x-4">
              <div className="flex items-center space-x-2">
                <Sword className="h-8 w-8 text-blue-600" />
                <h1 className="text-xl font-bold text-gray-900">RPG Quest Manager</h1>
              </div>
            </div>

            {/* Character Stats */}
            {character && (
              <div className="hidden md:flex items-center space-x-6">
                <div className="flex items-center space-x-2 text-sm">
                  <User className="h-4 w-4 text-gray-500" />
                  <span className="font-medium">{character.name}</span>
                  <span className="text-gray-500">Nível {character.level}</span>
                </div>
                
                <div className="flex items-center space-x-2 text-sm">
                  <Heart className="h-4 w-4 text-red-500" />
                  <span>{character.health}/{character.maxHealth}</span>
                </div>
                
                <div className="flex items-center space-x-2 text-sm">
                  <Coins className="h-4 w-4 text-yellow-500" />
                  <span>{character.gold}</span>
                </div>
                
                <div className="flex items-center space-x-2 text-sm">
                  <Shield className="h-4 w-4 text-blue-500" />
                  <span>{character.morale}/100</span>
                </div>
              </div>
            )}

            <div className="flex items-center space-x-4" data-tutorial="settings">
              <span className="text-sm text-gray-600">Olá, {user?.username}</span>
              <button
                onClick={() => setShowSoundSettings(true)}
                className="flex items-center space-x-1 text-gray-600 hover:text-gray-900 transition-colors"
                title="Configurações de áudio"
              >
                <Volume2 className="h-4 w-4" />
              </button>
              <button
                onClick={() => setShowSaveManager(true)}
                className="flex items-center space-x-1 text-gray-600 hover:text-gray-900 transition-colors"
                title="Gerenciador de saves"
              >
                <Save className="h-4 w-4" />
              </button>
              <button
                onClick={logout}
                className="flex items-center space-x-1 text-gray-600 hover:text-gray-900 transition-colors"
              >
                <LogOut className="h-4 w-4" />
                <span className="hidden sm:inline">Sair</span>
              </button>
            </div>
          </div>
        </div>
      </header>

      <div className="flex">
        {/* Sidebar */}
        <nav className="w-64 bg-white shadow-sm min-h-screen border-r border-gray-200">
          <div className="p-4">
            <ul className="space-y-2" data-tutorial="navigation">
              {navigation.map((item) => {
                const Icon = item.icon;
                return (
                  <li key={item.name}>
                    <Link
                      to={item.href}
                      className={`flex items-center space-x-3 px-3 py-2 rounded-lg transition-colors ${
                        isActive(item.href)
                          ? 'bg-blue-100 text-blue-700 font-medium'
                          : 'text-gray-700 hover:bg-gray-100'
                      }`}
                    >
                      <Icon className="h-5 w-5" />
                      <span>{item.name}</span>
                    </Link>
                  </li>
                );
              })}
            </ul>
          </div>
        </nav>

        {/* Main Content */}
        <main className="flex-1 p-6">
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
