import React, { createContext, useContext, useEffect, useState } from 'react';
import { characterService, type Character, type CharacterStats } from '../services/characters';

interface CharacterContextType {
  character: Character | null;
  stats: CharacterStats | null;
  isLoading: boolean;
  error: string | null;
  refreshCharacter: () => Promise<void>;
  updateCharacter: (updates: Partial<Character>) => Promise<void>;
}

const CharacterContext = createContext<CharacterContextType | undefined>(undefined);

export function CharacterProvider({ children }: { children: React.ReactNode }) {
  const [character, setCharacter] = useState<Character | null>(null);
  const [stats, setStats] = useState<CharacterStats | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const refreshCharacter = async () => {
    if (!character) return;
    
    try {
      setIsLoading(true);
      setError(null);
      
      const [characterData, statsData] = await Promise.all([
        characterService.getCharacter(character.id),
        characterService.getCharacterStats(character.id)
      ]);
      
      setCharacter(characterData);
      setStats(statsData);
    } catch (err: any) {
      setError(err.message || 'Erro ao carregar personagem');
    } finally {
      setIsLoading(false);
    }
  };

  const updateCharacter = async (updates: Partial<Character>) => {
    if (!character) return;
    
    try {
      const updatedCharacter = await characterService.updateCharacter(character.id, updates);
      setCharacter(updatedCharacter);
    } catch (err: any) {
      setError(err.message || 'Erro ao atualizar personagem');
    }
  };

  useEffect(() => {
    // Carregar o primeiro personagem disponível (assumindo que sempre existe um)
    const loadCharacter = async () => {
      try {
        setIsLoading(true);
        setError(null);
        
        // Por enquanto, vamos assumir que sempre existe um personagem com ID 1
        // Em uma implementação real, você listaria os personagens do usuário
        const characterData = await characterService.getCharacter(1);
        const statsData = await characterService.getCharacterStats(1);
        
        setCharacter(characterData);
        setStats(statsData);
      } catch (err: any) {
        setError(err.message || 'Erro ao carregar personagem');
      } finally {
        setIsLoading(false);
      }
    };

    loadCharacter();
  }, []);

  const value: CharacterContextType = {
    character,
    stats,
    isLoading,
    error,
    refreshCharacter,
    updateCharacter
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
