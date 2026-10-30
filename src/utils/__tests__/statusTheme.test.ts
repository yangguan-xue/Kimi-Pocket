import { describe, it, expect } from 'vitest';
import { getStatusTheme } from '../statusTheme';

describe('getStatusTheme 三态色彩映射逻辑测试', () => {
  it('未变黄和变红之前 (剩余 > 30%)，必须全部返回绿色 (#10B981)', () => {
    // 0% 已用，100% 剩余 -> 绿色
    const theme0 = getStatusTheme(0);
    expect(theme0.level).toBe('normal');
    expect(theme0.hex).toBe('#10B981');
    expect(theme0.bgClass).toBe('bg-emeraldGlow');

    // 50% 已用，50% 剩余 -> 绿色
    const theme50 = getStatusTheme(0.5);
    expect(theme50.level).toBe('normal');
    expect(theme50.hex).toBe('#10B981');

    // 68% 已用，32% 剩余 -> 绿色
    const theme68 = getStatusTheme(0.68);
    expect(theme68.level).toBe('normal');
    expect(theme68.hex).toBe('#10B981');
  });

  it('用量不足时 (剩余 <= 30%)，变成黄色 (#F59E0B)', () => {
    // 75% 已用，25% 剩余 -> 黄色
    const theme75 = getStatusTheme(0.75);
    expect(theme75.level).toBe('warning');
    expect(theme75.hex).toBe('#F59E0B');
    expect(theme75.textClass).toBe('text-amber-400');

    // 85% 已用，15% 剩余 -> 黄色
    const theme85 = getStatusTheme(0.85);
    expect(theme85.level).toBe('warning');
    expect(theme85.hex).toBe('#F59E0B');
  });

  it('严重不足或用尽时 (剩余 <= 10% 或 remaining === 0)，变成红色 (#EF4444)', () => {
    // 92% 已用，8% 剩余 -> 红色
    const theme92 = getStatusTheme(0.92);
    expect(theme92.level).toBe('critical');
    expect(theme92.hex).toBe('#EF4444');
    expect(theme92.textClass).toBe('text-red-400');

    // 剩余为 0 -> 红色
    const themeEmpty = getStatusTheme(0.5, 0);
    expect(themeEmpty.level).toBe('critical');
    expect(themeEmpty.hex).toBe('#EF4444');
  });
});
