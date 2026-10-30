import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import { TotalQuotaCard } from '../TotalQuotaCard';
import { WeeklyQuotaCard } from '../WeeklyQuotaCard';
import { RollingQuotaCard } from '../RollingQuotaCard';

describe('Portrait Cards 规范渲染测试', () => {
  it('总用量卡片严格渲染 4 个段容器', () => {
    render(
      <TotalQuotaCard
        data={{ limit: 100, remaining: 35, used: 65, usageRatio: 0.65 }}
      />
    );
    const container = screen.getByTestId('segment-container-4');
    expect(container).toBeInTheDocument();
    expect(container.children.length).toBe(4);
    expect(screen.getByText('65%')).toBeInTheDocument();
  });

  it('周用量卡片严格渲染 7 个段容器', () => {
    render(
      <WeeklyQuotaCard
        data={{
          limit: 100,
          remaining: 58,
          used: 42,
          usageRatio: 0.42,
          resetTime: '2026-11-03',
          resetCountdown: '3天后',
        }}
      />
    );
    const container = screen.getByTestId('segment-container-7');
    expect(container).toBeInTheDocument();
    expect(container.children.length).toBe(7);
    expect(screen.getByText('42%')).toBeInTheDocument();
    expect(screen.getByText(/3天后恢复/)).toBeInTheDocument();
  });

  it('5小时额度卡片渲染连续进度条 (非分段)', () => {
    render(
      <RollingQuotaCard
        data={{
          limit: 60,
          remaining: 42,
          used: 18,
          usageRatio: 0.3,
          durationMinutes: 300,
          resetTime: '2026-10-30',
          resetCountdown: '1小时42分后',
        }}
      />
    );
    const continuousBar = screen.getByTestId('continuous-bar');
    expect(continuousBar).toBeInTheDocument();
    expect(screen.getByText('30%')).toBeInTheDocument();
    expect(screen.getByText(/1小时42分后恢复/)).toBeInTheDocument();
  });
});
