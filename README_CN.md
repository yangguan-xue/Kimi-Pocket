<div align="center">

# Kimi Pocket

<p><strong>把 Kimi Code 用量，装进口袋。</strong></p>
<p>专为极客与开发者打造的跨平台 Kimi Code 订阅与用量监控工具。<br/>原生支持 <strong>macOS 状态栏 HUD</strong> · <strong>Windows 任务栏托盘</strong> · <strong>Android 原生 App</strong> · <strong>PWA 沉浸式网页</strong>。</p>

<p>
  <a href="./README.md">English</a> ·
  <a href="./README_CN.md"><strong>简体中文</strong></a>
</p>

<p>
  <a href="https://github.com/yangguan-xue/Kimi-Pocket/releases/latest"><img src="https://img.shields.io/github/v/release/yangguan-xue/Kimi-Pocket?color=10B981&label=Release" alt="Release"/></a>
  <a href="https://github.com/yangguan-xue/Kimi-Pocket/actions"><img src="https://img.shields.io/github/actions/workflow/status/yangguan-xue/Kimi-Pocket/release.yml?branch=main&label=CI%20Build" alt="Build Status"/></a>
  <a href="LICENSE"><img src="https://img.shields.io/badge/License-MIT-blue.svg" alt="License: MIT"/></a>
  <img src="https://img.shields.io/badge/Platform-macOS%20%7C%20Windows%20%7C%20Android%20%7C%20PWA-262626" alt="Platforms"/>
  <img src="https://img.shields.io/badge/Privacy-Local--First%20%2F%20Zero--Relay-059669" alt="Zero Relay Security"/>
</p>

<p>
  <a href="#核心痛点与定位">核心定位</a> •
  <a href="#功能特性">功能特性</a> •
  <a href="#全端下载">全端下载</a> •
  <a href="#系统架构与隐私模型">系统架构</a> •
  <a href="#凭证配置指南">凭证配置</a> •
  <a href="#刷新与预警策略">刷新策略</a> •
  <a href="#本地开发与构建">本地开发</a> •
  <a href="#工程规范与致谢">工程规范</a>
</p>

</div>

---

## 核心痛点与定位

在重度使用 Kimi Code 进行工程编码时，开发者频繁需要关注三项核心额度指标：
- **5 小时连续滚动用量窗口**与重置倒计时
- **自然周配额**累计消耗与重置节点
- **套餐总额度**周期消耗与到期期限

传统方式需要频繁中断工作流，登录网页控制台或打开浏览器开发者工具抓包翻查。**Kimi Pocket** 旨在消除这一繁琐流程，将完整的额度状态以极低视觉负担呈现于系统原生状态栏、桌面托盘或移动端仪表盘中，实现**即指即现、零点击直达**。

---

## 功能特性

### 🍏 macOS 原生状态栏 HUD
- **即指即现 (Zero-Click HUD)**：鼠标悬停状态栏图标（>180ms 防误触）即刻平滑展开 Obsidian Dark 毛玻璃浮窗，鼠标移出即自动淡出隐退，彻底告别多余点击。
- **极致轻量**：采用 Swift 6 + SwiftUI + AppKit 原生构建，闲置状态 0% CPU 占用，内存消耗稳定在 25MB 以内。
- **硬件级凭证安全**：Token 与凭证直接对接 macOS Keychain（系统钥匙串）硬件加密存储，不写入普通明文文件。
- **动态状态指示**：菜单栏图标根据当前最低额度动态展示状态徽标与多级色彩预警。

### 🪟 Windows 原生任务栏托盘
- **沉浸克制设计**：基于 .NET + WPF 原生开发，采用高对比度黑灰白实色面板与系统原生排版，清晰不刺眼。
- **无感悬浮感知**：支持通知区域悬浮即开、移出自动收起；支持 `Win+B`、`Tab` 与 `Esc` 键盘无障碍完全控制。
- **独立免安装部署**：单文件自包含运行架构，无需预装系统级 .NET 运行时，解压即用。
- **安全凭证托管**：对接 Windows 凭据管理器（Credential Manager）加密持久化。

### 📱 Android 原生 App 与沉浸式 PWA
- **响应式横竖双形态**：
  - **竖屏模式**：三块垂直直排分段卡片（总额度 4 段条、周额度 7 段条、5小时平滑滚动条与重置倒计时）。
  - **横屏模式**：灵动 StandBy 仪表盘，基于严格 SVG 数学几何等分实现 4 弧段总用量环、7 弧段周用量环与 5 小时高精度单环。
- **移动端细节打磨**：支持移动端剪贴板一键读取粘贴 Token、软键盘弹起自适应避让布局、全屏独立窗口展示。
- **离线快照容错**：网络波动或短暂离线时保留最新有效快照，绝不白屏崩溃。

