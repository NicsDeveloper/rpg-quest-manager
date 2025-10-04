import React, { createContext, useContext, useEffect, useState, useCallback, useRef } from 'react';
import { characterService, type Character, type CharacterStats } from '../services/characters';
import { useAuth } from './AuthContext';
import { cacheService } from '../services/cache';

interface CharacterContextType {
  character: Character | null;
  stats: CharacterStats | null;
  isLoading: boolean;
  error: string | null;
  refreshCharacter: () => Promise<void>;
  updateCharacter: (updates: Partial<Character>) => Promise<void>;
  updateCharacterGold: (newGold: number) => void;
  updateCharacterExperience: (newExperience: number) => void;
  updateCharacterLevel: (newLevel: number) => void;
  updateCharacterHealth: (newHealth: number) => void;
}

const CharacterContext = createContext<CharacterContextType | undefined>(undefined);

export function CharacterProvider({ children }: { children: React.ReactNode }) {
  const { user } = useAuth();
  const [character, setCharacter] = useState<Character | null>(null);
  const [stats, setStats] = useState<CharacterStats | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const loadingRef = useRef(false);

  const refreshCharacter = useCallback(async () => {
    if (!user || loadingRef.current) return;
    
    try {
      loadingRef.current = true;
      setIsLoading(true);
      setError(null);
      
      const [characterData, statsData] = await Promise.all([
        characterService.getCharacter(user.id),
        characterService.getCharacterStats(user.id)
      ]);
      
      setCharacter(characterData);
      setStats(statsData);
    } catch (err: any) {
      setError(err.message || 'Erro ao carregar personagem');
    } finally {
      setIsLoading(false);
      loadingRef.current = false;
    }
  }, [user]);

  const updateCharacter = async (updates: Partial<Character>) => {
    if (!character) return;
    
    try {
      const updatedCharacter = await characterService.updateCharacter(character.id, updates);
      setCharacter(updatedCharacter);
    } catch (err: any) {
      setError(err.message || 'Erro ao atualizar personagem');
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
    setCharacter(prev => prev ? { ...prev, health: newHealth } : null);
  };

  useEffect(() => {
    // Carregar o personagem do usuário autenticado
    const loadCharacter = async () => {
      console.log('[CharacterContext] Loading character, user:', user);
      
      if (!user) {
        console.log('[CharacterContext] No user, skipping character load');
        setIsLoading(false);
        setCharacter(null);
        setStats(null);
        return;
      }
      
      console.log('[CharacterContext] User ID:', user.id);
      
      try {
        setIsLoading(true);
        setError(null);
        
        // Limpar cache de personagens antigos
        cacheService.clear();
        
        // Usar o ID do usuário autenticado como characterId
        console.log('[CharacterContext] Fetching character with user.id:', user.id);
        const characterData = await characterService.getCharacter(user.id);
        const statsData = await characterService.getCharacterStats(user.id);
        
        setCharacter(characterData);
        setStats(statsData);
      } catch (err: any) {
        console.error('[CharacterContext] Error loading character:', err);
        setError(err.message || 'Erro ao carregar personagem');
      } finally {
        setIsLoading(false);
      }
    };

    loadCharacter();
  }, [user]);

  const value: CharacterContextType = {
    character,
    stats,
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
