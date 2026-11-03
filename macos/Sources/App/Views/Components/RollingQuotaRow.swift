import SwiftUI
import KimiPocketCore

public struct RollingQuotaRow: View {
    public let info: KimiUsageState.RollingQuotaInfo
    
    public init(info: KimiUsageState.RollingQuotaInfo) {
        self.info = info
    }
    
    // 超过 80% 变黄，超过 90% 变红
    private var progressGradient: [Color] {
        if info.usageRatio >= 0.90 {
            return [Color(red: 0.97, green: 0.44, blue: 0.44), Color(red: 0.94, green: 0.27, blue: 0.27)]
        } else if info.usageRatio >= 0.80 {
            return [Color(red: 0.98, green: 0.75, blue: 0.14), Color(red: 0.96, green: 0.62, blue: 0.04)]
        } else {
            return [Color(red: 0.0, green: 0.90, blue: 1.0), Color(red: 0.06, green: 0.73, blue: 0.51)]
        }
    }
    
    private var themeColor: Color {
        if info.usageRatio >= 0.90 {
            return Color(red: 0.94, green: 0.27, blue: 0.27)
        } else if info.usageRatio >= 0.80 {
            return Color(red: 0.96, green: 0.62, blue: 0.04)
        } else {
            return Color(red: 0.0, green: 0.90, blue: 1.0)
        }
    }
    
    private var percentageColor: Color {
        if info.usageRatio >= 0.90 {
            return Color(red: 0.94, green: 0.27, blue: 0.27)
        } else if info.usageRatio >= 0.80 {
            return Color(red: 0.96, green: 0.62, blue: 0.04)
        } else {
            return Color(red: 0.97, green: 0.98, blue: 0.99)
        }
    }
    
    public var body: some View {
        VStack(alignment: .leading, spacing: 6) {
            // 标题行：纯净百分比，去除“余多少/多少”
            HStack(alignment: .firstTextBaseline) {
                Text("5小时额度")
                    .font(.system(size: 11.5, weight: .medium))
                    .foregroundColor(Color(red: 0.58, green: 0.64, blue: 0.72))
                
                Spacer()
                
                Text("\(Int(round(info.usageRatio * 100)))%")
                    .font(.system(size: 14.5, weight: .bold, design: .monospaced))
                    .foregroundColor(percentageColor)
            }
            
            // 平滑连续进度条 (动态预警变色)
            GeometryReader { proxy in
                ZStack(alignment: .leading) {
                    Capsule()
                        .fill(Color.white.opacity(0.09))
                        .frame(height: 8)
                    
                    Capsule()
                        .fill(
                            LinearGradient(
                                colors: progressGradient,
                                startPoint: .leading,
                                endPoint: .trailing
                            )
                        )
                        .frame(width: max(0, proxy.size.width * CGFloat(min(1.0, info.usageRatio))), height: 8)
                        .shadow(color: themeColor.opacity(0.45), radius: 6, x: 0, y: 0)
                }
            }
            .frame(height: 8)
            
            // 倒计时重置标注
            if !info.resetCountdown.isEmpty {
                HStack(spacing: 5) {
                    Circle()
                        .fill(themeColor)
                        .frame(width: 5, height: 5)
                        .shadow(color: themeColor, radius: 3)
                    
                    Text("\(info.resetCountdown) 后开始恢复")
                        .font(.system(size: 11, design: .monospaced))
                        .foregroundColor(themeColor)
                }
                .padding(.top, 2)
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
