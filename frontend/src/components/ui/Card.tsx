import React from 'react';

interface CardProps {
  children: React.ReactNode;
  className?: string;
  title?: string;
  subtitle?: string;
  variant?: 'default' | 'epic' | 'legendary';
}

export function Card({ children, className = '', title, subtitle, variant = 'default' }: CardProps) {
  const getVariantClasses = () => {
    switch (variant) {
      case 'epic':
        return 'rpg-card border-purple-500 shadow-purple-500/20';
      case 'legendary':
        return 'rpg-card border-yellow-500 shadow-yellow-500/30 rpg-glow';
      default:
        return 'rpg-card';
    }
  };

  return (
    <div className={`${getVariantClasses()} ${className}`}>
      {(title || subtitle) && (
        <div className="px-6 py-4 border-b border-gold">
          {title && <h3 className="text-lg font-bold rpg-title">{title}</h3>}
          {subtitle && <p className="text-sm rpg-subtitle mt-1">{subtitle}</p>}
        </div>
      )}
      <div className="p-6">
        {children}
      </div>
    </div>
  );
}
