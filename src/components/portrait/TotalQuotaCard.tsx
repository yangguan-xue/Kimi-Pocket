import React from 'react';
import type { QuotaItem } from '../../types/kimi';
import { getStatusTheme } from '../../utils/statusTheme';

interface TotalQuotaCardProps {
  data: QuotaItem;
}

export const TotalQuotaCard: React.FC<TotalQuotaCardProps> = ({ data }) => {
  const percentage = Math.round(data.usageRatio * 100);
  const theme = getStatusTheme(data.usageRatio, data.remaining);

  // 4段逻辑：每段代表 25% (0.25)
  const segments = [0, 1, 2, 3].map((index) => {
    const segStart = index * 0.25;
    const segEnd = (index + 1) * 0.25;
    if (data.usageRatio >= segEnd) return 1; // 满段
    if (data.usageRatio <= segStart) return 0; // 空段
    return (data.usageRatio - segStart) / 0.25; // 部分填充
  });

  return (
    <div
      data-testid="total-quota-card"
      className="bg-cardDark border border-borderDark rounded-2xl p-5 shadow-xl backdrop-blur-md flex flex-col justify-between"
    >
      <div className="flex justify-between items-center mb-3">
        <span className="text-sm font-medium text-white/80">总用量 (Total Quota)</span>
        <div className="flex items-center gap-2">
          {theme.level === 'warning' && (
            <span className="text-[10px] px-2 py-0.5 rounded bg-amber-500/20 text-amber-300">
              用量偏高
            </span>
          )}
          {theme.level === 'critical' && (
            <span className="text-[10px] px-2 py-0.5 rounded bg-red-500/20 text-red-300 animate-pulse">
              额度紧张
            </span>
          )}
          <span className={`text-sm font-semibold ${theme.textClass} font-mono`}>
            {percentage}%
          </span>
        </div>
      </div>

      {/* 严格 4 段分段容器 */}
      <div
        data-testid="segment-container-4"
        className="grid grid-cols-4 gap-2.5 mb-3"
      >
        {segments.map((fillRatio, idx) => (
          <div
            key={`seg-4-${idx}`}
            className="h-4.5 rounded-full bg-white/10 relative overflow-hidden"
          >
            {fillRatio > 0 && (
              <div
                className={`h-full ${theme.bgClass} rounded-full transition-all duration-500 ${theme.glowShadow}`}
                style={{ width: `${fillRatio * 100}%` }}
              />
            )}
          </div>
        ))}
      </div>

      <div className="flex justify-between items-center text-xs text-white/40 font-mono">
        <span>已用 {data.used} / 总额 {data.limit}</span>
        <span className="text-white/30">4段阶段</span>
      </div>
    </div>
  );
};
