import React from 'react';
import type { QuotaItem } from '../../types/kimi';

interface TotalQuotaCardProps {
  data: QuotaItem;
}

export const TotalQuotaCard: React.FC<TotalQuotaCardProps> = ({ data }) => {
  const percentage = Math.round(data.usageRatio * 100);

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
      className="bg-cardDark border border-borderDark rounded-2xl p-4 shadow-xl backdrop-blur-md"
    >
      <div className="flex justify-between items-center mb-2.5">
        <span className="text-xs font-medium text-white/70">总用量 (Total Quota)</span>
        <span className="text-xs font-semibold text-cyanGlow font-mono">{percentage}%</span>
      </div>

      {/* 严格 4 段分段容器 */}
      <div
        data-testid="segment-container-4"
        className="grid grid-cols-4 gap-2 mb-2.5"
      >
        {segments.map((fillRatio, idx) => (
          <div
            key={`seg-4-${idx}`}
            className="h-4 rounded-full bg-white/10 relative overflow-hidden"
          >
            {fillRatio > 0 && (
              <div
                className="h-full bg-cyanGlow rounded-full transition-all duration-500 shadow-[0_0_12px_rgba(0,229,255,0.6)]"
                style={{ width: `${fillRatio * 100}%` }}
              />
            )}
          </div>
        ))}
      </div>

      <div className="flex justify-between items-center text-[11px] text-white/40 font-mono">
        <span>已用 {data.used} / 总额 {data.limit}</span>
        <span className="text-white/30">4段阶段</span>
      </div>
    </div>
  );
};
