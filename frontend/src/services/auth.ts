import axios from 'axios';
import { api } from './api';

const API_BASE_URL = 'http://localhost:5000/api';

export interface User {
  id: number;
  username: string;
  email: string;
  createdAt?: string;
  lastLoginAt?: string;
}

export interface AuthResponse {
  message: string;
  user: User;
  token: string;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
}

class AuthService {
  private token: string | null = null;

  constructor() {
    this.token = localStorage.getItem('token');
    if (this.token) {
      this.setAuthHeader(this.token);
    }
  }

  private setAuthHeader(token: string) {
    axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
    // Também atualizar o interceptor da instância api
    api.defaults.headers.common['Authorization'] = `Bearer ${token}`;
  }

  async login(credentials: LoginRequest): Promise<AuthResponse> {
    try {
      const response = await axios.post(`${API_BASE_URL}/auth/login`, credentials);
      const { token } = response.data;
      
      this.token = token;
      localStorage.setItem('token', token);
      this.setAuthHeader(token);
      
      return response.data;
    } catch (error: any) {
      console.error('Login error:', error);
      throw new Error(error.response?.data?.message || 'Erro ao fazer login');
    }
  }

  async register(userData: RegisterRequest): Promise<AuthResponse> {
    try {
      const response = await axios.post(`${API_BASE_URL}/auth/register`, userData);
      const { token } = response.data;
      
      this.token = token;
      localStorage.setItem('token', token);
      this.setAuthHeader(token);
      
      return response.data;
    } catch (error: any) {
      console.error('Register error:', error);
      throw new Error(error.response?.data?.message || 'Erro ao registrar usuário');
    }
  }

  async validateToken(): Promise<User | null> {
    if (!this.token) return null;
    
    try {
      const response = await axios.post(`${API_BASE_URL}/auth/validate`, {
        token: this.token
      }, {
        headers: {
          'Authorization': `Bearer ${this.token}`
        }
      });
      return response.data.user;
    } catch (error) {
      console.error('Token validation error:', error);
      this.logout();
      return null;
    }
  }

  logout() {
    this.token = null;
    localStorage.removeItem('token');
    delete axios.defaults.headers.common['Authorization'];
    delete api.defaults.headers.common['Authorization'];
  }

  isAuthenticated(): boolean {
    return !!this.token;
  }

  getToken(): string | null {
    return this.token;
  }
}

export const authService = new AuthService();
