import React from 'react';

interface SegmentedRingProps {
  segments: 4 | 7;
  ratio: number; // 0.0 ~ 1.0
  color: 'cyan' | 'emerald';
  title: string;
  sublabel: string;
  valueText: string;
  subValueText: string;
  testId?: string;
}

export const SegmentedRing: React.FC<SegmentedRingProps> = ({
  segments,
  ratio,
  color,
  title,
  sublabel,
  valueText,
  subValueText,
  testId,
}) => {
  const radius = 45;
  const circumference = 2 * Math.PI * radius; // ~282.74

  // 严格数学计算等分段与间隙
  const gap = segments === 4 ? 8 : 6;
  const segmentLength = (circumference - segments * gap) / segments;

  // 计算已填充的弧长
  const clampedRatio = Math.min(1, Math.max(0, ratio));
  const fullSegments = Math.floor(clampedRatio * segments);
  const partialSegmentRatio = (clampedRatio * segments) - fullSegments;
  
  let filledLength = fullSegments * (segmentLength + gap);
  if (partialSegmentRatio > 0) {
    filledLength += partialSegmentRatio * segmentLength;
  }

  const strokeColor = color === 'cyan' ? '#00E5FF' : '#10B981';
  const filterClass = color === 'cyan' ? 'drop-shadow-[0_0_8px_rgba(0,229,255,0.7)]' : 'drop-shadow-[0_0_8px_rgba(16,185,129,0.7)]';
  const textColorClass = color === 'cyan' ? 'text-cyanGlow' : 'text-emeraldGlow';

  return (
    <div
      data-testid={testId || `segmented-ring-${segments}`}
      className="flex flex-col items-center justify-center p-3"
    >
      <div className="relative w-36 h-36 flex items-center justify-center">
        <svg
          className="w-full h-full transform -rotate-90"
          viewBox="0 0 120 120"
        >
          {/* 背景底轨：严格 N 等分离散圆弧 */}
          <circle
            data-testid={`bg-arc-${segments}`}
            cx="60"
            cy="60"
            r={radius}
            fill="none"
            stroke="rgba(255, 255, 255, 0.1)"
            strokeWidth="8"
            strokeDasharray={`${segmentLength} ${gap}`}
          />

          {/* 高亮填充弧 */}
          {filledLength > 0 && (
            <circle
              data-testid={`active-arc-${segments}`}
              cx="60"
              cy="60"
              r={radius}
              fill="none"
              stroke={strokeColor}
              strokeWidth="8"
              strokeDasharray={`${filledLength} ${circumference}`}
              strokeLinecap="round"
              className={`transition-all duration-700 ease-out ${filterClass}`}
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

      {/* 环底标签与周期说明 */}
      <div className="mt-3 text-center">
        <div className="text-sm font-medium text-white/90">{title}</div>
        <div className={`text-[11px] font-mono mt-0.5 ${textColorClass}`}>
          {sublabel}
        </div>
      </div>
    </div>
  );
};
