// Sistema de cache para otimizar performance
interface CacheItem<T> {
  data: T;
  timestamp: number;
  expiresAt: number;
}

class CacheService {
  private cache: Map<string, CacheItem<any>> = new Map();
  private defaultTTL: number = 5 * 60 * 1000; // 5 minutos

  set<T>(key: string, data: T, ttl?: number): void {
    const now = Date.now();
    const expiresAt = now + (ttl || this.defaultTTL);
    
    this.cache.set(key, {
      data,
      timestamp: now,
      expiresAt
    });
  }

  get<T>(key: string): T | null {
    const item = this.cache.get(key);
    
    if (!item) {
      return null;
    }

    if (Date.now() > item.expiresAt) {
      this.cache.delete(key);
      return null;
    }

    return item.data as T;
  }

  has(key: string): boolean {
    const item = this.cache.get(key);
    
    if (!item) {
      return false;
    }

    if (Date.now() > item.expiresAt) {
      this.cache.delete(key);
      return false;
    }

    return true;
  }

  delete(key: string): boolean {
    return this.cache.delete(key);
  }

  clear(): void {
    this.cache.clear();
  }

  // Limpar itens expirados
  cleanup(): void {
    const now = Date.now();
    
    for (const [key, item] of this.cache.entries()) {
      if (now > item.expiresAt) {
        this.cache.delete(key);
      }
    }
  }

  // Estatísticas do cache
  getStats() {
    const now = Date.now();
    let expired = 0;
    let active = 0;

    for (const item of this.cache.values()) {
      if (now > item.expiresAt) {
        expired++;
      } else {
        active++;
      }
    }

    return {
      total: this.cache.size,
      active,
      expired,
      hitRate: this.calculateHitRate()
    };
  }

  private hitCount = 0;
  private missCount = 0;

  private calculateHitRate(): number {
    const total = this.hitCount + this.missCount;
    return total > 0 ? (this.hitCount / total) * 100 : 0;
  }

  // Wrapper para funções assíncronas com cache
  async cached<T>(
    key: string, 
    fetcher: () => Promise<T>, 
    ttl?: number
  ): Promise<T> {
    // Verificar cache primeiro
    const cached = this.get<T>(key);
    if (cached !== null) {
      this.hitCount++;
      return cached;
    }

    // Buscar dados
    this.missCount++;
    const data = await fetcher();
    
    // Armazenar no cache
    this.set(key, data, ttl);
    
    return data;
  }

  // Cache para dados de personagem
  setCharacter(characterId: number, data: any, ttl?: number): void {
    this.set(`character:${characterId}`, data, ttl);
  }

  getCharacter(characterId: number): any | null {
    return this.get(`character:${characterId}`);
  }

  // Cache para dados de inventário
  setInventory(characterId: number, data: any, ttl?: number): void {
    this.set(`inventory:${characterId}`, data, ttl);
  }

  getInventory(characterId: number): any | null {
    return this.get(`inventory:${characterId}`);
  }

  // Cache para dados de loja
  setShop(data: any, ttl?: number): void {
    this.set('shop:items', data, ttl);
  }

  getShop(): any | null {
    return this.get('shop:items');
  }

  // Cache para conquistas
  setAchievements(userId: number, data: any, ttl?: number): void {
    this.set(`achievements:${userId}`, data, ttl);
  }

  getAchievements(userId: number): any | null {
    return this.get(`achievements:${userId}`);
  }

  // Cache para grupos
  setParties(data: any, ttl?: number): void {
    this.set('parties:public', data, ttl);
  }

  getParties(): any | null {
    return this.get('parties:public');
  }

  // Invalidar cache relacionado a um personagem
  invalidateCharacter(characterId: number): void {
    this.delete(`character:${characterId}`);
    this.delete(`inventory:${characterId}`);
    this.delete(`achievements:${characterId}`);
  }

  // Invalidar cache de loja
  invalidateShop(): void {
    this.delete('shop:items');
  }

  // Invalidar cache de grupos
  invalidateParties(): void {
    this.delete('parties:public');
  }
}

export const cacheService = new CacheService();

// Limpeza automática do cache a cada 5 minutos
setInterval(() => {
  cacheService.cleanup();
}, 5 * 60 * 1000);
