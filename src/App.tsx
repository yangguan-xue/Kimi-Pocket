import React, { useState } from 'react';
import { useOrientation } from './hooks/useOrientation';
import { useKimiUsage } from './hooks/useKimiUsage';
import { Header } from './components/common/Header';
import { TokenModal } from './components/common/TokenModal';
import { PortraitView } from './components/portrait/PortraitView';
import { LandscapeHUD } from './components/landscape/LandscapeHUD';
import { AlertCircle, AlertTriangle } from 'lucide-react';

export const App: React.FC = () => {
  const { isLandscape: detectedLandscape } = useOrientation();
  const [manualOverride, setManualOverride] = useState<'auto' | 'portrait' | 'landscape'>('auto');
  const [tokenModalOpen, setTokenModalOpen] = useState(false);
  const { token, state, isDemo, saveToken, refresh } = useKimiUsage();

  const isLandscape =
    manualOverride === 'auto' ? detectedLandscape : manualOverride === 'landscape';

  return (
    <div className="h-screen w-screen bg-black text-white flex flex-col justify-between items-center p-3 sm:p-5 select-none font-sans overflow-hidden">
      {/* 桌面端模式切换辅助控制栏 */}
      <div className="fixed top-3 right-3 z-40 hidden md:flex items-center gap-1.5 bg-white/10 backdrop-blur-md px-3 py-1.5 rounded-full text-xs text-white/70 border border-white/10">
        <span className="text-[11px] text-white/40">视图:</span>
        <button
          onClick={() => setManualOverride('auto')}
          className={`px-2 py-0.5 rounded text-[11px] transition-colors ${
            manualOverride === 'auto' ? 'bg-white/20 text-white font-medium' : 'hover:bg-white/10'
          }`}
        >
          自适应
        </button>
        <button
          onClick={() => setManualOverride('portrait')}
          className={`px-2 py-0.5 rounded text-[11px] transition-colors ${
            manualOverride === 'portrait' ? 'bg-white/20 text-white font-medium' : 'hover:bg-white/10'
          }`}
        >
          竖屏
        </button>
        <button
          onClick={() => setManualOverride('landscape')}
          className={`px-2 py-0.5 rounded text-[11px] transition-colors ${
            manualOverride === 'landscape' ? 'bg-white/20 text-white font-medium' : 'hover:bg-white/10'
          }`}
        >
          横屏(三环)
        </button>
      </div>

      <div className="w-full flex-1 flex flex-col items-center justify-between overflow-hidden">
        {/* 顶部状态与操作栏 */}
        <Header
          state={state}
          isDemo={isDemo}
          onOpenTokenModal={() => setTokenModalOpen(true)}
          onRefresh={refresh}
        />

        {/* 异常提示横幅 */}
        {state.meta.error && (
          <div className="w-full max-w-md mx-auto mb-2 px-3.5 py-2 rounded-xl bg-red-500/10 border border-red-500/20 text-red-300 text-xs flex items-center gap-2 shrink-0">
            <AlertCircle className="w-4 h-4 shrink-0 text-red-400" />
            <span className="flex-1">{state.meta.error}</span>
            <button
              onClick={() => setTokenModalOpen(true)}
              className="underline text-[11px] hover:text-white"
            >
              修改Token
            </button>
          </div>
        )}

        {/* Token 即将过期提示 */}
        {state.meta.isTokenExpiring && (
          <div className="w-full max-w-md mx-auto mb-2 px-3.5 py-2 rounded-xl bg-amber-500/10 border border-amber-500/20 text-amber-300 text-xs flex items-center gap-2 shrink-0">
            <AlertTriangle className="w-4 h-4 shrink-0 text-amber-400" />
            <span>Kimi Token 即将到期（1小时内），请及时更新。</span>
          </div>
        )}

        {/* 主视口：竖屏直排 3 卡片 VS 横屏 StandBy 3 环仪表盘 */}
        <main className="w-full flex-1 flex items-center justify-center overflow-hidden transition-all duration-300">
          {isLandscape ? (
            <LandscapeHUD state={state} />
          ) : (
            <PortraitView state={state} />
          )}
        </main>
      </div>

      {/* Token 配置模态框 */}
      <TokenModal
        isOpen={tokenModalOpen}
        currentToken={token}
        onClose={() => setTokenModalOpen(false)}
        onSave={saveToken}
      />
    </div>
  );
};
