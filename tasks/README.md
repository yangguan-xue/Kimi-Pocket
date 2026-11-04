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
| **[Task 13](./phase-6-windows-tray/task-13-project-bootstrap.md)** | Windows 工程与可验证构建入口 | SPEC-07 | 见任务交付物与验证命令 | ✅ 工程验证通过 |
| **[Task 14](./phase-6-windows-tray/task-14-native-tray-host.md)** | 原生托盘图标与退出闭环 | SPEC-07 | 见任务交付物与验证命令 | 已实现，待完整验收 |
| **[Task 15](./phase-6-windows-tray/task-15-reference-panel.md)** | 按已确认示意图还原额度面板 | SPEC-07 | 见任务交付物与验证命令 | 已实现，待完整验收 |
| **[Task 16](./phase-6-windows-tray/task-16-hover-state-machine.md)** | 悬浮展开与离开收起闭环 | SPEC-07 | 见任务交付物与验证命令 | 已实现，待完整验收 |
| **[Task 17](./phase-6-windows-tray/task-17-dpi-popup-placement.md)** | 多屏定位与任务栏溢出区适配 | SPEC-07 | 见任务交付物与验证命令 | 已实现，待完整验收 |
| **[Task 18](./phase-6-windows-tray/task-18-usage-data-contracts.md)** | 真实额度模型与响应归一化 | SPEC-07 | 见任务交付物与验证命令 | 已实现，待完整验收 |
| **[Task 19](./phase-6-windows-tray/task-19-http-usage-client.md)** | 直连接口与独立分区错误处理 | SPEC-07 | 见任务交付物与验证命令 | 已实现，待完整验收 |
| **[Task 20](./phase-6-windows-tray/task-20-token-preferences.md)** | 凭据配置与首次真实额度验证 | SPEC-07 | 见任务交付物与验证命令 | 已实现，待完整验收 |
| **[Task 21](./phase-6-windows-tray/task-21-refresh-scheduler.md)** | 去重刷新与恢复调度 | SPEC-07 | 见任务交付物与验证命令 | 已实现，待完整验收 |
| **[Task 22](./phase-6-windows-tray/task-22-live-panel-states.md)** | 真实数据面板与异常状态闭环 | SPEC-07 | 见任务交付物与验证命令 | 已实现，待完整验收 |
| **[Task 23](./phase-6-windows-tray/task-23-keyboard-accessibility.md)** | 键盘入口与可访问操作 | SPEC-07 | 见任务交付物与验证命令 | 已实现，待完整验收 |
| **[Task 24](./phase-6-windows-tray/task-24-app-lifecycle.md)** | 单实例与系统生命周期恢复 | SPEC-07 | 见任务交付物与验证命令 | 已实现，待完整验收 |
| **[Task 25](./phase-6-windows-tray/task-25-windows-packaging.md)** | 自包含 ZIP 构建与使用说明 | SPEC-07 | 见任务交付物与验证命令 | 已实现，待完整验收 |
| **[Task 26](./phase-6-windows-tray/task-26-release-acceptance.md)** | 视觉回归与首版完整验收 | SPEC-07 | 见任务交付物与验证命令 | 🔄 验收进行中 |

---

## 阶段验收汇总证据 (Evidence)
- **编译与打包 (Android)**：`npm run build:android` 零类型报错，构建成功输出 `dist/` 并自动同步到 `android/` 原生目录。
- **自动化测试 (Web & Core)**：`npm test` 全量通过 18 项单元测试；`swift run TestRunner` 全量通过状态机、JWT、KimiClient 归一化与 Keychain 单元测试。
- **编译与打包 (macOS)**：`npm run build:macos` 单命令通过 Release 编译、组装 App Bundle、Ad-Hoc 签名并输出 `dist-macos/Kimi Pocket.app`。
- **CI/CD 流水线**：推送即可由 GitHub 官方云端自动编译出 `Kimi-Pocket-latest.apk`。


## Windows 阶段计划（Phase 6）

- [任务计划、依赖与检查点](./phase-6-windows-tray/README.md)
- [SPEC-07：Windows 托盘应用](../specs/SPEC-07-windows-tray-app.md)
- [用户已确认的视觉基准](../specs/assets/windows-tray/README.md)
- **当前进度**：Windows 首版已实现并打包；Release 零警告、零错误，75 项测试通过，解压 EXE 本机启动通过。Task 26 保持验收进行中，真实账号与多屏/系统交互待测，详见 [实施证据](./phase-6-windows-tray/evidence/README.md)。
