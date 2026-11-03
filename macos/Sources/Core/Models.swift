import Foundation

// MARK: - 业务归一化核心状态模型

public struct KimiUsageState: Equatable, Sendable {
    public struct PlanInfo: Equatable, Sendable {
        public let title: String
        public let endTime: String
        public let daysRemaining: Int
        public let isActive: Bool
        
        public init(title: String, endTime: String, daysRemaining: Int, isActive: Bool) {
            self.title = title
            self.endTime = endTime
            self.daysRemaining = daysRemaining
            self.isActive = isActive
        }
    }
    
    public struct RollingQuotaInfo: Equatable, Sendable {
        public let limit: Int
        public let remaining: Int
        public let used: Int
        public let usageRatio: Double
        public let resetTime: String
        public let resetCountdown: String
        
        public init(limit: Int, remaining: Int, used: Int, usageRatio: Double, resetTime: String, resetCountdown: String) {
            self.limit = limit
            self.remaining = remaining
            self.used = used
            self.usageRatio = usageRatio
            self.resetTime = resetTime
            self.resetCountdown = resetCountdown
        }
    }
    
    public struct WeeklyQuotaInfo: Equatable, Sendable {
        public let limit: Int
        public let remaining: Int
        public let used: Int
        public let usageRatio: Double
        public let resetTime: String
        public let resetCountdown: String
        
        public init(limit: Int, remaining: Int, used: Int, usageRatio: Double, resetTime: String, resetCountdown: String) {
            self.limit = limit
            self.remaining = remaining
            self.used = used
            self.usageRatio = usageRatio
            self.resetTime = resetTime
            self.resetCountdown = resetCountdown
        }
    }
    
    public struct TotalQuotaInfo: Equatable, Sendable {
        public let limit: Int
        public let remaining: Int
        public let used: Int
        public let usageRatio: Double
        
        public init(limit: Int, remaining: Int, used: Int, usageRatio: Double) {
            self.limit = limit
            self.remaining = remaining
            self.used = used
            self.usageRatio = usageRatio
        }
    }
    
    public struct HealthStatus: Equatable, Sendable {
        public let isExpired: Bool
        public let isExpiringSoon: Bool
        public let isDegraded: Bool
        public let lastUpdated: Date
        
        public init(isExpired: Bool, isExpiringSoon: Bool, isDegraded: Bool, lastUpdated: Date) {
            self.isExpired = isExpired
            self.isExpiringSoon = isExpiringSoon
            self.isDegraded = isDegraded
            self.lastUpdated = lastUpdated
        }
    }
    
    public let plan: PlanInfo
    public let rolling5h: RollingQuotaInfo
    public let weekly: WeeklyQuotaInfo
    public let total: TotalQuotaInfo
    public let status: HealthStatus
    
    public init(plan: PlanInfo, rolling5h: RollingQuotaInfo, weekly: WeeklyQuotaInfo, total: TotalQuotaInfo, status: HealthStatus) {
        self.plan = plan
        self.rolling5h = rolling5h
        self.weekly = weekly
        self.total = total
        self.status = status
    }
}

// MARK: - Kimi Connect Protocol 原始 JSON 数据契约

public struct GetSubscriptionResponse: Codable, Sendable {
    public struct Membership: Codable, Sendable {
        public let level: String?
        public let status: String?
        public let startTime: String?
        public let endTime: String?
        public let title: String?
        
        public init(level: String? = nil, status: String? = nil, startTime: String? = nil, endTime: String? = nil, title: String? = nil) {
            self.level = level
            self.status = status
            self.startTime = startTime
            self.endTime = endTime
            self.title = title
        }
    }
    
    public let membership: Membership?
    public init(membership: Membership?) {
        self.membership = membership
    }
}

public struct GetUsagesResponse: Codable, Sendable {
    public struct TotalQuota: Codable, Sendable {
        public let limit: String?
        public let remaining: String?
        
        public init(limit: String? = nil, remaining: String? = nil) {
            self.limit = limit
            self.remaining = remaining
        }
    }
    
    public struct UsageDetail: Codable, Sendable {
        public let limit: String?
        public let remaining: String?
        public let resetTime: String?
        
        public init(limit: String? = nil, remaining: String? = nil, resetTime: String? = nil) {
            self.limit = limit
            self.remaining = remaining
            self.resetTime = resetTime
        }
    }
    
    public struct WindowConfig: Codable, Sendable {
        public let duration: Int?
        public let timeUnit: String?
        
        public init(duration: Int? = nil, timeUnit: String? = nil) {
            self.duration = duration
            self.timeUnit = timeUnit
        }
    }
    
    public struct LimitItem: Codable, Sendable {
        public let detail: UsageDetail?
        public let window: WindowConfig?
        
        public init(detail: UsageDetail? = nil, window: WindowConfig? = nil) {
            self.detail = detail
            self.window = window
        }
    }
    
    public struct ScopeUsage: Codable, Sendable {
        public let scope: String?
        public let detail: UsageDetail?
        public let limits: [LimitItem]?
        
        public init(scope: String? = nil, detail: UsageDetail? = nil, limits: [LimitItem]? = nil) {
            self.scope = scope
            self.detail = detail
            self.limits = limits
        }
    }
    
    public let totalQuota: TotalQuota?
    public let usages: [ScopeUsage]?
    
    public init(totalQuota: TotalQuota? = nil, usages: [ScopeUsage]? = nil) {
        self.totalQuota = totalQuota
        self.usages = usages
    }
}
