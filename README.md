<div align="center">

# 📱 Kimi Pocket

**专为移动端打造的沉浸式 Kimi Code 订阅用量与额度观测仪**

[![Release](https://img.shields.io/github/v/release/yangguan-xue/Kimi-Pocket?color=10B981&label=Release&logo=android)](https://github.com/yangguan-xue/Kimi-Pocket/releases/latest)
[![Build Status](https://img.shields.io/github/actions/workflow/status/yangguan-xue/Kimi-Pocket/build-apk.yml?branch=main&label=Build&logo=githubactions)](https://github.com/yangguan-xue/Kimi-Pocket/actions)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![React](https://img.shields.io/badge/React-19-61DAFB?logo=react&logoColor=black)](https://react.dev/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5.7-3178C6?logo=typescript&logoColor=white)](https://www.typescriptlang.org/)
[![Capacitor](https://img.shields.io/badge/Capacitor-7-119EFF?logo=capacitor&logoColor=white)](https://capacitorjs.com/)

<p align="center">
  <a href="#-快速下载安装">📥 立即下载</a> •
  <a href="#-核心特性">✨ 核心特性</a> •
  <a href="#-凭证获取与配置">🔑 配置指南</a> •
  <a href="#-本地开发与构建">🛠️ 本地构建</a> •
  <a href="#-致谢与灵感">🙏 致谢</a>
</p>

</div>

---

## 💡 灵感与简介

**Kimi Pocket** 是一款极简纯粹、针对 OLED 屏幕深度优化的 Kimi 订阅额度掌上监控工具。

核心协议与灵感参考自桌面端优秀的开源项目 [nekocharm1207/kimi-island](https://github.com/nekocharm1207/kimi-island)。我们将其重构并全面适配移动设备，提供**原生 Android APK** 与 **PWA 渐进式 Web 应用** 双重形态，让你随时随地在手机上一目了然地掌握 Kimi Code 的当前用量、每周重置时间与会员到期状态。

---

## 📸 界面预览

<div align="center">
  <table>
    <tr>
      <td align="center" width="50%">
        <b>📱 竖屏卡片模式 (Portrait Cards)</b><br/><br/>
        <img src="preview-ui-v2.jpg" alt="竖屏预览" width="90%"/>
      </td>
      <td align="center" width="50%">
        <b>📟 横屏三环 HUD 模式 (Landscape HUD)</b><br/><br/>
        <img src="preview-ui-landscape.jpg" alt="横屏预览" width="90%"/>
      </td>
    </tr>
  </table>
</div>

---

## ✨ 核心特性

- 🌌 **极致 OLED 纯黑美学**：全局采用极致 `#000000` 纯黑背景与翡翠绿霓虹呼吸微光，功耗极低、视觉通透。
- 📱 **横竖屏动态自适应**：
  - **竖屏模式**：分段胶囊进度条卡片（总额度 4 段阶梯、周额度 7 段阶梯、5 小时平滑滚动条），状态与倒计时一览无余。
  - **横屏模式**：纯净无干扰的三环 HUD 仪表盘，精简居中显示大号百分比，数学级精准弧长渲染，适合桌面支架常亮展示。
- 🪟 **绝对沉浸式全屏**：原生屏蔽状态栏（信号/电量）与底部导航条，挖孔区及刘海区自动延伸延伸（`SHORT_EDGES`），杜绝任何黑白边与错位。
- 🔄 **一次配置·永久免维（静默自动续期）**：
  - 支持填入 `refresh_token`。
  - 内置智能拦截刷新机制：在 `access_token` 过期前 30 分钟或偶发 401 时，后台自动向 Kimi 官方网关换取新 Token 并无感重试，无需反复手动黏贴。
- 📋 **防遮挡与一键剪贴板**：
  - 输入框常驻【📋 一键粘贴】按钮，告别长按呼出系统菜单的烦恼，避免定制 ROM 的反色白屏 Bug。
  - 弹窗顶部常驻【保存】高亮按钮，配合 `adjustResize` 视口避让，无论键盘如何弹起都触手可及。
- 🔒 **纯客户端运行·零隐私泄露**：
  - 凭证仅保存在手机端本地 `LocalStorage`，端到端直接与 Kimi 官方网关通信。
  - 无任何中心服务器中转，不收集任何用户数据与日志。

---

## 📥 快速下载安装

直接前往 [GitHub Releases](https://github.com/yangguan-xue/Kimi-Pocket/releases/latest) 下载最新版本的 APK：

| 文件 | 描述 | 下载链接 |
| :--- | :--- | :--- |
| **Kimi-Pocket-latest.apk** | Android 最终稳定版安装包（推荐） | [点击下载 APK](https://github.com/yangguan-xue/Kimi-Pocket/releases/latest/download/Kimi-Pocket-latest.apk) |

> **提示**：安装时若提示“未受信任来源”，请选择“仍要安装”。

---

## 🔑 凭证获取与配置

使用本工具需要获取你的 Kimi 网页端凭证（`access_token` 与推荐的 `refresh_token`）：

1. 在电脑浏览器登录 [Kimi 官网](https://kimi.moonshot.cn/)。
2. 按 `F12` 打开开发者工具，切换到 **应用 (Application)** 面板。
3. 在左侧栏展开 **本地存储空间 (Local Storage)**，点击 `https://kimi.moonshot.cn`。
4. 在右侧键值列表中找到并复制：
   - `access_token`：用于当前接口鉴权（必填）。
   - `refresh_token`：用于自动静默换新凭证（**强烈推荐填入**，填入后永久免再次配置）。
5. 在手机端打开 **Kimi Pocket**，点击右上角设置图标（🔑）：
   - 点击 **【一键粘贴】** 自动导入剪贴板内容。
   - 点击右上角 **【保存】** 按钮即可生效！

---

## 🛠️ 本地开发与构建

### 1. 克隆项目与安装依赖

```bash
git clone https://github.com/yangguan-xue/Kimi-Pocket.git
cd Kimi-Pocket
npm install
```

### 2. 启动本地开发服务 (Web / PWA)

```bash
npm run dev
```

### 3. 运行单元测试

```bash
npm test
```

### 4. 编译前端并同步至 Android 原生项目

```bash
npm run build:android
```

### 5. 构建 Android Debug APK

```bash
cd android
./gradlew assembleDebug
```
产出物路径：`android/app/build/outputs/apk/debug/app-debug.apk`。

---

## 🤝 致谢与灵感

- [nekocharm1207/kimi-island](https://github.com/nekocharm1207/kimi-island) - 极其优秀的桌面端灵动岛 Kimi 用量监控工具，本项目协议解析与设计思路的重要启发。
- [Capacitor](https://capacitorjs.com/) - 现代跨平台原生移动应用运行时。
- [Tailwind CSS](https://tailwindcss.com/) & [Lucide Icons](https://lucide.dev/) - 简洁美观的现代 UI 与图标库。

---

## 📄 开源许可证

本项目基于 [MIT License](LICENSE) 开源，欢迎自由学习、分享与衍生开发。
