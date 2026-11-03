import SwiftUI
import KimiPocketCore

public struct HoverHUDView: View {
    @ObservedObject public var store: UsageStore
    public let isPinned: Bool
    public let onTogglePin: () -> Void
    public let onOpenPreferences: () -> Void
    
    @MainActor
    public init(
        store: UsageStore,
        isPinned: Bool = false,
        onTogglePin: @escaping () -> Void = {},
        onOpenPreferences: @escaping () -> Void = {}
    ) {
        self.store = store
        self.isPinned = isPinned
        self.onTogglePin = onTogglePin
        self.onOpenPreferences = onOpenPreferences
    }
    
    public var body: some View {
        VStack(alignment: .leading, spacing: 11) {
            // MARK: - 顶部 Header
            HStack(alignment: .center) {
                // 会员徽标
                HStack(spacing: 7) {
                    Circle()
                        .fill(Color(red: 0.06, green: 0.73, blue: 0.51))
                        .frame(width: 7, height: 7)
                        .shadow(color: Color(red: 0.06, green: 0.73, blue: 0.51).opacity(0.8), radius: 4)
                    
                    Text(store.usageState?.plan.title ?? "Kimi Pro")
                        .font(.system(size: 12, weight: .semibold))
                        .foregroundColor(Color(red: 0.22, green: 0.74, blue: 0.97))
                }
                .padding(EdgeInsets(top: 3, leading: 9, bottom: 3, trailing: 9))
                .background(
                    Capsule()
                        .fill(Color(red: 0.0, green: 0.90, blue: 1.0).opacity(0.12))
                        .overlay(
                            Capsule()
                                .stroke(Color(red: 0.0, green: 0.90, blue: 1.0).opacity(0.35), lineWidth: 1)
                        )
                )
                
                Spacer()
                
                // 到期倒计时与刷新操作
                HStack(spacing: 8) {
                    if let days = store.usageState?.plan.daysRemaining {
                        Text("剩 \(days) 天")
                            .font(.system(size: 11.5, design: .monospaced))
                            .foregroundColor(Color(red: 0.58, green: 0.64, blue: 0.72))
                    }
                    
                    Button(action: {
                        store.refreshImmediately()
                    }) {
                        Image(systemName: "arrow.clockwise")
                            .font(.system(size: 11, weight: .semibold))
                            .foregroundColor(Color(red: 0.8, green: 0.84, blue: 0.88))
                            .rotationEffect(.degrees(store.isLoading ? 360 : 0))
                            .animation(store.isLoading ? Animation.linear(duration: 1).repeatForever(autoreverses: false) : .default, value: store.isLoading)
                            .frame(width: 22, height: 22)
                            .background(Color.white.opacity(0.08))
                            .cornerRadius(5)
                    }
                    .buttonStyle(.plain)
                }
            }
            .padding(.bottom, 3)
            
            Divider()
                .background(Color.white.opacity(0.08))
            
            // MARK: - 核心内容区
            if !store.hasToken {
                VStack(spacing: 12) {
                    Text("尚未配置 Access Token")
                        .font(.system(size: 13, weight: .medium))
                        .foregroundColor(.white)
                    
                    Button("打开偏好设置") {
                        onOpenPreferences()
                    }
                    .buttonStyle(.borderedProminent)
                    .tint(Color(red: 0.0, green: 0.7, blue: 0.9))
                }
                .frame(maxWidth: .infinity, minHeight: 180)
            } else if let state = store.usageState {
                // 核心 1：5小时额度 (连续平滑单条)
                RollingQuotaRow(info: state.rolling5h)
                
                // 核心 2：本周用量
                SegmentedQuotaRow(
                    title: "本周用量",
                    segmentCount: 7,
                    usageRatio: state.weekly.usageRatio,
                    subtitle: state.weekly.resetCountdown.isEmpty ? "每周一 00:00 重置" : "\(state.weekly.resetCountdown)",
                    theme: .cyan
                )
                
                // 核心 3：总用量 (显示明确的到期时间)
                SegmentedQuotaRow(
                    title: "总用量",
                    segmentCount: 4,
                    usageRatio: state.total.usageRatio,
                    subtitle: state.plan.formattedExpirySubtitle,
                    theme: .emerald
                )
            } else {
                // 骨架屏占位
                VStack(spacing: 10) {
                    ProgressView()
                        .progressViewStyle(.circular)
                        .scaleEffect(0.8)
                    Text("正在同步用量数据...")
                        .font(.system(size: 11))
                        .foregroundColor(.gray)
                }
                .frame(maxWidth: .infinity, minHeight: 180)
            }
            
            Divider()
                .background(Color.white.opacity(0.08))
            
            // MARK: - 底部状态与操作栏
            HStack {
                Text("同步时间: \(store.lastSyncTimeFormatted)")
                    .font(.system(size: 10.5, design: .monospaced))
                    .foregroundColor(Color(red: 0.39, green: 0.45, blue: 0.55))
                
                Spacer()
                
                HStack(spacing: 8) {
                    Button(action: onTogglePin) {
                        HStack(spacing: 3) {
                            Text(isPinned ? "已固定" : "钉住")
                            Image(systemName: isPinned ? "pin.fill" : "pin")
                        }
                        .font(.system(size: 10.5))
                        .foregroundColor(isPinned ? Color(red: 0.0, green: 0.90, blue: 1.0) : Color(red: 0.8, green: 0.84, blue: 0.88))
                        .padding(EdgeInsets(top: 2, leading: 6, bottom: 2, trailing: 6))
                        .background(Color.white.opacity(isPinned ? 0.12 : 0.06))
                        .cornerRadius(4)
                    }
                    .buttonStyle(.plain)
                    
                    Button(action: onOpenPreferences) {
                        Image(systemName: "gearshape")
                            .font(.system(size: 11))
                            .foregroundColor(Color(red: 0.6, green: 0.65, blue: 0.72))
                    }
                    .buttonStyle(.plain)
                }
            }
        }
        .padding(14)
        .frame(width: 320)
        .background(Color(red: 0.07, green: 0.08, blue: 0.10).opacity(0.88)) // Obsidian Dark
    }
}
