namespace KimiPocket.Core.Usage;

public sealed class UsageStore
{
    public Part<Plan> Plan { get; private set; } = new(null, null);
    public Part<UsageData> Usage { get; private set; } = new(null, null);
    public void Clear() { Plan = new(null, null); Usage = new(null, null); }
    public void Apply(FetchResult result)
    {
        if (result.AuthFailed) { Plan = result.Plan with { Value = null, UpdatedAt = null }; Usage = result.Usage with { Value = null, UpdatedAt = null }; return; }
        Plan = Merge(Plan, result.Plan); Usage = Merge(Usage, result.Usage);
    }
    private static Part<T> Merge<T>(Part<T> old, Part<T> next) where T : class => next.Error == Failure.None ? next : next with { Value = old.Value, UpdatedAt = old.UpdatedAt };
}
