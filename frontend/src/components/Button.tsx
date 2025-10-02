import React, { ButtonHTMLAttributes } from 'react';

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'secondary' | 'danger' | 'success';
  children: React.ReactNode;
}

export const Button: React.FC<ButtonProps> = ({
  variant = 'primary',
  children,
  className = '',
  ...props
}) => {
  const variantClasses = {
    primary: 'btn-primary',
    secondary: 'btn-secondary',
    danger: 'btn-danger',
    success: 'bg-green-600 hover:bg-green-700 text-white font-bold py-2 px-4 rounded transition-colors',
  };

  return (
    <button className={`btn ${variantClasses[variant]} ${className}`} {...props}>
      {children}
    </button>
  );
};

