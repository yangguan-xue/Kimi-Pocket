# Task 03: 构建竖屏模式 3 卡片组件 (4段 / 7段 / 连续平滑条)

## 1. 任务元信息
- **任务编号**：`Task 03`
- **所属 Spec**：[SPEC-03-ui-responsive.md](../../specs/SPEC-03-ui-responsive.md)
- **研发阶段**：`Phase 3: 响应式组件与视觉体系`
- **前置任务**：`Task 01`
- **状态**：`PENDING`

---

## 2. 交付物清单
- `src/components/portrait/TotalQuotaCard.tsx`：4 段离散水平条组件。
- `src/components/portrait/WeeklyQuotaCard.tsx`：7 段离散水平条组件。
- `src/components/portrait/RollingQuotaCard.tsx`：5小时连续单条组件。
- `src/components/portrait/PortraitView.tsx`：垂直排版容器。
- `src/components/portrait/__tests__/PortraitCards.test.tsx`：测试段数渲染与不同比例下的点亮逻辑。

---

## 3. 验收标准
- 单元测试验证：
  - `TotalQuotaCard` DOM 中必定存在严格 4 个段容器。
  - `WeeklyQuotaCard` DOM 中必定存在严格 7 个段容器。
  - `RollingQuotaCard` 为单条连续结构。
