import AppKit
import KimiPocketCore

@main
@MainActor
class AppDelegate: NSObject, NSApplicationDelegate {
    private var statusItemController: StatusItemController?

    func applicationDidFinishLaunching(_ notification: Notification) {
        // 设置为后台辅助 App，不在 Dock 栏常驻
        NSApp.setActivationPolicy(.accessory)
        
        let controller = StatusItemController()
        self.statusItemController = controller
        
        NSLog("[KimiPocket] Application launched as menubar accessory.")
        
        // 首次启动若未找到 Token，立即弹出偏好设置窗口指引用户
        if !UsageStore.shared.hasToken {
            controller.openPreferencesAction()
        }
    }
    
    func applicationWillTerminate(_ notification: Notification) {
        NSLog("[KimiPocket] Application terminating.")
    }
}
