import React from 'react';
import type { TimedQuotaItem } from '../../types/kimi';
import { getStatusTheme } from '../../utils/statusTheme';

interface WeeklyQuotaCardProps {
  data: TimedQuotaItem;
}

export const WeeklyQuotaCard: React.FC<WeeklyQuotaCardProps> = ({ data }) => {
  const percentage = Math.round(data.usageRatio * 100);
  const theme = getStatusTheme(data.usageRatio, data.remaining);

  // 7段逻辑：每段代表 1/7
  const segmentStep = 1 / 7;
  const segments = [0, 1, 2, 3, 4, 5, 6].map((index) => {
    const segStart = index * segmentStep;
    const segEnd = (index + 1) * segmentStep;
    if (data.usageRatio >= segEnd) return 1;
    if (data.usageRatio <= segStart) return 0;
    return (data.usageRatio - segStart) / segmentStep;
  });

  return (
    <div
      data-testid="weekly-quota-card"
      className="bg-cardDark border border-borderDark rounded-2xl p-5 shadow-xl backdrop-blur-md flex flex-col justify-between"
    >
      <div className="flex justify-between items-center mb-3">
        <span className="text-sm font-medium text-white/80">周用量 (Weekly Quota)</span>
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

      {/* 严格 7 段分段容器：高度为标准的 h-5 (20px) */}
      <div
        data-testid="segment-container-7"
        className="grid grid-cols-7 gap-2 mb-3"
      >
        {segments.map((fillRatio, idx) => (
          <div
            key={`seg-7-${idx}`}
            className="h-5 rounded-full bg-white/10 relative overflow-hidden"
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

      <div className="flex justify-between items-center text-xs font-mono">
        <span className="text-white/40">已用 {data.used} / 总额 {data.limit}</span>
        <span className={theme.textClass}>{data.resetCountdown}恢复</span>
      </div>
    </div>
  );
};
