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
    public let used: Int
    public let limit: Int
    public let subtitle: String
    public let theme: SegmentTheme
    
    public init(
        title: String,
        segmentCount: Int,
        usageRatio: Double,
        used: Int,
        limit: Int,
        subtitle: String,
        theme: SegmentTheme
    ) {
        self.title = title
        self.segmentCount = segmentCount
        self.usageRatio = max(0.0, min(1.0, usageRatio))
        self.used = used
        self.limit = limit
        self.subtitle = subtitle
        self.theme = theme
    }
    
    public var body: some View {
        VStack(alignment: .leading, spacing: 6) {
            // 标题与数值
            HStack(alignment: .firstTextBaseline) {
                Text(title)
                    .font(.system(size: 11.5, weight: .medium))
                    .foregroundColor(Color(red: 0.58, green: 0.64, blue: 0.72))
                
                Spacer()
                
                HStack(alignment: .firstTextBaseline, spacing: 6) {
                    Text("\(Int(round(usageRatio * 100)))%")
                        .font(.system(size: 14.5, weight: .bold, design: .monospaced))
                        .foregroundColor(Color(red: 0.97, green: 0.98, blue: 0.99))
                    
                    Text("已用 \(used) / \(limit)")
                        .font(.system(size: 11, design: .monospaced))
                        .foregroundColor(Color(red: 0.39, green: 0.45, blue: 0.55))
                }
            }
            
            // 离散药丸条网格
            HStack(spacing: 5) {
                ForEach(0..<segmentCount, id: \.self) { index in
                    let step = 1.0 / Double(segmentCount)
                    let isActive = usageRatio >= (Double(index) * step + 0.001)
                    
                    RoundedRectangle(cornerRadius: 2)
                        .fill(isActive ? theme.activeColor : Color.white.opacity(0.09))
                        .frame(height: 7)
                        .shadow(
                            color: isActive ? theme.activeColor.opacity(0.4) : .clear,
                            radius: 4,
                            x: 0,
                            y: 0
                        )
                }
            }
            
            // 纯净业务副标题（严禁包含“7段/4段”等工程文字）
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
