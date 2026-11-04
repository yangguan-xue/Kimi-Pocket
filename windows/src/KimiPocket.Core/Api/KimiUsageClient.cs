using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using KimiPocket.Core.Auth;
using KimiPocket.Core.Usage;
namespace KimiPocket.Core.Api;

public sealed class KimiUsageClient(HttpClient http, TimeProvider? time = null) : IUsageClient
{
    private readonly TimeProvider clock = time ?? TimeProvider.System;
    public async Task<FetchResult> FetchAsync(string token, CancellationToken cancellationToken)
    {
        TokenClaims claims;
        try { claims = JwtPrecheck.Parse(token, clock.GetUtcNow()); }
        catch (TokenExpiredException) { return new(new(null, null, Failure.Expired), new(null, null, Failure.Expired)); }
        catch (FormatException) { return new(new(null, null, Failure.InvalidToken), new(null, null, Failure.InvalidToken)); }
        var plan = SendAsync("kimi.gateway.membership.v2.MembershipService/GetSubscription", "{}", UsageNormalizer.ParsePlan, token, claims, cancellationToken);
        var usage = SendAsync("kimi.gateway.billing.v1.BillingService/GetUsages", """{"scope":["FEATURE_CODING"]}""", UsageNormalizer.ParseUsage, token, claims, cancellationToken);
        await Task.WhenAll(plan, usage).ConfigureAwait(false); return new(await plan.ConfigureAwait(false), await usage.ConfigureAwait(false));
    }
    private async Task<Part<T>> SendAsync<T>(string path, string body, Func<string, T?> parse, string token, TokenClaims claims, CancellationToken ct) where T : class
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, "https://www.kimi.com/apiv2/" + path);
        req.Content = new StringContent(body, Encoding.UTF8, "application/json"); req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Trim());
        req.Headers.Add("x-traffic-id", claims.Subject); req.Headers.Add("x-msh-session-id", claims.Session); req.Headers.Add("x-msh-platform", "web"); req.Headers.Add("x-msh-version", "1.0.0"); req.Headers.Add("x-language", "zh-CN"); req.Headers.Add("connect-protocol-version", "1");
        try
        {
            using var response = await http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
            var now = clock.GetUtcNow();
            if (!response.IsSuccessStatusCode)
            {
                var failure = response.StatusCode switch { HttpStatusCode.Unauthorized => Failure.Unauthorized, HttpStatusCode.Forbidden => Failure.Forbidden, HttpStatusCode.TooManyRequests => Failure.RateLimited, _ => Failure.Network };
                var retry = response.Headers.RetryAfter; var retryAt = retry?.Date ?? (retry?.Delta is TimeSpan delta ? now + delta : null);
                return new(null, null, failure, retryAt > now ? retryAt : null);
            }
            // Limit the response before buffering; never log or persist server payloads.
            using var stream = await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false); using var buffer = new MemoryStream(); var chunk = new byte[8192]; int read;
            while ((read = await stream.ReadAsync(chunk, ct).ConfigureAwait(false)) > 0) { if (buffer.Length + read > 2 * 1024 * 1024) return new(null, null, Failure.InvalidData); buffer.Write(chunk, 0, read); }
            var value = parse(Encoding.UTF8.GetString(buffer.ToArray())); return value is null ? new(null, null, Failure.InvalidData) : new(value, now);
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested) { return new(null, null, Failure.Network); }
        catch (IOException) { return new(null, null, Failure.Network); }
        catch (HttpRequestException) { return new(null, null, Failure.Network); }
        catch (JsonException) { return new(null, null, Failure.InvalidData); }
    }
}
