import api from './api';

export interface UserInfo {
  id: number;
  username: string;
  email: string;
  role: string;
  hasSeenTutorial: boolean;
  gold: number;
  createdAt: string;
}

export const userService = {
  getCurrentUser: async (): Promise<UserInfo> => {
    const response = await api.get<UserInfo>('/user/me');
    return response.data;
  },

  completeTutorial: async (): Promise<void> => {
    await api.post('/user/complete-tutorial');
  },

  skipTutorial: async (): Promise<void> => {
    await api.post('/user/skip-tutorial');
  },
};

