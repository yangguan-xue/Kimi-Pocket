import React, { useState } from 'react';
import { X, Key, ShieldCheck, Clipboard as ClipboardIcon, Check } from 'lucide-react';
import { Clipboard } from '@capacitor/clipboard';

interface TokenModalProps {
  isOpen: boolean;
  currentToken: string;
  currentRefreshToken?: string;
  onClose: () => void;
  onSave: (token: string, refreshToken?: string) => void;
}

export const TokenModal: React.FC<TokenModalProps> = ({
  isOpen,
  currentToken,
  currentRefreshToken = '',
  onClose,
  onSave,
}) => {
  const [inputVal, setInputVal] = useState(currentToken);
  const [refreshVal, setRefreshVal] = useState(currentRefreshToken);
  const [pastedTarget, setPastedTarget] = useState<'access' | 'refresh' | null>(null);

  if (!isOpen) return null;

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSave(inputVal.trim(), refreshVal.trim());
    onClose();
  };

  const handlePaste = async (target: 'access' | 'refresh') => {
    let clipText = '';
    try {
      const res = await Clipboard.read();
      clipText = res.value || '';
    } catch {
      try {
        clipText = await navigator.clipboard.readText();
      } catch {
        // clipboard access denied
      }
    }

    if (clipText) {
      if (target === 'access') {
        setInputVal(clipText.trim());
      } else {
        setRefreshVal(clipText.trim());
      }
      setPastedTarget(target);
      setTimeout(() => setPastedTarget(null), 2000);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-start sm:items-center justify-center p-3 pt-6 sm:p-4 bg-black/85 backdrop-blur-md overflow-y-auto animate-fade-in">
      <div className="bg-[#0A0A0A] border border-white/10 rounded-2xl w-full max-w-md p-4 sm:p-5 shadow-2xl text-white max-h-[90vh] flex flex-col">
        {/* 顶部标题栏：常驻保存按钮，无论软键盘如何弹起都绝对不被遮挡 */}
        <div className="flex items-center justify-between pb-3 border-b border-white/10 mb-3 shrink-0">
          <div className="flex items-center gap-2">
            <Key className="w-4 h-4 text-emeraldGlow" />
            <h2 className="text-sm font-semibold">配置凭证与自动续期</h2>
          </div>
          <div className="flex items-center gap-2">
            <button
              type="button"
              onClick={handleSubmit}
              className="px-3.5 py-1 rounded-lg bg-emeraldGlow text-black font-semibold text-xs hover:bg-emeraldGlow/90 transition-all shadow-[0_0_10px_rgba(16,185,129,0.5)] active:scale-95"
            >
              保存
            </button>
            <button
              type="button"
              onClick={onClose}
              className="p-1.5 rounded-lg hover:bg-white/10 text-white/50 hover:text-white transition-colors"
            >
              <X className="w-4 h-4" />
            </button>
          </div>
        </div>

        {/* 表单内容区：支持自适应上下平滑滚动避让软键盘 */}
        <form onSubmit={handleSubmit} className="flex-1 overflow-y-auto pr-1 flex flex-col gap-3">
          <div>
            <div className="flex justify-between items-center mb-1.5">
              <label className="text-xs text-white/80 font-medium">
                Access Token <span className="text-red-400">*</span>
              </label>
              <button
                type="button"
                onClick={() => handlePaste('access')}
                className={`flex items-center gap-1 px-2 py-0.5 rounded text-[11px] transition-all font-medium ${
                  pastedTarget === 'access'
                    ? 'bg-emeraldGlow/20 text-emeraldGlow border border-emeraldGlow/40'
                    : 'bg-white/10 hover:bg-white/15 text-white/80 hover:text-white border border-white/10 active:scale-95'
                }`}
              >
                {pastedTarget === 'access' ? (
                  <>
                    <Check className="w-3 h-3 text-emeraldGlow" />
                    <span>已粘贴 ✓</span>
                  </>
                ) : (
                  <>
                    <ClipboardIcon className="w-3 h-3 text-white/60" />
                    <span>一键粘贴</span>
                  </>
                )}
              </button>
            </div>
            <textarea
              value={inputVal}
              onChange={(e) => setInputVal(e.target.value)}
              rows={2}
              autoCapitalize="none"
              autoCorrect="off"
              spellCheck={false}
              placeholder="必填：点击右上角【一键粘贴】或长按输入"
              className="w-full p-2 rounded-xl bg-white/5 border border-white/10 text-xs font-mono text-white placeholder:text-white/20 focus:outline-none focus:border-emeraldGlow/50 transition-colors resize-none select-text"
            />
          </div>

          <div>
            <div className="flex justify-between items-center mb-1.5">
              <label className="text-xs text-emeraldGlow font-medium flex items-center gap-1">
                Refresh Token <span className="text-[10px] text-emeraldGlow/70">(自动续期)</span>
              </label>
              <button
                type="button"
                onClick={() => handlePaste('refresh')}
                className={`flex items-center gap-1 px-2 py-0.5 rounded text-[11px] transition-all font-medium ${
                  pastedTarget === 'refresh'
                    ? 'bg-emeraldGlow/20 text-emeraldGlow border border-emeraldGlow/40'
                    : 'bg-emeraldGlow/10 hover:bg-emeraldGlow/20 text-emeraldGlow/90 border border-emeraldGlow/20 active:scale-95'
                }`}
              >
                {pastedTarget === 'refresh' ? (
                  <>
                    <Check className="w-3 h-3 text-emeraldGlow" />
                    <span>已粘贴 ✓</span>
                  </>
                ) : (
                  <>
                    <ClipboardIcon className="w-3 h-3 text-emeraldGlow/70" />
                    <span>一键粘贴</span>
                  </>
                )}
              </button>
            </div>
            <textarea
              value={refreshVal}
              onChange={(e) => setRefreshVal(e.target.value)}
              rows={2}
              autoCapitalize="none"
              autoCorrect="off"
              spellCheck={false}
              placeholder="可选：粘贴 refresh_token，开启永久静默自动续期"
              className="w-full p-2 rounded-xl bg-white/5 border border-emeraldGlow/20 text-xs font-mono text-white placeholder:text-white/20 focus:outline-none focus:border-emeraldGlow/60 transition-colors resize-none select-text"
            />
            <p className="text-[10px] text-white/40 mt-1 leading-relaxed">
              提示：填入 refresh_token 后，客户端会自动静默向 Kimi 换取新 Token，无需重复手动配置。
            </p>
          </div>

          <div className="flex items-center gap-1.5 text-[11px] text-white/40">
            <ShieldCheck className="w-3.5 h-3.5 text-emeraldGlow shrink-0" />
            <span>凭证仅加密存储于本机 LocalStorage，绝不离端传输。</span>
          </div>

          {/* 底部操作区 (常规屏幕备用) */}
          <div className="flex items-center justify-between pt-2 mt-auto border-t border-white/5">
            <button
              type="button"
              onClick={() => {
                setInputVal('');
                setRefreshVal('');
                onSave('', '');
                onClose();
              }}
              className="text-xs text-red-400/80 hover:text-red-400 transition-colors"
            >
              清除凭证
            </button>

            <div className="flex items-center gap-2">
              <button
                type="button"
                onClick={onClose}
                className="px-3 py-1.5 rounded-lg bg-white/10 text-xs text-white/70 hover:bg-white/20 transition-colors"
              >
                取消
              </button>
              <button
                type="submit"
                className="px-4 py-1.5 rounded-lg bg-emeraldGlow text-black font-semibold text-xs hover:bg-emeraldGlow/90 transition-all shadow-[0_0_12px_rgba(16,185,129,0.4)]"
              >
                保存并生效
              </button>
            </div>
          </div>
        </form>
      </div>
    </div>
  );
};
