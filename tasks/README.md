# Kimi Pocket: 任务注册表与规范驱动开发 (SDD) 进度看板

本项目严格遵循 **规范驱动开发 (Spec-Driven Development)**：
> **无 Spec 不拆 Task，无 Task 不写代码，每一步均有可执行验收证据与 Git 提交。**

---

## 任务索引与状态总览

| 任务编号 | 任务名称 | 所属 Spec | 核心产出 | 状态 |
|---|---|---|---|:---:|
| **[Task 01](./phase-1-infrastructure/task-01-project-init.md)** | 初始化 Vite + React 19 + TypeScript + TailwindCSS + Vitest 工程骨架 | SPEC-01 | `package.json`, `vite.config.ts`, `tailwind.config.js` | ✅ 已验收 |
| **[Task 02](./phase-2-api-contracts/task-02-api-client.md)** | 实现 Kimi API 客户端、JWT 解析与开发期反向代理 | SPEC-02 | `src/api/`, `src/types/`, 单元测试套件 | ✅ 已验收 (8单测Pass) |
| **[Task 03](./phase-3-ui-engine/task-03-portrait-cards.md)** | 构建竖屏模式 3 卡片组件 (4段 / 7段 / 连续平滑条) | SPEC-03 | `src/components/portrait/` 及其单测 | ✅ 已验收 (3单测Pass) |
| **[Task 04](./phase-3-ui-engine/task-04-landscape-hud.md)** | 构建横屏 StandBy 3 环仪表盘组件 (SVG 4弧 / 7弧 / 连续环) | SPEC-03 | `src/components/landscape/` 及其单测 | ✅ 已验收 (4单测Pass) |
| **[Task 05](./phase-3-ui-engine/task-05-responsive-shell.md)** | 整合响应式外壳、Token 输入与全局状态生命周期 | SPEC-01, 03 | `src/App.tsx`, `src/hooks/useKimiUsage.ts` | ✅ 已验收 |
| **[Task 06](./phase-4-android-pwa/task-06-pwa-android.md)** | 配置 Android PWA Web App Manifest、图标与屏幕常亮特性 | SPEC-04 | `manifest.webmanifest`, Service Worker, PWA 图标 | ✅ 已验收 (Build输出SW) |
| **[Task 07](./phase-4-android-pwa/task-07-capacitor-apk.md)** | 集成 Capacitor 原生 Android 工程与 GitHub Actions 自动打包流水线 | SPEC-05 | `android/`, `build-apk.yml`, `CapacitorHttp` | ✅ 已配置并就绪 |
| **[Task 08](./phase-5-macos-menubar/task-08-project-init-statusitem.md)** | 初始化 macOS 原生工程骨架与状态栏控制器 (SPM + NSStatusItem) | SPEC-06 | `macos/Package.swift`, `StatusItemController` | ✅ 已验收 |
| **[Task 09](./phase-5-macos-menubar/task-09-hover-popover-window.md)** | 悬停防抖状态机与毛玻璃无边框浮窗 (HoverCoordinator + PopoverWindow) | SPEC-06 | `HoverCoordinator.swift`, `PopoverWindow.swift` | ✅ 已验收 |
| **[Task 10](./phase-5-macos-menubar/task-10-core-data-keychain.md)** | 原生 Swift 数据核心与 Keychain 安全存储 (KimiClient + Keychain + Store) | SPEC-02, 06 | `KimiClient.swift`, `UsageStore.swift`, 单元测试 | ✅ 已验收 |
| **[Task 11](./phase-5-macos-menubar/task-11-hud-views-preferences.md)** | 构建 Obsidian Dark 纯净版悬停 HUD 与偏好设置面板 (HoverHUDView + Preferences) | SPEC-06 | `HoverHUDView.swift`, 纯净文案卡片组件 | ✅ 已验收 |
| **[Task 12](./phase-5-macos-menubar/task-12-packaging-e2e.md)** | macOS 端一键构建、打包脚本与端到端验收 (Build & Release Packaging) | SPEC-06 | `build-macos.sh`, `Kimi Pocket.app` | ✅ 已验收 |

---

## 阶段验收汇总证据 (Evidence)
- **编译与打包 (Android)**：`npm run build:android` 零类型报错，构建成功输出 `dist/` 并自动同步到 `android/` 原生目录。
- **自动化测试 (Web & Core)**：`npm test` 全量通过 18 项单元测试；`swift run TestRunner` 全量通过状态机、JWT、KimiClient 归一化与 Keychain 单元测试。
- **编译与打包 (macOS)**：`npm run build:macos` 单命令通过 Release 编译、组装 App Bundle、Ad-Hoc 签名并输出 `dist-macos/Kimi Pocket.app`。
- **CI/CD 流水线**：推送即可由 GitHub 官方云端自动编译出 `Kimi-Pocket-latest.apk`。
