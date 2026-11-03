import Foundation
import Security

public final class KeychainManager: @unchecked Sendable {
    public static let shared = KeychainManager()
    
    private let service = "com.yangguan.kimipocket.token"
    private let account = "kimi_access_token"
    private let lock = NSLock()
    private var cachedToken: String?
    private var isExplicitlyCleared = false
    
    public init() {}
    
    @discardableResult
    public func saveToken(_ token: String) -> Bool {
        lock.lock()
        defer { lock.unlock() }
        
        let trimmed = token.trimmingCharacters(in: .whitespacesAndNewlines)
        guard !trimmed.isEmpty, let data = trimmed.data(using: .utf8) else { return false }
        
        self.cachedToken = trimmed
        self.isExplicitlyCleared = false
        
        let query: [String: Any] = [
            kSecClass as String: kSecClassGenericPassword,
            kSecAttrService as String: service,
            kSecAttrAccount as String: account
        ]
        SecItemDelete(query as CFDictionary)
        
        var attributes = query
        attributes[kSecValueData as String] = data
        attributes[kSecAttrAccessible as String] = kSecAttrAccessibleAfterFirstUnlock
        
        SecItemAdd(attributes as CFDictionary, nil)
        return true
    }
    
    public func getToken() -> String? {
        lock.lock()
        defer { lock.unlock() }
        
        if isExplicitlyCleared {
            return nil
        }
        
        if let token = cachedToken, !token.isEmpty {
            return token
        }
        
        let query: [String: Any] = [
            kSecClass as String: kSecClassGenericPassword,
            kSecAttrService as String: service,
            kSecAttrAccount as String: account,
            kSecReturnData as String: true,
            kSecMatchLimit as String: kSecMatchLimitOne
        ]
        
        var item: CFTypeRef?
        let status = SecItemCopyMatching(query as CFDictionary, &item)
        guard status == errSecSuccess, let data = item as? Data, let token = String(data: data, encoding: .utf8) else {
            return cachedToken
        }
        
        self.cachedToken = token
        return token
    }
    
    @discardableResult
    public func deleteToken() -> Bool {
        lock.lock()
        defer { lock.unlock() }
        
        self.cachedToken = nil
        self.isExplicitlyCleared = true
        
        let query: [String: Any] = [
            kSecClass as String: kSecClassGenericPassword,
            kSecAttrService as String: service,
            kSecAttrAccount as String: account
        ]
        
        _ = SecItemDelete(query as CFDictionary)
        return true
    }
}
