import React from 'react';

interface ContinuousRingProps {
  ratio: number;
  title: string;
  sublabel: string;
  valueText: string;
  subValueText: string;
  testId?: string;
}

export const ContinuousRing: React.FC<ContinuousRingProps> = ({
  ratio,
  title,
  sublabel,
  valueText,
  subValueText,
  testId,
}) => {
  const radius = 45;
  const circumference = 2 * Math.PI * radius; // ~282.74
  const clampedRatio = Math.min(1, Math.max(0, ratio));
  const filledLength = clampedRatio * circumference;

  return (
    <div
      data-testid={testId || 'continuous-ring'}
      className="flex flex-col items-center justify-center p-3"
    >
      <div className="relative w-36 h-36 flex items-center justify-center">
        <svg
          className="w-full h-full transform -rotate-90"
          viewBox="0 0 120 120"
        >
          {/* 背景底轨：完整连续圆环 */}
          <circle
            data-testid="bg-continuous-arc"
            cx="60"
            cy="60"
            r={radius}
            fill="none"
            stroke="rgba(255, 255, 255, 0.1)"
            strokeWidth="8"
          />

          {/* 高亮填充弧：平滑连续，带发光光晕 */}
          {filledLength > 0 && (
            <circle
              data-testid="active-continuous-arc"
              cx="60"
              cy="60"
              r={radius}
              fill="none"
              stroke="#00E5FF"
              strokeWidth="8"
              strokeDasharray={`${filledLength} ${circumference}`}
              strokeLinecap="round"
              className="transition-all duration-700 ease-out drop-shadow-[0_0_8px_rgba(0,229,255,0.8)]"
            />
          )}
        </svg>

        {/* 环心数据指标 */}
        <div className="absolute inset-0 flex flex-col items-center justify-center pointer-events-none">
          <span className="text-2xl font-bold font-mono tracking-tight text-white">
            {valueText}
          </span>
          <span className="text-[11px] font-mono text-white/50 mt-0.5">
            {subValueText}
          </span>
        </div>
      </div>

      {/* 环底标签 */}
      <div className="mt-3 text-center">
        <div className="text-sm font-medium text-white/90">{title}</div>
        <div className="text-[11px] font-mono mt-0.5 text-cyanGlow">
          {sublabel}
        </div>
      </div>
    </div>
  );
};
