# Task 14: 原生托盘图标与退出闭环

## 1. 任务元信息

- **任务编号**：`Task 14`
- **所属 Spec**：[SPEC-07](../../specs/SPEC-07-windows-tray-app.md)
- **视觉基准**：[已确认示意图与对照规则](../../specs/assets/windows-tray/README.md)
- **研发阶段**：`Phase 6: Windows 原生托盘端`
- **前置任务**：Task 13
- **状态**：`IMPLEMENTED`（2026-11-04；见本任务证据及阶段验收矩阵）
- **范围**：M
- **验收映射**：W01、W09 的图标部分

## 2. 输入与交付

**输入**：SPEC-07 §4.2、§6；使用真实 Windows 通知区域验证。

**交付**：注册 Win32 托盘图标、消息宿主及基础右键退出菜单，处理版本 4 回调编码。

交付文件与相关伴随文件如下；模型与系统适配在实现时按职责合并，实际目录以工程为准：

- `windows/src/KimiPocket.Windows/Tray/TrayIconHost.cs`
- `windows/src/KimiPocket.Windows/Interop/Native.cs`
- `windows/src/KimiPocket.App/App.xaml.cs`
- `windows/src/KimiPocket.App/Assets/Tray/`
- `windows/tests/KimiPocket.Windows.Tests/TrayCallbackTests.cs`

## 3. 技术约束

NIM_ADD 后设置 NOTIFYICON_VERSION_4；独立释放 HICON 与窗口句柄；不依赖 Shell 自动反色；不启用普通 Tooltip。首期图标使用简洁单色 Kimi Pocket 形状。

## 4. 验收标准

- [ ] 启动只出现一个图标，无主窗口和任务栏按钮；深/浅任务栏下可辨认。
- [ ] 正确区分鼠标、键盘、右键回调及坐标/图标 ID，不把版本 4 消息当旧式编码。
- [ ] 右键退出真正终止进程并删除图标；退出路径释放句柄，重复启动问题由 Task 24 补齐。

## 5. 验证命令

在仓库根目录运行；测试筛选对应的测试类必须存在且执行非零用例。UI 能力仍需下列实机检查，不以纯逻辑测试替代。

```powershell
dotnet test .\windows\tests\KimiPocket.Windows.Tests --configuration Release --filter FullyQualifiedName~TrayCallbackTests
dotnet run --project .\windows\src\KimiPocket.App
```

**实机 / 人工验证**：实机分别点击、右键、退出，检查任务管理器与通知区域；保存深/浅任务栏截图。

## 6. 完成证据

消息解码测试、托盘截图、退出后的进程/图标记录。 执行完成后在阶段 evidence 目录记录结果并更新两个任务索引。任务不得仅因代码已写或命令入口已存在而标为完成。

## 7. 本轮实施记录（2026-11-04）

Shell_NotifyIcon v4、图标资源、回调与退出释放已实现；原生托盘注册/GetRect 测试通过。

详见 [构建与证据索引](evidence/README.md) 和 [W01–W14 实测边界](evidence/acceptance.md)。未勾选项仍有待测条件；`IMPLEMENTED` 不代表完整验收通过。
