import React, { InputHTMLAttributes } from 'react';

interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  error?: string;
}

export const Input: React.FC<InputProps> = ({ label, error, className = '', ...props }) => {
  return (
    <div className="mb-4">
      {label && <label className="label">{label}</label>}
      <input className={`input ${error ? 'border-red-500' : ''} ${className}`} {...props} />
      {error && <p className="mt-1 text-sm text-red-500">{error}</p>}
    </div>
  );
};

