import AppKit

public enum StatusIconState {
    case normal
    case warning      // 5小时用量 < 20%
    case critical     // 5小时用量 < 5%
    case offline      // 未连接或 Token 失效
}

public final class StatusIconFactory {
    
    /// 生成适配 macOS 顶栏的矢量图标 (优先系统高清晰度 SF Symbol，备选精细绘制)
    public static func makeStatusIcon(state: StatusIconState = .normal) -> NSImage {
        let symbolName: String
        switch state {
        case .normal, .warning:
            symbolName = "sparkles"
        case .critical:
            symbolName = "exclamationmark.circle"
        case .offline:
            symbolName = "sparkles"
        }
        
        if let sfImage = NSImage(systemSymbolName: symbolName, accessibilityDescription: "Kimi Pocket") {
            sfImage.isTemplate = true
            return sfImage
        }
        
        // 兜底绘制
        let size = NSSize(width: 18, height: 18)
        let image = NSImage(size: size, flipped: false) { rect in
            guard let _ = NSGraphicsContext.current?.cgContext else { return false }
            let center = CGPoint(x: rect.midX, y: rect.midY)
            NSColor.black.setStroke()
            let r: CGFloat = 4.2
            let offset: CGFloat = 2.8
            let p1 = CGPoint(x: center.x, y: center.y + offset)
            let p2 = CGPoint(x: center.x - offset * 0.866, y: center.y - offset * 0.5)
            let p3 = CGPoint(x: center.x + offset * 0.866, y: center.y - offset * 0.5)
            
            let path = NSBezierPath()
            path.lineWidth = 1.35
            path.appendOval(in: NSRect(x: p1.x - r, y: p1.y - r, width: r * 2, height: r * 2))
            path.appendOval(in: NSRect(x: p2.x - r, y: p2.y - r, width: r * 2, height: r * 2))
            path.appendOval(in: NSRect(x: p3.x - r, y: p3.y - r, width: r * 2, height: r * 2))
            path.stroke()
            return true
        }
        image.isTemplate = true
        return image
    }
}
