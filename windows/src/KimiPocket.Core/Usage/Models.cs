namespace KimiPocket.Core.Usage;

public sealed record Quota(double Limit, double Remaining, DateTimeOffset? ResetTime = null)
{ public double Ratio => Remaining / Limit; }
public sealed record Plan(string? Title, DateTimeOffset? EndsAt, bool? Active);
public sealed record UsageData(Quota? Total, Quota? Weekly, Quota? Rolling);
public enum Failure { None, InvalidToken, Expired, Unauthorized, Forbidden, RateLimited, Network, InvalidData }
public sealed record Part<T>(T? Value, DateTimeOffset? UpdatedAt, Failure Error = Failure.None, DateTimeOffset? RetryAt = null) where T : class;
public sealed record FetchResult(Part<Plan> Plan, Part<UsageData> Usage)
{
    public bool AuthFailed => Plan.Error is Failure.Unauthorized or Failure.Expired or Failure.InvalidToken || Usage.Error is Failure.Unauthorized or Failure.Expired or Failure.InvalidToken;
    public bool Successful => Plan.Error == Failure.None && Usage.Error == Failure.None;
}
public interface IUsageClient { Task<FetchResult> FetchAsync(string token, CancellationToken cancellationToken); }
public interface ICredentialStore { string? Read(); void Save(string token); void Delete(); }
