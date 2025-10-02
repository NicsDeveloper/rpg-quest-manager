import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { useAuth } from '../contexts/AuthContext';
import { Input } from '../components/Input';
import { Button } from '../components/Button';
import { Card } from '../components/Card';

export const Register: React.FC = () => {
  const { t } = useTranslation();
  const { register } = useAuth();
  const navigate = useNavigate();

  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    if (password !== confirmPassword) {
      setError('As senhas não coincidem');
      return;
    }

    setLoading(true);

    try {
      await register(username, email, password);
      navigate('/login');
    } catch (err: any) {
      setError(err.response?.data?.message || t('auth.register_error'));
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-gray-900 via-gray-800 to-gray-900 p-4">
      <Card className="max-w-md w-full">
        <h1 className="text-3xl font-bold text-center mb-6 text-amber-500">{t('app_title')}</h1>
        <h2 className="text-xl font-semibold text-center mb-6">{t('auth.register')}</h2>

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
            label={t('auth.email')}
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />

          <Input
            label={t('auth.password')}
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />

          <Input
            label={t('auth.confirm_password')}
            type="password"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            required
          />

          {error && <div className="mb-4 p-3 bg-red-900/30 border border-red-700 rounded text-red-400 text-sm">{error}</div>}

          <Button type="submit" disabled={loading} className="w-full">
            {loading ? t('common.loading') : t('auth.register')}
          </Button>
        </form>

        <p className="mt-6 text-center text-gray-400 text-sm">
          {t('auth.already_have_account')}{' '}
          <Link to="/login" className="text-amber-500 hover:text-amber-400 font-semibold">
            {t('auth.login')}
          </Link>
        </p>
      </Card>
    </div>
  );
};

