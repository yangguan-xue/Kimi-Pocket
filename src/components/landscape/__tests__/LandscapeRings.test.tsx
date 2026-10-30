import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import { SegmentedRing } from '../SegmentedRing';
import { ContinuousRing } from '../ContinuousRing';
import { LandscapeHUD } from '../LandscapeHUD';
import type { KimiUsageState } from '../../../types/kimi';

describe('Landscape HUD 矢量仪表盘数学与结构测试', () => {
  it('SegmentedRing 4 段圆环具备正确的 strokeDasharray 间隙参数', () => {
    render(
      <SegmentedRing
        segments={4}
        ratio={0.65}
        color="cyan"
        title="总用量"
        sublabel="4段圆弧"
        valueText="65%"
        subValueText="65 / 100"
      />
    );
    const bgArc = screen.getByTestId('bg-arc-4');
    expect(bgArc).toBeInTheDocument();
    // 周长 282.74, 4段，gap=8 -> segmentLength = (282.74 - 32)/4 = 62.68
    const dashArray = bgArc.getAttribute('stroke-dasharray');
    expect(dashArray).toContain('8'); // gap 为 8
  });

  it('SegmentedRing 7 段圆环具备正确的 strokeDasharray 间隙参数', () => {
    render(
      <SegmentedRing
        segments={7}
        ratio={0.42}
        color="emerald"
        title="周用量"
        sublabel="3天后重置"
        valueText="42%"
        subValueText="42 / 100"
      />
    );
    const bgArc = screen.getByTestId('bg-arc-7');
    expect(bgArc).toBeInTheDocument();
    // 7段，gap=6
    const dashArray = bgArc.getAttribute('stroke-dasharray');
    expect(dashArray).toContain('6'); // gap 为 6
  });

  it('ContinuousRing 渲染连续无 dash 间隙的背景圆环', () => {
    render(
      <ContinuousRing
        ratio={0.3}
        title="5小时额度"
        sublabel="1小时后重置"
        valueText="30%"
        subValueText="余 42 / 60"
      />
    );
    const bgArc = screen.getByTestId('bg-continuous-arc');
    expect(bgArc).toBeInTheDocument();
    expect(bgArc.getAttribute('stroke-dasharray')).toBeNull(); // 完整无 gap
  });

  it('LandscapeHUD 完整组合三环并正确展示', () => {
    const mockState: KimiUsageState = {
      plan: { title: 'Kimi Pro', endTime: '2026-11-28', daysRemaining: 29, isActive: true },
      totalQuota: { limit: 100, remaining: 35, used: 65, usageRatio: 0.65 },
      weeklyQuota: { limit: 100, remaining: 58, used: 42, usageRatio: 0.42, resetTime: '', resetCountdown: '3天后' },
      rollingQuota: { limit: 60, remaining: 42, used: 18, usageRatio: 0.3, durationMinutes: 300, resetTime: '', resetCountdown: '1小时42分后' },
      meta: { lastUpdated: new Date(), loading: false, error: null, isTokenExpiring: false },
    };

    render(<LandscapeHUD state={mockState} />);
    expect(screen.getByTestId('ring-total-quota-4')).toBeInTheDocument();
    expect(screen.getByTestId('ring-weekly-quota-7')).toBeInTheDocument();
    expect(screen.getByTestId('ring-rolling-quota-smooth')).toBeInTheDocument();
  });
});
