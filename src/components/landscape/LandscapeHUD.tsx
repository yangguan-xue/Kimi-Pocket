import React from 'react';
import type { KimiUsageState } from '../../types/kimi';
import { SegmentedRing } from './SegmentedRing';
import { ContinuousRing } from './ContinuousRing';

interface LandscapeHUDProps {
  state: KimiUsageState;
}

export const LandscapeHUD: React.FC<LandscapeHUDProps> = ({ state }) => {
  const totalPercent = Math.round(state.totalQuota.usageRatio * 100);
  const weeklyPercent = Math.round(state.weeklyQuota.usageRatio * 100);
  const rollingPercent = Math.round(state.rollingQuota.usageRatio * 100);

  return (
    <div
      data-testid="landscape-hud"
      className="w-full max-w-5xl mx-auto h-full flex flex-col justify-center bg-cardDark/80 border border-borderDark rounded-3xl p-6 sm:p-8 backdrop-blur-2xl shadow-2xl transition-all duration-300"
    >
      <div className="grid grid-cols-3 gap-6 items-center justify-items-center">
        {/* 左环：总用量 (严格 4 段圆弧) */}
        <SegmentedRing
          segments={4}
          ratio={state.totalQuota.usageRatio}
          remaining={state.totalQuota.remaining}
          title="总用量"
          sublabel="4段阶段"
          valueText={`${totalPercent}%`}
          subValueText={`${state.totalQuota.used} / ${state.totalQuota.limit}`}
          testId="ring-total-quota-4"
        />

        {/* 中环：周用量 (严格 7 段圆弧) */}
        <SegmentedRing
          segments={7}
          ratio={state.weeklyQuota.usageRatio}
          remaining={state.weeklyQuota.remaining}
          title="周用量"
          sublabel={`${state.weeklyQuota.resetCountdown}重置 (7段)`}
          valueText={`${weeklyPercent}%`}
          subValueText={`${state.weeklyQuota.used} / ${state.weeklyQuota.limit}`}
          testId="ring-weekly-quota-7"
        />

        {/* 右环：5小时额度 (连续平滑单环) */}
        <ContinuousRing
          ratio={state.rollingQuota.usageRatio}
          remaining={state.rollingQuota.remaining}
          title="5小时额度"
          sublabel={`${state.rollingQuota.resetCountdown}重置`}
          valueText={`${rollingPercent}%`}
          subValueText={`余 ${state.rollingQuota.remaining} / ${state.rollingQuota.limit}`}
          testId="ring-rolling-quota-smooth"
        />
      </div>
    </div>
  );
};
