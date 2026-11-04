using System.Net;
using System.Text;
using System.Text.Json;
using KimiPocket.Core.Api;
using KimiPocket.Core.Auth;
using KimiPocket.Core.Usage;
namespace KimiPocket.Core.Tests;

public class KimiUsageClientTests
{
    private sealed class Handler(Func<HttpRequestMessage, HttpResponseMessage> reply) : HttpMessageHandler { public int Calls; protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct) { Interlocked.Increment(ref Calls); return Task.FromResult(reply(request)); } }
    private static string Token(object? payload = null) => "e30." + Convert.ToBase64String(JsonSerializer.SerializeToUtf8Bytes(payload ?? new { sub = "subject", ssid = "session", exp = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds() })).TrimEnd('=').Replace('+', '-').Replace('/', '_') + ".signature";
    [Fact] public async Task InvalidTokenMakesNoNetworkRequests() { var h = new Handler(_ => throw new Exception()); using var http = new HttpClient(h); var result = await new KimiUsageClient(http).FetchAsync("invalid", default); Assert.Equal(Failure.InvalidToken, result.Usage.Error); Assert.Equal(0, h.Calls); }
    [Fact] public async Task ExpiredTokenMakesNoNetworkRequests() { var h = new Handler(_ => throw new Exception()); using var http = new HttpClient(h); var result = await new KimiUsageClient(http).FetchAsync(Token(new { sub = "s", ssid = "x", exp = 1 }), default); Assert.Equal(Failure.Expired, result.Usage.Error); Assert.Equal(0, h.Calls); }
    [Theory]
    [InlineData(401, Failure.Unauthorized)]
    [InlineData(403, Failure.Forbidden)]
    [InlineData(429, Failure.RateLimited)]
    [InlineData(500, Failure.Network)]
    public async Task ClassifiesStatusWithoutReadingErrorBody(int code, Failure failure) { var h = new Handler(_ => new((HttpStatusCode)code)); using var http = new HttpClient(h); var result = await new KimiUsageClient(http).FetchAsync(Token(), default); Assert.Equal(failure, result.Usage.Error); Assert.Equal(failure, result.Plan.Error); Assert.Equal(2, h.Calls); }
    [Fact] public async Task SendsRequiredHeadersAndPreservesSuccessfulPartition() { var h = new Handler(r => { Assert.Equal(HttpMethod.Post, r.Method); Assert.Equal("Bearer", r.Headers.Authorization!.Scheme); Assert.Equal("subject", r.Headers.GetValues("x-traffic-id").Single()); Assert.Equal("session", r.Headers.GetValues("x-msh-session-id").Single()); return r.RequestUri!.AbsolutePath.EndsWith("GetSubscription") ? new(HttpStatusCode.OK) { Content = new StringContent("{\"subscription\":{\"title\":\"Pro\"}}") } : new(HttpStatusCode.Forbidden); }); using var http = new HttpClient(h); var result = await new KimiUsageClient(http).FetchAsync(Token(), default); Assert.Equal(Failure.None, result.Plan.Error); Assert.Equal(Failure.Forbidden, result.Usage.Error); }
    [Fact] public async Task HonorsRetryAfter() { var h = new Handler(_ => { var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests); response.Headers.RetryAfter = new(TimeSpan.FromMinutes(5)); return response; }); using var http = new HttpClient(h); var before = DateTimeOffset.UtcNow; var result = await new KimiUsageClient(http).FetchAsync(Token(), default); Assert.InRange(result.Usage.RetryAt!.Value, before.AddMinutes(5), DateTimeOffset.UtcNow.AddMinutes(5)); }
    [Theory]
    [InlineData("not json")]
    [InlineData("null")]
    [InlineData("[]")]
    public async Task MalformedPayloadIsUnavailable(string json) { var h = new Handler(_ => new(HttpStatusCode.OK) { Content = new StringContent(json) }); using var http = new HttpClient(h); var result = await new KimiUsageClient(http).FetchAsync(Token(), default); Assert.Equal(Failure.InvalidData, result.Usage.Error); }
    [Fact] public async Task ResponseSizeIsBounded() { var h = new Handler(_ => new(HttpStatusCode.OK) { Content = new StringContent(new string('x', 2 * 1024 * 1024 + 1)) }); using var http = new HttpClient(h); var result = await new KimiUsageClient(http).FetchAsync(Token(), default); Assert.Equal(Failure.InvalidData, result.Usage.Error); }
    [Fact] public void RejectsHeaderControlCharacters() { Assert.Throws<FormatException>(() => JwtPrecheck.Parse(Token(new { sub = "a\r\nb", ssid = "s", exp = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds() }), DateTimeOffset.UtcNow)); }
}
