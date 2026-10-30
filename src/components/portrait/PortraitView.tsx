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
      className="flex flex-col gap-3.5 w-full max-w-md mx-auto"
    >
      <TotalQuotaCard data={state.totalQuota} />
      <WeeklyQuotaCard data={state.weeklyQuota} />
      <RollingQuotaCard data={state.rollingQuota} />
    </div>
  );
};
