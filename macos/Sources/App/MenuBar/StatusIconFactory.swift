import AppKit

public enum StatusIconState {
    case normal
    case warning      // 5小时用量 < 20%
    case critical     // 5小时用量 < 5%
    case offline      // 未连接或 Token 失效
}

public final class StatusIconFactory {
    
    /// 生成适配 macOS 顶栏的 Template 矢量图标 (18x18 pt)
    public static func makeStatusIcon(state: StatusIconState = .normal) -> NSImage {
        let size = NSSize(width: 18, height: 18)
        let image = NSImage(size: size, flipped: false) { rect in
            guard let context = NSGraphicsContext.current?.cgContext else { return false }
            
            // 绘制主图标：Kimi 标志性极简几何月牙与三环弧
            context.saveGState()
            
            let center = CGPoint(x: rect.midX, y: rect.midY)
            let strokeColor = NSColor.black // isTemplate 会自动反色为系统菜单栏色
            strokeColor.setStroke()
            
            // 绘制三环花瓣/连环交叉符号
            let r: CGFloat = 4.2
            let offset: CGFloat = 2.8
            
            let p1 = CGPoint(x: center.x, y: center.y + offset)
            let p2 = CGPoint(x: center.x - offset * 0.866, y: center.y - offset * 0.5)
            let p3 = CGPoint(x: center.x + offset * 0.866, y: center.y - offset * 0.5)
            
            let path = NSBezierPath()
            path.lineWidth = 1.35
            path.lineCapStyle = .round
            path.lineJoinStyle = .round
            
            // 绘制 3 个相切环
            path.appendOval(in: NSRect(x: p1.x - r, y: p1.y - r, width: r * 2, height: r * 2))
            path.appendOval(in: NSRect(x: p2.x - r, y: p2.y - r, width: r * 2, height: r * 2))
            path.appendOval(in: NSRect(x: p3.x - r, y: p3.y - r, width: r * 2, height: r * 2))
            path.stroke()
            
            context.restoreGState()
            
            // 针对状态绘制微型 Badge
            if state == .warning || state == .critical {
                context.saveGState()
                let dotRect = NSRect(x: rect.maxX - 4, y: rect.minY, width: 4, height: 4)
                let dotPath = NSBezierPath(ovalIn: dotRect)
                NSColor.black.setFill()
                dotPath.fill()
                context.restoreGState()
            }
            
            return true
        }
        
        image.isTemplate = true // 关键：系统自动适应暗色/浅色模式
        return image
    }
}
