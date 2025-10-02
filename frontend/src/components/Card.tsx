import React from 'react';

interface CardProps {
  children: React.ReactNode;
  className?: string;
  variant?: 'default' | 'hero' | 'quest' | 'item';
}

export const Card: React.FC<CardProps> = ({ children, className = '', variant = 'default' }) => {
  const variantClasses = {
    default: 'card',
    hero: 'card-hero',
    quest: 'card-quest',
    item: 'card-item',
  };

  return <div className={`${variantClasses[variant]} ${className}`}>{children}</div>;
};

