import api from './api';

export interface Enemy {
  id: number;
  name: string;
  description: string;
  level: number;
  health: number;
  strength: number;
  defense: number;
  goldDrop: number;
  experienceDrop: number;
  createdAt: string;
}

export interface CreateEnemyRequest {
  name: string;
  description: string;
  level: number;
  health: number;
  strength: number;
  defense: number;
  goldDrop: number;
  experienceDrop: number;
}

export interface UpdateEnemyRequest {
  name?: string;
  description?: string;
  level?: number;
  health?: number;
  strength?: number;
  defense?: number;
  goldDrop?: number;
  experienceDrop?: number;
}

export const enemyService = {
  getAll: async (): Promise<Enemy[]> => {
    const response = await api.get<Enemy[]>('/enemies');
    return response.data;
  },

  getById: async (id: number): Promise<Enemy> => {
    const response = await api.get<Enemy>(`/enemies/${id}`);
    return response.data;
  },

  create: async (data: CreateEnemyRequest): Promise<Enemy> => {
    const response = await api.post<Enemy>('/enemies', data);
    return response.data;
  },

  update: async (id: number, data: UpdateEnemyRequest): Promise<Enemy> => {
    const response = await api.put<Enemy>(`/enemies/${id}`, data);
    return response.data;
  },

  delete: async (id: number): Promise<void> => {
    await api.delete(`/enemies/${id}`);
  },
};

