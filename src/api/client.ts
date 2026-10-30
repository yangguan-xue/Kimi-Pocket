import { Capacitor } from '@capacitor/core';
import { extractJwtPayload, isTokenExpired, isTokenNearExpiry } from './jwt';
import type { GetSubscriptionResponse, GetUsagesResponse, KimiUsageState } from '../types/kimi';

export class KimiApiError extends Error {
  constructor(
    message: string,
    public readonly code: 'AUTH_EXPIRED' | 'AUTH_INVALID' | 'NETWORK_ERROR' | 'UNKNOWN',
    public readonly status?: number
  ) {
    super(message);
    this.name = 'KimiApiError';
  }
}

/**
 * 格式化未来时间点的倒计时
 */
export function formatTimeCountdown(isoString: string): string {
  if (!isoString) return '--';
  const target = new Date(isoString).getTime();
  const now = Date.now();
  const diffMs = target - now;

  if (diffMs <= 0) return '即将重置';

  const diffMinutes = Math.floor(diffMs / (1000 * 60));
  const diffHours = Math.floor(diffMinutes / 60);
  const diffDays = Math.floor(diffHours / 24);

  if (diffDays > 0) {
    const remainHours = diffHours % 24;
    return remainHours > 0 ? `${diffDays}天${remainHours}小时后` : `${diffDays}天后`;
  }
  if (diffHours > 0) {
    const remainMinutes = diffMinutes % 60;
    return `${diffHours}小时${remainMinutes}分后`;
  }
  return `${diffMinutes}分钟后`;
}

/**
 * 将原始 API 响应归一化为业务 UI 所需的数据结构
 */
export function normalizeKimiData(
  subResp: GetSubscriptionResponse,
  usageResp: GetUsagesResponse,
  token: string
): KimiUsageState {
  // 1. 套餐信息
  const sub = subResp.subscription;
  const currentEndTime = sub?.currentEndTime || '';
  let daysRemaining = 0;
  if (currentEndTime) {
    const diffMs = new Date(currentEndTime).getTime() - Date.now();
    daysRemaining = Math.max(0, Math.ceil(diffMs / (1000 * 60 * 60 * 24)));
  }

  const plan = {
    title: sub?.goods?.title || 'Kimi Pro',
    endTime: currentEndTime,
    daysRemaining,
    isActive: sub?.active ?? true,
  };

  // 2. 总用量 (Total Quota - 对应 4 段)
  // 优先取 totalQuota，其次取 balances
  let totalLimit = 100;
  let totalRemaining = 100;
  if (usageResp.totalQuota) {
    totalLimit = Number.parseInt(usageResp.totalQuota.limit, 10) || 100;
    totalRemaining = Number.parseInt(usageResp.totalQuota.remaining, 10) || 0;
  }
  const totalUsed = Math.max(0, totalLimit - totalRemaining);
  const totalUsageRatio = totalLimit > 0 ? totalUsed / totalLimit : 0;

  const totalQuota = {
    limit: totalLimit,
    remaining: totalRemaining,
    used: totalUsed,
    usageRatio: Math.min(1, Math.max(0, totalUsageRatio)),
  };

  // 3. 周用量 (Weekly Quota - 对应 7 段)
  // 取 FEATURE_CODING 的主 detail
  const codingUsage = usageResp.usages?.find((u) => u.scope === 'FEATURE_CODING');
  let weeklyLimit = 100;
  let weeklyRemaining = 100;
  let weeklyResetTime = '';

  if (codingUsage?.detail) {
    weeklyLimit = Number.parseInt(codingUsage.detail.limit, 10) || 100;
    weeklyRemaining = Number.parseInt(codingUsage.detail.remaining, 10) || 0;
    weeklyResetTime = codingUsage.detail.resetTime || '';
  }
  const weeklyUsed = Math.max(0, weeklyLimit - weeklyRemaining);
  const weeklyUsageRatio = weeklyLimit > 0 ? weeklyUsed / weeklyLimit : 0;

  const weeklyQuota = {
    limit: weeklyLimit,
    remaining: weeklyRemaining,
    used: weeklyUsed,
    usageRatio: Math.min(1, Math.max(0, weeklyUsageRatio)),
    resetTime: weeklyResetTime,
    resetCountdown: formatTimeCountdown(weeklyResetTime),
  };

  // 4. 5小时额度 (5-Hour Rolling - 连续单条，不分段)
  // 查找 duration=300 分钟 (5小时) 或 duration=5 小时的 limit 项
  const fiveHourLimitItem = codingUsage?.limits?.find(
    (l) =>
      (l.window.timeUnit === 'TIME_UNIT_MINUTE' && l.window.duration === 300) ||
      (l.window.timeUnit === 'TIME_UNIT_HOUR' && l.window.duration === 5) ||
      l.window.timeUnit === 'TIME_UNIT_MINUTE'
  );

  let rollingLimit = 60;
  let rollingRemaining = 60;
  let rollingResetTime = '';

  if (fiveHourLimitItem?.detail) {
    rollingLimit = Number.parseInt(fiveHourLimitItem.detail.limit, 10) || 60;
    rollingRemaining = Number.parseInt(fiveHourLimitItem.detail.remaining, 10) || 0;
    rollingResetTime = fiveHourLimitItem.detail.resetTime || '';
  }
  const rollingUsed = Math.max(0, rollingLimit - rollingRemaining);
  const rollingUsageRatio = rollingLimit > 0 ? rollingUsed / rollingLimit : 0;

  const rollingQuota = {
    limit: rollingLimit,
    remaining: rollingRemaining,
    used: rollingUsed,
    usageRatio: Math.min(1, Math.max(0, rollingUsageRatio)),
    durationMinutes: 300,
    resetTime: rollingResetTime,
    resetCountdown: formatTimeCountdown(rollingResetTime),
  };

  return {
    plan,
    totalQuota,
    weeklyQuota,
    rollingQuota,
    meta: {
      lastUpdated: new Date(),
      loading: false,
      error: null,
      isTokenExpiring: isTokenNearExpiry(token),
    },
  };
}

