# Task 22: 真实数据面板与异常状态闭环

## 1. 任务元信息

- **任务编号**：`Task 22`
- **所属 Spec**：[SPEC-07](../../specs/SPEC-07-windows-tray-app.md)
- **视觉基准**：[已确认示意图与对照规则](../../specs/assets/windows-tray/README.md)
- **研发阶段**：`Phase 6: Windows 原生托盘端`
- **前置任务**：Task 15、Task 16、Task 17、Task 20、Task 21
- **状态**：`IMPLEMENTED`（2026-11-04；见本任务证据及阶段验收矩阵）
- **范围**：M
- **验收映射**：W04–W06

## 2. 输入与交付

**输入**：已还原面板、真实 UsageStore、SPEC-07 §6–8。

**交付**：连接真实数据与刷新/设置动作，完成加载、旧缓存、认证失败、低额度及分区不可用状态。

交付文件与相关伴随文件如下；模型与系统适配在实现时按职责合并，实际目录以工程为准：

- `windows/src/KimiPocket.App/ViewModels/UsagePanelViewModel.cs`
- `windows/src/KimiPocket.App/Views/UsagePanel.xaml`
- `windows/src/KimiPocket.App/App.xaml.cs`
- `windows/src/KimiPocket.App/Design/UsagePanelPreview.cs`
- `windows/tests/KimiPocket.Windows.Tests/PanelRenderingTests.cs`

## 3. 技术约束

真实运行与设计演示隔离；不默认 Kimi Pro，不捏造额度；刷新不销毁已有布局；时间只在可见时更新。错误文案不泄露实现细节，状态扩高不遮按钮。

## 4. 验收标准

- [ ] 真实账号的套餐及三项余额展示正确，刷新/设置可用；正常态仍与已确认图一致。
- [ ] 未配置、加载、离线有/无缓存、401、403、429、解析异常、过期缓存、低额度和精确耗尽均可复现且语义明确。
- [ ] 设计预览新增 --preview-state 参数复现状态矩阵；删除/替换 Token 后不闪现旧账号，隐藏时停止 UI 秒级计时。

## 5. 验证命令

在仓库根目录运行；测试筛选对应的测试类必须存在且执行非零用例。UI 能力仍需下列实机检查，不以纯逻辑测试替代。

```powershell
dotnet test .\windows\tests\KimiPocket.Windows.Tests --configuration Release --filter FullyQualifiedName~PanelRenderingTests
dotnet run --project .\windows\src\KimiPocket.App -- --design-preview --preview-state offline
dotnet run --project .\windows\src\KimiPocket.App -- --design-preview
```

**实机 / 人工验证**：保存 normal、unconfigured、loading、offline、unauthorized、forbidden、rate-limited、unavailable、exhausted 预览截图；再核对本机真实数据。

## 6. 完成证据

状态映射测试、全部状态截图与脱敏真实联调记录。 执行完成后在阶段 evidence 目录记录结果并更新两个任务索引。任务不得仅因代码已写或命令入口已存在而标为完成。

## 7. 本轮实施记录（2026-11-04）

各分区更新时间、旧数据/认证/限流/缺值文案已接入；10 状态真实 WPF 布局与捕获通过。

详见 [构建与证据索引](evidence/README.md) 和 [W01–W14 实测边界](evidence/acceptance.md)。未勾选项仍有待测条件；`IMPLEMENTED` 不代表完整验收通过。
