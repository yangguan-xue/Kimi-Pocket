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
      className="bg-cardDark border border-borderDark rounded-2xl p-4 shadow-xl backdrop-blur-md"
    >
      <div className="flex justify-between items-center mb-2.5">
        <span className="text-xs font-medium text-white/70">5小时额度 (5-Hour Rolling)</span>
        <div className="flex items-center gap-1.5">
          {theme.level === 'warning' && (
            <span className="text-[10px] px-1.5 py-0.5 rounded bg-amber-500/20 text-amber-300">
              用量偏高
            </span>
          )}
          {theme.level === 'critical' && (
            <span className="text-[10px] px-1.5 py-0.5 rounded bg-red-500/20 text-red-300 animate-pulse">
              额度紧张
            </span>
          )}
          <span className={`text-xs font-semibold ${theme.textClass} font-mono`}>
            {percentage}%
          </span>
        </div>
      </div>

      {/* 连续平滑单进度条 (不分段) */}
      <div
        data-testid="continuous-bar"
        className="h-4 w-full bg-white/10 rounded-full overflow-hidden p-0.5 mb-2.5"
      >
        <div
          className={`h-full ${theme.bgClass} rounded-full transition-all duration-500 ${theme.glowShadow}`}
          style={{ width: `${Math.min(100, Math.max(0, percentage))}%` }}
        />
      </div>

      <div className="flex justify-between items-center text-[11px] font-mono">
        <span className="text-white/40">
          已用 {data.used} / 总额 {data.limit} (余 {data.remaining})
        </span>
        <span className={theme.textClass}>{data.resetCountdown}重置</span>
      </div>
    </div>
  );
};
