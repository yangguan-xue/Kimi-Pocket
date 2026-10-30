# Task 06: 配置 Android PWA Web App Manifest、图标与屏幕常亮特性

## 1. 任务元信息
- **任务编号**：`Task 06`
- **所属 Spec**：[SPEC-04-android-pwa.md](../../specs/SPEC-04-android-pwa.md)
- **研发阶段**：`Phase 4: 安卓独立端体验与封装`
- **前置任务**：`Task 05`
- **状态**：`PENDING`

---

## 2. 交付物清单
- `public/manifest.webmanifest`：配置 `display: standalone`、主题色 `#0B0C10`。
- `public/icons/`：生成标准 192x192 与 512x512 PNG 图标。
- `src/hooks/useWakeLock.ts`：横屏 StandBy 待机模式下的屏幕防息屏 Hook。
- `index.html`：注入 mobile web meta 标签与 PWA 引导。

---

## 3. 验收标准
- 使用 Lighthouse 或 Chrome DevTools 检查 PWA 审计项无缺失。
- 安卓浏览器访问页面能触发「安装应用 / 添加到主屏幕」。
- 独立打开应用时无浏览器地址栏，全屏黑底沉浸运行。
