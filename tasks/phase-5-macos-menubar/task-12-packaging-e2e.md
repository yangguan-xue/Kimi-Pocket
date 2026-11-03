# Task 12: macOS 端一键构建、打包脚本与端到端验收 (Build & Release Packaging)

## 1. 任务元信息
- **任务编号**：`Task 12`
- **所属 Spec**：[SPEC-06-macos-menubar-app.md](../../specs/SPEC-06-macos-menubar-app.md)
- **研发阶段**：`Phase 5: macOS 桌面原生端体验与常驻扩展`
- **前置任务**：`Task 11`
- **状态**：`PENDING`

---

## 2. 交付物清单
- `scripts/build-macos.sh`：自动化编译与 App Bundle 组装脚本。
- `package.json`：补充 `npm run build:macos` 脚本指令。
- 最终产物：`dist-macos/Kimi Pocket.app`。

---

## 3. 技术约束与实现要点
1. **自动化 App 打包流程**：
   - 脚本执行 `swift build -c release`；
   - 自动生成符合 Apple 规范的 `.app` 目录结构（`Contents/MacOS/`, `Contents/Resources/`, `Contents/Info.plist`）；
   - 自动注入矢量 App 图标与 `LSUIElement=true` 声明。
2. **免签名本地运行体验**：
   - 自动执行本地 Ad-Hoc 代码签名（`codesign -s - --force --deep`），避免 macOS Gatekeeper 误报损坏，用户双击或拖入 `/Applications` 即可直接运行。
3. **资源占用验收**：
   - 使用 Activity Monitor 验证常驻内存 <= 25MB，后台闲置 CPU 0.0%。

---

## 4. 验收标准
- 运行 `npm run build:macos` 无报错，并输出 `dist-macos/Kimi Pocket.app`。
- 双击运行该 App：
  1. 顶栏出现 Kimi 图标，Dock 栏无图标；
  2. 鼠标悬停达 180ms 自动弹出精致毛玻璃 HUD 浮窗；
  3. 鼠标移开平滑淡出，点击左键支持钉住 (Pin)，点击右键支持退出与偏好设置；
  4. 界面文案纯净，无“7段”、“4段”等内部特征文字。
