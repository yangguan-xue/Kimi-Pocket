import { useState, useEffect } from 'react';

/**
 * 监听设备屏幕方向（横屏 / 竖屏），结合 matchMedia 与窗口尺寸做双重鲁棒判断
 */
export function useOrientation() {
  const [isLandscape, setIsLandscape] = useState<boolean>(() => {
    if (typeof window === 'undefined') return false;
    return window.innerWidth > window.innerHeight;
  });

  useEffect(() => {
    const checkOrientation = () => {
      const landscapeByRatio = window.innerWidth > window.innerHeight;
      const mediaQuery = window.matchMedia('(orientation: landscape)');
      setIsLandscape(landscapeByRatio || mediaQuery.matches);
    };

    window.addEventListener('resize', checkOrientation);
    window.addEventListener('orientationchange', checkOrientation);

    return () => {
      window.removeEventListener('resize', checkOrientation);
      window.removeEventListener('orientationchange', checkOrientation);
    };
  }, []);

  return { isLandscape };
}
