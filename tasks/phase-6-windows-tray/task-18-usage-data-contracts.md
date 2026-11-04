# Task 18: 真实额度模型与响应归一化

## 1. 任务元信息

- **任务编号**：`Task 18`
- **所属 Spec**：[SPEC-07](../../specs/SPEC-07-windows-tray-app.md)
- **视觉基准**：[已确认示意图与对照规则](../../specs/assets/windows-tray/README.md)
- **研发阶段**：`Phase 6: Windows 原生托盘端`
- **前置任务**：Task 13
- **状态**：`IMPLEMENTED`（2026-11-04；见本任务证据及阶段验收矩阵）
- **范围**：M
- **验收映射**：W04、W06 的数据前置

## 2. 输入与交付

**输入**：SPEC-02、SPEC-07 §7.1；现有 Web/Swift 解析仅作字段参考，不沿用其虚构默认值。

**交付**：不可变额度快照、独立分区状态、JWT 预检与可控时间下的响应归一化。

交付文件与相关伴随文件如下；模型与系统适配在实现时按职责合并，实际目录以工程为准：

- `windows/src/KimiPocket.Core/Usage/Models.cs`
- `windows/src/KimiPocket.Core/Usage/UsageNormalizer.cs`
- `windows/src/KimiPocket.Core/Auth/JwtPrecheck.cs`
- `windows/tests/KimiPocket.Core.Tests/UsageNormalizerTests.cs`
- `windows/tests/KimiPocket.Core.Tests/fixtures/`

## 3. 技术约束

5 小时仅匹配 300 分钟或 5 小时；缺失与真实 0 分离；异常上限/剩余明确不可用。套餐、周、5 小时不互相替代；凭证 Payload 不视为授权。

## 4. 验收标准

- [ ] 脱敏 fixture 可得到正确套餐与三类额度、各分区时间和失败分类；整数/字符串与非法值按契约处理。
- [ ] JWT 格式和过期边界可测试；时间缺失/过去重置时间不把额度改为满值。
- [ ] 未知窗口、缺字段、负值、剩余超限、上限为 0 均有测试，生产没有演示余额兜底。

## 5. 验证命令

在仓库根目录运行；测试筛选对应的测试类必须存在且执行非零用例。UI 能力仍需下列实机检查，不以纯逻辑测试替代。

```powershell
dotnet test .\windows\tests\KimiPocket.Core.Tests --configuration Release --filter FullyQualifiedName~UsageNormalizerTests
```

**实机 / 人工验证**：人工检查 fixture 无真实 Token 或个人标识，并与 SPEC-02 字段对应关系核对。

## 6. 完成证据

归一化与 JWT 测试、脱敏 fixture 说明。 执行完成后在阶段 evidence 目录记录结果并更新两个任务索引。任务不得仅因代码已写或命令入口已存在而标为完成。

## 7. 本轮实施记录（2026-11-04）

Models、UsageNormalizer、JwtPrecheck 与合成 fixture 已落地，缺值/边界测试通过。

详见 [构建与证据索引](evidence/README.md) 和 [W01–W14 实测边界](evidence/acceptance.md)。未勾选项仍有待测条件；`IMPLEMENTED` 不代表完整验收通过。
