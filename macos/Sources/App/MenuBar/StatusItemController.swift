import AppKit
import SwiftUI
import Combine
import KimiPocketCore

@MainActor
public final class StatusItemController: NSObject, HoverCoordinatorDelegate {
    private var statusItem: NSStatusItem?
    private var trackingArea: NSTrackingArea?
    public let hoverCoordinator = HoverCoordinator()
    private var popoverWindow: PopoverWindow?
    private var preferencesWindow: NSWindow?
    private let store = UsageStore.shared
    private var cancellables = Set<AnyCancellable>()
    
    public override init() {
        super.init()
        hoverCoordinator.delegate = self
        setupStatusItem()
        setupPopoverWindow()
        setupStoreBinding()
        store.startPolling()
    }
    
    private func setupStoreBinding() {
        store.$usageState
            .receive(on: DispatchQueue.main)
            .sink { [weak self] state in
                guard let self = self else { return }
                if let ratio = state?.rolling5h.usageRatio {
                    if ratio >= 0.90 {
                        self.statusItem?.button?.image = StatusIconFactory.makeStatusIcon(state: .critical)
                    } else if ratio >= 0.80 {
                        self.statusItem?.button?.image = StatusIconFactory.makeStatusIcon(state: .warning)
                    } else {
                        self.statusItem?.button?.image = StatusIconFactory.makeStatusIcon(state: .normal)
                    }
                }
                self.updateHUDViewContent()
            }
            .store(in: &cancellables)
    }
    
    private func setupStatusItem() {
        let item = NSStatusBar.system.statusItem(withLength: NSStatusItem.squareLength)
        self.statusItem = item
        
        if let button = item.button {
            button.image = StatusIconFactory.makeStatusIcon(state: .normal)
            button.imagePosition = .imageOnly
            button.toolTip = "Kimi Pocket (鼠标悬停查看用量)"
            button.target = self
            button.action = #selector(handleButtonClick(_:))
            button.sendAction(on: [.leftMouseUp, .rightMouseUp])
            
            let area = NSTrackingArea(
                rect: .zero,
                options: [.mouseEnteredAndExited, .activeAlways, .inVisibleRect],
                owner: self,
                userInfo: nil
            )
            button.addTrackingArea(area)
            self.trackingArea = area
        }
    }
    
    private func setupPopoverWindow() {
        let defaultRect = NSRect(x: 0, y: 0, width: 320, height: 360)
        let window = PopoverWindow(contentRect: defaultRect, hoverCoordinator: hoverCoordinator)
        
        let hudView = HoverHUDView(
            store: store,
            onOpenPreferences: { [weak self] in
                self?.openPreferencesAction()
            }
        )
        window.setContent(hudView)
        self.popoverWindow = window
    }
    
    // MARK: - 鼠标事件转发到 HoverCoordinator
    
    @objc nonisolated public func mouseEntered(with event: NSEvent) {
        Task { @MainActor [weak self] in
            self?.hoverCoordinator.onButtonMouseEnter()
        }
    }
    
    @objc nonisolated public func mouseExited(with event: NSEvent) {
        Task { @MainActor [weak self] in
            self?.hoverCoordinator.onButtonMouseExit()
        }
    }
    
    // MARK: - 按钮点击
    
    @objc private func handleButtonClick(_ sender: NSStatusBarButton) {
        guard let currentEvent = NSApp.currentEvent else { return }
        
        if currentEvent.type == .rightMouseUp {
            showContextMenu()
        } else {
            hoverCoordinator.toggleVisibility()
        }
    }
    
    // MARK: - HoverCoordinatorDelegate
    
    public func hoverCoordinatorShouldShowHUD(_ coordinator: HoverCoordinator) {
        guard let button = statusItem?.button else { return }
        store.onHoverHUDOpened()
        updateHUDViewContent()
        popoverWindow?.showAnchored(to: button)
    }
    
    public func hoverCoordinatorShouldHideHUD(_ coordinator: HoverCoordinator) {
        popoverWindow?.hideWithFade()
    }
    
    private func updateHUDViewContent() {
        let hudView = HoverHUDView(
            store: store,
            onOpenPreferences: { [weak self] in
                self?.openPreferencesAction()
            }
        )
        popoverWindow?.setContent(hudView)
    }
    
    // MARK: - 右键上下文菜单与偏好设置
    
    public func showContextMenu() {
        let menu = NSMenu()
        
        let refreshItem = NSMenuItem(title: "立即刷新", action: #selector(refreshAction), keyEquivalent: "r")
        refreshItem.target = self
        menu.addItem(refreshItem)
        
        menu.addItem(NSMenuItem.separator())
        
        let settingsItem = NSMenuItem(title: "偏好设置...", action: #selector(openPreferencesAction), keyEquivalent: ",")
        settingsItem.target = self
        menu.addItem(settingsItem)
        
        menu.addItem(NSMenuItem.separator())
        
        let quitItem = NSMenuItem(title: "退出 Kimi Pocket", action: #selector(quitAction), keyEquivalent: "q")
        quitItem.target = self
        menu.addItem(quitItem)
        
        statusItem?.menu = menu
        statusItem?.button?.performClick(nil)
        statusItem?.menu = nil
    }
    
    @objc private func refreshAction() {
        store.refreshImmediately()
    }
    
    @objc public func openPreferencesAction() {
        if let window = preferencesWindow {
            window.makeKeyAndOrderFront(nil)
            NSApp.activate(ignoringOtherApps: true)
            return
        }
        
        let window = NSWindow(
            contentRect: NSRect(x: 0, y: 0, width: 440, height: 280),
            styleMask: [.titled, .closable, .miniaturizable],
            backing: .buffered,
            defer: false
        )
        window.center()
        window.title = "Kimi Pocket 偏好设置"
        window.isReleasedWhenClosed = false
        window.collectionBehavior = [.canJoinAllSpaces, .fullScreenAuxiliary]
        window.level = .floating
        window.contentView = NSHostingView(rootView: PreferencesView(store: store))
        
        self.preferencesWindow = window
        window.makeKeyAndOrderFront(nil)
        NSApp.activate(ignoringOtherApps: true)
    }
    
    @objc private func quitAction() {
        NSApplication.shared.terminate(nil)
    }
}
