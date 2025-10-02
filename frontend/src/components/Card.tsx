import React from 'react';

interface CardProps {
  children: React.ReactNode;
  className?: string;
  variant?: 'default' | 'hero' | 'quest' | 'item';
  onClick?: () => void;
}

export const Card: React.FC<CardProps> = ({ children, className = '', variant = 'default', onClick }) => {
  const variantClasses = {
    default: 'card',
    hero: 'card-hero',
    quest: 'card-quest',
    item: 'card-item',
  };

  return (
    <div 
      className={`${variantClasses[variant]} ${className}`}
      onClick={onClick}
    >
      {children}
    </div>
  );
};

