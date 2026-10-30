import { useState, useEffect, useCallback, useRef } from 'react';
import type { KimiUsageState } from '../types/kimi';
import { fetchKimiUsageData, KimiApiError } from '../api/client';

const STORAGE_KEY = 'kimi_pocket_token';

// 初始默认展示数据 (未填 Token 时的演示/就绪态)
const DEMO_STATE: KimiUsageState = {
  plan: {
    title: 'Kimi Pro (示例)',
    endTime: '2026-11-28T23:59:59Z',
    daysRemaining: 29,
    isActive: true,
  },
  totalQuota: {
    limit: 100,
    remaining: 35,
    used: 65,
    usageRatio: 0.65,
  },
  weeklyQuota: {
    limit: 100,
    remaining: 58,
    used: 42,
    usageRatio: 0.42,
    resetTime: '2026-11-03T00:00:00Z',
    resetCountdown: '3天后',
  },
  rollingQuota: {
    limit: 60,
    remaining: 42,
    used: 18,
    usageRatio: 0.3,
    durationMinutes: 300,
    resetTime: '2026-10-30T14:30:00Z',
    resetCountdown: '1小时42分后',
  },
  meta: {
    lastUpdated: new Date(),
    loading: false,
    error: null,
    isTokenExpiring: false,
  },
};

export function useKimiUsage() {
  const [token, setToken] = useState<string>(() => {
    return localStorage.getItem(STORAGE_KEY) || '';
  });
  const [state, setState] = useState<KimiUsageState>(DEMO_STATE);
  const [isDemo, setIsDemo] = useState<boolean>(() => !localStorage.getItem(STORAGE_KEY));
  const pollTimerRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  const refreshData = useCallback(async (activeToken: string) => {
    if (!activeToken) {
      setIsDemo(true);
      setState((prev) => ({
        ...prev,
        meta: { ...prev.meta, loading: false },
      }));
      return;
    }

    setState((prev) => ({
      ...prev,
      meta: { ...prev.meta, loading: true, error: null },
    }));

    try {
      const data = await fetchKimiUsageData(activeToken);
      setState(data);
      setIsDemo(false);
    } catch (err) {
      const message = err instanceof KimiApiError ? err.message : '获取数据异常';
      setState((prev) => ({
        ...prev,
        meta: {
          ...prev.meta,
          loading: false,
          error: message,
        },
      }));
    }
  }, []);

  const saveToken = useCallback((newToken: string) => {
    const clean = newToken.trim();
    if (clean) {
      localStorage.setItem(STORAGE_KEY, clean);
      setToken(clean);
      refreshData(clean);
    } else {
      localStorage.removeItem(STORAGE_KEY);
      setToken('');
      setIsDemo(true);
      setState(DEMO_STATE);
    }
  }, [refreshData]);

  // 智能自适应轮询
  useEffect(() => {
    if (!token || isDemo) return;

    const schedulePoll = () => {
      // 额度紧张时加快轮询频率
      const lowestRemainingRatio = Math.min(
        1 - state.totalQuota.usageRatio,
        1 - state.weeklyQuota.usageRatio,
        1 - state.rollingQuota.usageRatio
      );

      let intervalMs = 60000; // 正常 60s
      if (lowestRemainingRatio <= 0.1) intervalMs = 15000; // <10% 告急: 15s
      else if (lowestRemainingRatio <= 0.3) intervalMs = 30000; // <30% 紧张: 30s

      pollTimerRef.current = setTimeout(() => {
        if (document.visibilityState === 'visible') {
          refreshData(token);
        }
        schedulePoll();
      }, intervalMs);
    };

    schedulePoll();

    const handleVisibilityChange = () => {
      if (document.visibilityState === 'visible' && token) {
        refreshData(token);
      }
    };
    document.addEventListener('visibilitychange', handleVisibilityChange);

    return () => {
      if (pollTimerRef.current) clearTimeout(pollTimerRef.current);
      document.removeEventListener('visibilitychange', handleVisibilityChange);
    };
  }, [token, isDemo, state.totalQuota.usageRatio, state.weeklyQuota.usageRatio, state.rollingQuota.usageRatio, refreshData]);

  return {
    token,
    state,
    isDemo,
    saveToken,
    refresh: () => refreshData(token),
  };
}
