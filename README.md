# Kimi Pocket (kimi-pocket)

> 掌上 Kimi 订阅用量与额度观测工具（移动端）

灵感与核心协议参考自桌面端项目 [nekocharm1207/kimi-island](https://github.com/nekocharm1207/kimi-island)。

## 🎯 项目目标
在手机端随时便捷地查看 Kimi Code 套餐用量、每周额度使用率、有效期以及 RPM/TPM/RPD 频限状态。

## 📱 核心功能特性（规划中）
- **套餐余量监控**：实时查看本周 Coding 额度消耗、剩余 Token 及百分比
- **速率限制预警**：RPM（每分钟请求数）、TPM（每分钟 Token 数）、RPD（每天请求数）状态观测
- **会员有效期提醒**：订阅到期倒计时与续费预警
- **移动端多形态形态支持**：
  - 方案 A：iOS 桌面小组件 / 灵动岛实时活动（Scriptable 或 Swift/WidgetKit）
  - 方案 B：轻量级响应式 PWA / 移动 Web 页面（可直接「添加到主屏幕」）
  - 方案 C：推送通知服务（Bark / 微信推送 / 飞书推送，定时或阈值告警）

## 🔗 相关接口参考
- 会员订阅信息：`POST https://www.kimi.com/apiv2/kimi.gateway.membership.v2.MembershipService/GetSubscription`
- 用量频限信息：`POST https://www.kimi.com/apiv2/kimi.gateway.billing.v1.BillingService/GetUsages`
