import SwiftUI

public enum SegmentTheme {
    case cyan
    case emerald
    
    public var activeColor: Color {
        switch self {
        case .cyan: return Color(red: 0.0, green: 0.90, blue: 1.0)
        case .emerald: return Color(red: 0.06, green: 0.73, blue: 0.51)
        }
    }
}

public struct SegmentedQuotaRow: View {
    public let title: String
    public let segmentCount: Int
    public let usageRatio: Double
    public let subtitle: String
    public let theme: SegmentTheme
    
    public init(
        title: String,
        segmentCount: Int,
        usageRatio: Double,
        subtitle: String,
        theme: SegmentTheme
    ) {
        self.title = title
        self.segmentCount = segmentCount
        self.usageRatio = max(0.0, min(1.0, usageRatio))
        self.subtitle = subtitle
        self.theme = theme
    }
    
    // 超过 80% 变黄，超过 90% 变红
    private var activeSegmentColor: Color {
        if usageRatio >= 0.90 {
            return Color(red: 0.94, green: 0.27, blue: 0.27) // 珊瑚红 #ef4444
        } else if usageRatio >= 0.80 {
            return Color(red: 0.96, green: 0.62, blue: 0.04) // 琥珀黄 #f59e0b
        } else {
            return theme.activeColor
        }
    }
    
    private var percentageColor: Color {
        if usageRatio >= 0.90 {
            return Color(red: 0.94, green: 0.27, blue: 0.27)
        } else if usageRatio >= 0.80 {
            return Color(red: 0.96, green: 0.62, blue: 0.04)
        } else {
            return Color(red: 0.97, green: 0.98, blue: 0.99)
        }
    }
    
    public var body: some View {
        VStack(alignment: .leading, spacing: 6) {
            // 标题与纯净百分比数值 (去除多余的已用/总额)
            HStack(alignment: .firstTextBaseline) {
                Text(title)
                    .font(.system(size: 11.5, weight: .medium))
                    .foregroundColor(Color(red: 0.58, green: 0.64, blue: 0.72))
                
                Spacer()
                
                Text("\(Int(round(usageRatio * 100)))%")
                    .font(.system(size: 14.5, weight: .bold, design: .monospaced))
                    .foregroundColor(percentageColor)
            }
            
            // 离散药丸条网格 (动态变色与阴影)
            HStack(spacing: 5) {
                ForEach(0..<segmentCount, id: \.self) { index in
                    let step = 1.0 / Double(segmentCount)
                    let isActive = usageRatio >= (Double(index) * step + 0.001)
                    
                    RoundedRectangle(cornerRadius: 2)
                        .fill(isActive ? activeSegmentColor : Color.white.opacity(0.09))
                        .frame(height: 7)
                        .shadow(
                            color: isActive ? activeSegmentColor.opacity(0.4) : .clear,
                            radius: 4,
                            x: 0,
                            y: 0
                        )
                }
            }
            
            // 纯净业务副标题
            if !subtitle.isEmpty {
                Text(subtitle)
                    .font(.system(size: 10.5, design: .monospaced))
                    .foregroundColor(Color(red: 0.39, green: 0.45, blue: 0.55))
                    .padding(.top, 1)
            }
        }
        .padding(EdgeInsets(top: 10, leading: 12, bottom: 10, trailing: 12))
        .background(
            RoundedRectangle(cornerRadius: 10)
                .fill(Color.white.opacity(0.03))
                .overlay(
                    RoundedRectangle(cornerRadius: 10)
                        .stroke(Color.white.opacity(0.06), lineWidth: 1)
                )
        )
    }
}
