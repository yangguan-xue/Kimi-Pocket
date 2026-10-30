import React from 'react';
import type { TimedQuotaItem } from '../../types/kimi';

interface WeeklyQuotaCardProps {
  data: TimedQuotaItem;
}

export const WeeklyQuotaCard: React.FC<WeeklyQuotaCardProps> = ({ data }) => {
  const percentage = Math.round(data.usageRatio * 100);

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
      className="bg-cardDark border border-borderDark rounded-2xl p-4 shadow-xl backdrop-blur-md"
    >
      <div className="flex justify-between items-center mb-2.5">
        <span className="text-xs font-medium text-white/70">周用量 (Weekly Quota)</span>
        <span className="text-xs font-semibold text-emeraldGlow font-mono">{percentage}%</span>
      </div>

      {/* 严格 7 段分段容器 */}
      <div
        data-testid="segment-container-7"
        className="grid grid-cols-7 gap-1.5 mb-2.5"
      >
        {segments.map((fillRatio, idx) => (
          <div
            key={`seg-7-${idx}`}
            className="h-4 rounded-full bg-white/10 relative overflow-hidden"
          >
            {fillRatio > 0 && (
              <div
                className="h-full bg-emeraldGlow rounded-full transition-all duration-500 shadow-[0_0_12px_rgba(16,185,129,0.6)]"
                style={{ width: `${fillRatio * 100}%` }}
              />
            )}
          </div>
        ))}
      </div>

      <div className="flex justify-between items-center text-[11px] font-mono">
        <span className="text-white/40">已用 {data.used} / 总额 {data.limit}</span>
        <span className="text-emeraldGlow/90">{data.resetCountdown}重置 (7段)</span>
      </div>
    </div>
  );
};
