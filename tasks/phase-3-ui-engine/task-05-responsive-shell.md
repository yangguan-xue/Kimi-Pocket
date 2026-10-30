# Task 05: 整合响应式外壳、Token 输入与全局状态生命周期

## 1. 任务元信息
- **任务编号**：`Task 05`
- **所属 Spec**：[SPEC-01-architecture.md](../../specs/SPEC-01-architecture.md), [SPEC-03-ui-responsive.md](../../specs/SPEC-03-ui-responsive.md)
- **研发阶段**：`Phase 3: 响应式组件与视觉体系`
- **前置任务**：`Task 02`, `Task 03`, `Task 04`
- **状态**：`PENDING`

---

## 2. 交付物清单
- `src/hooks/useOrientation.ts`：横竖屏监听 Hook（双重判定：`matchMedia` 与窗口宽高比）。
- `src/hooks/useKimiUsage.ts`：数据获取、缓存读取、定时轮询、页面可见性管理 Hook。
- `src/components/common/Header.tsx`：状态指示灯、套餐标签、到期倒计时头部。
- `src/components/common/TokenModal.tsx`：极简 Token 录入与保存对话框。
- `src/App.tsx`：顶层调度容器。

---

## 3. 验收标准
- 浏览器在宽屏/横屏模式下渲染 `LandscapeHUD`，竖屏模式下渲染 `PortraitView`。
- 未输入 Token 时弹出友好的引导输入框；输入后自动刷新并持久化。
