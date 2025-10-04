import { api } from './api';

export interface Hero {
  id: number;
  name: string;
  class: string;
  level: number;
  experience: number;
  strength: number;
  intelligence: number;
  dexterity: number;
  maxHealth: number;
  currentHealth: number;
  gold: number;
  createdAt: string;
  isInActiveParty: boolean;
  partySlot?: number;
}

export interface CreateHeroRequest {
  name: string;
  class: string;
}

export interface UserProfile {
  id: number;
  username: string;
  email: string;
  gold: number;
  totalHeroes: number;
  activePartyCount: number;
  hasSeenTutorial: boolean;
  createdAt: string;
}

class HeroService {
  async getCurrentUser(): Promise<UserProfile> {
    const { data } = await api.get('/profile/me');
    return data;
  }

  async getUserProfile(): Promise<UserProfile> {
    const { data } = await api.get('/profile/me');
    return data;
  }

  async getAllHeroes(): Promise<Hero[]> {
    const { data } = await api.get('/profile/my-heroes');
    return data;
  }

  async getMyHeroes(): Promise<Hero[]> {
    const { data } = await api.get('/profile/my-heroes');
    return data;
  }

  async getActiveParty(): Promise<Hero[]> {
    const { data } = await api.get('/profile/active-party');
    return data;
  }

  async getHero(id: number): Promise<Hero> {
    const { data } = await api.get(`/profile/hero/${id}`);
    return data;
  }

  async createHero(heroData: CreateHeroRequest): Promise<Hero> {
    const { data } = await api.post('/profile/create-hero', heroData);
    return data;
  }

  async addToParty(heroId: number): Promise<void> {
    await api.post(`/profile/add-to-party/${heroId}`);
  }

  async removeFromParty(heroId: number): Promise<void> {
    await api.post(`/profile/remove-from-party/${heroId}`);
  }

  async updateHero(heroId: number, updates: Partial<Hero>): Promise<Hero> {
    const { data } = await api.put(`/profile/hero/${heroId}`, updates);
    return data;
  }

  async markTutorialSeen(): Promise<void> {
    await api.post('/profile/mark-tutorial-seen');
  }

  // Métodos utilitários
  getClassAttributes(heroClass: string) {
    switch (heroClass) {
      case 'Guerreiro': return { strength: 18, intelligence: 12, dexterity: 14 };
      case 'Mago': return { strength: 10, intelligence: 22, dexterity: 16 };
      case 'Arqueiro': return { strength: 14, intelligence: 15, dexterity: 20 };
      case 'Paladino': return { strength: 16, intelligence: 18, dexterity: 14 };
      case 'Ladino': return { strength: 12, intelligence: 14, dexterity: 18 };
      case 'Clérigo': return { strength: 12, intelligence: 20, dexterity: 12 };
      case 'Bárbaro': return { strength: 20, intelligence: 11, dexterity: 13 };
      case 'Bruxo': return { strength: 11, intelligence: 23, dexterity: 14 };
      case 'Druida': return { strength: 13, intelligence: 19, dexterity: 16 };
      case 'Monge': return { strength: 14, intelligence: 16, dexterity: 18 };
      default: return { strength: 13, intelligence: 13, dexterity: 13 };
    }
  }

  getAvailableClasses(): string[] {
    return [
      'Guerreiro',
      'Mago',
      'Arqueiro',
      'Paladino',
      'Ladino',
      'Clérigo',
      'Bárbaro',
      'Bruxo',
      'Druida',
      'Monge'
    ];
  }

  getClassDescription(heroClass: string): string {
    const descriptions: Record<string, string> = {
      'Guerreiro': 'Especialista em combate corpo a corpo com alta resistência e força.',
      'Mago': 'Mestre das artes arcanas com poderosos feitiços e magias.',
      'Arqueiro': 'Perito em combate à distância com precisão e agilidade.',
      'Paladino': 'Cavaleiro sagrado que combina força física e poderes divinos.',
      'Ladino': 'Especialista em furtividade, roubo e ataques precisos.',
      'Clérigo': 'Servo divino com poderes de cura e proteção.',
      'Bárbaro': 'Guerreiro selvagem com força bruta e resistência extrema.',
      'Bruxo': 'Invocador de poderes sombrios através de pactos místicos.',
      'Druida': 'Guardião da natureza com poderes de transformação e cura.',
      'Monge': 'Mestre das artes marciais com disciplina e autocontrole.'
    };
    
    return descriptions[heroClass] || 'Uma classe única com habilidades especiais.';
  }

  getClassIcon(heroClass: string): string {
    const icons: Record<string, string> = {
      'Guerreiro': '⚔️',
      'Mago': '🔮',
      'Arqueiro': '🏹',
      'Paladino': '🛡️',
      'Ladino': '🗡️',
      'Clérigo': '⛪',
      'Bárbaro': '🪓',
      'Bruxo': '👹',
      'Druida': '🌿',
      'Monge': '🥋'
    };
    
    return icons[heroClass] || '⚔️';
  }

  getClassColor(heroClass: string): string {
    const colors: Record<string, string> = {
      'Guerreiro': 'from-red-500 to-red-600',
      'Mago': 'from-purple-500 to-purple-600',
      'Arqueiro': 'from-green-500 to-green-600',
      'Paladino': 'from-blue-500 to-blue-600',
      'Ladino': 'from-gray-500 to-gray-600',
      'Clérigo': 'from-yellow-500 to-yellow-600',
      'Bárbaro': 'from-orange-500 to-orange-600',
      'Bruxo': 'from-indigo-500 to-indigo-600',
      'Druida': 'from-emerald-500 to-emerald-600',
      'Monge': 'from-amber-500 to-amber-600'
    };
    
    return colors[heroClass] || 'from-gray-500 to-gray-600';
  }
}

export const heroService = new HeroService();
