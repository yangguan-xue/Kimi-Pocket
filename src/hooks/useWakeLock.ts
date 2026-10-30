import { useState, useEffect, useCallback } from 'react';

/**
 * Screen Wake Lock API: 横屏桌面 HUD 模式下保持屏幕常亮
 */
export function useWakeLock() {
  const [isLocked, setIsLocked] = useState(false);
  const [isSupported, setIsSupported] = useState(false);

  useEffect(() => {
    setIsSupported('wakeLock' in navigator);
  }, []);

  const requestWakeLock = useCallback(async () => {
    if (!('wakeLock' in navigator)) return;
    try {
      const sentinel = await navigator.wakeLock.request('screen');
      setIsLocked(true);
      sentinel.addEventListener('release', () => {
        setIsLocked(false);
      });
    } catch {
      setIsLocked(false);
    }
  }, []);

  return { isSupported, isLocked, requestWakeLock };
}
