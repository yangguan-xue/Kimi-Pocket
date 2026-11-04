# Task 23: 键盘入口与可访问操作

## 1. 任务元信息

- **任务编号**：`Task 23`
- **所属 Spec**：[SPEC-07](../../specs/SPEC-07-windows-tray-app.md)
- **视觉基准**：[已确认示意图与对照规则](../../specs/assets/windows-tray/README.md)
- **研发阶段**：`Phase 6: Windows 原生托盘端`
- **前置任务**：Task 22
- **状态**：`IMPLEMENTED`（2026-11-04；见本任务证据及阶段验收矩阵）
- **范围**：M
- **验收映射**：W03、W09、W10

## 2. 输入与交付

**输入**：SPEC-07 §4.3、§3.2；可操作的真实面板与设置窗口。

**交付**：系统托盘键盘入口、焦点恢复、AutomationProperties、Tab 顺序与高对比度适配。

交付文件与相关伴随文件如下；模型与系统适配在实现时按职责合并，实际目录以工程为准：

- `windows/src/KimiPocket.App/Views/PanelWindow.cs`
- `windows/src/KimiPocket.App/Views/UsagePanel.xaml`
- `windows/src/KimiPocket.App/Views/PreferencesWindow.xaml`
- `windows/src/KimiPocket.App/Resources/Theme.xaml`
- `windows/tests/KimiPocket.Windows.Tests/NativeIntegrationTests.cs`

## 3. 技术约束

鼠标悬浮仍不激活；键盘明确进入时才激活，鼠标在外不误关；不全局拦截 Esc。减少动画时直接显隐，纯色表面不做透明度动画。

## 4. 验收标准

- [ ] 从系统通知区域 Enter/Space 打开，Tab 遍历刷新/设置，Esc 关闭已激活窗口并合理恢复焦点。
- [ ] 屏幕阅读器可读标签、剩余百分比、状态和按钮名称；高对比度/文字放大仍可读且操作不截断。
- [ ] 鼠标与键盘模式切换、失焦关闭有测试；W03 的不抢输入焦点实机回归通过。

## 5. 验证命令

在仓库根目录运行；测试筛选对应的测试类必须存在且执行非零用例。UI 能力仍需下列实机检查，不以纯逻辑测试替代。

```powershell
dotnet test .\windows\tests\KimiPocket.Windows.Tests --configuration Release --filter FullyQualifiedName~NativeIntegrationTests
dotnet run --project .\windows\src\KimiPocket.App -- --design-preview --tray
```

**实机 / 人工验证**：使用键盘、Narrator、高对比度、系统减少动画和文字放大验证；检查 28 DIP 按钮命中区。

## 6. 完成证据

激活策略测试、键盘流程记录、可访问性实测。 执行完成后在阶段 evidence 目录记录结果并更新两个任务索引。任务不得仅因代码已写或命令入口已存在而标为完成。

## 7. 本轮实施记录（2026-11-04）

键盘入口、焦点模式、UIA、灰阶焦点/高对比度/字号适配已实现；真实键盘与 Narrator 待测。

详见 [构建与证据索引](evidence/README.md) 和 [W01–W14 实测边界](evidence/acceptance.md)。未勾选项仍有待测条件；`IMPLEMENTED` 不代表完整验收通过。
