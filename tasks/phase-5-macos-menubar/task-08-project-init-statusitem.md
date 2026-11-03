# Task 08: 初始化 macOS 原生工程骨架与状态栏控制器 (SPM + NSStatusItem)

## 1. 任务元信息
- **任务编号**：`Task 08`
- **所属 Spec**：[SPEC-06-macos-menubar-app.md](../../specs/SPEC-06-macos-menubar-app.md)
- **研发阶段**：`Phase 5: macOS 桌面原生端体验与常驻扩展`
- **前置任务**：无（基于已评审的 SPEC-06 启动）
- **状态**：`DONE` (已通过 SPM 编译与单测)

---

## 2. 交付物清单
- `macos/Package.swift`：定义 Swift Package Manager 项目结构、依赖与构建目标（最低支持 macOS 13.0）。
- `macos/Sources/KimiPocketApp.swift`：主应用入口（SwiftUI App 协议与 `NSApplicationDelegateAdaptor`）。
- `macos/Sources/MenuBar/StatusItemController.swift`：负责创建和管理系统菜单栏 `NSStatusItem`，设置单色 Template 图标。
- `macos/Resources/Info.plist`：配置 `LSUIElement = true`（确保不在 Dock 栏显示，也不在 `⌘Tab` 切换器中出现）。

---

## 3. 技术约束与实现要点
1. **轻量零依赖**：除系统 AppKit / SwiftUI 外，不引入庞大第三方依赖，保持构建极简高效。
2. **顶栏单色适配**：菜单栏图标配置 `isTemplate = true`，自动反色适配系统深浅模式与动态壁纸。
3. **后台纯常驻**：严格设置 `LSUIElement`，确保启动后仅在顶栏显示图标。

---

## 4. 验收标准
- 进入 `macos/` 目录执行 `swift build` 零编译警告和报错。
- 启动可执行文件后，macOS 顶栏成功常驻应用图标，Dock 栏无残留图标。
