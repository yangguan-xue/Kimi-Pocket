# Task 26: 视觉回归与首版完整验收

## 1. 任务元信息

- **任务编号**：`Task 26`
- **所属 Spec**：[SPEC-07](../../specs/SPEC-07-windows-tray-app.md)
- **视觉基准**：[已确认示意图与对照规则](../../specs/assets/windows-tray/README.md)
- **研发阶段**：`Phase 6: Windows 原生托盘端`
- **前置任务**：Task 25
- **状态**：`IN_PROGRESS`（2026-11-04；见本任务证据及阶段验收矩阵）
- **范围**：M；仅验收与证据整理，修复回归原任务。
- **验收映射**：W01–W14 汇总

## 2. 输入与交付

**输入**：Task 13–25 的测试和实测证据、两张固定基准图、SPEC-07 W01–W14。

**交付**：首版验收记录、正常/异常截图、性能与资源记录、遗留项清单。

交付文件与相关伴随文件如下；模型与系统适配在实现时按职责合并，实际目录以工程为准：

- `tasks/phase-6-windows-tray/evidence/acceptance.md`
- `tasks/phase-6-windows-tray/evidence/screenshots/`
- `tasks/phase-6-windows-tray/evidence/performance.md`
- `tasks/phase-6-windows-tray/README.md`
- `tasks/README.md`

## 3. 技术约束

本任务不塞入未拆分的新功能；发现缺陷回到所属任务修正并针对性复验。截图脱敏且不覆盖批准基准；缺设备或未测组合保持待验收，不把模拟测试当实机。

## 4. 验收标准

- [ ] W01–W14 逐项关联测试/实测证据，所有必要实测通过；未完成项明确阻断对应完成状态。
- [ ] 100%/150% 正常态与基准对照、异常态与高对比度截图齐全，无未经确认的视觉偏移。
- [ ] Release 下记录 60 秒 CPU、稳定 5 分钟资源、100 次开合、启动和悬浮响应；超标问题明确处理。

## 5. 验证命令

在仓库根目录运行；测试筛选对应的测试类必须存在且执行非零用例。UI 能力仍需下列实机检查，不以纯逻辑测试替代。

```powershell
dotnet build .\windows\KimiPocket.sln --configuration Release -warnaserror
dotnet test .\windows\KimiPocket.sln --configuration Release --no-build --logger "console;verbosity=normal"
dotnet format .\windows\KimiPocket.sln --verify-no-changes --no-restore
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\build-windows.ps1
```

**实机 / 人工验证**：执行 SPEC-07 的完整矩阵；每项记录系统、DPI、版本与结果。测试通过不等同用户已验收，状态分别填写。

## 6. 完成证据

验收矩阵、最终构建产物标识、截图、性能报告与遗留项。 执行完成后在阶段 evidence 目录记录结果并更新两个任务索引。任务不得仅因代码已写或命令入口已存在而标为完成。

## 7. 本轮实施记录（2026-11-04）

75 项测试、12 张截图、资源基线及逐项矩阵已整理；完整系统交互与性能验收仍在进行。

详见 [构建与证据索引](evidence/README.md) 和 [W01–W14 实测边界](evidence/acceptance.md)。未勾选项仍有待测条件；`IMPLEMENTED` 不代表完整验收通过。
