# Task 09: 悬停防抖状态机与毛玻璃无边框浮窗 (HoverCoordinator + PopoverWindow)

## 1. 任务元信息
- **任务编号**：`Task 09`
- **所属 Spec**：[SPEC-06-macos-menubar-app.md](../../specs/SPEC-06-macos-menubar-app.md)
- **研发阶段**：`Phase 5: macOS 桌面原生端体验与常驻扩展`
- **前置任务**：`Task 08`
- **状态**：`DONE` (已通过 180ms/250ms 状态机自动化单测与 NSPanel 编译)

---

## 2. 交付物清单
- `macos/Sources/MenuBar/HoverCoordinator.swift`：实现 180ms 激活防抖与 250ms 退出容错状态机。
- `macos/Sources/MenuBar/PopoverWindow.swift`：基于 `NSPanel` 构建毛玻璃（`.ultraThinMaterial`）无边框浮窗，具备 X 轴居中对齐与屏幕边界自适应。
- `macos/Sources/MenuBar/StatusItemController.swift`：挂载 `NSTrackingArea` 监听 `mouseEntered` 与 `mouseExited`，绑定左右键交互。

---

## 3. 技术约束与实现要点
1. **防抖与防误触**：
   - 鼠标悬停未达到 `180ms`（例如快速滑过顶栏）严禁弹出浮窗；
   - 鼠标离开图标后提供 `250ms` 逃逸缓冲区，光标平滑进入下方浮窗时保持显示；
   - 离开图标与浮窗超过 `250ms` 执行 `150ms` 平滑淡出。
2. **窗口属性规范**：
   - 使用 `NSPanel`，设置样式掩码 `.nonactivatingPanel`，避免抢占当前前台应用的输入焦点；
   - 背景配置 `NSVisualEffectView` 暗色毛玻璃，边框 `1px rgba(255, 255, 255, 0.12)`。
3. **按键与右键菜单**：
   - 支持左键点击固定 (Pin)；
   - 支持右键点击弹出原生上下文菜单（`立即刷新`、`开机启动`、`设置 Token`、`退出`）；
   - 支持按 `ESC` 或点击浮窗外部关闭未固定的浮窗。

---

## 4. 验收标准
- 鼠标快速划过菜单栏时浮窗无闪现；
- 停留超过 180ms 稳定展开，移动光标进入浮窗内容区不闪退；
- 右键点击图标可呼出包含“退出”的原生菜单。