export function getResolvedApiBase(customBase?: string): string {
  if (customBase) return customBase;
  // 原生 Android APK 环境下直连 Kimi 官方接口 (由 CapacitorHttp 原生接管，无跨域限制)
  if (Capacitor.isNativePlatform()) {
    return 'https://www.kimi.com/apiv2';
  }
  return '/apiv2';
}

/**
 * 核心请求函数：拉取 Kimi 订阅与用量数据
 */
export async function fetchKimiUsageData(
  token: string,
  apiBase?: string
): Promise<KimiUsageState> {
  const resolvedBase = getResolvedApiBase(apiBase);
  const cleanToken = token.trim();
  if (!cleanToken) {
    throw new KimiApiError('未配置 Kimi 浏览器 Token', 'AUTH_INVALID');
  }

  if (isTokenExpired(cleanToken)) {
    throw new KimiApiError('Token 已过期，请重新获取并粘贴', 'AUTH_EXPIRED');
  }

  const payload = extractJwtPayload(cleanToken);
  const sub = payload?.sub || 'unknown_sub';
  const ssid = payload?.ssid || '0';

  const headers: Record<string, string> = {
    Authorization: `Bearer ${cleanToken}`,
    'x-traffic-id': sub,
    'x-msh-session-id': ssid,
    'x-msh-platform': 'web',
    'x-msh-version': '1.0.0',
    'x-language': 'zh-CN',
    'connect-protocol-version': '1',
    'Content-Type': 'application/json',
  };

  try {
    const [subRes, usageRes] = await Promise.all([
      fetch(`${resolvedBase}/kimi.gateway.membership.v2.MembershipService/GetSubscription`, {
        method: 'POST',
        headers,
        body: JSON.stringify({}),
      }),
      fetch(`${resolvedBase}/kimi.gateway.billing.v1.BillingService/GetUsages`, {
        method: 'POST',
        headers,
        body: JSON.stringify({ scope: ['FEATURE_CODING'] }),
      }),
    ]);

    if (subRes.status === 401 || usageRes.status === 401) {
      throw new KimiApiError('Token 鉴权失败 (401 Unauthorized)', 'AUTH_EXPIRED', 401);
    }

    if (!subRes.ok) {
      throw new KimiApiError(`获取会员信息失败: HTTP ${subRes.status}`, 'NETWORK_ERROR', subRes.status);
    }
    if (!usageRes.ok) {
      throw new KimiApiError(`获取用量信息失败: HTTP ${usageRes.status}`, 'NETWORK_ERROR', usageRes.status);
    }

    const subData: GetSubscriptionResponse = await subRes.json();
    const usageData: GetUsagesResponse = await usageRes.json();

    return normalizeKimiData(subData, usageData, cleanToken);
  } catch (err) {
    if (err instanceof KimiApiError) throw err;
    throw new KimiApiError(
      `网络请求失败: ${err instanceof Error ? err.message : String(err)}`,
      'NETWORK_ERROR'
    );
  }
}
