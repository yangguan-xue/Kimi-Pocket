import Foundation

public struct JWTPayload: Equatable, Sendable {
    public let sub: String?
    public let ssid: String?
    public let exp: TimeInterval?
    
    public init(sub: String?, ssid: String?, exp: TimeInterval?) {
        self.sub = sub
        self.ssid = ssid
        self.exp = exp
    }
}

public enum JWTParser {
    
    /// 解析 JWT Token Payload
    public static func extractPayload(_ token: String) -> JWTPayload? {
        let trimmed = token.trimmingCharacters(in: .whitespacesAndNewlines)
        let parts = trimmed.components(separatedBy: ".")
        guard parts.count == 3 else { return nil }
        
        var base64 = parts[1]
            .replacingOccurrences(of: "-", with: "+")
            .replacingOccurrences(of: "_", with: "/")
        
        while base64.count % 4 != 0 {
            base64.append("=")
        }
        
        guard let data = Data(base64Encoded: base64) else { return nil }
        
        do {
            guard let json = try JSONSerialization.jsonObject(with: data) as? [String: Any] else {
                return nil
            }
            
            let sub = json["sub"] as? String
            let ssid = json["ssid"] as? String
            let exp = (json["exp"] as? NSNumber)?.doubleValue
            
            return JWTPayload(sub: sub, ssid: ssid, exp: exp)
        } catch {
            return nil
        }
    }
    
    /// 检查 Token 是否已过期
    public static func isTokenExpired(_ token: String, now: Date = Date()) -> Bool {
        guard let payload = extractPayload(token), let exp = payload.exp else { return true }
        return now.timeIntervalSince1970 >= exp
    }
    
    /// 检查 Token 是否即将过期 (默认提前 3600 秒 / 1小时)
    public static func isTokenNearExpiry(_ token: String, bufferSeconds: TimeInterval = 3600, now: Date = Date()) -> Bool {
        guard let payload = extractPayload(token), let exp = payload.exp else { return false }
        return (now.timeIntervalSince1970 + bufferSeconds) >= exp
    }
}
