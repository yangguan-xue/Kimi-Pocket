# Task 24: 单实例与系统生命周期恢复

## 1. 任务元信息

- **任务编号**：`Task 24`
- **所属 Spec**：[SPEC-07](../../specs/SPEC-07-windows-tray-app.md)
- **视觉基准**：[已确认示意图与对照规则](../../specs/assets/windows-tray/README.md)
- **研发阶段**：`Phase 6: Windows 原生托盘端`
- **前置任务**：Task 14、Task 20、Task 21、Task 22
- **状态**：`IMPLEMENTED`（2026-11-04；见本任务证据及阶段验收矩阵）
- **范围**：M
- **验收映射**：W11、W13

## 2. 输入与交付

**输入**：SPEC-07 §5、§7.2、§9.3；托盘与刷新生命周期入口。

**交付**：单实例、Explorer 恢复、休眠/锁屏/网络事件接线、当前用户开机启动设置。

交付文件与相关伴随文件如下；模型与系统适配在实现时按职责合并，实际目录以工程为准：

- `windows/src/KimiPocket.Windows/Lifecycle/SingleInstance.cs`
- `windows/src/KimiPocket.Windows/Lifecycle/StartupPreference.cs`
- `windows/src/KimiPocket.App/App.xaml.cs`
- `windows/src/KimiPocket.App/Views/PreferencesWindow.xaml 与其 ViewModel`
- `windows/tests/KimiPocket.Windows.Tests/NativeIntegrationTests.cs`

## 3. 技术约束

普通启动默认不开机自启；用户明确开启才写当前用户配置。路径正确引用且移动后可修复；预览实例不注册启动项。系统恢复不主动展开面板，不清理用户其他启动项。

## 4. 验收标准

- [ ] 重复启动不重复图标；TaskbarCreated 后恢复一个图标，全部退出路径释放非托管资源与计时器。
- [ ] 锁屏/休眠隐藏并暂停，唤醒/网络恢复合并一次刷新；候选悬浮不因晚到事件重新弹出。
- [ ] 开机启动开/关、路径含空格、便携目录移动均处理；关闭设置仍常驻，退出后无残留进程。

## 5. 验证命令

在仓库根目录运行；测试筛选对应的测试类必须存在且执行非零用例。UI 能力仍需下列实机检查，不以纯逻辑测试替代。

```powershell
dotnet test .\windows\tests\KimiPocket.Windows.Tests --configuration Release --filter FullyQualifiedName~NativeIntegrationTests
dotnet run --project .\windows\src\KimiPocket.App
```

**实机 / 人工验证**：在可控桌面会话中手动验证 Explorer 重启和睡眠恢复，不擅自终止用户当前 Explorer；启动项与目录移动使用临时测试目录。

## 6. 完成证据

生命周期测试、单实例/系统恢复/启动项验证记录。 执行完成后在阶段 evidence 目录记录结果并更新两个任务索引。任务不得仅因代码已写或命令入口已存在而标为完成。

## 7. 本轮实施记录（2026-11-04）

单实例、当前用户启动项、恢复/暂停与退出释放已接入；真实 Mutex/管道测试通过，系统恢复实测待补。

详见 [构建与证据索引](evidence/README.md) 和 [W01–W14 实测边界](evidence/acceptance.md)。未勾选项仍有待测条件；`IMPLEMENTED` 不代表完整验收通过。
