import { describe, it, expect } from 'vitest';
import { normalizeKimiData, formatTimeCountdown } from '../client';
import type { GetSubscriptionResponse, GetUsagesResponse } from '../../types/kimi';

describe('normalizeKimiData 业务数据归一化转换', () => {
  const dummyToken = 'eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJ1c2VyMTIzIiwic3NpZCI6IjEiLCJleHAiOjIwMDAwMDAwMDB9.sig';

  const mockSubResponse: GetSubscriptionResponse = {
    subscription: {
      subscriptionId: 'sub_999',
      goods: {
        id: 'g1',
        title: 'Kimi Pro',
        durationDays: 30,
        membershipLevel: 'LEVEL_PRO',
      },
      currentEndTime: '2026-11-28T23:59:59Z',
      status: 'ACTIVE',
      active: true,
    },
  };

  const mockUsageResponse: GetUsagesResponse = {
    totalQuota: {
      limit: '100',
      remaining: '35', // 已用 65, 65%
    },
    usages: [
      {
        scope: 'FEATURE_CODING',
        detail: {
          limit: '100',
          remaining: '58', // 已用 42, 42%
          resetTime: new Date(Date.now() + 3 * 24 * 3600 * 1000).toISOString(),
        },
        limits: [
          {
            window: {
              duration: 300,
              timeUnit: 'TIME_UNIT_MINUTE',
            },
            detail: {
              limit: '60',
              remaining: '42', // 已用 18, 30%
              resetTime: new Date(Date.now() + 100 * 60 * 1000).toISOString(),
            },
          },
        ],
      },
    ],
  };

  it('正确归一化总用量 (4段基准)', () => {
    const result = normalizeKimiData(mockSubResponse, mockUsageResponse, dummyToken);
    expect(result.totalQuota.limit).toBe(100);
    expect(result.totalQuota.remaining).toBe(35);
    expect(result.totalQuota.used).toBe(65);
    expect(result.totalQuota.usageRatio).toBeCloseTo(0.65, 2);
  });

  it('正确归一化周用量 (7段基准)', () => {
    const result = normalizeKimiData(mockSubResponse, mockUsageResponse, dummyToken);
    expect(result.weeklyQuota.limit).toBe(100);
    expect(result.weeklyQuota.remaining).toBe(58);
    expect(result.weeklyQuota.used).toBe(42);
    expect(result.weeklyQuota.usageRatio).toBeCloseTo(0.42, 2);
    expect(result.weeklyQuota.resetCountdown).toContain('天');
  });

  it('正确归一化5小时额度 (连续单条基准)', () => {
    const result = normalizeKimiData(mockSubResponse, mockUsageResponse, dummyToken);
    expect(result.rollingQuota.limit).toBe(60);
    expect(result.rollingQuota.remaining).toBe(42);
    expect(result.rollingQuota.used).toBe(18);
    expect(result.rollingQuota.usageRatio).toBeCloseTo(0.3, 2);
    expect(result.rollingQuota.durationMinutes).toBe(300);
    expect(result.rollingQuota.resetCountdown).toContain('小时');
  });

  it('格式化倒计时函数 formatTimeCountdown', () => {
    const future3Days = new Date(Date.now() + 3 * 24 * 3600 * 1000 + 3600000).toISOString();
    expect(formatTimeCountdown(future3Days)).toBe('3天1小时后');

    const pastTime = new Date(Date.now() - 1000).toISOString();
    expect(formatTimeCountdown(pastTime)).toBe('即将重置');
  });
});
