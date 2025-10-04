import { useEffect, useState } from 'react';

interface PulseProps {
  children: React.ReactNode;
  duration?: number;
  className?: string;
}

export function Pulse({ children, duration = 1000, className = '' }: PulseProps) {
  const [isPulsing, setIsPulsing] = useState(false);

  useEffect(() => {
    const interval = setInterval(() => {
      setIsPulsing(prev => !prev);
    }, duration);

    return () => clearInterval(interval);
  }, [duration]);

  return (
    <div
      className={`transition-transform duration-300 ${isPulsing ? 'scale-105' : 'scale-100'} ${className}`}
    >
      {children}
    </div>
  );
}
