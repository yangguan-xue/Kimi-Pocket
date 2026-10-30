# SPEC-05: Android 原生 APK 打包与 CI/CD 自动化流水线规范

- **文档编号**：`SPEC-05`
- **对应阶段**：`Phase 4: 安卓独立端体验与封装`
- **状态**：`APPROVED` (已评审)

---

## 1. 架构与方案对比

| 方案 | 优缺点 | 结论 |
|---|---|:---:|
| **本地 Android Studio 编译** | 需要本地下载几个 G 的 SDK、Gradle 与环境配置，易发生环境冲突 | ❌ 成本高、易踩坑 |
| **Capacitor + GitHub Actions 云端构建** | 本地零环境负担，GitHub 虚拟机全自动编译并输出 APK 制品，随时下载 | ✅ **标准推荐方案** |

---

## 2. 关键技术特性与突破

1. **彻底解决 CORS 跨域限制**：
   - 启用 `@capacitor/cli` 的 `CapacitorHttp` 原生拦截插件。
   - 原生 App 内发起的网络请求直接调用 Android 原生底层网络库（OkHttp），绕过浏览器同源策略限制，直接直连 Kimi 官方接口 `https://www.kimi.com/apiv2`。
2. **纯黑 OLED 原生适配**：
   - 状态栏与导航栏原生背景设为 `#000000`。
3. **CI/CD 自动化流水线**：
   - 配置文件：`.github/workflows/build-apk.yml`。
   - 产物：`kimi-pocket-debug.apk`，每次推送或手动触发均可在 GitHub Actions Artifacts 页面直接下载。
