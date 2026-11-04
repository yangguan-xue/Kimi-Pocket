using System.ComponentModel;
using System.Runtime.CompilerServices;
using KimiPocket.Core.Usage;
using KimiPocket.Core.Presentation;
namespace KimiPocket.App.ViewModels;

public sealed class UsagePanelViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public string PlanLine { get; private set; } = "添加 Token，开始查看额度";
    public string HeroNumber { get; private set; } = "—"; public string HeroSuffix { get; private set; } = "";
    public string RollingReset { get; private set; } = "重置时间暂不可用"; public string WeeklyReset { get; private set; } = "重置时间暂不可用";
    public string WeeklyText { get; private set; } = "剩余 —"; public string TotalText { get; private set; } = "剩余 —";
    public string Footer { get; private set; } = "尚未更新"; public string Message { get; private set; } = ""; public string HeroWarning { get; private set; } = "";
    public string RollingName => $"5 小时额度，剩余 {HeroNumber}{HeroSuffix}";
    public double? RollingRatio { get; private set; }
    public double? WeeklyRatio { get; private set; }
    public double? TotalRatio { get; private set; }
    public bool CanRefresh { get; private set; }
    public bool HasMessage => Message.Length > 0; public bool HasWarning => HeroWarning.Length > 0;
    public void Update(UsageStore store, bool configured, bool loading, DateTimeOffset now, DateTimeOffset rateLimit)
    {
        var p = store.Plan; var u = store.Usage; var plan = p.Value;
        var days = plan?.EndsAt is DateTimeOffset end ? Math.Max(0, (int)Math.Ceiling((end - now).TotalDays)) : (int?)null;
        PlanLine = !configured ? "添加 Token，开始查看额度" : plan is null ? "套餐信息暂不可用" : $"{plan.Title ?? "套餐信息暂不可用"}   · {(days is null ? "到期时间暂不可用" : days == 0 ? "会员已到期" : $"会员剩余 {days} 天")}";
        var stalePlan = p.UpdatedAt is DateTimeOffset pt && (now - pt > TimeSpan.FromMinutes(2) || p.Error != Failure.None);
        if (stalePlan) PlanLine += " · 未更新";
        var data = u.Value; RollingRatio = data?.Rolling?.Ratio; WeeklyRatio = data?.Weekly?.Ratio; TotalRatio = data?.Total?.Ratio;
        var percent = QuotaPresentation.Percent(RollingRatio); HeroNumber = percent.EndsWith('%') ? percent[..^1] : percent; HeroSuffix = percent.EndsWith('%') ? "%" : "";
        RollingReset = RollingRatio is null ? "额度暂不可用" : QuotaPresentation.Countdown(data?.Rolling?.ResetTime, now);
        WeeklyReset = WeeklyRatio is null ? "额度暂不可用" : QuotaPresentation.Countdown(data?.Weekly?.ResetTime, now);
        WeeklyText = "剩余 " + QuotaPresentation.Percent(WeeklyRatio); TotalText = "剩余 " + QuotaPresentation.Percent(TotalRatio);
        var stale = u.UpdatedAt is DateTimeOffset ut && (now - ut > TimeSpan.FromMinutes(2) || u.Error != Failure.None);
        if (stale) { RollingReset += " · 未更新"; WeeklyReset += " · 未更新"; TotalText += " · 未更新"; }
        HeroWarning = stale ? "" : QuotaPresentation.Warning(RollingRatio);
        Footer = loading ? "正在更新" : u.UpdatedAt is DateTimeOffset t ? (now - t).TotalMinutes < 1 ? "刚刚更新" : $"{Math.Max(1, (int)(now - t).TotalMinutes)} 分钟前更新" : "尚未更新";
        var error = u.Error != Failure.None ? u.Error : p.Error;
        Message = !configured ? "打开设置，添加浏览器 access_token" : error switch
        {
            Failure.InvalidToken or Failure.Unauthorized or Failure.Expired => "登录凭证已失效，请打开设置更新 Token",
            Failure.Forbidden => "当前账号无法访问此额度，可重试或检查套餐",
            Failure.RateLimited => $"请求较频繁，{Math.Max(1, (int)(rateLimit - now).TotalSeconds)} 秒后自动重试",
            Failure.Network => data is null ? "网络不可用，请重试" : "网络不可用 · 显示上次数据",
            Failure.InvalidData => "部分数据暂不可用，请稍后重试",
            _ => loading && data is null ? "正在获取额度" : stale || stalePlan ? "数据未更新 · 显示上次数据" : ""
        };
        CanRefresh = configured && !loading && now >= rateLimit && error is not (Failure.InvalidToken or Failure.Unauthorized or Failure.Expired);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
    }
}
