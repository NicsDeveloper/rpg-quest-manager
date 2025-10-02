import api from './api';

interface LoginResponse {
  token: string;
  userId: number;
  username: string;
  email: string;
  role: string;
}

interface RegisterRequest {
  username: string;
  email: string;
  password: string;
}

export const authService = {
  login: async (username: string, password: string): Promise<LoginResponse> => {
    const response = await api.post<LoginResponse>('/auth/login', { username, password });
    return response.data;
  },

  register: async (username: string, email: string, password: string): Promise<void> => {
    const data: RegisterRequest = { username, email, password };
    await api.post('/auth/register', data);
  },
};

