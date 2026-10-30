import React from 'react';
import type { KimiUsageState } from '../../types/kimi';
import { TotalQuotaCard } from './TotalQuotaCard';
import { WeeklyQuotaCard } from './WeeklyQuotaCard';
import { RollingQuotaCard } from './RollingQuotaCard';

interface PortraitViewProps {
  state: KimiUsageState;
}

export const PortraitView: React.FC<PortraitViewProps> = ({ state }) => {
  return (
    <div
      data-testid="portrait-view"
      className="flex flex-col gap-4 w-full max-w-md mx-auto flex-1 justify-around py-2"
    >
      <TotalQuotaCard data={state.totalQuota} daysRemaining={state.plan.daysRemaining} />
      <WeeklyQuotaCard data={state.weeklyQuota} />
      <RollingQuotaCard data={state.rollingQuota} />
    </div>
  );
};
