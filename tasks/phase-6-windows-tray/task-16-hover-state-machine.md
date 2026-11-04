# Task 16: 悬浮展开与离开收起闭环

## 1. 任务元信息

- **任务编号**：`Task 16`
- **所属 Spec**：[SPEC-07](../../specs/SPEC-07-windows-tray-app.md)
- **视觉基准**：[已确认示意图与对照规则](../../specs/assets/windows-tray/README.md)
- **研发阶段**：`Phase 6: Windows 原生托盘端`
- **前置任务**：Task 14、Task 15
- **状态**：`IMPLEMENTED`（2026-11-04；见本任务证据及阶段验收矩阵）
- **范围**：M
- **验收映射**：W02、W03

## 2. 输入与交付

**输入**：SPEC-07 §4；托盘回调入口、已还原的静态面板。

**交付**：可控时钟下的纯悬浮状态机，并接入托盘与无边框不激活浮窗。

交付文件与相关伴随文件如下；模型与系统适配在实现时按职责合并，实际目录以工程为准：

- `windows/src/KimiPocket.Core/Interaction/HoverCoordinator.cs`
- `windows/src/KimiPocket.App/Views/PanelWindow.cs`
- `windows/src/KimiPocket.App/TrayController.cs`
- `windows/src/KimiPocket.App/App.xaml.cs`
- `windows/tests/KimiPocket.Core.Tests/HoverCoordinatorTests.cs`

## 3. 技术约束

180 ms 展开、300 ms 关闭；取消/代次隔离旧回调；主动隐藏后必须退出图标才能再次武装。只在候选/可见期间探测指针，待机停止；不加全局 Hook。

## 4. 验收标准

- [ ] 悬浮、移入面板、跨越间隙、离开、左键切换、右键接管符合状态表，不闪退或自动重开。
- [ ] 编辑器持续输入时打开面板，前台句柄及输入位置不变；关闭不吞掉面板外点击。
- [ ] 测试覆盖 179/180、299/300 ms、过期回调、菜单抑制与快速反复进入，隐藏后无活动指针轮询。

## 5. 验证命令

在仓库根目录运行；测试筛选对应的测试类必须存在且执行非零用例。UI 能力仍需下列实机检查，不以纯逻辑测试替代。

```powershell
dotnet test .\windows\tests\KimiPocket.Core.Tests --configuration Release --filter FullyQualifiedName~HoverCoordinatorTests
dotnet run --project .\windows\src\KimiPocket.App -- --design-preview --tray
```

**实机 / 人工验证**：设计预览的 --tray 变体挂载真实托盘，仅使用固定数据；实机记录从图标移入面板及编辑器输入不中断。

## 6. 完成证据

状态机测试、悬浮短录屏、焦点与计时器记录。 执行完成后在阶段 evidence 目录记录结果并更新两个任务索引。任务不得仅因代码已写或命令入口已存在而标为完成。

## 7. 本轮实施记录（2026-11-04）

180/300 ms 状态机、有限探测、点击抑制已接入；纯状态机测试通过，真实指针矩阵待测。

详见 [构建与证据索引](evidence/README.md) 和 [W01–W14 实测边界](evidence/acceptance.md)。未勾选项仍有待测条件；`IMPLEMENTED` 不代表完整验收通过。
