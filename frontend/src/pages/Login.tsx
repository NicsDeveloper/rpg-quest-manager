import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { useAuth } from '../contexts/AuthContext';
import { Input } from '../components/Input';
import { Button } from '../components/Button';

export const Login: React.FC = () => {
  const { t } = useTranslation();
  const { login } = useAuth();
  const navigate = useNavigate();

  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      await login(username, password);
      navigate('/');
    } catch (err: any) {
      setError(err.response?.data?.message || t('auth.login_error'));
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center p-4 relative overflow-hidden">
      <div className="absolute inset-0 overflow-hidden">
        <div className="absolute w-96 h-96 bg-amber-500/20 rounded-full blur-3xl -top-20 -left-20 animate-pulse-slow"></div>
        <div className="absolute w-96 h-96 bg-purple-500/20 rounded-full blur-3xl -bottom-20 -right-20 animate-pulse-slow" style={{ animationDelay: '1s' }}></div>
        <div className="absolute w-64 h-64 bg-orange-500/20 rounded-full blur-3xl top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 animate-pulse-slow" style={{ animationDelay: '2s' }}></div>
      </div>

      <div className="glass rounded-3xl p-8 max-w-md w-full relative z-10 shadow-2xl border-2 border-gray-700/50">
        <div className="text-center mb-8 animate-float">
          <div className="inline-block p-4 bg-gradient-to-br from-amber-500 to-orange-600 rounded-2xl shadow-lg shadow-amber-500/50 mb-4">
            <svg className="w-16 h-16 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z" />
            </svg>
          </div>
          <h1 className="text-4xl font-black mb-2 hero-title">{t('app_title')}</h1>
          <p className="text-gray-400 text-sm">Aventure-se no mundo épico de RPG</p>
        </div>

        <form onSubmit={handleSubmit} className="space-y-5">
          <Input
            label={t('auth.username')}
            type="text"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
            autoFocus
            placeholder="Digite seu usuário"
          />

          <Input
            label={t('auth.password')}
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            placeholder="Digite sua senha"
          />

          {error && (
            <div className="bg-red-900/30 border-2 border-red-500 rounded-xl p-4 text-red-400 text-sm backdrop-blur-sm animate-pulse">
              ⚠️ {error}
            </div>
          )}

          <Button type="submit" disabled={loading} className="w-full text-lg">
            {loading ? (
              <span className="flex items-center justify-center gap-2">
                <svg className="animate-spin h-5 w-5" fill="none" viewBox="0 0 24 24">
                  <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                  <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                Entrando...
              </span>
            ) : (
              <span>🚀 {t('auth.login')}</span>
            )}
          </Button>
        </form>

        <div className="mt-6 text-center">
          <p className="text-gray-400 text-sm">
            {t('auth.dont_have_account')}{' '}
            <Link to="/register" className="text-amber-400 hover:text-amber-300 font-bold hover:underline transition-colors">
              {t('auth.register')}
            </Link>
          </p>
        </div>

        <div className="mt-6 pt-6 border-t border-gray-700/50">
          <p className="text-xs text-gray-500 text-center mb-2">🎮 Teste rápido:</p>
          <div className="flex gap-2 text-xs">
            <div className="flex-1 bg-gray-800/50 rounded-lg p-2 backdrop-blur-sm">
              <p className="text-gray-400">Admin</p>
              <p className="text-amber-400 font-mono">admin / admin123</p>
            </div>
            <div className="flex-1 bg-gray-800/50 rounded-lg p-2 backdrop-blur-sm">
              <p className="text-gray-400">Player</p>
              <p className="text-blue-400 font-mono">player1 / senha123</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};
