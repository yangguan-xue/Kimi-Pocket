export type WarningLevel = 'normal' | 'warning' | 'critical';

export interface StatusTheme {
  level: WarningLevel;
  hex: string;
  bgClass: string;
  textClass: string;
  glowShadow: string;
  filterDrop: string;
  borderClass: string;
}

/**
 * 根据额度使用率与剩余情况计算当前三态主题色：
 * - 剩余 > 30%：正常，一律高级翡翠绿 (#10B981)
 * - 剩余 <= 30% (已用 >= 70%)：用量不足，黄色预警 (#F59E0B)
 * - 剩余 <= 10% (已用 >= 90%) 或剩余为 0：严重不足，红色告警 (#EF4444)
 */
export function getStatusTheme(usageRatio: number, remaining?: number): StatusTheme {
  const remainRatio = 1 - Math.min(1, Math.max(0, usageRatio));

  if (remaining === 0 || remainRatio <= 0.1) {
    return {
      level: 'critical',
      hex: '#EF4444',
      bgClass: 'bg-red-500',
      textClass: 'text-red-400',
      glowShadow: 'shadow-[0_0_12px_rgba(239,68,68,0.8)]',
      filterDrop: 'drop-shadow-[0_0_8px_rgba(239,68,68,0.8)]',
      borderClass: 'border-red-500/30',
    };
  }

  if (remainRatio <= 0.3) {
    return {
      level: 'warning',
      hex: '#F59E0B',
      bgClass: 'bg-amber-400',
      textClass: 'text-amber-400',
      glowShadow: 'shadow-[0_0_12px_rgba(245,158,11,0.8)]',
      filterDrop: 'drop-shadow-[0_0_8px_rgba(245,158,11,0.8)]',
      borderClass: 'border-amber-400/30',
    };
  }

  // 正常未变色前：统一纯粹的高级质感冷翠绿
  return {
    level: 'normal',
    hex: '#10B981',
    bgClass: 'bg-emeraldGlow',
    textClass: 'text-emeraldGlow',
    glowShadow: 'shadow-[0_0_12px_rgba(16,185,129,0.7)]',
    filterDrop: 'drop-shadow-[0_0_8px_rgba(16,185,129,0.7)]',
    borderClass: 'border-emeraldGlow/20',
  };
}
