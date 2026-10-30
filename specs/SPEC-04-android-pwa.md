# SPEC-04: 安卓 PWA 独立安装与运行规范 (Android PWA Spec)

- **文档编号**：`SPEC-04`
- **对应阶段**：`Phase 4: 安卓独立端体验与封装`
- **状态**：`APPROVED` (已评审)

---

## 1. Web App Manifest 核心配置 (manifest.webmanifest)

为了让应用在 Android Chrome / Edge / 各品牌自带浏览器中被识别为独立原生 App，必须满足以下配置：

```json
{
  "name": "Kimi Pocket",
  "short_name": "KimiPocket",
  "description": "Kimi Code 掌上订阅额度与用量监控",
  "start_url": "/",
  "display": "standalone",
  "orientation": "any",
  "background_color": "#0B0C10",
  "theme_color": "#0B0C10",
  "icons": [
    {
      "src": "/icons/icon-192.png",
      "sizes": "192x192",
      "type": "image/png",
      "purpose": "any maskable"
    },
    {
      "src": "/icons/icon-512.png",
      "sizes": "512x512",
      "type": "image/png",
      "purpose": "any maskable"
    }
  ]
}
```

---

## 2. 安卓原生级特性支持

1. **全屏沉浸与透明状态栏**：
   - 注入 `<meta name="theme-color" content="#0B0C10">` 与 `<meta name="mobile-web-app-capable" content="yes">`。
   - 隐藏浏览器所有地址栏与底部导航条。
2. **屏幕常亮唤醒锁 (Screen Wake Lock API)**：
   - 当用户在横屏模式充当桌面 StandBy HUD 仪表盘时，提供「常亮保持」开关，通过 `navigator.wakeLock.request('screen')` 防止手机自动熄屏。
3. **离线可用 (Service Worker)**：
   - 离线缓存静态资源（JS/CSS/图标），在网络闪断或飞机模式下仍能正常打开并展示上次离线快照。
