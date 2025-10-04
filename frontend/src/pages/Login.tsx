import React, { useState } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { Sword, Shield, Crown, Scroll } from 'lucide-react';

export default function Login() {
  const [isLogin, setIsLogin] = useState(true);
  const [formData, setFormData] = useState({
    username: '',
    email: '',
    password: ''
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const { login, register } = useAuth();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      if (isLogin) {
        await login(formData.username, formData.password);
      } else {
        await register(formData.username, formData.email, formData.password);
      }
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData(prev => ({
      ...prev,
      [e.target.name]: e.target.value
    }));
  };

  return (
    <div className="min-h-screen relative overflow-hidden">
      <div className="absolute inset-0 bg-gradient-to-br from-slate-900 via-purple-900 to-slate-900"></div>
      
      <div className="absolute inset-0 overflow-hidden">
        {[...Array(20)].map((_, i) => (
          <div
            key={i}
            className="absolute w-2 h-2 bg-amber-400 rounded-full opacity-30 animate-pulse"
            style={{
              left: `${Math.random() * 100}%`,
              top: `${Math.random() * 100}%`,
              animationDelay: `${Math.random() * 3}s`,
              animationDuration: `${2 + Math.random() * 3}s`
            }}
          />
        ))}
      </div>

      <div className="relative z-10 flex items-center justify-center min-h-screen p-4">
        <div className="w-full max-w-md">
          <div className="text-center mb-8">
            <div className="flex justify-center items-center space-x-3 mb-6">
              <Crown className="h-12 w-12 text-amber-400 animate-pulse" />
              <Sword className="h-10 w-10 text-blue-400" />
              <Shield className="h-10 w-10 text-red-400" />
              <Scroll className="h-10 w-10 text-green-400" />
            </div>
            
            <h1 className="text-5xl font-black mb-3 hero-title animate-float">
              RPG Quest Manager
            </h1>
            
            <p className="text-xl text-gray-400 font-medium">
              Entre na sua aventura épica
            </p>
            
            <div className="mt-4 flex justify-center space-x-2">
              <div className="w-16 h-1 bg-gradient-to-r from-amber-400 to-orange-600 rounded"></div>
              <div className="w-8 h-1 bg-gradient-to-r from-blue-400 to-blue-600 rounded"></div>
              <div className="w-16 h-1 bg-gradient-to-r from-purple-400 to-purple-600 rounded"></div>
            </div>
          </div>

          <div className="card backdrop-blur-sm bg-black/20">
            <form onSubmit={handleSubmit} className="space-y-6">
              {error && (
                <div className="card bg-gradient-to-br from-red-900/20 to-red-800/20 border-red-700/30">
                  <div className="text-center">
                    <div className="inline-block p-4 bg-red-600/20 rounded-full mb-4">
                      <svg className="w-12 h-12 text-red-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.732 16.5c-.77.833.192 2.5 1.732 2.5z" />
                      </svg>
                    </div>
                    <p className="text-red-400 text-lg">{error}</p>
                  </div>
                </div>
              )}

              <div>
                <label htmlFor="username" className="label">
                  Nome do Aventureiro
                </label>
                <input
                  type="text"
                  id="username"
                  name="username"
                  value={formData.username}
                  onChange={handleChange}
                  required
                  className="input text-lg"
                  placeholder="Digite seu nome de herói"
                />
              </div>

              {!isLogin && (
                <div>
                  <label htmlFor="email" className="label">
                    Pergaminho de Contato
                  </label>
                  <input
                    type="email"
                    id="email"
                    name="email"
                    value={formData.email}
                    onChange={handleChange}
                    required
                    className="input text-lg"
                    placeholder="Seu pergaminho mágico"
                  />
                </div>
              )}

              <div>
                <label htmlFor="password" className="label">
                  Senha Secreta
                </label>
                <input
                  type="password"
                  id="password"
                  name="password"
                  value={formData.password}
                  onChange={handleChange}
                  required
                  className="input text-lg"
                  placeholder="Sua palavra-chave mágica"
                />
              </div>

              <button
                type="submit"
                disabled={loading}
                className="btn btn-primary w-full text-lg py-4"
              >
                {loading ? (
                  <div className="flex items-center justify-center">
                    <div className="animate-spin rounded-full h-6 w-6 border-b-2 border-white mr-2"></div>
                    Carregando...
                  </div>
                ) : (
                  isLogin ? '🚀 Iniciar Aventura' : '✨ Criar Herói'
                )}
              </button>
            </form>

            <div className="mt-8 text-center">
              <button
                type="button"
                onClick={() => setIsLogin(!isLogin)}
                className="text-gray-400 hover:text-amber-400 transition-colors duration-300 font-medium text-lg"
              >
                {isLogin 
                  ? '🎭 Novo no reino? Crie sua conta' 
                  : '⚔️ Já é um herói? Entre aqui'
                }
              </button>
            </div>
          </div>

          <div className="mt-8 text-center">
            <div className="card bg-black/30 backdrop-blur-sm">
              <div className="text-center">
                <h3 className="text-gradient text-lg mb-2">🏰 Credenciais de Demonstração</h3>
                <p className="text-gray-100 text-lg font-bold">
                  <span className="text-amber-400">Admin:</span> admin
                </p>
                <p className="text-gray-100 text-lg font-bold">
                  <span className="text-amber-400">Senha:</span> admin123
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
