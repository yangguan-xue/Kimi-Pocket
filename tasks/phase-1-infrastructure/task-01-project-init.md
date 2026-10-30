# Task 01: 初始化 Vite + React 19 + TypeScript + TailwindCSS + Vitest 工程骨架

## 1. 任务元信息
- **任务编号**：`Task 01`
- **所属 Spec**：[SPEC-01-architecture.md](../../specs/SPEC-01-architecture.md)
- **研发阶段**：`Phase 1: 基础设施搭建`
- **前置任务**：无（首置任务）
- **状态**：`READY` (就绪待开工)

---

## 2. 交付物清单
- `package.json`：配置 React 19、TypeScript 5.x、TailwindCSS、Lucide React、Vitest。
- `tsconfig.json`：启用严格模式 (`strict: true`)。
- `vite.config.ts`：集成 Tailwind、配置 `/apiv2` 代理至 `https://www.kimi.com`。
- `tailwind.config.js`：定义 Obsidian Dark 调色板及发光滤镜扩展。

---

## 3. 验收标准
- 执行 `npm install` 依赖无冲突安装。
- 执行 `npm run test` Vitest 单元测试运行通过。
- 执行 `npm run build` 成功输出 `dist/`，零 TypeScript 类型报错。
