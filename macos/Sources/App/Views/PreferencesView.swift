import SwiftUI
import ServiceManagement
import KimiPocketCore

public struct PreferencesView: View {
    @ObservedObject public var store: UsageStore
    @State private var inputToken: String = ""
    @State private var launchAtLogin: Bool = false
    @State private var saveMessage: String?
    @State private var isError: Bool = false
    
    @MainActor
    public init(store: UsageStore) {
        self.store = store
    }
    
    public var body: some View {
        VStack(alignment: .leading, spacing: 18) {
            Text("Kimi Pocket 偏好设置")
                .font(.headline)
                .foregroundColor(.white)
            
            // Token 配置区
            VStack(alignment: .leading, spacing: 8) {
                Text("Kimi Access Token (JWT):")
                    .font(.caption)
                    .foregroundColor(.gray)
                
                SecureField("粘贴从 kimi.com 获取的 access_token", text: $inputToken)
                    .textFieldStyle(.roundedBorder)
                    .font(.system(size: 12, design: .monospaced))
                
                // 实时预检提示
                if !inputToken.isEmpty {
                    if let payload = JWTParser.extractPayload(inputToken) {
                        if JWTParser.isTokenExpired(inputToken) {
                            Text("⚠️ 该 Token 已经过期，无法使用")
                                .font(.caption2)
                                .foregroundColor(.red)
                        } else if JWTParser.isTokenNearExpiry(inputToken) {
                            Text("⏳ Token 有效（将于 1 小时内过期）")
                                .font(.caption2)
                                .foregroundColor(.orange)
                        } else {
                            Text("✅ Token 格式有效 (用户 ID: \(payload.sub ?? "未知"))")
                                .font(.caption2)
                                .foregroundColor(.green)
                        }
                    } else {
                        Text("❌ 非合法的 JWT Token 格式")
                            .font(.caption2)
                            .foregroundColor(.red)
                    }
                }
                
                HStack {
                    Button("保存并同步") {
                        handleSaveToken()
                    }
                    .buttonStyle(.borderedProminent)
                    .tint(Color(red: 0.0, green: 0.7, blue: 0.9))
                    .disabled(inputToken.trimmingCharacters(in: .whitespacesAndNewlines).isEmpty)
                    
                    if store.hasToken {
                        Button("清除 Token", role: .destructive) {
                            store.deleteToken()
                            inputToken = ""
                            saveMessage = "Token 已成功清除"
                            isError = false
                        }
                    }
                }
            }
            
            Divider()
                .background(Color.white.opacity(0.1))
            
            // 开机启动选项
            VStack(alignment: .leading, spacing: 8) {
                Toggle("登录系统时自动启动", isOn: $launchAtLogin)
                    .onChange(of: launchAtLogin) { newValue in
                        updateLaunchAtLogin(newValue)
                    }
                    .font(.system(size: 13))
            }
            
            if let msg = saveMessage {
                Text(msg)
                    .font(.caption)
                    .foregroundColor(isError ? .red : .green)
            }
            
            Spacer()
        }
        .padding(20)
        .frame(width: 440, height: 280)
        .background(Color(red: 0.09, green: 0.10, blue: 0.13))
        .onAppear {
            checkCurrentSettings()
        }
    }
    
    private func checkCurrentSettings() {
        if let current = KeychainManager.shared.getToken() {
            self.inputToken = current
        }
        
        if #available(macOS 13.0, *) {
            self.launchAtLogin = (SMAppService.mainApp.status == .enabled)
        }
    }
    
    private func handleSaveToken() {
        let trimmed = inputToken.trimmingCharacters(in: .whitespacesAndNewlines)
        guard !trimmed.isEmpty else { return }
        
        guard JWTParser.extractPayload(trimmed) != nil else {
            saveMessage = "保存失败: 不是有效的 JWT 格式"
            isError = true
            return
        }
        
        if JWTParser.isTokenExpired(trimmed) {
            saveMessage = "警告: 该 Token 已过期，请确认后保存"
            isError = true
        }
        
        store.saveToken(trimmed)
        saveMessage = "Token 已安全保存至系统钥匙串并开始同步"
        isError = false
    }
    
    private func updateLaunchAtLogin(_ enable: Bool) {
        if #available(macOS 13.0, *) {
            do {
                if enable {
                    try SMAppService.mainApp.register()
                } else {
                    try SMAppService.mainApp.unregister()
                }
            } catch {
                NSLog("[Preferences] Failed to toggle launch at login: \(error)")
            }
        }
    }
}
