# Task 20: 凭据配置与首次真实额度验证

## 1. 任务元信息

- **任务编号**：`Task 20`
- **所属 Spec**：[SPEC-07](../../specs/SPEC-07-windows-tray-app.md)
- **视觉基准**：[已确认示意图与对照规则](../../specs/assets/windows-tray/README.md)
- **研发阶段**：`Phase 6: Windows 原生托盘端`
- **前置任务**：Task 15、Task 19
- **状态**：`IMPLEMENTED`（2026-11-04；见本任务证据及阶段验收矩阵）
- **范围**：M；聚焦凭证设置，开机启动由 Task 24 接入。
- **验收映射**：W06、W13 的凭据部分

## 2. 输入与交付

**输入**：SPEC-07 §8、§9.3；已实现网络客户端和灰阶样式。

**交付**：Credential Manager 适配、受保护的 Token 输入与首次设置流程，完成保存验证/替换/删除闭环。

交付文件与相关伴随文件如下；模型与系统适配在实现时按职责合并，实际目录以工程为准：

- `windows/src/KimiPocket.Windows/Security/CredentialManager.cs`
- `windows/src/KimiPocket.App/Views/PreferencesWindow.xaml 与其 ViewModel`
- `windows/src/KimiPocket.App/App.xaml.cs`
- `windows/tests/KimiPocket.Windows.Tests/NativeIntegrationTests.cs`
- `windows/tests/KimiPocket.Core.Tests/RefreshCoordinatorTests.cs`

## 3. 技术约束

Generic Credential 目标 KimiPocket/AccessToken；候选先验证再提交，失败保留旧凭证；存储失败绝不落明文。预览模式禁止写凭证；测试使用独立目标并清理。

## 4. 验收标准

- [ ] 首次无凭证打开设置一次，关闭后托盘继续常驻；输入默认隐藏，保存结果准确且可继续编辑。
- [ ] 真实成功响应可在内存验证为快照，失败替换不破坏旧凭证；删除清空数据并使旧请求失效。
- [ ] 存取/替换/删除、超长输入与系统写入失败均验证；配置文件、日志和截图无敏感内容。

## 5. 验证命令

在仓库根目录运行；测试筛选对应的测试类必须存在且执行非零用例。UI 能力仍需下列实机检查，不以纯逻辑测试替代。

```powershell
dotnet test .\windows\tests\KimiPocket.Windows.Tests --configuration Release --filter FullyQualifiedName~NativeIntegrationTests
dotnet test .\windows\tests\KimiPocket.Core.Tests --configuration Release --filter FullyQualifiedName~RefreshCoordinatorTests
dotnet run --project .\windows\src\KimiPocket.App
```

**实机 / 人工验证**：用户在本机界面输入真实 Token 做一次联调；不通过聊天索取或打印 Token。联调证据只记结果与脱敏状态。

## 6. 完成证据

凭据适配测试、替换失败测试和脱敏首次验证记录。 执行完成后在阶段 evidence 目录记录结果并更新两个任务索引。任务不得仅因代码已写或命令入口已存在而标为完成。

## 7. 本轮实施记录（2026-11-04）

设置、显式在线验证后保存、删除、Credential Manager 已实现；隔离系统存取测试通过，真实账号待测。

详见 [构建与证据索引](evidence/README.md) 和 [W01–W14 实测边界](evidence/acceptance.md)。未勾选项仍有待测条件；`IMPLEMENTED` 不代表完整验收通过。
