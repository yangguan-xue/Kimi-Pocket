import Foundation

public enum KimiClientError: LocalizedError, Sendable {
    case invalidToken
    case networkError(String)
    case unauthorized
    case decodingError(String)
    
    public var errorDescription: String? {
        switch self {
        case .invalidToken: return "Token 无效或格式不正确"
        case .networkError(let msg): return "网络请求失败: \(msg)"
        case .unauthorized: return "Token 已失效或未授权 (401)"
        case .decodingError(let msg): return "数据解析失败: \(msg)"
        }
    }
}

public final class KimiClient: Sendable {
    public static let shared = KimiClient()
    
    private let baseURL = "https://www.kimi.com/apiv2"
    private let session: URLSession
    
    public init(session: URLSession = .shared) {
        self.session = session
    }
    
    public func fetchUsageData(token: String) async throws -> KimiUsageState {
        guard let payload = JWTParser.extractPayload(token) else {
            throw KimiClientError.invalidToken
        }
        
        let sub = payload.sub ?? ""
        let ssid = payload.ssid ?? ""
        
        // 构造公共请求头
        let headers: [String: String] = [
            "Authorization": "Bearer \(token)",
            "x-traffic-id": sub,
            "x-msh-session-id": ssid,
            "x-msh-platform": "web",
            "x-language": "zh-CN",
            "connect-protocol-version": "1",
            "Content-Type": "application/json"
        ]
        
        async let subTask = fetchSubscription(headers: headers)
        async let usageTask = fetchUsages(headers: headers)
        
        let (subResp, usageResp) = try await (subTask, usageTask)
        return Self.normalizeUsageData(subResp: subResp, usageResp: usageResp, token: token)
    }
    
    private func fetchSubscription(headers: [String: String]) async throws -> GetSubscriptionResponse {
        let url = URL(string: "\(baseURL)/kimi.gateway.membership.v2.MembershipService/GetSubscription")!
        var request = URLRequest(url: url)
        request.httpMethod = "POST"
        request.allHTTPHeaderFields = headers
        request.httpBody = "{}".data(using: .utf8)
        
        let (data, response) = try await session.data(for: request)
        if let http = response as? HTTPURLResponse, http.statusCode == 401 {
            throw KimiClientError.unauthorized
        }
        
        do {
            return try JSONDecoder().decode(GetSubscriptionResponse.self, from: data)
        } catch {
            throw KimiClientError.decodingError(error.localizedDescription)
        }
    }
    
    private func fetchUsages(headers: [String: String]) async throws -> GetUsagesResponse {
        let url = URL(string: "\(baseURL)/kimi.gateway.billing.v1.BillingService/GetUsages")!
        var request = URLRequest(url: url)
        request.httpMethod = "POST"
        request.allHTTPHeaderFields = headers
        request.httpBody = "{\"scope\":[\"FEATURE_CODING\"]}".data(using: .utf8)
        
        let (data, response) = try await session.data(for: request)
        if let http = response as? HTTPURLResponse, http.statusCode == 401 {
            throw KimiClientError.unauthorized
        }
        
        do {
            return try JSONDecoder().decode(GetUsagesResponse.self, from: data)
        } catch {
            throw KimiClientError.decodingError(error.localizedDescription)
        }
    }
    
    // MARK: - 数据归一化纯函数 (便于 100% 单测)
    
