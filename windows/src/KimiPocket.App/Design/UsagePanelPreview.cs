using KimiPocket.Core.Usage;
namespace KimiPocket.App.Design;

public static class UsagePanelPreview
{
    public static UsageStore Create(string state, DateTimeOffset now)
    {
        var store = new UsageStore(); var plan = new Part<Plan>(new("Kimi Pro", now.AddDays(21), true), now);
        var rolling = state == "low" ? .04 : state == "empty" ? 0 : .68;
        var usage = new Part<UsageData>(new(new(100, 74), new(100, 58, now.AddDays(3).AddHours(4)), new(100, rolling * 100, now.AddHours(2).AddMinutes(18))), now);
        store.Apply(new(plan, usage));
        if (state is "offline" or "forbidden" or "rate-limited") store.Apply(new(plan, new(null, null, state == "offline" ? Failure.Network : state == "forbidden" ? Failure.Forbidden : Failure.RateLimited, now.AddSeconds(60))));
        if (state == "expired") store.Apply(new(new(null, null, Failure.Expired), new(null, null, Failure.Expired)));
        if (state == "missing") store.Apply(new(plan, new(new(null, null, null), now)));
        if (state is "unconfigured" or "loading") store.Clear(); return store;
    }
    public static readonly string[] States = ["normal", "offline", "expired", "forbidden", "rate-limited", "missing", "low", "empty", "unconfigured", "loading"];
}
