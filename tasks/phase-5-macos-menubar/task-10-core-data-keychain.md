# Task 10: 原生 Swift 数据核心与 Keychain 安全存储 (KimiClient + Keychain + Store)

## 1. 任务元信息
- **任务编号**：`Task 10`
- **所属 Spec**：[SPEC-02-api-contracts.md](../../specs/SPEC-02-api-contracts.md), [SPEC-06-macos-menubar-app.md](../../specs/SPEC-06-macos-menubar-app.md)
- **研发阶段**：`Phase 5: macOS 桌面原生端体验与常驻扩展`
- **前置任务**：`Task 08`
- **状态**：`DONE` (已通过全套 JWT、归一化数据模型及 Keychain 自动化单测)

---

## 2. 交付物清单
- `macos/Sources/Core/Models.swift`：对标 SPEC-02 接口契约的 Swift 强类型模型（`KimiUsageState`、`GetSubscriptionResponse`、`GetUsagesResponse`）。
- `macos/Sources/Core/JWTParser.swift`：Swift 原生 JWT Payload 解析与过期检测。
- `macos/Sources/Core/KeychainManager.swift`：基于 Security 框架的 Token 加密持久化存储。
- `macos/Sources/Core/KimiClient.swift`：封装原生 `URLSession` 调用 Kimi Connect Protocol 接口（直连 `https://www.kimi.com/apiv2`）。
- `macos/Sources/Core/UsageStore.swift`：响应式状态中心，负责智能轮询、唤醒重连与用量归一化。
- `macos/Tests/KimiPocketTests/CoreTests.swift`：针对 JWT 解析、数据模型映射与 Keychain 读写的单元测试。

---

## 3. 技术约束与实现要点
1. **网络与协议**：严格注入 `x-traffic-id`、`x-msh-session-id`、`connect-protocol-version: 1` 等 Header，直接通过系统底层网络请求，无浏览器跨域阻碍。
2. **凭据安全**：Token 仅保存在 macOS Keychain (`kSecClassGenericPassword`) 中，禁止明文落盘。
3. **低功耗轮询**：
   - 默认 60 秒轮询，浮窗展开且缓存超过 15 秒时触发即刻静默刷新；
   - 监听 `NSWorkspace.willSleepNotification` 挂起定时器，`didWakeNotification` 立即唤醒刷新。

---

## 4. 验收标准
- 执行 `swift test`，全套数据解析与模型映射单元测试 100% Pass。
- 正确配置 Token 后，能成功返回归一化后的 KimiUsageState 真实数据。
