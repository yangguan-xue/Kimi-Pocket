# Task 21: 去重刷新与恢复调度

## 1. 任务元信息

- **任务编号**：`Task 21`
- **所属 Spec**：[SPEC-07](../../specs/SPEC-07-windows-tray-app.md)
- **视觉基准**：[已确认示意图与对照规则](../../specs/assets/windows-tray/README.md)
- **研发阶段**：`Phase 6: Windows 原生托盘端`
- **前置任务**：Task 19、Task 20
- **状态**：`IMPLEMENTED`（2026-11-04；见本任务证据及阶段验收矩阵）
- **范围**：M
- **验收映射**：W06、W11 的调度部分

## 2. 输入与交付

**输入**：SPEC-07 §7.2；客户端、凭据代次和内存快照。

**交付**：单一刷新协调器、可控时钟/调度接口及分区更新时间管理。

交付文件与相关伴随文件如下；模型与系统适配在实现时按职责合并，实际目录以工程为准：

- `windows/src/KimiPocket.Core/Usage/UsageStore.cs`
- `windows/src/KimiPocket.Core/Usage/RefreshCoordinator.cs`
- `windows/src/KimiPocket.Core/Usage/RefreshCoordinator.cs`
- `windows/tests/KimiPocket.Core.Tests/RefreshCoordinatorTests.cs`

## 3. 技术约束

60/30 秒周期、15 秒悬浮阈值、60/120/300 秒退避；429 不被手动绕过，401 停止，403 不误报过期。内存缓存，不落原始响应；旧凭证结果不得晚到覆盖。

## 4. 验收标准

- [ ] 启动/手动/悬浮刷新共享在途请求，成功/失败/部分成功维持各自真实时间，不清除其他分区过期标记。
- [ ] 替换/删除、取消、限流、倒计时归零与睡眠/网络恢复事件均去重；无定时请求积压。
- [ ] 可控时钟测试覆盖阈值和竞态，不依赖真实等待；普通后台刷新不触发整窗重绘闪烁。

## 5. 验证命令

在仓库根目录运行；测试筛选对应的测试类必须存在且执行非零用例。UI 能力仍需下列实机检查，不以纯逻辑测试替代。

```powershell
dotnet test .\windows\tests\KimiPocket.Core.Tests --configuration Release --filter FullyQualifiedName~RefreshCoordinatorTests
```

**实机 / 人工验证**：用替身统计请求次数，确认手动与悬浮同时触发只发一轮；生命周期系统事件接线由 Task 24 完成。

## 6. 完成证据

调度边界、代次隔离、限流与重复事件测试。 执行完成后在阶段 evidence 目录记录结果并更新两个任务索引。任务不得仅因代码已写或命令入口已存在而标为完成。

## 7. 本轮实施记录（2026-11-04）

周期/缓存、请求合并、取消、退避与代次隔离已实现；调度与竞态测试通过。

详见 [构建与证据索引](evidence/README.md) 和 [W01–W14 实测边界](evidence/acceptance.md)。未勾选项仍有待测条件；`IMPLEMENTED` 不代表完整验收通过。
