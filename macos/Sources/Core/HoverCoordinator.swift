import AppKit
import Foundation

@MainActor
public protocol HoverCoordinatorDelegate: AnyObject {
    func hoverCoordinatorShouldShowHUD(_ coordinator: HoverCoordinator)
    func hoverCoordinatorShouldHideHUD(_ coordinator: HoverCoordinator)
    func hoverCoordinatorPinStateDidChange(_ coordinator: HoverCoordinator, isPinned: Bool)
}

@MainActor
public final class HoverCoordinator: NSObject {
    public weak var delegate: HoverCoordinatorDelegate?
    
    // 状态配置参数 (来自 SPEC-06)
    public let hoverDwellThreshold: TimeInterval = 0.18 // 180ms
    public let exitGracePeriod: TimeInterval = 0.25     // 250ms
    
    // 交互状态
    public private(set) var isHoveringButton = false
    public private(set) var isHoveringHUD = false
    public private(set) var isPinned = false
    public private(set) var isHUDVisible = false
    
    private var dwellTimer: Timer?
    private var exitTimer: Timer?
    
    public override init() {
        super.init()
    }
    
    // MARK: - 状态栏图标事件
    
    public func onButtonMouseEnter() {
        isHoveringButton = true
        exitTimer?.invalidate()
        exitTimer = nil
        
        guard !isHUDVisible else { return }
        
        // 启动 180ms 防抖计时
        dwellTimer?.invalidate()
        dwellTimer = Timer.scheduledTimer(withTimeInterval: hoverDwellThreshold, repeats: false) { [weak self] _ in
            Task { @MainActor [weak self] in
                guard let self = self, self.isHoveringButton else { return }
                self.showHUD()
            }
        }
    }
    
    public func onButtonMouseExit() {
        isHoveringButton = false
        dwellTimer?.invalidate()
        dwellTimer = nil
        
        scheduleExitCheck()
    }
    
    // MARK: - HUD 浮窗区域事件
    
    public func onHUDMouseEnter() {
        isHoveringHUD = true
        exitTimer?.invalidate()
        exitTimer = nil
    }
    
    public func onHUDMouseExit() {
        isHoveringHUD = false
        scheduleExitCheck()
    }
    
    // MARK: - 点击与模式切换
    
    public func togglePin() {
        isPinned.toggle()
        delegate?.hoverCoordinatorPinStateDidChange(self, isPinned: isPinned)
        
        if isPinned && !isHUDVisible {
            showHUD()
        } else if !isPinned && !isHoveringButton && !isHoveringHUD {
            hideHUD()
        }
    }
    
    public func setPinned(_ pinned: Bool) {
        guard isPinned != pinned else { return }
        isPinned = pinned
        delegate?.hoverCoordinatorPinStateDidChange(self, isPinned: isPinned)
    }
    
    public func dismissImmediately() {
        dwellTimer?.invalidate()
        dwellTimer = nil
        exitTimer?.invalidate()
        exitTimer = nil
        isPinned = false
        hideHUD()
    }
    
    // MARK: - 私有控制方法
    
    private func showHUD() {
        guard !isHUDVisible else { return }
        isHUDVisible = true
        delegate?.hoverCoordinatorShouldShowHUD(self)
    }
    
    private func hideHUD() {
        guard isHUDVisible, !isPinned else { return }
        isHUDVisible = false
        delegate?.hoverCoordinatorShouldHideHUD(self)
    }
    
    private func scheduleExitCheck() {
        guard isHUDVisible, !isPinned else { return }
        
        exitTimer?.invalidate()
        exitTimer = Timer.scheduledTimer(withTimeInterval: exitGracePeriod, repeats: false) { [weak self] _ in
            Task { @MainActor [weak self] in
                guard let self = self else { return }
                if !self.isHoveringButton && !self.isHoveringHUD && !self.isPinned {
                    self.hideHUD()
                }
            }
        }
    }
}
