import AppKit
import Foundation

@MainActor
public protocol HoverCoordinatorDelegate: AnyObject {
    func hoverCoordinatorShouldShowHUD(_ coordinator: HoverCoordinator)
    func hoverCoordinatorShouldHideHUD(_ coordinator: HoverCoordinator)
}

@MainActor
public final class HoverCoordinator: NSObject {
    public weak var delegate: HoverCoordinatorDelegate?
    
    // 状态配置参数 (纯粹的即时悬停模型)
    public let hoverDwellThreshold: TimeInterval = 0.18 // 180ms 激活防抖
    public let exitGracePeriod: TimeInterval = 0.25     // 250ms 离开容错缓冲
    
    // 交互状态 (彻底移除 Pin，纯即时悬停)
    public private(set) var isHoveringButton = false
    public private(set) var isHoveringHUD = false
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
    
    // MARK: - 显隐控制 (非锁定，单纯开关)
    
    public func toggleVisibility() {
        if isHUDVisible {
            dismissImmediately()
        } else {
            showHUD()
        }
    }
    
    public func dismissImmediately() {
        dwellTimer?.invalidate()
        dwellTimer = nil
        exitTimer?.invalidate()
        exitTimer = nil
        hideHUD()
    }
    
    // MARK: - 私有控制方法
    
    private func showHUD() {
        guard !isHUDVisible else { return }
        isHUDVisible = true
        delegate?.hoverCoordinatorShouldShowHUD(self)
    }
    
    private func hideHUD() {
        guard isHUDVisible else { return }
        isHUDVisible = false
        delegate?.hoverCoordinatorShouldHideHUD(self)
    }
    
    private func scheduleExitCheck() {
        guard isHUDVisible else { return }
        
        exitTimer?.invalidate()
        exitTimer = Timer.scheduledTimer(withTimeInterval: exitGracePeriod, repeats: false) { [weak self] _ in
            Task { @MainActor [weak self] in
                guard let self = self else { return }
                if !self.isHoveringButton && !self.isHoveringHUD {
                    self.hideHUD()
                }
            }
        }
    }
}
