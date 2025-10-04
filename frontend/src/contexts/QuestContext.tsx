import React, { createContext, useContext, useState, useEffect, useCallback, useRef } from 'react';
import { questsService, type Quest } from '../services/quests';
import { heroService, type Hero } from '../services/heroService';

interface QuestContextType {
  availableQuests: Quest[];
  activeQuest: Quest | null;
  completedQuests: Quest[];
  isLoading: boolean;
  error: string | null;
  refreshQuests: () => Promise<void>;
  startQuest: (questId: number, heroId: number) => Promise<void>;
  completeQuest: (questId: number) => Promise<void>;
  setActiveQuest: (quest: Quest | null) => void;
}

const QuestContext = createContext<QuestContextType | undefined>(undefined);

export function QuestProvider({ children }: { children: React.ReactNode }) {
  const [availableQuests, setAvailableQuests] = useState<Quest[]>([]);
  const [activeQuest, setActiveQuest] = useState<Quest | null>(null);
  const [completedQuests, setCompletedQuests] = useState<Quest[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [currentHero, setCurrentHero] = useState<Hero | null>(null);
  const loadingRef = useRef(false);

  // Carregar herói atual
  useEffect(() => {
    const loadCurrentHero = async () => {
      try {
        const heroes = await heroService.getActiveParty();
        if (heroes.length > 0) {
          setCurrentHero(heroes[0]);
        }
      } catch (error) {
        console.error('Erro ao carregar herói:', error);
      }
    };

    loadCurrentHero();
  }, []);

  const refreshQuests = useCallback(async () => {
    if (!currentHero || loadingRef.current) return;
    
    try {
      loadingRef.current = true;
      setIsLoading(true);
      setError(null);
      
      const [available, completed] = await Promise.all([
        questsService.getAvailableQuests(currentHero.id),
        questsService.getCompletedQuests(currentHero.id)
      ]);
      
      setAvailableQuests(available);
      setCompletedQuests(completed);
      
      // Buscar missão ativa
      const active = available.find(q => q.status === 1) || null; // status 1 = InProgress
      setActiveQuest(active);
    } catch (err: any) {
      setError(err.message || 'Erro ao carregar missões');
    } finally {
      setIsLoading(false);
      loadingRef.current = false;
    }
  }, [currentHero]);

  const startQuest = async (questId: number, heroId: number) => {
    try {
      await questsService.startQuest(questId, heroId);
      
      // Atualizar estado local
      const quest = availableQuests.find(q => q.id === questId);
      if (quest) {
        const updatedQuest = { ...quest, status: 1 }; // InProgress
        setActiveQuest(updatedQuest);
        setAvailableQuests(prev => 
          prev.map(q => q.id === questId ? updatedQuest : q)
        );
      }
    } catch (err: any) {
      setError(err.message || 'Erro ao iniciar missão');
    }
  };

  const completeQuest = async (questId: number) => {
    if (!currentHero) return;
    
    try {
      await questsService.completeQuest(questId);
      
      // Atualizar estado local
      const quest = activeQuest;
      if (quest && quest.id === questId) {
        const completedQuest = { ...quest, status: 2 }; // Completed
        setCompletedQuests(prev => [...prev, completedQuest]);
        setActiveQuest(null);
        setAvailableQuests(prev => 
          prev.map(q => q.id === questId ? completedQuest : q)
        );
      }
    } catch (err: any) {
      setError(err.message || 'Erro ao completar missão');
    }
  };

  useEffect(() => {
    if (currentHero) {
      refreshQuests();
    }
  }, [currentHero, refreshQuests]);

  const value: QuestContextType = {
    availableQuests,
    activeQuest,
    completedQuests,
    isLoading,
    error,
    refreshQuests,
    startQuest,
    completeQuest,
    setActiveQuest
  };

  return (
    <QuestContext.Provider value={value}>
      {children}
    </QuestContext.Provider>
  );
}

export function useQuests() {
  const context = useContext(QuestContext);
  if (context === undefined) {
    throw new Error('useQuests must be used within a QuestProvider');
  }
  return context;
}
