import AppKit
import SwiftUI
import KimiPocketCore

@MainActor
public final class PopoverWindow: NSPanel {
    private let visualEffectView = NSVisualEffectView()
    private weak var hoverCoordinator: HoverCoordinator?
    private var trackingArea: NSTrackingArea?
    private var eventMonitor: Any?
    private var clickMonitor: Any?
    private var mouseCheckTimer: Timer?
    
    public init(contentRect: NSRect, hoverCoordinator: HoverCoordinator) {
        self.hoverCoordinator = hoverCoordinator
        
        super.init(
            contentRect: contentRect,
            styleMask: [.nonactivatingPanel, .borderless, .fullSizeContentView],
            backing: .buffered,
            defer: false
        )
        
        setupWindow()
        setupVisualEffect()
        setupTrackingArea()
        setupGlobalEscMonitor()
    }
    
    private func setupWindow() {
        self.isOpaque = false
        self.backgroundColor = .clear
        self.level = .floating
        self.hasShadow = true
        self.isMovableByWindowBackground = false
        self.hidesOnDeactivate = false
        self.collectionBehavior = [.canJoinAllSpaces, .transient]
    }
    
    private func setupVisualEffect() {
        visualEffectView.material = .hudWindow
        visualEffectView.state = .active
        visualEffectView.blendingMode = .behindWindow
        visualEffectView.wantsLayer = true
        visualEffectView.layer?.cornerRadius = 14
        visualEffectView.layer?.masksToBounds = true
        visualEffectView.layer?.borderWidth = 1
        visualEffectView.layer?.borderColor = NSColor(white: 1.0, alpha: 0.12).cgColor
        
        self.contentView = visualEffectView
    }
    
    public func setContent<Content: View>(_ view: Content) {
        let hostingView = NSHostingView(rootView: view)
        hostingView.translatesAutoresizingMaskIntoConstraints = false
        visualEffectView.subviews.forEach { $0.removeFromSuperview() }
        visualEffectView.addSubview(hostingView)
        
        NSLayoutConstraint.activate([
            hostingView.topAnchor.constraint(equalTo: visualEffectView.topAnchor),
            hostingView.leadingAnchor.constraint(equalTo: visualEffectView.leadingAnchor),
            hostingView.trailingAnchor.constraint(equalTo: visualEffectView.trailingAnchor),
            hostingView.bottomAnchor.constraint(equalTo: visualEffectView.bottomAnchor)
        ])
    }
    
    private func setupTrackingArea() {
        let area = NSTrackingArea(
            rect: .zero,
            options: [.mouseEnteredAndExited, .activeAlways, .inVisibleRect],
            owner: self,
            userInfo: nil
        )
        visualEffectView.addTrackingArea(area)
        self.trackingArea = area
    }
    
    private func setupGlobalEscMonitor() {
        eventMonitor = NSEvent.addLocalMonitorForEvents(matching: .keyDown) { [weak self] event in
            guard let self = self, self.isVisible else { return event }
            if event.keyCode == 53 { // ESC 键
                self.hoverCoordinator?.dismissImmediately()
                return nil
            }
            return event
        }
    }
    
    public override func mouseEntered(with event: NSEvent) {
        super.mouseEntered(with: event)
        hoverCoordinator?.onHUDMouseEnter()
    }
    
    public override func mouseExited(with event: NSEvent) {
        super.mouseExited(with: event)
        hoverCoordinator?.onHUDMouseExit()
    }
    
