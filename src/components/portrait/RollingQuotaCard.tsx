import React from 'react';
import type { TimedQuotaItem } from '../../types/kimi';

interface RollingQuotaCardProps {
  data: TimedQuotaItem & { durationMinutes: number };
}

export const RollingQuotaCard: React.FC<RollingQuotaCardProps> = ({ data }) => {
  const percentage = Math.round(data.usageRatio * 100);

  return (
    <div
      data-testid="rolling-quota-card"
      className="bg-cardDark border border-borderDark rounded-2xl p-4 shadow-xl backdrop-blur-md"
    >
      <div className="flex justify-between items-center mb-2.5">
        <span className="text-xs font-medium text-white/70">5小时额度 (5-Hour Rolling)</span>
        <span className="text-xs font-semibold text-cyanGlow font-mono">{percentage}%</span>
      </div>

      {/* 连续平滑单进度条 (不分段) */}
      <div
        data-testid="continuous-bar"
        className="h-4 w-full bg-white/10 rounded-full overflow-hidden p-0.5 mb-2.5"
      >
        <div
          className="h-full bg-cyanGlow rounded-full transition-all duration-500 shadow-[0_0_12px_rgba(0,229,255,0.7)]"
          style={{ width: `${Math.min(100, Math.max(0, percentage))}%` }}
        />
      </div>

      <div className="flex justify-between items-center text-[11px] font-mono">
        <span className="text-white/40">
          已用 {data.used} / 总额 {data.limit} (余 {data.remaining})
        </span>
        <span className="text-cyanGlow/90">{data.resetCountdown}重置</span>
      </div>
    </div>
  );
};
