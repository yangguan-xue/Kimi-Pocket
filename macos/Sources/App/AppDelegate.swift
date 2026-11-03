import AppKit

@main
@MainActor
class AppDelegate: NSObject, NSApplicationDelegate {
    private var statusItemController: StatusItemController?

    func applicationDidFinishLaunching(_ notification: Notification) {
        // 设置为后台辅助 App，即使未打包为 .app 也能隐藏 Dock 图标
        NSApp.setActivationPolicy(.accessory)
        
        statusItemController = StatusItemController()
        NSLog("[KimiPocket] Application launched as menubar accessory.")
    }
    
    func applicationWillTerminate(_ notification: Notification) {
        NSLog("[KimiPocket] Application terminating.")
    }
}