    public static func normalizeUsageData(
        subResp: GetSubscriptionResponse,
        usageResp: GetUsagesResponse,
        token: String,
        now: Date = Date()
    ) -> KimiUsageState {
        // 1. 套餐解析 (兼容 subscription 与 membership 字段)
        let sub = subResp.subscription
        let membership = subResp.membership
        let planTitle = sub?.goods?.title ?? membership?.title ?? membership?.level ?? "Kimi Pro"
        let endTimeStr = sub?.currentEndTime ?? membership?.endTime ?? ""
        var daysRemaining = 0
        var isActive = sub?.active ?? (membership?.status == "ACTIVE")
        
        if !endTimeStr.isEmpty {
            let formatter = ISO8601DateFormatter()
            formatter.formatOptions = [.withInternetDateTime, .withFractionalSeconds]
            var parsed = formatter.date(from: endTimeStr)
            if parsed == nil {
                let fallbackFormatter = ISO8601DateFormatter()
                parsed = fallbackFormatter.date(from: endTimeStr)
            }
            if let targetDate = parsed {
                let diff = targetDate.timeIntervalSince(now)
                daysRemaining = max(0, Int(ceil(diff / 86400.0)))
                isActive = diff > 0
            }
        }
        
        let planInfo = KimiUsageState.PlanInfo(
            title: planTitle,
            endTime: endTimeStr,
            daysRemaining: daysRemaining,
            isActive: isActive
        )
        
        // 2. 总用量 (Total Quota - 4段)
        var totalLimit = 100
        var totalRemaining = 0
        if let tq = usageResp.totalQuota {
            totalLimit = Int(tq.limit ?? "") ?? 100
            totalRemaining = Int(tq.remaining ?? "") ?? 0
        }
        let totalUsed = max(0, totalLimit - totalRemaining)
        let totalRatio = totalLimit > 0 ? Double(totalUsed) / Double(totalLimit) : 0.0
        let totalQuota = KimiUsageState.TotalQuotaInfo(
            limit: totalLimit,
            remaining: totalRemaining,
            used: totalUsed,
            usageRatio: min(1.0, max(0.0, totalRatio))
        )
        
        // 3. 周用量 (Weekly Quota - 7段)
        let codingUsage = usageResp.usages?.first(where: { $0.scope == "FEATURE_CODING" })
        var weeklyLimit = 100
        var weeklyRemaining = 100
        var weeklyResetTime = ""
        
        if let detail = codingUsage?.detail {
            weeklyLimit = Int(detail.limit ?? "") ?? 100
            weeklyRemaining = Int(detail.remaining ?? "") ?? 0
            weeklyResetTime = detail.resetTime ?? ""
        }
        let weeklyUsed = max(0, weeklyLimit - weeklyRemaining)
        let weeklyRatio = weeklyLimit > 0 ? Double(weeklyUsed) / Double(weeklyLimit) : 0.0
        let weeklyCountdown = formatCountdown(weeklyResetTime, now: now)
        let weeklyQuota = KimiUsageState.WeeklyQuotaInfo(
            limit: weeklyLimit,
            remaining: weeklyRemaining,
            used: weeklyUsed,
            usageRatio: min(1.0, max(0.0, weeklyRatio)),
            resetTime: weeklyResetTime,
            resetCountdown: weeklyCountdown
        )
        
        // 4. 5小时额度 (Rolling 5-Hour - 连续平滑)
        let fiveHourItem = codingUsage?.limits?.first(where: {
            ($0.window?.timeUnit == "TIME_UNIT_MINUTE" && $0.window?.duration == 300) ||
            ($0.window?.timeUnit == "TIME_UNIT_HOUR" && $0.window?.duration == 5) ||
            $0.window?.timeUnit == "TIME_UNIT_MINUTE"
        })
        
        var rollingLimit = 60
        var rollingRemaining = 60
        var rollingResetTime = ""
        
        if let detail = fiveHourItem?.detail {
            rollingLimit = Int(detail.limit ?? "") ?? 60
            rollingRemaining = Int(detail.remaining ?? "") ?? 0
            rollingResetTime = detail.resetTime ?? ""
        }
        let rollingUsed = max(0, rollingLimit - rollingRemaining)
        let rollingRatio = rollingLimit > 0 ? Double(rollingUsed) / Double(rollingLimit) : 0.0
        let rollingCountdown = formatCountdown(rollingResetTime, now: now)
        let rollingQuota = KimiUsageState.RollingQuotaInfo(
            limit: rollingLimit,
            remaining: rollingRemaining,
            used: rollingUsed,
            usageRatio: min(1.0, max(0.0, rollingRatio)),
            resetTime: rollingResetTime,
            resetCountdown: rollingCountdown
        )
        
        // 5. 健康状态
        let isExpired = JWTParser.isTokenExpired(token, now: now)
        let isExpiringSoon = JWTParser.isTokenNearExpiry(token, now: now)
        let health = KimiUsageState.HealthStatus(
            isExpired: isExpired,
            isExpiringSoon: isExpiringSoon,
            isDegraded: false,
            lastUpdated: now
        )
        
        return KimiUsageState(
            plan: planInfo,
            rolling5h: rollingQuota,
            weekly: weeklyQuota,
            total: totalQuota,
            status: health
        )
    }
    
    public static func formatCountdown(_ isoString: String, now: Date = Date()) -> String {
        guard !isoString.isEmpty else { return "" }
        let formatter = ISO8601DateFormatter()
        formatter.formatOptions = [.withInternetDateTime, .withFractionalSeconds]
        var target = formatter.date(from: isoString)
        if target == nil {
            let fallback = ISO8601DateFormatter()
            target = fallback.date(from: isoString)
        }
        guard let dest = target else { return "" }
        let diff = dest.timeIntervalSince(now)
        guard diff > 0 else { return "00:00:00" }
        
        let hours = Int(diff) / 3600
        let minutes = (Int(diff) % 3600) / 60
        let seconds = Int(diff) % 60
        
        if hours >= 24 {
            let days = Int(ceil(Double(hours) / 24.0))
            return "\(days) 天后重置"
        }
        return String(format: "%02d:%02d:%02d", hours, minutes, seconds)
    }
}
