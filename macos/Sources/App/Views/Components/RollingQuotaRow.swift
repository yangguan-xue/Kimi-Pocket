import SwiftUI
import KimiPocketCore

public struct RollingQuotaRow: View {
    public let info: KimiUsageState.RollingQuotaInfo
    
    public init(info: KimiUsageState.RollingQuotaInfo) {
        self.info = info
    }
    
    public var body: some View {
        VStack(alignment: .leading, spacing: 6) {
            // 标题行
            HStack(alignment: .firstTextBaseline) {
                Text("5小时额度")
                    .font(.system(size: 11.5, weight: .medium))
                    .foregroundColor(Color(red: 0.58, green: 0.64, blue: 0.72)) // #94a3b8
                
                Spacer()
                
                HStack(alignment: .firstTextBaseline, spacing: 6) {
                    Text("\(Int(round(info.usageRatio * 100)))%")
                        .font(.system(size: 14.5, weight: .bold, design: .monospaced))
                        .foregroundColor(Color(red: 0.97, green: 0.98, blue: 0.99)) // #f8fafc
                    
                    Text("余 \(info.remaining) / \(info.limit) 次")
                        .font(.system(size: 11, design: .monospaced))
                        .foregroundColor(Color(red: 0.39, green: 0.45, blue: 0.55)) // #64748b
                }
            }
            
            // 平滑连续进度条
            GeometryReader { proxy in
                ZStack(alignment: .leading) {
                    // 底槽
                    Capsule()
                        .fill(Color.white.opacity(0.09))
                        .frame(height: 8)
                    
                    // 渐变高亮进度条
                    Capsule()
                        .fill(
                            LinearGradient(
                                colors: [
                                    Color(red: 0.0, green: 0.90, blue: 1.0), // 冰蓝
                                    Color(red: 0.06, green: 0.73, blue: 0.51) // 冷翠绿
                                ],
                                startPoint: .leading,
                                endPoint: .trailing
                            )
                        )
                        .frame(width: max(0, proxy.size.width * CGFloat(min(1.0, info.usageRatio))), height: 8)
                        .shadow(color: Color(red: 0.0, green: 0.90, blue: 1.0).opacity(0.4), radius: 6, x: 0, y: 0)
                }
            }
            .frame(height: 8)
            
            // 倒计时重置标注
            if !info.resetCountdown.isEmpty {
                HStack(spacing: 5) {
                    Circle()
                        .fill(Color(red: 0.0, green: 0.90, blue: 1.0))
                        .frame(width: 5, height: 5)
                        .shadow(color: Color(red: 0.0, green: 0.90, blue: 1.0), radius: 3)
                    
                    Text("\(info.resetCountdown) 后开始恢复")
                        .font(.system(size: 11, design: .monospaced))
                        .foregroundColor(Color(red: 0.22, green: 0.74, blue: 0.97)) // #38bdf8
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