### ⚡ 核心监控引擎与刷新策略
- **自适应智能轮询**：根据三项指标中最低剩余比例动态调频（正常 60s / 紧张 30s / 告急 15s），页面/应用切入后台自动暂停以节省电量，切回前台立即触发刷新。
- **静默无感续期**：支持配置 `refresh_token`，在访问凭证（`access_token`）临近过期或鉴权失效时，自动在后台发起静默续期。
- **零中转隐私架构 (Zero-Relay)**：所有网络请求由客户端直连 Moonshot 官方 API，无任何第三方中间服务器，无使用日志收集。

---

## 全端下载

各平台安装包统一发布于 GitHub Releases，点击即可获取对应最新产物：

| 平台 | 形态 / 运行方式 | 架构与系统要求 | 下载链接 |
| :--- | :--- | :--- | :--- |
| 🍏 **macOS** | 原生状态栏 HUD 浮窗 (纯悬停呼出) | Apple Silicon (M1/M2/M3/M4), macOS 14+ | [**下载 DMG 镜像**](https://github.com/yangguan-xue/Kimi-Pocket/releases/latest) · [下载 ZIP 便携包](https://github.com/yangguan-xue/Kimi-Pocket/releases/latest) |
| 🪟 **Windows** | 原生任务栏托盘面板 (自带运行时) | Windows 11 x64 (免安装解压即用) | [**下载 Windows 便携包 (ZIP)**](https://github.com/yangguan-xue/Kimi-Pocket/releases/latest) |
| 📱 **Android** | 原生 App (横屏三环 HUD / 竖屏卡片) | Android 8.0+ | [**下载 Android APK**](https://github.com/yangguan-xue/Kimi-Pocket/releases/latest) |
| 🌐 **Web / PWA** | 浏览器运行或添加至主屏幕 | 现代现代移动/桌面浏览器 (Chrome / Safari / Edge) | [直接部署 / 源码构建](#本地开发与构建) |

> 校验和文件（SHA256）与完整历史更新日志详见 [GitHub Releases 页面](https://github.com/yangguan-xue/Kimi-Pocket/releases)。

---

## 系统架构与隐私模型

Kimi Pocket 严格遵循**本地优先 (Local-First)** 与**零中转 (Zero-Relay)** 的安全设计哲学：

```
┌────────────────────────────────────────────────────────┐
│             客户端 (macOS / Windows / Android)          │
│                                                        │
│  ┌──────────────────────────────────────────────────┐  │
│  │                    UI 渲染层                     │  │
│  │  - macOS: SwiftUI + AppKit                       │  │
│  │  - Windows: WPF + Win32 NotifyIcon               │  │
│  │  - Android / Web: React 19 + Tailwind CSS        │  │
│  └────────────────────────┬─────────────────────────┘  │
│                           │                            │
│                           ▼                            │
│  ┌──────────────────────────────────────────────────┐  │
│  │               系统原生安全凭据存储               │  │
│  │  - macOS Keychain / Windows Credential Manager   │  │
│  │  - Android EncryptedStorage / LocalStorage       │  │
│  └────────────────────────┬─────────────────────────┘  │
└───────────────────────────┼────────────────────────────┘
                            │ HTTPS 直连 (携带 Bearer Token)
                            ▼
┌────────────────────────────────────────────────────────┐
│              Moonshot Kimi 官方接口服务                │
│                                                        │
│  • GET  /apiv2/usages        (总用量、周用量、5小时窗口)│
│  • GET  /apiv2/subscription  (套餐级别、订阅有效期)   │
│  • POST /apiv2/token/refresh (凭证静默自动轮换续期)    │
└────────────────────────────────────────────────────────┘
```

- **凭证不出本地**：Token 仅在本地物理存储与 Moonshot 官方域名之间流动，绝对不会上传至任何中继服务器或统计平台。
- **直连无 CORS 限制**：桌面客户端采用原生系统网络栈（Swift `URLSession` / .NET `HttpClient`）发起请求；Web/PWA 端可通过内置反向代理或轻量转发配置解决跨域同源策略限制。

---

## 凭证配置指南

Kimi Pocket 需要通过你的 Kimi 授权凭证向官方获取用量数据。获取步骤如下：

1. 在桌面浏览器中访问并登录 [Kimi 官网](https://kimi.moonshot.cn/)；
2. 按 `F12`（或右键“检查”）打开开发者工具，切换至 **Application（应用）** 标签页；
3. 在左侧面板展开 **Storage → Local Storage**，点击 `https://kimi.moonshot.cn`；
4. 在右侧列表中找到并复制对应键值：

| 键名 | 必要性 | 说明 |
| :--- | :--- | :--- |
| `access_token` | **必须** | 核心接口调用鉴权凭证（JWT 格式） |
| `refresh_token` | **推荐** | 用于在 `access_token` 过期后自动静默换新，免去频繁手动重填 |

5. 打开 Kimi Pocket 设置面板，将获取的凭证粘贴保存即可。

> [!WARNING]
> 凭证代表你的个人账户操作权限，请妥善保管，切勿在公开截屏或公共仓库中泄露。

---

## 刷新与预警策略

应用根据三项用量指标中**最低剩余百分比**自动切换轮询心跳与界面色彩状态：

| 剩余额度状态 | 刷新心跳间隔 | 视觉色调 / 状态指示 | 触发条件 |
| :--- | :--- | :--- | :--- |
| **充足 (Normal)** | 60 秒 | 翡翠绿 (`#10B981`) / 常态图标 | 所有额度剩余 > 30% |
| **紧张 (Warning)** | 30 秒 | 琥珀黄 (`#F59E0B`) / 警示指示 | 任一额度剩余 ≤ 30% |
| **告急 (Critical)** | 15 秒 | 绯红紫 (`#EF4444`) / 告急高亮 | 任一额度剩余 ≤ 10% |

- **后台自省**：应用进入后台或屏幕熄灭时，轮询自动休眠；重新切回前台时，立即触发同步。
- **离线与容错**：接口鉴权失效（401）时停止无意义轮询并提示更新凭证；网络断开时维持缓存快照并标记离线状态。

---

## 本地开发与构建

### 仓库代码拉取

```bash
git clone https://github.com/yangguan-xue/Kimi-Pocket.git
cd Kimi-Pocket
```

### 1. Web & Android 端 (React 19 + Capacitor 7)

- **前置依赖**：Node.js 20+、npm；打包 Android 原生工程需 JDK 21 与 Android SDK Platform 35。

```bash
# 安装依赖
npm ci

# 启动本地开发服务 (支持反向代理 /apiv2)
npm run dev

# 运行单元测试
npm test

# 生产环境构建 Web / PWA 资源
npm run build

# 同步并更新 Android Capacitor 原生工程
npm run build:android

# 编译 Android Debug APK
cd android && ./gradlew assembleDebug
```
产物输出路径：`android/app/build/outputs/apk/debug/app-debug.apk`。

### 2. macOS 状态栏端 (Swift 6 + SwiftUI)

- **前置依赖**：macOS 14+、Xcode 16+ 或独立 Command Line Tools (Swift 6.0+)。

```bash
# 进入工程目录
cd macos

# 运行单元测试
swift test

# 编译与打包 DMG 镜像 / ZIP 归档
bash ../scripts/build-macos.sh
```
产物输出路径：`dist-macos/KimiPocket-v{VERSION}-macOS.dmg` 与 `.zip`。

### 3. Windows 托盘端 (.NET 10 + WPF)

- **前置依赖**：Windows 11 x64、.NET 10 SDK。

```powershell
# 运行单元测试与格式化检查
dotnet test windows/KimiPocket.sln --configuration Release

# 一键打包单文件便携 ZIP 包 (包含自带运行时)
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\build-windows.ps1
```
产物输出路径：`dist-windows/Kimi-Pocket-windows-x64-{VERSION}.zip`。

---

## 代码结构

```text
kimi-pocket/
├── android/            # Android Capacitor 原生工程与 Gradle 配置
├── macos/              # macOS 原生工程 (Swift 6 + SwiftUI + AppKit)
│   ├── Sources/App/    # 状态栏图标、悬浮 HUD 与控制器
│   └── Sources/Core/   # 额度模型、Keychain 存储与网络客户端
├── windows/            # Windows 原生工程 (.NET + WPF)
│   ├── src/            # 托盘图标、WPF 面板与凭据管理
│   └── tests/          # WPF 渲染及数据层单元测试
├── src/                # Web / Android 前端业务源码 (React 19 + TS)
│   ├── components/     # 竖屏分段卡片与横屏三环 HUD 组件
│   ├── services/       # 官方 API 契约协议实现
│   └── hooks/          # 自适应轮询与状态机 Hooks
├── specs/              # 规范驱动开发 (SDD) 架构与技术契约设计文档
├── scripts/            # 跨端自动化打包构建脚本
└── tasks/              # 分阶段迭代追踪任务卡
```

---

## 工程规范与致谢

- **规范驱动开发 (SDD)**：所有核心设计、接口变更与多端移植均严格遵循 `specs/` 契约文档驱动，拒绝无依据编码。
- **协议与灵感致谢**：感谢 [nekocharm1207/kimi-island](https://github.com/nekocharm1207/kimi-island) 提供的协议分析与产品思路参考。
- **技术开源项目**：感谢 [Capacitor](https://capacitorjs.com/)、[Tailwind CSS](https://tailwindcss.com/) 与 [Lucide Icons](https://lucide.dev/) 团队提供的优秀开发基座。

---

## 开源许可证

本项目基于 [MIT License](LICENSE) 协议开源。
