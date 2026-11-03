import AppKit
import KimiPocketCore

@MainActor
class AppDelegate: NSObject, NSApplicationDelegate {
    private var statusItemController: StatusItemController?

    func applicationDidFinishLaunching(_ notification: Notification) {
        // 设置为后台辅助 App，不在 Dock 栏常驻
        NSApp.setActivationPolicy(.accessory)
        
        // 激活基础系统编辑菜单 (支持 ⌘C, ⌘V, ⌘X, ⌘A 剪贴板快捷键)
        setupMainMenu()
        
        let controller = StatusItemController()
        self.statusItemController = controller
        
        NSLog("[KimiPocket] Application launched as menubar accessory.")
        
        // 首次启动若未找到 Token，立即弹出偏好设置窗口指引用户
        if !UsageStore.shared.hasToken {
            controller.openPreferencesAction()
        }
    }
    
    private func setupMainMenu() {
        let mainMenu = NSMenu()
        
        let editMenuItem = NSMenuItem()
        let editMenu = NSMenu(title: "Edit")
        editMenu.addItem(withTitle: "Undo", action: Selector(("undo:")), keyEquivalent: "z")
        editMenu.addItem(withTitle: "Redo", action: Selector(("redo:")), keyEquivalent: "Z")
        editMenu.addItem(NSMenuItem.separator())
        editMenu.addItem(withTitle: "Cut", action: #selector(NSText.cut(_:)), keyEquivalent: "x")
        editMenu.addItem(withTitle: "Copy", action: #selector(NSText.copy(_:)), keyEquivalent: "c")
        editMenu.addItem(withTitle: "Paste", action: #selector(NSText.paste(_:)), keyEquivalent: "v")
        editMenu.addItem(withTitle: "Select All", action: #selector(NSText.selectAll(_:)), keyEquivalent: "a")
        
        editMenuItem.submenu = editMenu
        mainMenu.addItem(editMenuItem)
        NSApp.mainMenu = mainMenu
    }
    
    func applicationWillTerminate(_ notification: Notification) {
        NSLog("[KimiPocket] Application terminating.")
    }
}
