# SPEC-02: Kimi Connect Protocol 接口契约与数据模型规范 (API Contracts)

- **文档编号**：`SPEC-02`
- **对应阶段**：`Phase 2: 数据核心与接口契约`
- **状态**：`APPROVED` (已评审)

---

## 1. 认证机制与 JWT 解析规范

用户在浏览器开发者工具或登录后获取到的凭据为 JWT 格式的 `access_token`：
- **格式**：`header.payload.signature`
- **Payload 解析字段**：
  - `sub`：用户 ID（Traffic ID）
  - `ssid`：会话 ID（Session ID）
  - `exp`：Token 到期时间戳（秒）

### 预检逻辑
- 客户端在发起网络请求前，计算当前时间与 `exp`：
  - `now >= exp`：判定为已过期，立即提示登录，不发送无效请求；
  - `now + 3600 >= exp`：判定为即将过期（1小时内），在界面显示预警黄条。

---

## 2. 核心接口规范

### 2.1 会员订阅接口 (GetSubscription)
- **请求地址**：`POST /apiv2/kimi.gateway.membership.v2.MembershipService/GetSubscription`
- **请求体**：`{}`
- **公共请求头**：
  - `Authorization: Bearer <access_token>`
  - `x-traffic-id: <sub_from_jwt>`
  - `x-msh-session-id: <ssid_from_jwt>`
  - `x-msh-platform: web`
  - `x-language: zh-CN`
  - `connect-protocol-version: 1`
  - `Content-Type: application/json`

### 2.2 用量接口 (GetUsages)
- **请求地址**：`POST /apiv2/kimi.gateway.billing.v1.BillingService/GetUsages`
- **请求体**：
  ```json
  {
    "scope": ["FEATURE_CODING"]
  }
  ```

---

## 3. 核心数据模型映射（TypeScript 规范定义）

```typescript
// 归一化后的核心业务数据模型
export interface KimiUsageState {
  // 套餐信息
  plan: {
    title: string;           // e.g. "Kimi Pro"
    endTime: string;         // RFC3339 字符串
    daysRemaining: number;   // 距离到期天数
    isActive: boolean;
  };
  
  // 1. 总用量 (拆成 4 段)
  totalQuota: {
    limit: number;
    remaining: number;
    used: number;
    usageRatio: number;      // 0.0 ~ 1.0
  };

  // 2. 周用量 (拆成 7 段)
  weeklyQuota: {
    limit: number;
    remaining: number;
    used: number;
    usageRatio: number;      // 0.0 ~ 1.0
    resetTime: string;       // 本周重置时间 ISO
    resetCountdown: string;  // 人性化展示，如 "3天后"
  };

  // 3. 5小时额度 (连续平滑，不拆)
  rollingQuota: {
    limit: number;
    remaining: number;
    used: number;
    usageRatio: number;      // 0.0 ~ 1.0
    durationMinutes: number; // 300 (5小时)
    resetTime: string;       // 窗口恢复时间
    resetCountdown: string;  // 人性化展示，如 "1小时42分后"
  };

  // 状态指标
  meta: {
    lastUpdated: Date;
    loading: boolean;
    error: string | null;
    isTokenExpiring: boolean;
  };
}
```
