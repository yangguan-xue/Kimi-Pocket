# Task 17: 多屏定位与任务栏溢出区适配

## 1. 任务元信息

- **任务编号**：`Task 17`
- **所属 Spec**：[SPEC-07](../../specs/SPEC-07-windows-tray-app.md)
- **视觉基准**：[已确认示意图与对照规则](../../specs/assets/windows-tray/README.md)
- **研发阶段**：`Phase 6: Windows 原生托盘端`
- **前置任务**：Task 16
- **状态**：`IMPLEMENTED`（2026-11-04；见本任务证据及阶段验收矩阵）
- **范围**：M
- **验收映射**：W07、W08

## 2. 输入与交付

**输入**：SPEC-07 §5；真实图标矩形和已可悬浮的面板。

**交付**：图标锚定、坐标转换、工作区钳制和显示器/DPI 变化后的重新定位。

交付文件与相关伴随文件如下；模型与系统适配在实现时按职责合并，实际目录以工程为准：

- `windows/src/KimiPocket.Windows/Positioning/PopupPlacement.cs`
- `windows/src/KimiPocket.Windows/Interop/Native.cs`
- `windows/src/KimiPocket.App/Views/PanelWindow.cs`
- `windows/src/KimiPocket.App/app.manifest`
- `windows/tests/KimiPocket.Windows.Tests/PopupPlacementTests.cs`

## 3. 技术约束

使用 Shell_NotifyIconGetRect，明确物理坐标/DIP 边界，支持负坐标；不得硬编码主屏右下角。图标不可见不悬浮打开；自动隐藏任务栏不抢前台。

## 4. 验收标准

- [ ] 100/125/150/200% DPI、负坐标与不同工作区下不越界，图标锚点正确，热插拔后重算。
- [ ] 溢出区与自动隐藏任务栏中跨区移动可操作，图标消失时停止无效展开，定位失败不跳到其他屏幕。
- [ ] 几何测试覆盖边缘与可用宽高不足；实机能力不足的组合明确记录待测，不冒充通过。

## 5. 验证命令

在仓库根目录运行；测试筛选对应的测试类必须存在且执行非零用例。UI 能力仍需下列实机检查，不以纯逻辑测试替代。

```powershell
dotnet test .\windows\tests\KimiPocket.Windows.Tests --configuration Release --filter FullyQualifiedName~PopupPlacementTests
dotnet run --project .\windows\src\KimiPocket.App -- --design-preview --tray
```

**实机 / 人工验证**：执行混合 DPI 双屏、负坐标屏、溢出区和自动隐藏任务栏矩阵；截图标注系统缩放。

## 6. 完成证据

几何测试、显示器布局与各任务栏模式实测记录。 执行完成后在阶段 evidence 目录记录结果并更新两个任务索引。任务不得仅因代码已写或命令入口已存在而标为完成。

## 7. 本轮实施记录（2026-11-04）

物理像素定位、工作区钳制、PMv2 已实现；几何测试通过，多屏/溢出区实测待补。

详见 [构建与证据索引](evidence/README.md) 和 [W01–W14 实测边界](evidence/acceptance.md)。未勾选项仍有待测条件；`IMPLEMENTED` 不代表完整验收通过。
