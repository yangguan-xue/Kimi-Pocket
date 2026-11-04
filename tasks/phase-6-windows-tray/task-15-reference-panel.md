# Task 15: 按已确认示意图还原额度面板

## 1. 任务元信息

- **任务编号**：`Task 15`
- **所属 Spec**：[SPEC-07](../../specs/SPEC-07-windows-tray-app.md)
- **视觉基准**：[已确认示意图与对照规则](../../specs/assets/windows-tray/README.md)
- **研发阶段**：`Phase 6: Windows 原生托盘端`
- **前置任务**：Task 13
- **状态**：`IMPLEMENTED`（2026-11-04；见本任务证据及阶段验收矩阵）
- **范围**：M；样式与面板作为一个可直接预览的交付单元。
- **验收映射**：W04、W05

## 2. 输入与交付

**输入**：已确认两张 PNG、视觉基准 README、SPEC-07 §3；演示值必须与参考图一致。

**交付**：可单独运行的设计预览面板、灰阶资源、主额度和 4/7 段组件，以及分段数学测试。

交付文件与相关伴随文件如下；模型与系统适配在实现时按职责合并，实际目录以工程为准：

- `windows/src/KimiPocket.App/Views/UsagePanel.xaml 与其必要的代码后置`
- `windows/src/KimiPocket.App/Resources/Theme.xaml`
- `windows/src/KimiPocket.App/Design/UsagePanelPreview.cs`
- `windows/src/KimiPocket.Core/Presentation/QuotaPresentation.cs`
- `windows/tests/KimiPocket.Core.Tests/QuotaPresentationTests.cs`

## 3. 技术约束

提供 --design-preview 入口，使用固定脱敏数据；预览模式禁用网络、凭据写入与启动项操作。360 × 376 DIP、不透明 #141414、52 DIP 主数字、320 × 5 DIP 条；图标按钮有 28 DIP 命中区。

## 4. 验收标准

- [ ] 正常态与参考图的排版、颜色、比例及留白一致，主要几何误差 ≤2 DIP，不出现毛玻璃、蓝色、光晕或嵌套卡片。
- [ ] 连续条、7 段、4 段均为剩余语义，支持部分填充及 <1% / >99% 端点表达，数学测试覆盖无效值与边界。
- [ ] 可运行设计预览并保存 100% 与 150% 缩放截图；参考图不被测试输出覆盖。

## 5. 验证命令

在仓库根目录运行；测试筛选对应的测试类必须存在且执行非零用例。UI 能力仍需下列实机检查，不以纯逻辑测试替代。

```powershell
dotnet test .\windows\tests\KimiPocket.Core.Tests --configuration Release --filter FullyQualifiedName~QuotaPresentationTests
dotnet run --project .\windows\src\KimiPocket.App -- --design-preview
```

**实机 / 人工验证**：以视觉基准中规定的演示值截图，按 DIP 对齐后并排/叠加对照；检查纯色背景和数字基线。

## 6. 完成证据

设计预览截图、视觉差异说明、段条数学测试。 执行完成后在阶段 evidence 目录记录结果并更新两个任务索引。任务不得仅因代码已写或命令入口已存在而标为完成。

## 7. 本轮实施记录（2026-11-04）

360×376 DIP 正常态、灰阶主题与 1/7/4 段已落地；真实 WPF 布局和截图已验证。

详见 [构建与证据索引](evidence/README.md) 和 [W01–W14 实测边界](evidence/acceptance.md)。未勾选项仍有待测条件；`IMPLEMENTED` 不代表完整验收通过。
