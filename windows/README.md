# Kimi Pocket for Windows

Windows 11 x64 原生托盘应用。深色实色面板；悬浮显示 Kimi Code 5 小时、本周及套餐总额度，统一表示剩余比例。

## 下载包与运行

[下载 Windows v0.1.0 预览版](https://github.com/yangguan-xue/Kimi-Pocket/releases/tag/windows-v0.1.0)。

解压 `Kimi-Pocket-windows-x64-0.1.0.zip`，打开其中 `Kimi-Pocket/KimiPocket.exe`。需要保留同目录的 DLL 与 Assets 文件夹；包内自带 .NET 运行时，无需管理员安装。首次打开显示设置，以后常驻任务栏右下角通知区域。当前为未签名的 Windows 预览版。

首次使用：在 kimi.com 登录，从浏览器开发者工具的 Application / Local Storage 中复制完整 `access_token`，在设置中点击“保存并验证”。只有在线验证成功并写入 Windows 凭据管理器后才替换原凭据；关闭设置会取消尚未完成的验证。JWT 本地检查不代替服务器验证。

## 日常操作

- 鼠标停在托盘图标上约 180 毫秒展开，移出图标和面板约 300 毫秒收起。左键切换显示；右键菜单可刷新、设置、退出。
- 图标可能在系统托盘溢出区中，展开箭头查看，或手动将它拖到常驻通知区域。
- 键盘使用 Win+B 进入通知区域，再用方向键选择图标并按 Enter 打开；Tab 切换按钮，Esc 关闭面板。悬浮打开不会抢占前台窗口。
- 设置中的“登录 Windows 时启动”默认关闭。启用后将当前 EXE 路径写入当前用户启动项。移动解压目录后请重新启用一次，以更新路径。
- 保存后输入框清空；临时显示 Token 只在本次设置窗口中生效。删除凭据会立即清除内存中的额度数据。
- 低额度仍使用灰阶：5 小时剩余不超过 20% 提示“额度偏低”，不超过 5% 提示“即将用尽”，耗尽提示“当前额度已用尽”；本版不会自动变黄或变红。
- 未知额度显示 `—`；离线保留上次数据并标注未更新；401/过期停止请求，403 等待手动重试，429 等待服务端指定时间。
- 退出使用托盘右键菜单。再次启动已有实例会打开原实例的设置；`--show` 可打开键盘面板。

## 本地开发

安装 .NET 10 SDK（`global.json` 记录 10.0.403，允许最新补丁），在仓库根目录执行：

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\build-windows.ps1
# 可指定非系统 SDK：-DotnetPath C:\path\to\dotnet.exe
```

脚本执行还原、格式验证、Release 编译、非零测试、自包含 win-x64 发布与压缩；失败以非零退出，不上传。产物位于 `dist-windows/`，包括 ZIP、SHA256 和 `latest-build.json` 中的可执行文件路径。已有其他端的构建不受影响。

开发命令（在 `windows/` 下）：

```powershell
dotnet restore KimiPocket.sln --configfile NuGet.Config
dotnet test KimiPocket.sln --configuration Release
dotnet run --project src/KimiPocket.App -- --design-preview
dotnet run --project src/KimiPocket.App -- --design-preview --tray
dotnet run --project src/KimiPocket.App -- --design-preview --preview-state offline --capture D:/Temp/kimi-offline.png
```

设计预览不访问网络、不读写凭据、不改启动项。`--capture-scale 1|1.5|2` 控制离屏渲染倍率，不代表修改或验证了系统 DPI。可选状态：normal、offline、expired、forbidden、rate-limited、missing、low、empty、unconfigured、loading。

## 工程与验证

- Core：额度数学、数据解析、JWT 预检、HTTP 客户端、刷新调度、悬浮状态机。
- Windows：原生托盘、凭据管理器、工作区定位、启动项、单实例。
- App：WPF 面板、设置、状态与原生服务组合。
- 测试：Core 单元测试；Windows 原生集成与真实 WPF 布局测试。集成测试需要交互式 Windows 桌面与 Explorer，会使用临时测试凭据目标并自动清理。

视觉基准见 `specs/assets/windows-tray/`，规格为 `specs/SPEC-07-windows-tray-app.md`；本次实测与尚未完成的外部验收记录在 `tasks/phase-6-windows-tray/evidence/README.md`。真实账号联网、多屏混合 DPI、托盘溢出区及无运行时干净环境验收需按该记录继续。
