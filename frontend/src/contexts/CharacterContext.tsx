import React, { createContext, useContext, useEffect, useState, useCallback, useRef } from 'react';
import { heroService, type Hero } from '../services/heroService';
import { useAuth } from './AuthContext';
import { cacheService } from '../services/cache';

interface CharacterContextType {
  character: Hero | null;
  isLoading: boolean;
  error: string | null;
  refreshCharacter: () => Promise<void>;
  updateCharacter: (updates: Partial<Hero>) => Promise<void>;
  updateCharacterGold: (newGold: number) => void;
  updateCharacterExperience: (newExperience: number) => void;
  updateCharacterLevel: (newLevel: number) => void;
  updateCharacterHealth: (newHealth: number) => void;
}

const CharacterContext = createContext<CharacterContextType | undefined>(undefined);

export function CharacterProvider({ children }: { children: React.ReactNode }) {
  const { user } = useAuth();
  const [character, setCharacter] = useState<Hero | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const loadingRef = useRef(false);

  const refreshCharacter = useCallback(async () => {
    if (!user || loadingRef.current) return;
    
    try {
      loadingRef.current = true;
      setIsLoading(true);
      setError(null);
      
      // Buscar o primeiro herói da party ativa do usuário
      const heroes = await heroService.getActiveParty();
      if (heroes.length > 0) {
        setCharacter(heroes[0]); // Usar o primeiro herói da party ativa
      } else {
        // Se não há heróis na party ativa, buscar todos os heróis do usuário
        const allHeroes = await heroService.getMyHeroes();
        if (allHeroes.length > 0) {
          setCharacter(allHeroes[0]);
        } else {
          setCharacter(null);
        }
      }
    } catch (err: any) {
      console.error('Erro ao carregar herói:', err);
      setError(err.message || 'Erro ao carregar herói');
    } finally {
      setIsLoading(false);
      loadingRef.current = false;
    }
  }, [user]);

  const updateCharacter = async (updates: Partial<Hero>) => {
    if (!character) return;
    
    try {
      const updatedCharacter = await heroService.updateHero(character.id, updates);
      setCharacter(updatedCharacter);
    } catch (err: any) {
      setError(err.message || 'Erro ao atualizar herói');
    }
  };

  const updateCharacterGold = (newGold: number) => {
    if (!character) return;
    setCharacter(prev => prev ? { ...prev, gold: newGold } : null);
  };

  const updateCharacterExperience = (newExperience: number) => {
    if (!character) return;
    setCharacter(prev => prev ? { ...prev, experience: newExperience } : null);
  };

  const updateCharacterLevel = (newLevel: number) => {
    if (!character) return;
    setCharacter(prev => prev ? { ...prev, level: newLevel } : null);
  };

  const updateCharacterHealth = (newHealth: number) => {
    if (!character) return;
    setCharacter(prev => prev ? { ...prev, currentHealth: newHealth } : null);
  };

  useEffect(() => {
    // Carregar o herói do usuário autenticado
    const loadCharacter = async () => {
      console.log('[CharacterContext] Loading hero, user:', user);
      
      if (!user) {
        console.log('[CharacterContext] No user, skipping hero load');
        setIsLoading(false);
        setCharacter(null);
        return;
      }
      
      console.log('[CharacterContext] User ID:', user.id);
      
      try {
        setIsLoading(true);
        setError(null);
        
        // Limpar cache antigo
        cacheService.clear();
        
        // Buscar heróis do usuário
        console.log('[CharacterContext] Fetching heroes for user.id:', user.id);
        await refreshCharacter();
        
      } catch (err: any) {
        console.error('[CharacterContext] Error loading hero:', err);
        setError(err.message || 'Erro ao carregar herói');
      } finally {
        setIsLoading(false);
      }
    };

    loadCharacter();
  }, [user, refreshCharacter]);

  const value: CharacterContextType = {
    character,
    isLoading,
    error,
    refreshCharacter,
    updateCharacter,
    updateCharacterGold,
    updateCharacterExperience,
    updateCharacterLevel,
    updateCharacterHealth
  };

  return (
    <CharacterContext.Provider value={value}>
      {children}
    </CharacterContext.Provider>
  );
}

export function useCharacter() {
  const context = useContext(CharacterContext);
  if (context === undefined) {
    throw new Error('useCharacter must be used within a CharacterProvider');
  }
  return context;
}
