# Kimi Pocket (kimi-pocket) — 工程规范与开发指南

## 1. 项目定位与目标
- **定位**：专为移动端（首期为 Android）打造的 Kimi Code 套餐用量与额度监控工具。
- **核心体验**：
  - **务实**：数据精准直达，彻底告别浏览器控制台繁琐翻查。
  - **高级与简约**：Obsidian Dark 深空黑冷调极简设计，磨砂质感卡片与冰蓝/冷翠呼吸光晕。
  - **横竖双形态响应式**：
    - **竖屏**：3 块垂直直排卡片（总用量 4 段分段条、周用量 7 段分段条、5小时连续单条）。
    - **横屏**：3 环 StandBy 仪表盘（总用量 4 弧段圆环、周用量 7 弧段圆环、5小时平滑单圆环），基于 SVG 严格数学等分。
  - **Android 沉浸式 PWA**：支持一键添加到安卓主屏幕，提供与原生 App 完全无异的全屏独立窗口、App 图标与离线可用能力。

---

## 2. 规范驱动开发 (SDD) 铁律
本项目严格遵循规范驱动开发原则：
1. **无 Spec 不拆 Task**：所有系统架构、接口契约与 UI/UX 规则必须先沉淀于 `specs/` 目录。
2. **无 Task 不写代码**：开发任务原子化，每个任务有明确的输入、交付物、技术约束与可执行的验证命令。
3. **真实性验证与自动化测试**：对数据解析与核心组件必须编写单元测试验证，段数与数学计算严格测试。
4. **Git 提交契约**：遵循 Conventional Commits 规范（`feat:`, `fix:`, `docs:`, `test:`, `refactor:`, `chore:`）。

---

## 3. 技术栈规范
- **开发框架**：React 19 + TypeScript (严格模式 `strict: true`, 零 `any`)
- **构建工具**：Vite 6.x
- **样式方案**：Tailwind CSS v3.4+
- **测试框架**：Vitest + React Testing Library
- **图标与字体**：Lucide React + 系统原生等宽字体栈 (SF Mono / JetBrains Mono / Roboto Mono)
- **PWA 插件**：`vite-plugin-pwa`（Workbox 缓存策略 + Web App Manifest）
