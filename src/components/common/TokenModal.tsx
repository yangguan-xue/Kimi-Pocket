import React, { useState } from 'react';
import { X, Key, ShieldCheck } from 'lucide-react';

interface TokenModalProps {
  isOpen: boolean;
  currentToken: string;
  onClose: () => void;
  onSave: (token: string) => void;
}

export const TokenModal: React.FC<TokenModalProps> = ({
  isOpen,
  currentToken,
  onClose,
  onSave,
}) => {
  const [inputVal, setInputVal] = useState(currentToken);

  if (!isOpen) return null;

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSave(inputVal);
    onClose();
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/70 backdrop-blur-md animate-fade-in">
      <div className="bg-[#12141A] border border-white/10 rounded-2xl w-full max-w-md p-5 shadow-2xl text-white">
        <div className="flex items-center justify-between mb-4">
          <div className="flex items-center gap-2">
            <Key className="w-4 h-4 text-emeraldGlow" />
            <h2 className="text-sm font-semibold">配置 Kimi 浏览器 Token</h2>
          </div>
          <button
            onClick={onClose}
            className="p-1 rounded-lg hover:bg-white/10 text-white/50 hover:text-white transition-colors"
          >
            <X className="w-4 h-4" />
          </button>
        </div>

        <form onSubmit={handleSubmit} className="flex flex-col gap-3.5">
          <p className="text-xs text-white/60 leading-relaxed">
            请从浏览器控制台 (<code className="text-emeraldGlow">kimi.com/code/console</code>) 的
            LocalStorage 中复制 <code className="text-white/80">access_token</code> 并粘贴至下方：
          </p>

          <textarea
            value={inputVal}
            onChange={(e) => setInputVal(e.target.value)}
            rows={4}
            placeholder="粘贴 eyJhbGciOiJIUzI1NiJ9... 格式的 access_token"
            className="w-full p-2.5 rounded-xl bg-white/5 border border-white/10 text-xs font-mono text-white placeholder:text-white/20 focus:outline-none focus:border-emeraldGlow/50 transition-colors resize-none"
          />

          <div className="flex items-center gap-1.5 text-[11px] text-white/40">
            <ShieldCheck className="w-3.5 h-3.5 text-emeraldGlow" />
            <span>Token 仅保存在本机 LocalStorage，绝不上传任何第三方。</span>
          </div>

          <div className="flex items-center justify-between pt-2">
            <button
              type="button"
              onClick={() => {
                setInputVal('');
                onSave('');
                onClose();
              }}
              className="text-xs text-red-400/80 hover:text-red-400 transition-colors"
            >
              清除 Token (切换演示)
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
