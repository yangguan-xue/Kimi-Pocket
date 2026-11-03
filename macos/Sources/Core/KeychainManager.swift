import Foundation
import Security

public final class KeychainManager: @unchecked Sendable {
    public static let shared = KeychainManager()
    
    private let service = "com.yangguan.kimipocket.token"
    private let account = "kimi_access_token"
    private let lock = NSLock()
    private var cachedToken: String?
    
    public init() {}
    
    @discardableResult
    public func saveToken(_ token: String) -> Bool {
        lock.lock()
        defer { lock.unlock() }
        
        let trimmed = token.trimmingCharacters(in: .whitespacesAndNewlines)
        guard let data = trimmed.data(using: .utf8) else { return false }
        
        // 先删除已存在的项
        let query: [String: Any] = [
            kSecClass as String: kSecClassGenericPassword,
            kSecAttrService as String: service,
            kSecAttrAccount as String: account
        ]
        SecItemDelete(query as CFDictionary)
        
        // 添加新项
        var attributes = query
        attributes[kSecValueData as String] = data
        attributes[kSecAttrAccessible as String] = kSecAttrAccessibleAfterFirstUnlock
        
        let status = SecItemAdd(attributes as CFDictionary, nil)
        if status == errSecSuccess {
            self.cachedToken = trimmed
            return true
        }
        return false
    }
    
    public func getToken() -> String? {
        lock.lock()
        defer { lock.unlock() }
        
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
            return nil
        }
        
        self.cachedToken = token
        return token
    }
    
    @discardableResult
    public func deleteToken() -> Bool {
        lock.lock()
        defer { lock.unlock() }
        
        let query: [String: Any] = [
            kSecClass as String: kSecClassGenericPassword,
            kSecAttrService as String: service,
            kSecAttrAccount as String: account
        ]
        
        let status = SecItemDelete(query as CFDictionary)
        self.cachedToken = nil
        return status == errSecSuccess || status == errSecItemNotFound
    }
}
