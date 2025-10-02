import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { useAuth } from '../contexts/AuthContext';
import { Input } from '../components/Input';
import { Button } from '../components/Button';
import { Card } from '../components/Card';

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
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-gray-900 via-gray-800 to-gray-900 p-4">
      <Card className="max-w-md w-full">
        <h1 className="text-3xl font-bold text-center mb-6 text-amber-500">{t('app_title')}</h1>
        <h2 className="text-xl font-semibold text-center mb-6">{t('auth.login')}</h2>

        <form onSubmit={handleSubmit}>
          <Input
            label={t('auth.username')}
            type="text"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
            autoFocus
          />

          <Input
            label={t('auth.password')}
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />

          {error && <div className="mb-4 p-3 bg-red-900/30 border border-red-700 rounded text-red-400 text-sm">{error}</div>}

          <Button type="submit" disabled={loading} className="w-full">
            {loading ? t('common.loading') : t('auth.login')}
          </Button>
        </form>

        <p className="mt-6 text-center text-gray-400 text-sm">
          {t('auth.dont_have_account')}{' '}
          <Link to="/register" className="text-amber-500 hover:text-amber-400 font-semibold">
            {t('auth.register')}
          </Link>
        </p>
      </Card>
    </div>
  );
};

