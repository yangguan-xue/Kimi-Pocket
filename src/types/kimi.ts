// ============================================
// Raw API Types (Kimi Connect Protocol)
// ============================================

export interface GetSubscriptionResponse {
  subscription?: {
    subscriptionId: string;
    goods: {
      id: string;
      title: string;
      durationDays: number;
      membershipLevel: string;
    };
    currentStartTime?: string;
    currentEndTime: string;
    status: string;
    active: boolean;
  };
  balances?: Array<{
    id: string;
    feature: string;
    type: string;
    unit: string;
    amountUsedRatio: number;
    expireTime: string;
  }>;
  subscribed?: boolean;
  capabilities?: Array<{
    feature: string;
    constraint?: {
      parallelism: number;
    };
  }>;
}

export interface GetUsagesResponse {
  usages?: Array<{
    scope: string;
    detail: {
      limit: string;
      remaining: string;
      resetTime: string;
    };
    limits?: Array<{
      window: {
        duration: number;
        timeUnit: string;
      };
      detail: {
        limit: string;
        remaining: string;
        resetTime: string;
      };
    }>;
  }>;
  totalQuota?: {
    limit: string;
    remaining: string;
  };
}

// ============================================
// Normalized Business State (Frontend Domain)
// ============================================

export interface QuotaItem {
  limit: number;
  remaining: number;
  used: number;
  usageRatio: number; // 0.0 ~ 1.0
}

export interface TimedQuotaItem extends QuotaItem {
  resetTime: string;
  resetCountdown: string;
}

export interface KimiUsageState {
  plan: {
    title: string;
    endTime: string;
    daysRemaining: number;
    isActive: boolean;
  };
  // 1. 总用量 (4段)
  totalQuota: QuotaItem;
  // 2. 周用量 (7段)
  weeklyQuota: TimedQuotaItem;
  // 3. 5小时额度 (连续单条/平滑单环)
  rollingQuota: TimedQuotaItem & { durationMinutes: number };
  meta: {
    lastUpdated: Date | null;
    loading: boolean;
    error: string | null;
    isTokenExpiring: boolean;
  };
}