    /// 将浮窗以动画淡入并锚定在状态栏按钮下方
    public func showAnchored(to button: NSStatusBarButton) {
        guard let buttonWindow = button.window, let screen = buttonWindow.screen else { return }
        
        let buttonFrameOnScreen = buttonWindow.convertToScreen(button.bounds)
        // 若系统尚未完成状态栏窗口布局定位（Y 坐标接近 0），延迟 0.15 秒重试
        if buttonFrameOnScreen.origin.y < 100 {
            DispatchQueue.main.asyncAfter(deadline: .now() + 0.15) { [weak self, weak button] in
                guard let self = self, let button = button else { return }
                self.showAnchored(to: button)
            }
            return
        }
        let hudSize = self.frame.size
        
        // 计算目标 X: 居中对齐按钮中心
        var targetX = buttonFrameOnScreen.midX - (hudSize.width / 2.0)
        let targetY = buttonFrameOnScreen.minY - hudSize.height - 6.0
        
        // 屏幕边缘自适应防溢出
        let screenFrame = screen.visibleFrame
        if targetX + hudSize.width > screenFrame.maxX - 8 {
            targetX = screenFrame.maxX - hudSize.width - 8
        }
        if targetX < screenFrame.minX + 8 {
            targetX = screenFrame.minX + 8
        }
        
        let targetOrigin = CGPoint(x: targetX, y: targetY)
        self.setFrameOrigin(targetOrigin)
        
        self.alphaValue = 0.0
        self.orderFrontRegardless()
        
        // 注册全局点击监听：点击外部区域立即收起（非钉住状态下）
        setupOutsideClickMonitor()
        
        // 启动光标位置轮询守卫，确保离开区域后平滑收起
        startTrackingMouseExit(button: button)
        
        NSAnimationContext.runAnimationGroup { context in
            context.duration = 0.15
            context.timingFunction = CAMediaTimingFunction(name: .easeOut)
            self.animator().alphaValue = 1.0
        }
    }
    
    private func setupOutsideClickMonitor() {
        if clickMonitor == nil {
            clickMonitor = NSEvent.addGlobalMonitorForEvents(matching: [.leftMouseDown, .rightMouseDown]) { [weak self] _ in
                Task { @MainActor [weak self] in
                    guard let self = self, self.isVisible else { return }
                    self.hoverCoordinator?.dismissImmediately()
                }
            }
        }
    }
    
    private func startTrackingMouseExit(button: NSStatusBarButton) {
        mouseCheckTimer?.invalidate()
        mouseCheckTimer = Timer.scheduledTimer(withTimeInterval: 0.1, repeats: true) { [weak self, weak button] timer in
            Task { @MainActor [weak self, weak button] in
                guard let self = self, let button = button, self.isVisible else {
                    timer.invalidate()
                    return
                }
                guard let coordinator = self.hoverCoordinator else { return }
                
                let mouseLoc = NSEvent.mouseLocation
                let buttonFrame = button.window?.convertToScreen(button.bounds) ?? .zero
                
                // 允许 14px 容差缓冲区（移动过程中不发生闪烁）
                let hudBounds = self.frame.insetBy(dx: -14, dy: -14)
                let buttonBounds = buttonFrame.insetBy(dx: -8, dy: -8)
                
                let isInside = hudBounds.contains(mouseLoc) || buttonBounds.contains(mouseLoc)
                if !isInside {
                    coordinator.onButtonMouseExit()
                    coordinator.onHUDMouseExit()
                }
            }
        }
    }
    
    /// 退出淡出动画
    public func hideWithFade() {
        mouseCheckTimer?.invalidate()
        mouseCheckTimer = nil
        if let monitor = clickMonitor {
            NSEvent.removeMonitor(monitor)
            clickMonitor = nil
        }
        
        NSAnimationContext.runAnimationGroup({ context in
            context.duration = 0.15
            context.timingFunction = CAMediaTimingFunction(name: .easeIn)
            self.animator().alphaValue = 0.0
        }, completionHandler: { [weak self] in
            self?.orderOut(nil)
        })
    }
    
    deinit {
        mouseCheckTimer?.invalidate()
        if let monitor = eventMonitor {
            NSEvent.removeMonitor(monitor)
        }
        if let monitor = clickMonitor {
            NSEvent.removeMonitor(monitor)
        }
    }
}
