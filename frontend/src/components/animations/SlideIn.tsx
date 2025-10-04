import { useEffect, useState } from 'react';

interface SlideInProps {
  children: React.ReactNode;
  direction?: 'left' | 'right' | 'up' | 'down';
  delay?: number;
  duration?: number;
  className?: string;
}

export function SlideIn({ 
  children, 
  direction = 'left', 
  delay = 0, 
  duration = 300, 
  className = '' 
}: SlideInProps) {
  const [isVisible, setIsVisible] = useState(false);

  useEffect(() => {
    const timer = setTimeout(() => {
      setIsVisible(true);
    }, delay);

    return () => clearTimeout(timer);
  }, [delay]);

  const getTransform = () => {
    if (!isVisible) {
      switch (direction) {
        case 'left':
          return 'translateX(-100%)';
        case 'right':
          return 'translateX(100%)';
        case 'up':
          return 'translateY(-100%)';
        case 'down':
          return 'translateY(100%)';
        default:
          return 'translateX(-100%)';
      }
    }
    return 'translateX(0) translateY(0)';
  };

  return (
    <div
      className={`transition-transform duration-${duration} ${className}`}
      style={{
        transform: getTransform(),
        opacity: isVisible ? 1 : 0
      }}
    >
      {children}
    </div>
  );
}
