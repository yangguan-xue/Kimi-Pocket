# Task 07: 集成 Capacitor 原生 Android 工程与 GitHub Actions 自动打包流水线

## 1. 任务元信息
- **任务编号**：`Task 07`
- **所属 Spec**：[SPEC-05-android-apk-packaging.md](../../specs/SPEC-05-android-apk-packaging.md)
- **研发阶段**：`Phase 4: 安卓独立端体验与封装`
- **前置任务**：`Task 06`
- **状态**：`READY`

---

## 2. 交付物清单
- `capacitor.config.ts`：Capacitor 配置文件（开启原生 HTTP 拦截）。
- `android/`：生成的标准 Android 工程目录。
- `.github/workflows/build-apk.yml`：GitHub Actions 自动构建 APK 工作流。
- `src/api/client.ts`：原生环境下直连官方 API。

---

## 3. 验收标准
- `npm run build:android` 零报错完成 Web 编译与 Android 原生资源同步。
- 推送代码至 GitHub 触发 Actions 流水线，成功产出并上传 `kimi-pocket-debug.apk`。
