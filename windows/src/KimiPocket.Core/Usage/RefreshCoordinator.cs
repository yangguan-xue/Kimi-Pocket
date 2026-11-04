namespace KimiPocket.Core.Usage;

public enum RefreshReason { Automatic, Hover, Manual, Recovery, Reset }
public sealed class RefreshCoordinator(IUsageClient client, TimeProvider? time = null) : IDisposable
{
    private readonly TimeProvider clock = time ?? TimeProvider.System;
    private readonly object gate = new();
    private string? token; private long generation; private CancellationTokenSource? cancel; private Task? flight; private int failures; private bool stopped; private bool forbidden; private bool paused;
    public UsageStore Store { get; } = new();
    public DateTimeOffset NextRefresh { get; private set; } = DateTimeOffset.MaxValue;
    public DateTimeOffset RateLimitUntil { get; private set; } = DateTimeOffset.MinValue;
    public bool Loading { get; private set; }
    public bool HasToken => token != null;
    public bool Paused
    {
        get { lock (gate) return paused; }
        set
        {
            lock (gate) { if (paused == value) return; paused = value; if (value) { generation++; cancel?.Cancel(); cancel = null; flight = null; Loading = false; } }
            Changed?.Invoke();
        }
    }
    public event Action? Changed;
    public void SetToken(string? value, FetchResult? initial = null)
    {
        lock (gate) { generation++; cancel?.Cancel(); cancel = null; flight = null; token = value; stopped = false; forbidden = false; Loading = false; failures = 0; NextRefresh = DateTimeOffset.MinValue; RateLimitUntil = DateTimeOffset.MinValue; Store.Clear(); if (initial != null) { Store.Apply(initial); NextRefresh = clock.GetUtcNow() + TimeSpan.FromSeconds(initial.Usage.Value?.Rolling?.Ratio < .3 ? 30 : 60); } }
        Changed?.Invoke();
    }
    public Task RefreshAsync(RefreshReason reason = RefreshReason.Manual)
    {
        lock (gate)
        {
            if (flight != null) return flight; var now = clock.GetUtcNow(); if (token == null || Paused || stopped || now < RateLimitUntil) return Task.CompletedTask;
            if (forbidden && reason != RefreshReason.Manual) return Task.CompletedTask;
            if (reason == RefreshReason.Automatic && now < NextRefresh) return Task.CompletedTask;
            if (reason == RefreshReason.Hover && Store.Usage.UpdatedAt is DateTimeOffset last && Store.Plan.UpdatedAt is DateTimeOffset plan && now - last < TimeSpan.FromSeconds(15) && now - plan < TimeSpan.FromSeconds(15)) return Task.CompletedTask;
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(15)); cancel = source; Loading = true; flight = ExecuteAsync(token, generation, source); return flight;
        }
    }
    private async Task ExecuteAsync(string credential, long epoch, CancellationTokenSource source)
    {
        await Task.Yield(); Changed?.Invoke();
        try
        {
            var result = await client.FetchAsync(credential, source.Token).ConfigureAwait(false);
            lock (gate)
            {
                if (epoch != generation) return; Store.Apply(result); forbidden = result.Plan.Error == Failure.Forbidden || result.Usage.Error == Failure.Forbidden; var now = clock.GetUtcNow();
                if (result.AuthFailed) { stopped = true; NextRefresh = DateTimeOffset.MaxValue; }
                else
                {
                    failures = result.Successful ? 0 : failures + 1;
                    var seconds = failures == 0 ? (Store.Usage.Value?.Rolling?.Ratio < .3 ? 30 : 60) : failures switch { 1 => 60, 2 => 120, _ => 300 };
                    NextRefresh = now + TimeSpan.FromSeconds(seconds);
                    var retry = new[] { result.Plan.RetryAt, result.Usage.RetryAt }.Where(d => d.HasValue).Select(d => d!.Value).DefaultIfEmpty(DateTimeOffset.MinValue).Max();
                    if (result.Plan.Error == Failure.RateLimited || result.Usage.Error == Failure.RateLimited) { RateLimitUntil = retry > now ? retry : NextRefresh; NextRefresh = RateLimitUntil; }
                }
            }
        }
        catch (OperationCanceledException) { lock (gate) { if (epoch != generation) return; Store.Apply(new(new(null, null, Failure.Network), new(null, null, Failure.Network))); failures++; NextRefresh = clock.GetUtcNow() + TimeSpan.FromSeconds(failures switch { 1 => 60, 2 => 120, _ => 300 }); } }
        finally { lock (gate) { if (epoch == generation) { Loading = false; flight = null; cancel = null; } } source.Dispose(); Changed?.Invoke(); }
    }
    public void Dispose() { lock (gate) { generation++; cancel?.Cancel(); cancel = null; token = null; } }
}
