# Task 19: 直连接口与独立分区错误处理

## 1. 任务元信息

- **任务编号**：`Task 19`
- **所属 Spec**：[SPEC-07](../../specs/SPEC-07-windows-tray-app.md)
- **视觉基准**：[已确认示意图与对照规则](../../specs/assets/windows-tray/README.md)
- **研发阶段**：`Phase 6: Windows 原生托盘端`
- **前置任务**：Task 18
- **状态**：`IMPLEMENTED`（2026-11-04；见本任务证据及阶段验收矩阵）
- **范围**：M
- **验收映射**：W06 的网络前置

## 2. 输入与交付

**输入**：SPEC-02 请求头/请求体、SPEC-07 §7；归一化模型。

**交付**：可注入 HttpMessageHandler 的 HttpClient 客户端、请求取消、整体超时和错误分类。

交付文件与相关伴随文件如下；模型与系统适配在实现时按职责合并，实际目录以工程为准：

- `windows/src/KimiPocket.Core/Api/KimiUsageClient.cs`
- `windows/src/KimiPocket.Core/Usage/Models.cs`
- `windows/src/KimiPocket.Core/Api/KimiUsageClient.cs`
- `windows/tests/KimiPocket.Core.Tests/KimiUsageClientTests.cs`

## 3. 技术约束

只直连指定 HTTPS 基地址，无中转；一轮 15 秒超时；401/403/429/网络/解析错误区分，保留 Retry-After。日志不含认证头、响应正文或个人标识。

## 4. 验收标准

- [ ] 替身验证两个 POST 的完整路径、请求体与公共请求头；正常和单接口失败可得到独立分区结果。
- [ ] 取消、整体超时、401、403、429 与非法 JSON 被正确分类，取消不降格为伪造成功。
- [ ] 测试只用合成凭证，不访问真实账号，不把自动重试写入传输层造成多层重试。

## 5. 验证命令

在仓库根目录运行；测试筛选对应的测试类必须存在且执行非零用例。UI 能力仍需下列实机检查，不以纯逻辑测试替代。

```powershell
dotnet test .\windows\tests\KimiPocket.Core.Tests --configuration Release --filter FullyQualifiedName~KimiUsageClientTests
```

**实机 / 人工验证**：检查 HttpClient 生命周期、取消传播和脱敏日志；真实联调在 Task 20 通过用户本机输入凭证进行。

## 6. 完成证据

传输契约与错误分类测试。 执行完成后在阶段 evidence 目录记录结果并更新两个任务索引。任务不得仅因代码已写或命令入口已存在而标为完成。

## 7. 本轮实施记录（2026-11-04）

双接口独立请求、头部、限流/认证/解析分类已实现；传输替身测试通过。

详见 [构建与证据索引](evidence/README.md) 和 [W01–W14 实测边界](evidence/acceptance.md)。未勾选项仍有待测条件；`IMPLEMENTED` 不代表完整验收通过。
