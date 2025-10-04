import { useEffect, useState } from 'react';

interface BounceInProps {
  children: React.ReactNode;
  delay?: number;
  duration?: number;
  className?: string;
}

export function BounceIn({ children, delay = 0, duration = 500, className = '' }: BounceInProps) {
  const [isVisible, setIsVisible] = useState(false);

  useEffect(() => {
    const timer = setTimeout(() => {
      setIsVisible(true);
    }, delay);

    return () => clearTimeout(timer);
  }, [delay]);

  return (
    <div
      className={`transition-all duration-${duration} ${className}`}
      style={{
        transform: isVisible ? 'scale(1)' : 'scale(0)',
        opacity: isVisible ? 1 : 0
      }}
    >
      {children}
    </div>
  );
}
