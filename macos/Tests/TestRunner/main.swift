import Foundation
import KimiPocketCore

print("🚀 Running Kimi Pocket Core & MenuBar Test Suite...")

// 1. 基础版本号检查
assert(KimiPocketCoreInfo.version == "0.2.0", "Core version must be 0.2.0")
print("✅ Core version check passed.")

// 2. 悬停防抖状态机 (HoverCoordinator) 自动化测试 (纯即时悬停模型)
@MainActor
final class MockHoverDelegate: HoverCoordinatorDelegate {
    var showCount = 0
    var hideCount = 0
    
    func hoverCoordinatorShouldShowHUD(_ coordinator: HoverCoordinator) {
        showCount += 1
    }
    
    func hoverCoordinatorShouldHideHUD(_ coordinator: HoverCoordinator) {
        hideCount += 1
    }
}

@MainActor
func testCoordinator() {
    let delegate = MockHoverDelegate()
    let coordinator = HoverCoordinator()
    coordinator.delegate = delegate

    assert(!coordinator.isHoveringButton, "Initial isHoveringButton must be false")
    assert(!coordinator.isHUDVisible, "Initial isHUDVisible must be false")

    coordinator.onButtonMouseEnter()
    Thread.sleep(forTimeInterval: 0.05)
    coordinator.onButtonMouseExit()
    Thread.sleep(forTimeInterval: 0.20)
    assert(delegate.showCount == 0, "Fast pass must NOT trigger showHUD")
    print("✅ Quick-pass debounce (<180ms) test passed.")

    coordinator.onButtonMouseEnter()
    let loopUntil = Date().addingTimeInterval(0.25)
    while Date() < loopUntil {
        RunLoop.current.run(mode: .default, before: Date().addingTimeInterval(0.05))
    }
    assert(delegate.showCount == 1, "Dwell >=180ms must trigger showHUD once")
    print("✅ Dwell trigger (>=180ms) test passed.")

    coordinator.dismissImmediately()
    assert(!coordinator.isHUDVisible, "HUD must be dismissed")
    print("✅ Dismiss immediately test passed.")
}

MainActor.assumeIsolated {
    testCoordinator()
}

// 3. Task 10: JWT 解析与过期验证单测
func makeMockJWT(sub: String, ssid: String, exp: TimeInterval) -> String {
    let header = "{\"alg\":\"HS256\",\"typ\":\"JWT\"}".data(using: .utf8)!.base64EncodedString()
    let payloadDict: [String: Any] = ["sub": sub, "ssid": ssid, "exp": exp]
    let payload = try! JSONSerialization.data(withJSONObject: payloadDict).base64EncodedString()
    return "\(header).\(payload).mocksignature"
}

let futureExp = Date().timeIntervalSince1970 + 7200 // 2小时后
let nearExp = Date().timeIntervalSince1970 + 1800   // 30分钟后 (near expiry)
let pastExp = Date().timeIntervalSince1970 - 100    // 已过期

let validToken = makeMockJWT(sub: "user_12345", ssid: "session_abc", exp: futureExp)
let parsed = JWTParser.extractPayload(validToken)
assert(parsed?.sub == "user_12345", "JWT sub extraction failed")
assert(parsed?.ssid == "session_abc", "JWT ssid extraction failed")
assert(!JWTParser.isTokenExpired(validToken), "Valid token should NOT be expired")
assert(!JWTParser.isTokenNearExpiry(validToken), "Valid token should NOT be near expiry")

let expiringToken = makeMockJWT(sub: "user_exp", ssid: "session_exp", exp: nearExp)
assert(JWTParser.isTokenNearExpiry(expiringToken), "Expiring token SHOULD be near expiry (30m < 1h)")

let expiredToken = makeMockJWT(sub: "user_old", ssid: "session_old", exp: pastExp)
assert(JWTParser.isTokenExpired(expiredToken), "Past token MUST be expired")

assert(JWTParser.extractPayload("invalid.token") == nil, "Malformed token must return nil")
print("✅ JWT parsing and expiry tests passed.")

// 4. Task 10: KimiClient 数据归一化转换单测
let mockSub = GetSubscriptionResponse(
    membership: .init(
        level: "Kimi Pro",
        status: "ACTIVE",
        startTime: "2026-01-01T00:00:00Z",
        endTime: "2026-11-30T23:59:59Z",
        title: "Kimi Pro"
    )
)

let mockUsages = GetUsagesResponse(
    totalQuota: .init(limit: "10000", remaining: "3500"),
    usages: [
        .init(
            scope: "FEATURE_CODING",
            detail: .init(limit: "1000", remaining: "580", resetTime: "2026-11-09T00:00:00Z"),
            limits: [
                .init(
                    detail: .init(limit: "60", remaining: "41", resetTime: "2026-11-03T16:00:00Z"),
                    window: .init(duration: 300, timeUnit: "TIME_UNIT_MINUTE")
                )
            ]
        )
    ]
)

let fixedNow = Date(timeIntervalSince1970: 1793700000) // 模拟固定时间
let normalized = KimiClient.normalizeUsageData(subResp: mockSub, usageResp: mockUsages, token: validToken, now: fixedNow)

// 校验套餐
assert(normalized.plan.title == "Kimi Pro", "Plan title must be Kimi Pro")
assert(normalized.plan.isActive, "Plan must be active")
assert(normalized.plan.formattedExpirySubtitle.hasPrefix("到期时间: 2026-"), "Expiry subtitle must be formatted with date prefix")

// 校验 5小时额度
assert(normalized.rolling5h.limit == 60, "Rolling limit must be 60")
assert(normalized.rolling5h.remaining == 41, "Rolling remaining must be 41")
assert(normalized.rolling5h.used == 19, "Rolling used must be 19")
assert(abs(normalized.rolling5h.usageRatio - (19.0 / 60.0)) < 0.001, "Usage ratio mismatch")

// 校验周用量
assert(normalized.weekly.limit == 1000, "Weekly limit must be 1000")
assert(normalized.weekly.remaining == 580, "Weekly remaining must be 580")
assert(normalized.weekly.used == 420, "Weekly used must be 420")

// 校验总用量
assert(normalized.total.limit == 10000, "Total limit must be 10000")
assert(normalized.total.remaining == 3500, "Total remaining must be 3500")
assert(normalized.total.used == 6500, "Total used must be 6500")
print("✅ KimiClient usage data normalization tests passed.")

// 5. Task 10: KeychainManager 安全持久化单测
let keychain = KeychainManager()
let testSecret = "test_keychain_token_\(UUID().uuidString)"
assert(keychain.saveToken(testSecret), "Keychain save failed")
assert(keychain.getToken() == testSecret, "Keychain read mismatch")
assert(keychain.deleteToken(), "Keychain delete failed")
assert(keychain.getToken() == nil, "Keychain delete should clear token")
print("✅ KeychainManager storage tests passed.")

print("🎉 All Task 08, 09, 10 tests passed successfully!")
