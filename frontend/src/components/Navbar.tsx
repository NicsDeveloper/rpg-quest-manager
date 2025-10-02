import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { useAuth } from '../contexts/AuthContext';
import { Button } from './Button';

export const Navbar: React.FC = () => {
  const { t } = useTranslation();
  const { user, isAdmin, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <nav className="bg-gray-800 border-b border-gray-700 px-6 py-4">
      <div className="container mx-auto flex justify-between items-center">
        <Link to="/" className="text-2xl font-bold text-amber-500 hover:text-amber-400 transition-colors">
          {t('app_title')}
        </Link>

        <div className="flex items-center gap-6">
          <Link to="/" className="text-gray-300 hover:text-amber-500 transition-colors">
            {t('nav.dashboard')}
          </Link>
          <Link to="/heroes" className="text-gray-300 hover:text-amber-500 transition-colors">
            {t('nav.heroes')}
          </Link>
          <Link to="/quests" className="text-gray-300 hover:text-amber-500 transition-colors">
            {t('nav.quests')}
          </Link>
          <Link to="/items" className="text-gray-300 hover:text-amber-500 transition-colors">
            {t('nav.items')}
          </Link>
          {isAdmin && (
            <Link to="/enemies" className="text-gray-300 hover:text-amber-500 transition-colors">
              {t('nav.enemies')}
            </Link>
          )}

          <div className="flex items-center gap-3 ml-6 pl-6 border-l border-gray-700">
            <span className="text-sm text-gray-400">
              {user?.username} {isAdmin && <span className="text-amber-500">({t('common.admin_only')})</span>}
            </span>
            <Button variant="secondary" onClick={handleLogout}>
              {t('auth.logout')}
            </Button>
          </div>
        </div>
      </div>
    </nav>
  );
};

