import React from 'react';

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'secondary' | 'danger' | 'success' | 'epic' | 'legendary';
  size?: 'sm' | 'md' | 'lg';
  loading?: boolean;
  children: React.ReactNode;
}

export function Button({ 
  variant = 'primary', 
  size = 'md', 
  loading = false, 
  children, 
  className = '', 
  disabled,
  ...props 
}: ButtonProps) {
  const baseClasses = 'inline-flex items-center justify-center font-medium transition-all duration-300 focus:outline-none focus:ring-2 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed';
  
  const variantClasses = {
    primary: 'rpg-button',
    secondary: 'bg-gray-600 text-white hover:bg-gray-700 focus:ring-gray-500 border-2 border-gray-700 rounded-lg',
    danger: 'bg-red-600 text-white hover:bg-red-700 focus:ring-red-500 border-2 border-red-700 rounded-lg',
    success: 'bg-green-600 text-white hover:bg-green-700 focus:ring-green-500 border-2 border-green-700 rounded-lg',
    epic: 'bg-purple-600 text-white hover:bg-purple-700 focus:ring-purple-500 border-2 border-purple-700 rounded-lg font-bold shadow-lg shadow-purple-500/25',
    legendary: 'bg-gradient-to-r from-yellow-400 via-yellow-500 to-yellow-600 text-yellow-900 hover:from-yellow-300 hover:via-yellow-400 hover:to-yellow-500 focus:ring-yellow-500 border-2 border-yellow-700 rounded-lg font-bold shadow-lg shadow-yellow-500/30 animate-pulse'
  };
  
  const sizeClasses = {
    sm: 'px-3 py-1.5 text-sm',
    md: 'px-4 py-2 text-base',
    lg: 'px-6 py-3 text-lg'
  };
  
  const classes = `${baseClasses} ${variantClasses[variant]} ${sizeClasses[size]} ${className}`;
  
  return (
    <button 
      className={classes}
      disabled={disabled || loading}
      {...props}
    >
      {loading && (
        <svg className="animate-spin -ml-1 mr-2 h-4 w-4" fill="none" viewBox="0 0 24 24">
          <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
          <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
        </svg>
      )}
      {children}
    </button>
  );
}
