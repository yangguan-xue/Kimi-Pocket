# Task 02: 实现 Kimi API 客户端、JWT 解析与开发期反向代理

## 1. 任务元信息
- **任务编号**：`Task 02`
- **所属 Spec**：[SPEC-02-api-contracts.md](../../specs/SPEC-02-api-contracts.md)
- **研发阶段**：`Phase 2: 数据核心与接口契约`
- **前置任务**：`Task 01`
- **状态**：`PENDING` (待前置完成)

---

## 2. 交付物清单
- `src/types/kimi.ts`：强类型定义（GetSubscriptionResponse、GetUsagesResponse、KimiUsageState）。
- `src/api/jwt.ts`：JWT 解析函数 `extractJwtPayload` 与过期检查 `isTokenExpired`。
- `src/api/client.ts`：封装 `fetchKimiUsageData(token)`，自动提取 sub、ssid 并组装请求头。
- `src/api/__tests__/jwt.test.ts`：针对各类合法、即将过期、非法 Token 的全套单测。
- `src/api/__tests__/client.test.ts`：针对 Mock API 响应归一化转换的逻辑测试。

---

## 3. 验收标准
- 执行 `npm test src/api/__tests__/jwt.test.ts` 全部 Pass。
- 执行 `npm test src/api/__tests__/client.test.ts` 转换字段 100% 吻合规范。
