# Task 13: Windows 工程与可验证构建入口

## 1. 任务元信息

- **任务编号**：`Task 13`
- **所属 Spec**：[SPEC-07](../../specs/SPEC-07-windows-tray-app.md)
- **视觉基准**：[已确认示意图与对照规则](../../specs/assets/windows-tray/README.md)
- **研发阶段**：`Phase 6: Windows 原生托盘端`
- **前置任务**：无（依赖已确认 SPEC-07）
- **状态**：`VERIFIED`（2026-11-04；见本任务证据及阶段验收矩阵）
- **范围**：M；骨架由工具生成，手写改动聚焦编译约束、启动策略和边界验证。
- **验收映射**：W01 的工程前置

## 2. 输入与交付

**输入**：SPEC-07 §9–10、当前仓库工程约定；本机 .NET SDK 状态需要在执行时核实。

**交付**：建立 Windows solution、WPF App、Core 与 Windows 适配项目、两个 xUnit 测试项目；固定 SDK 与公共编译约束。

交付文件与相关伴随文件如下；模型与系统适配在实现时按职责合并，实际目录以工程为准：

- `windows/KimiPocket.sln`
- `windows/global.json`
- `windows/Directory.Build.props`
- `windows/src/ 与 windows/tests/ 下的项目骨架`
- `.gitignore`

## 3. 技术约束

目录小写 kebab-case 用于阶段/功能目录，C# 项目名按类型命名；WinExe、nullable、警告作为错误；Core 不引用 WPF。仅搭最小启动入口，不加入业务占位成功逻辑。

## 4. 验收标准

- [x] 还原、Release 编译与测试入口可运行；至少有一项真实架构边界验证，不能靠空测试套件声称通过。
- [x] 启动不显示控制台；应用关闭策略适合托盘常驻，尚未完成的托盘能力不标为已实现。
- [x] SDK、生成目录忽略规则与命令均可在 Windows 11 x64 重现。

## 5. 验证命令

在仓库根目录运行；测试筛选对应的测试类必须存在且执行非零用例。UI 能力仍需下列实机检查，不以纯逻辑测试替代。

```powershell
dotnet restore .\windows\KimiPocket.sln
dotnet build .\windows\KimiPocket.sln --configuration Release --no-restore -warnaserror
dotnet test .\windows\KimiPocket.sln --configuration Release --no-build
```

**实机 / 人工验证**：记录实际 SDK、系统版本与启动表现；无 SDK 时先完成本任务前置环境，不改用其他 UI 技术。

## 6. 完成证据

构建、架构边界测试结果和 SDK 版本。 执行完成后在阶段 evidence 目录记录结果并更新两个任务索引。任务不得仅因代码已写或命令入口已存在而标为完成。

## 7. 本轮实施记录（2026-11-04）

建立五项目 solution、SDK 约束、官方 NuGet 配置；Release 构建与非零测试已通过。

详见 [构建与证据索引](evidence/README.md) 和 [W01–W14 实测边界](evidence/acceptance.md)。未勾选项仍有待测条件；`IMPLEMENTED` 不代表完整验收通过。
