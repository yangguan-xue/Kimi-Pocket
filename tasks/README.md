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

---

## 阶段验收汇总证据 (Evidence)
- **编译与打包**：`npm run build` 零类型报错，构建成功输出 `dist/`，含 PWA Service Worker。
- **自动化测试**：`npm test` 全量通过 15 项单元测试（涵盖 JWT 解析、数据归一化映射、竖屏 4/7 段与单条、横屏 4/7 弧与连续环）。
