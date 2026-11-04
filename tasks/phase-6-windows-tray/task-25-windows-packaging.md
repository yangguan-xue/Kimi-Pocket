# Task 25: 自包含 ZIP 构建与使用说明

## 1. 任务元信息

- **任务编号**：`Task 25`
- **所属 Spec**：[SPEC-07](../../specs/SPEC-07-windows-tray-app.md)
- **视觉基准**：[已确认示意图与对照规则](../../specs/assets/windows-tray/README.md)
- **研发阶段**：`Phase 6: Windows 原生托盘端`
- **前置任务**：Task 23、Task 24
- **状态**：`IMPLEMENTED`（2026-11-04；见本任务证据及阶段验收矩阵）
- **范围**：M
- **验收映射**：W12

## 2. 输入与交付

**输入**：SPEC-07 §10；完整可运行 Windows 应用及已有测试。

**交付**：一键校验/发布/压缩脚本、版本化 x64 ZIP、Windows 用户使用说明。

交付文件与相关伴随文件如下；模型与系统适配在实现时按职责合并，实际目录以工程为准：

- `scripts/build-windows.ps1`
- `windows/src/KimiPocket.App/KimiPocket.App.csproj`
- `windows/README.md`
- `README.md`
- `.gitignore`

## 3. 技术约束

自包含 win-x64，PublishTrimmed=false，不强推单 EXE；脚本先检查依赖、测试、编译、格式再打包，失败即非零退出。脚本只处理已验证在 dist-windows 内的输出路径。无需管理员、无控制台；不自动上传发布。

## 4. 验收标准

- [ ] PowerShell 命令生成 Kimi-Pocket-windows-x64-<version>.zip，压缩内容足以解压运行，不带凭据、fixture、个人配置。
- [ ] 无 SDK/Runtime 的干净 Windows 11 x64 环境可运行，入口/图标/版本信息正确。
- [ ] 使用文档覆盖 Token 配置、托盘溢出区、开机启动、退出与便携路径变更，命令与产物一致。

## 5. 验证命令

在仓库根目录运行；测试筛选对应的测试类必须存在且执行非零用例。UI 能力仍需下列实机检查，不以纯逻辑测试替代。

```powershell
dotnet format .\windows\KimiPocket.sln --verify-no-changes --no-restore
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\build-windows.ps1
```

**实机 / 人工验证**：干净用户环境中解压 ZIP 执行 KimiPocket.exe；检查不安装运行时与管理员请求，核对 ZIP 内容。

## 6. 完成证据

打包日志、ZIP SHA-256、版本、干净环境运行记录。 执行完成后在阶段 evidence 目录记录结果并更新两个任务索引。任务不得仅因代码已写或命令入口已存在而标为完成。

## 7. 本轮实施记录（2026-11-04）

构建脚本已生成 0.1.0 自包含 ZIP；解压 EXE 和包内运行时检查通过，干净系统待测。

详见 [构建与证据索引](evidence/README.md) 和 [W01–W14 实测边界](evidence/acceptance.md)。未勾选项仍有待测条件；`IMPLEMENTED` 不代表完整验收通过。
