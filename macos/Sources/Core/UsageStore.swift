import Foundation
import Combine
import AppKit

@MainActor
public final class UsageStore: ObservableObject {
    public static let shared = UsageStore()
    
    @Published public private(set) var usageState: KimiUsageState?
    @Published public private(set) var isLoading: Bool = false
    @Published public private(set) var errorMessage: String?
    @Published public private(set) var hasToken: Bool = false
    @Published public private(set) var lastSyncTimeFormatted: String = "刚刚"
    
    private var pollTimer: Timer?
    private var lastFetchTimestamp: Date?
    private let client: KimiClient
    private let keychain: KeychainManager
    private var cancellables = Set<AnyCancellable>()
    
    public init(client: KimiClient = .shared, keychain: KeychainManager = .shared) {
        self.client = client
        self.keychain = keychain
        
        checkTokenStatus()
        setupSleepWakeListeners()
    }
    
    public func checkTokenStatus() {
        let token = keychain.getToken()
        self.hasToken = (token != nil && !token!.isEmpty)
    }
    
    public func startPolling() {
        checkTokenStatus()
        refreshImmediately()
        scheduleNextPoll(interval: 60)
    }
    
    public func stopPolling() {
        pollTimer?.invalidate()
        pollTimer = nil
    }
    
    public func refreshImmediately() {
        guard let token = keychain.getToken(), !token.isEmpty else {
            self.hasToken = false
            self.errorMessage = "请先配置 Access Token"
            return
        }
        
        self.hasToken = true
        self.isLoading = true
        self.errorMessage = nil
        
        Task { [weak self] in
            guard let self = self else { return }
            do {
                let state = try await self.client.fetchUsageData(token: token)
                self.usageState = state
                self.lastFetchTimestamp = Date()
                self.lastSyncTimeFormatted = Self.formatTime(Date())
                self.isLoading = false
                
                // 根据剩余用量智能动态调整轮询周期
                let nextInterval: TimeInterval = state.rolling5h.usageRatio > 0.7 ? 30.0 : 60.0
                self.scheduleNextPoll(interval: nextInterval)
            } catch {
                self.isLoading = false
                self.errorMessage = error.localizedDescription
                self.scheduleNextPoll(interval: 60.0)
            }
        }
    }
    
    /// 当悬停浮窗打开时，若缓存超过 15 秒则触发即刻静默刷新
    public func onHoverHUDOpened() {
        guard hasToken else { return }
        if let last = lastFetchTimestamp {
            if Date().timeIntervalSince(last) > 15.0 {
                refreshImmediately()
            }
        } else {
            refreshImmediately()
        }
    }
    
    public func saveToken(_ token: String) {
        let trimmed = token.trimmingCharacters(in: .whitespacesAndNewlines)
        if keychain.saveToken(trimmed) {
            self.hasToken = true
            self.errorMessage = nil
            refreshImmediately()
        }
    }
    
    public func deleteToken() {
        keychain.deleteToken()
        self.hasToken = false
        self.usageState = nil
        self.errorMessage = "Token 已移除"
    }
    
    private func scheduleNextPoll(interval: TimeInterval) {
        pollTimer?.invalidate()
        pollTimer = Timer.scheduledTimer(withTimeInterval: interval, repeats: false) { [weak self] _ in
            Task { @MainActor [weak self] in
                self?.refreshImmediately()
            }
        }
    }
    
    private func setupSleepWakeListeners() {
        NSWorkspace.shared.notificationCenter.addObserver(
            forName: NSWorkspace.willSleepNotification,
            object: nil,
            queue: .main
        ) { [weak self] _ in
            Task { @MainActor [weak self] in
                self?.stopPolling()
            }
        }
        
        NSWorkspace.shared.notificationCenter.addObserver(
            forName: NSWorkspace.didWakeNotification,
            object: nil,
            queue: .main
        ) { [weak self] _ in
            Task { @MainActor [weak self] in
                self?.startPolling()
            }
        }
    }
    
    private static func formatTime(_ date: Date) -> String {
        let formatter = DateFormatter()
        formatter.dateFormat = "HH:mm:ss"
        return formatter.string(from: date)
    }
}
