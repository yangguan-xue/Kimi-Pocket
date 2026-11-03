# Task 11: 构建 Obsidian Dark 纯净版悬停 HUD 与偏好设置面板 (HoverHUDView + PreferencesView)

## 1. 任务元信息
- **任务编号**：`Task 11`
- **所属 Spec**：[SPEC-06-macos-menubar-app.md](../../specs/SPEC-06-macos-menubar-app.md)
- **研发阶段**：`Phase 5: macOS 桌面原生端体验与常驻扩展`
- **前置任务**：`Task 09`, `Task 10`
- **状态**：`DONE` (已通过全套纯净文案 HUD 与偏好设置面板编译与集成)

---

## 2. 交付物清单
- `macos/Sources/Views/HoverHUDView.swift`：HUD 浮窗主视图（顶部套餐行、三大用量卡片、底部同步与钉住操作栏）。
- `macos/Sources/Views/Components/RollingQuotaRow.swift`：5小时额度单条平滑渐变进度条（冰蓝到冷翠渐变与呼吸光晕）。
- `macos/Sources/Views/Components/SegmentedQuotaRow.swift`：7段/4段离散药丸进度条通用组件。
- `macos/Sources/Views/PreferencesView.swift`：设置面板（Token 输入、掩码校验、开机自启动开关）。

---

## 3. 技术约束与文案纯净铁律
1. **文案去工程化铁律 (Zero-Engineering Jargon)**：
   - 严禁在界面文案中出现“7段节奏”、“4段生命周期”、“(连续平滑)”等研发工程标签；
   - 标题纯净表达：`5小时额度`、`本周用量`、`总用量`；
   - 辅助文字纯业务表达：`01:24:30 后开始恢复`、`X 天后重置`、`当前周期有效`；
   - 分段意图 100% 由几何组件本身的视觉形态（7 个药丸块、4 个药丸块、平滑胶囊）直观表达。
2. **视觉设计质感**：
   - 遵循 Obsidian Dark 调色体系（背景微透暗曜黑、边框 1px 细微光、等宽数字字体）；
   - 5小时额度紧张时（<20%）平滑变更为琥珀警示色。
3. **开机自启动**：
   - 使用 macOS 13+ 原生 `SMAppService.mainApp.register()` 与 `unregister()` 控制开机自启。

---

## 4. 验收标准
- 浮窗在暗色模式下完美贴合系统顶栏，排版与高保真效果图完全一致；
- 界面中文案干净纯粹，无任何“X段/平滑”字样；
- 设置面板中更新 Token 后，浮窗即刻同步刷新并持久化到钥匙串。
