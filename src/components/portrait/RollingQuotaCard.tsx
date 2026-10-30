import React from 'react';
import type { TimedQuotaItem } from '../../types/kimi';
import { getStatusTheme } from '../../utils/statusTheme';

interface RollingQuotaCardProps {
  data: TimedQuotaItem & { durationMinutes: number };
}

export const RollingQuotaCard: React.FC<RollingQuotaCardProps> = ({ data }) => {
  const percentage = Math.round(data.usageRatio * 100);
  const theme = getStatusTheme(data.usageRatio, data.remaining);

  return (
    <div
      data-testid="rolling-quota-card"
      className="bg-cardDark border border-borderDark rounded-2xl p-5 shadow-xl backdrop-blur-md flex flex-col justify-between"
    >
      <div className="flex justify-between items-center mb-3">
        <span className="text-sm font-medium text-white/80">5小时额度 (5-Hour Rolling)</span>
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

      {/* 连续平滑单进度条 (不分段) */}
      <div
        data-testid="continuous-bar"
        className="h-4.5 w-full bg-white/10 rounded-full overflow-hidden p-0.5 mb-3"
      >
        <div
          className={`h-full ${theme.bgClass} rounded-full transition-all duration-500 ${theme.glowShadow}`}
          style={{ width: `${Math.min(100, Math.max(0, percentage))}%` }}
        />
      </div>

      <div className="flex justify-between items-center text-xs font-mono">
        <span className="text-white/40">
          已用 {data.used} / 总额 {data.limit} (余 {data.remaining})
        </span>
        <span className={theme.textClass}>{data.resetCountdown}重置</span>
      </div>
    </div>
  );
};
