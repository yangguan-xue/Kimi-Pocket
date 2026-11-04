using KimiPocket.Core.Usage;
namespace KimiPocket.Core.Tests;

public class RefreshCoordinatorTests
{
    private sealed class Clock : TimeProvider { public DateTimeOffset Now = new(2026, 11, 4, 12, 0, 0, TimeSpan.Zero); public override DateTimeOffset GetUtcNow() => Now; }
    private sealed class Client : IUsageClient
    {
        public int Calls; public Func<string, CancellationToken, Task<FetchResult>> Reply = (_, _) => throw new InvalidOperationException();
        public Task<FetchResult> FetchAsync(string token, CancellationToken ct) { Interlocked.Increment(ref Calls); return Reply(token, ct); }
    }
    private static FetchResult Good(DateTimeOffset now, double ratio = .68) => new(new(new("Pro", now.AddDays(21), true), now), new(new(new(100, 74), new(100, 58), new(100, ratio * 100)), now));
    private static FetchResult Bad(Failure error, DateTimeOffset? retry = null) => new(new(null, null, error, retry), new(null, null, error, retry));
    [Theory]
    [InlineData(.68, 60)]
    [InlineData(.29, 30)]
    [InlineData(.30, 60)]
    public async Task SuccessfulRefreshSchedulesByRemainingRatio(double ratio, int seconds) { var clock = new Clock(); var client = new Client { Reply = (_, _) => Task.FromResult(Good(clock.Now, ratio)) }; using var c = new RefreshCoordinator(client, clock); c.SetToken("one"); await c.RefreshAsync(); Assert.Equal(clock.Now.AddSeconds(seconds), c.NextRefresh); await c.RefreshAsync(RefreshReason.Automatic); Assert.Equal(1, client.Calls); }
    [Fact]
    public async Task HoverUsesFifteenSecondBoundary() { var clock = new Clock(); var client = new Client { Reply = (_, _) => Task.FromResult(Good(clock.Now)) }; using var c = new RefreshCoordinator(client, clock); c.SetToken("one", Good(clock.Now)); clock.Now = clock.Now.AddSeconds(14); await c.RefreshAsync(RefreshReason.Hover); Assert.Equal(0, client.Calls); clock.Now = clock.Now.AddSeconds(1); await c.RefreshAsync(RefreshReason.Hover); Assert.Equal(1, client.Calls); }
    [Fact]
    public async Task ConcurrentTriggersShareOneRequest() { var clock = new Clock(); var reply = new TaskCompletionSource<FetchResult>(TaskCreationOptions.RunContinuationsAsynchronously); var client = new Client { Reply = (_, _) => reply.Task }; using var c = new RefreshCoordinator(client, clock); c.SetToken("one"); var first = c.RefreshAsync(); Assert.Same(first, c.RefreshAsync(RefreshReason.Hover)); Assert.Same(first, c.RefreshAsync(RefreshReason.Automatic)); reply.SetResult(Good(clock.Now)); await first; Assert.Equal(1, client.Calls); Assert.False(c.Loading); }
    [Fact]
    public async Task FailuresBackoffAndSuccessRestoresCadence() { var clock = new Clock(); var result = Bad(Failure.Network); var client = new Client { Reply = (_, _) => Task.FromResult(result) }; using var c = new RefreshCoordinator(client, clock); c.SetToken("one"); foreach (var seconds in new[] { 60, 120, 300, 300 }) { await c.RefreshAsync(); Assert.Equal(clock.Now.AddSeconds(seconds), c.NextRefresh); } result = Good(clock.Now); await c.RefreshAsync(); Assert.Equal(clock.Now.AddSeconds(60), c.NextRefresh); }
    [Fact]
    public async Task RateLimitCannotBeBypassedByAnyTrigger() { var clock = new Clock(); var until = clock.Now.AddMinutes(5); var client = new Client { Reply = (_, _) => Task.FromResult(Bad(Failure.RateLimited, until)) }; using var c = new RefreshCoordinator(client, clock); c.SetToken("one"); await c.RefreshAsync(); foreach (var reason in Enum.GetValues<RefreshReason>()) await c.RefreshAsync(reason); Assert.Equal(1, client.Calls); Assert.Equal(until, c.NextRefresh); clock.Now = until; await c.RefreshAsync(); Assert.Equal(2, client.Calls); }
    [Fact]
    public async Task UnauthorizedClearsDataAndStopsRequests() { var clock = new Clock(); var client = new Client { Reply = (_, _) => Task.FromResult(Bad(Failure.Unauthorized)) }; using var c = new RefreshCoordinator(client, clock); c.SetToken("one", Good(clock.Now)); await c.RefreshAsync(); Assert.Null(c.Store.Usage.Value); foreach (var reason in Enum.GetValues<RefreshReason>()) await c.RefreshAsync(reason); Assert.Equal(1, client.Calls); }
    [Fact]
    public async Task ForbiddenStopsBackgroundRetriesButAllowsExplicitRetry() { var clock = new Clock(); var client = new Client { Reply = (_, _) => Task.FromResult(Bad(Failure.Forbidden)) }; using var c = new RefreshCoordinator(client, clock); c.SetToken("one"); await c.RefreshAsync(); clock.Now = clock.Now.AddHours(1); foreach (var reason in new[] { RefreshReason.Automatic, RefreshReason.Hover, RefreshReason.Reset, RefreshReason.Recovery }) await c.RefreshAsync(reason); Assert.Equal(1, client.Calls); await c.RefreshAsync(); Assert.Equal(2, client.Calls); }
    [Fact]
    public async Task ReplacingOrDeletingTokenDiscardsLateResults() { var clock = new Clock(); var old = new TaskCompletionSource<FetchResult>(TaskCreationOptions.RunContinuationsAsynchronously); var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously); var client = new Client { Reply = (token, _) => { if (token == "old") { started.TrySetResult(); return old.Task; } return Task.FromResult(Good(clock.Now, .2)); } }; using var c = new RefreshCoordinator(client, clock); c.SetToken("old"); var first = c.RefreshAsync(); await started.Task.WaitAsync(TimeSpan.FromSeconds(3)); c.SetToken("new"); await c.RefreshAsync(); old.SetResult(Good(clock.Now, .9)); await first; Assert.Equal(.2, c.Store.Usage.Value!.Rolling!.Ratio); c.SetToken(null); Assert.Null(c.Store.Usage.Value); Assert.False(c.HasToken); }
    [Fact]
    public async Task PausingCancelsRequestAndRecoveryCanRefresh() { var clock = new Clock(); var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously); var cancelled = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously); var client = new Client { Reply = async (_, ct) => { started.TrySetResult(); using var registration = ct.Register(() => cancelled.TrySetResult()); await Task.Delay(Timeout.Infinite, ct); return Good(clock.Now); } }; using var c = new RefreshCoordinator(client, clock); c.SetToken("one"); var pending = c.RefreshAsync(); await started.Task.WaitAsync(TimeSpan.FromSeconds(3)); c.Paused = true; await cancelled.Task.WaitAsync(TimeSpan.FromSeconds(2)); await pending; Assert.False(c.Loading); await c.RefreshAsync(); Assert.Equal(1, client.Calls); c.Paused = false; client.Reply = (_, _) => Task.FromResult(Good(clock.Now)); await c.RefreshAsync(RefreshReason.Recovery); Assert.Equal(2, client.Calls); }
    [Fact]
    public void IndependentFailurePreservesOnlyItsOwnTimestamp() { var clock = new Clock(); var s = new UsageStore(); s.Apply(Good(clock.Now)); var later = clock.Now.AddMinutes(3); s.Apply(new(new(new("New", null, true), later), new(null, null, Failure.Network))); Assert.Equal(later, s.Plan.UpdatedAt); Assert.Equal(clock.Now, s.Usage.UpdatedAt); Assert.Equal(.68, s.Usage.Value!.Rolling!.Ratio); Assert.Equal(Failure.Network, s.Usage.Error); }
}
