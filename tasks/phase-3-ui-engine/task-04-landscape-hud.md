# Task 04: 构建横屏 StandBy 3 环仪表盘组件 (SVG 4弧 / 7弧 / 连续环)

## 1. 任务元信息
- **任务编号**：`Task 04`
- **所属 Spec**：[SPEC-03-ui-responsive.md](../../specs/SPEC-03-ui-responsive.md)
- **研发阶段**：`Phase 3: 响应式组件与视觉体系`
- **前置任务**：`Task 01`
- **状态**：`PENDING`

---

## 2. 交付物清单
- `src/components/landscape/SegmentedRing.tsx`：通用的精确 SVG 分段弧度渲染器。
- `src/components/landscape/ContinuousRing.tsx`：通用的精确 SVG 连续平滑环渲染器。
- `src/components/landscape/LandscapeHUD.tsx`：横向并排三环仪表盘（左 4 弧、中 7 弧、右连续环）。
- `src/components/landscape/__tests__/LandscapeRings.test.tsx`：单测验证分段弧计算数学精度与 SVG 属性。

---

## 3. 验收标准
- 单元测试验证：
  - 左环分段数参数为 4，SVG 中存在严格 4 段背景与高亮弧度；
  - 中环分段数参数为 7，SVG 中存在严格 7 段背景与高亮弧度；
  - 右环无分段 gap，连续百分比渲染正确。
