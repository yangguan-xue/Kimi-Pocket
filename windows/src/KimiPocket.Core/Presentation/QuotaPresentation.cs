using System.Globalization;
namespace KimiPocket.Core.Presentation;

public static class QuotaPresentation
{
    public static string Percent(double? ratio)
    {
        if (ratio is not double r || !double.IsFinite(r) || r < 0 || r > 1) return "—";
        var rounded = Math.Round(r * 100, MidpointRounding.AwayFromZero);
        return r > 0 && rounded == 0 ? "<1%" : r < 1 && rounded == 100 ? ">99%" : rounded.ToString("0", CultureInfo.InvariantCulture) + "%";
    }
    public static double[] Segments(double? ratio, int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);
        double r = ratio is double v && double.IsFinite(v) ? Math.Clamp(v, 0, 1) : 0;
        return Enumerable.Range(0, count).Select(i => Math.Clamp(count * r - i, 0, 1)).ToArray();
    }
    public static string Countdown(DateTimeOffset? reset, DateTimeOffset now)
    {
        if (reset is null) return "重置时间暂不可用";
        var d = reset.Value - now; if (d <= TimeSpan.Zero) return "等待额度更新";
        if (d.TotalDays >= 1) return $"{(int)d.TotalDays} 天 {d.Hours} 小时后重置";
        if (d.TotalHours >= 1) return $"{(int)d.TotalHours} 小时 {d.Minutes} 分后重置";
        return $"{Math.Max(1, (int)d.TotalMinutes)} 分钟后重置";
    }
    public static string Warning(double? ratio) => ratio switch { 0 => "当前额度已用尽", > 0 and <= .05 => "即将用尽", > .05 and <= .2 => "额度偏低", _ => "" };
}
