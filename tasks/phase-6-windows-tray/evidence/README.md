# Windows 首版实现与验证记录

日期：2026-11-04。当前可交付 **0.1.0 Windows 预览包**；Task 13–25 已有实现，Task 26 持续验收。已通过的自动化测试和本机验证见下；完整 W01–W14 尚未全部通过，用户尚未对实际应用作最终验收。

- [逐项验收与待测场景](acceptance.md)
- [性能和资源记录](performance.md)
- [代码复核记录](code-review.md)
- [结构化构建结果](build-results.json)
- [正常态 1×](screenshots/normal-100.png)、[1.5×](screenshots/normal-150.png)、[2×](screenshots/normal-200.png)
- [10 种状态总览](screenshots/states-contact-sheet.png)
- [正常态批准基准](../../../specs/assets/windows-tray/windows-tray-panel.png)

## 已执行

环境为 Windows 11 Pro 10.0.26200 / x64，SDK 10.0.403、.NET 运行时 10.0.14。执行：

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\build-windows.ps1
```

脚本完成 NuGet 还原、格式检查、Release 编译（零警告、零错误）、75 项测试（55 Core + 20 Windows，零失败、零跳过）、win-x64 自包含发布与 ZIP 压缩。Core 架构测试验证不引用 WPF/Windows 适配程序集。

原始 TRX 在本地 `dist-windows/build-b2d933c0d97f4a91ba7a0d141b17c20c/test-results/`，不纳入版本库；计数与原生资源结果保存在 build-results.json。测试使用合成凭证，真实 Credential Manager 集成测试使用独立随机目标并清理。

解压实际 ZIP 后，直接运行包内 EXE，10 种预览状态与正常态两种附加倍率共 12 次捕获均以退出码 0 结束。检查进程模块确认 `coreclr.dll` 来自解压目录；未使用系统运行时路径。该结果证明本机解压包可用，尚不等同于干净 Windows 用户环境通过。

## 产物

- 本地文件：`dist-windows/Kimi-Pocket-windows-x64-0.1.0.zip`
- ZIP：63,232,595 字节，约 60.30 MiB；解压后 413 个文件。
- SHA256：`2BE2769D947BCCE2E742C8129CF5E1CDA15E47E36BB1BAF869DA8749C861DB8A`
- 解压后入口：`Kimi-Pocket/KimiPocket.exe`，运行所需 DLL 和 Assets 应保留。
- 未签名的预览包；发布标签为 [`windows-v0.1.0`](https://github.com/yangguan-xue/Kimi-Pocket/releases/tag/windows-v0.1.0)，安装与版本范围见 [发布说明](../../../releases/windows-v0.1.0.md)。具体本地路径见 `dist-windows/latest-build.json` 和 `dist-windows/release-smoke.json`。

## 截图口径

截图来自打包后的真实 WPF 视图，使用 `--design-preview` 固定合成数据。正常态为 360×376 DIP；主数字 68%，周额度 58%，总额度 74%，分别为连续条、7 段和 4 段。截图已与批准 PNG 并排检查，主体布局、灰阶及无玻璃效果一致；尚未完成逐像素 ≤2 DIP 的自动差分测量。

`--capture-scale 1/1.5/2` 是离屏栅格渲染倍率，不代表操作系统已切换到对应 DPI；系统混合 DPI 验收仍待执行。截图不包含真实账号信息。

```powershell
# 在仓库根目录执行
$exe = (Get-Content .\dist-windows\latest-build.json | ConvertFrom-Json).executable
& $exe --design-preview --preview-state normal --capture D:/Temp/kimi-normal.png --capture-scale 1.5
```

状态包括 normal、offline、expired、forbidden、rate-limited、missing、low、empty、unconfigured、loading。设计预览不联网、不读写产品凭据、不修改启动项。
