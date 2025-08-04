import { useState, useEffect } from 'react';

/**
 * Hook para lidar com problemas de hidratação
 * Garante que o componente só renderize no cliente
 */
export function useHydration() {
  const [isHydrated, setIsHydrated] = useState(false);

  useEffect(() => {
    setIsHydrated(true);
  }, []);

  return isHydrated;
} 