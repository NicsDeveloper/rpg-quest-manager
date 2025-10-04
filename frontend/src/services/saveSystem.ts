// Sistema de save/load para o jogo
import { cacheService } from './cache';

export interface SaveData {
  id: string;
  name: string;
  timestamp: number;
  characterId: number;
  characterData: any;
  gameState: {
    currentQuest?: number;
    lastLocation?: string;
    achievements: number[];
    unlockedAbilities: number[];
    partyId?: number;
  };
  metadata: {
    version: string;
    playTime: number;
    lastSave: number;
  };
}

class SaveSystem {
  private readonly STORAGE_KEY = 'rpg_quest_manager_saves';
  private readonly MAX_SAVES = 10;
  private readonly CURRENT_VERSION = '1.0.0';

  // Salvar jogo
  async saveGame(
    characterId: number, 
    characterData: any, 
    gameState: any, 
    saveName?: string
  ): Promise<SaveData> {
    const timestamp = Date.now();
    const id = `save_${timestamp}`;
    const name = saveName || `Save ${new Date(timestamp).toLocaleString()}`;

    const saveData: SaveData = {
      id,
      name,
      timestamp,
      characterId,
      characterData,
      gameState: {
        currentQuest: gameState.currentQuest,
        lastLocation: gameState.lastLocation,
        achievements: gameState.achievements || [],
        unlockedAbilities: gameState.unlockedAbilities || [],
        partyId: gameState.partyId
      },
      metadata: {
        version: this.CURRENT_VERSION,
        playTime: gameState.playTime || 0,
        lastSave: timestamp
      }
    };

    // Obter saves existentes
    const existingSaves = this.getSaveList();
    
    // Adicionar novo save
    existingSaves.push(saveData);
    
    // Manter apenas os saves mais recentes
    if (existingSaves.length > this.MAX_SAVES) {
      existingSaves.sort((a, b) => b.timestamp - a.timestamp);
      existingSaves.splice(this.MAX_SAVES);
    }

    // Salvar no localStorage
    localStorage.setItem(this.STORAGE_KEY, JSON.stringify(existingSaves));

    // Invalidar cache para forçar recarregamento
    cacheService.invalidateCharacter(characterId);

    return saveData;
  }

  // Carregar jogo
  loadGame(saveId: string): SaveData | null {
    const saves = this.getSaveList();
    const save = saves.find(s => s.id === saveId);
    
    if (!save) {
      return null;
    }

    // Verificar compatibilidade de versão
    if (save.metadata.version !== this.CURRENT_VERSION) {
      console.warn(`Save da versão ${save.metadata.version} pode não ser compatível com ${this.CURRENT_VERSION}`);
    }

    return save;
  }

  // Listar saves
  getSaveList(): SaveData[] {
    try {
      const saved = localStorage.getItem(this.STORAGE_KEY);
      if (!saved) return [];
      
      const saves = JSON.parse(saved) as SaveData[];
      return saves.sort((a, b) => b.timestamp - a.timestamp);
    } catch (error) {
      console.error('Erro ao carregar saves:', error);
      return [];
    }
  }

  // Deletar save
  deleteSave(saveId: string): boolean {
    try {
      const saves = this.getSaveList();
      const filteredSaves = saves.filter(s => s.id !== saveId);
      
      if (filteredSaves.length === saves.length) {
        return false; // Save não encontrado
      }

      localStorage.setItem(this.STORAGE_KEY, JSON.stringify(filteredSaves));
      return true;
    } catch (error) {
      console.error('Erro ao deletar save:', error);
      return false;
    }
  }

  // Exportar save
  exportSave(saveId: string): string | null {
    const save = this.loadGame(saveId);
    if (!save) return null;

    return JSON.stringify(save, null, 2);
  }

  // Importar save
  importSave(saveData: string): SaveData | null {
    try {
      const save = JSON.parse(saveData) as SaveData;
      
      // Validar estrutura do save
      if (!this.validateSaveData(save)) {
        throw new Error('Dados de save inválidos');
      }

      // Adicionar aos saves existentes
      const existingSaves = this.getSaveList();
      existingSaves.push(save);
      
      localStorage.setItem(this.STORAGE_KEY, JSON.stringify(existingSaves));
      
      return save;
    } catch (error) {
      console.error('Erro ao importar save:', error);
      return null;
    }
  }

  // Validar dados do save
  private validateSaveData(save: any): save is SaveData {
    return (
      save &&
      typeof save.id === 'string' &&
      typeof save.name === 'string' &&
      typeof save.timestamp === 'number' &&
      typeof save.characterId === 'number' &&
      save.characterData &&
      save.gameState &&
      save.metadata
    );
  }

  // Limpar todos os saves
  clearAllSaves(): void {
    localStorage.removeItem(this.STORAGE_KEY);
  }

  // Obter estatísticas dos saves
  getSaveStats() {
    const saves = this.getSaveList();
    
    if (saves.length === 0) {
      return {
        totalSaves: 0,
        totalPlayTime: 0,
        oldestSave: null,
        newestSave: null
      };
    }

    const totalPlayTime = saves.reduce((total, save) => total + (save.metadata.playTime || 0), 0);
    const oldestSave = saves[saves.length - 1];
    const newestSave = saves[0];

    return {
      totalSaves: saves.length,
      totalPlayTime,
      oldestSave: oldestSave.timestamp,
      newestSave: newestSave.timestamp
    };
  }

  // Auto-save
  async autoSave(characterId: number, characterData: any, gameState: any): Promise<void> {
    try {
      await this.saveGame(characterId, characterData, gameState, 'Auto Save');
    } catch (error) {
      console.error('Erro no auto-save:', error);
    }
  }

  // Verificar se há saves
  hasSaves(): boolean {
    return this.getSaveList().length > 0;
  }

  // Obter último save
  getLastSave(): SaveData | null {
    const saves = this.getSaveList();
    return saves.length > 0 ? saves[0] : null;
  }
}

export const saveSystem = new SaveSystem();
