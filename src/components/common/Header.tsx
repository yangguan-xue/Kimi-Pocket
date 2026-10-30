import React from 'react';
import { RefreshCw, Key } from 'lucide-react';
import type { KimiUsageState } from '../../types/kimi';

interface HeaderProps {
  state: KimiUsageState;
  isDemo: boolean;
  onOpenTokenModal: () => void;
  onRefresh: () => void;
}

export const Header: React.FC<HeaderProps> = ({
  state,
  isDemo,
  onOpenTokenModal,
  onRefresh,
}) => {
  return (
    <header className="flex items-center justify-between w-full max-w-4xl mx-auto mb-3 px-3 pt-2">
      <div className="flex items-center">
        <span
          className={`w-2.5 h-2.5 rounded-full mr-2 shrink-0 inline-block ${
            state.meta.loading
              ? 'bg-yellow-400 animate-pulse'
              : isDemo
              ? 'bg-white/40'
              : 'bg-emeraldGlow shadow-[0_0_8px_rgba(16,185,129,0.8)]'
          }`}
        />
        <span className="text-sm md:text-base font-semibold text-white mr-2 shrink-0 inline-block">
          Kimi Pocket
        </span>
        {state.plan.title && (
          <span className="text-[11px] font-medium px-2.5 py-0.5 rounded-full bg-white/10 text-emeraldGlow border border-emeraldGlow/30 shrink-0 inline-block">
            {state.plan.title}
          </span>
        )}
        {isDemo && (
          <span className="text-[10px] px-1.5 py-0.5 rounded bg-amber-500/20 text-amber-300 ml-1.5 shrink-0 inline-block">
            演示
          </span>
        )}
      </div>

      <div className="flex items-center gap-2 sm:gap-3 shrink-0">
        <span className="text-xs text-white/50 font-mono hidden sm:inline whitespace-nowrap">
          到期: {state.plan.daysRemaining}天后
        </span>

        <button
          onClick={onRefresh}
          disabled={state.meta.loading}
          aria-label="刷新"
          className="p-1.5 rounded-lg bg-white/10 text-white/70 hover:text-white hover:bg-white/20 transition-colors disabled:opacity-50"
        >
          <RefreshCw
            className={`w-3.5 h-3.5 ${state.meta.loading ? 'animate-spin' : ''}`}
          />
        </button>

        <button
          onClick={onOpenTokenModal}
          className="flex items-center gap-1.5 px-2.5 py-1.5 rounded-lg bg-white/10 text-xs text-white/80 hover:bg-white/20 transition-colors whitespace-nowrap"
        >
          <Key className="w-3.5 h-3.5 text-emeraldGlow" />
          <span className="hidden sm:inline">Token 设置</span>
        </button>
      </div>
    </header>
  );
};
