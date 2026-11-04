using KimiPocket.Core.Usage;
namespace KimiPocket.Core.Tests;

public class UsageNormalizerTests
{
    [Theory]
    [InlineData("0", "100", 0d)]
    [InlineData("58", "100", .58)]
    [InlineData("101", "100", null)]
    [InlineData("-1", "100", null)]
    [InlineData("a", "100", null)]
    [InlineData("0", "0", null)]
    public void InvalidFieldsStayUnavailable(string remain, string limit, double? expected) { var q = UsageNormalizer.ParseUsage(System.Text.Json.JsonSerializer.Serialize(new { totalQuota = new { remaining = remain, limit } })); Assert.Equal(expected, q.Total?.Ratio); }
    [Fact] public void OnlyExactFiveHourWindowMatches() { var q = UsageNormalizer.ParseUsage("""{"usages":[{"scope":"FEATURE_CODING","limits":[{"window":{"duration":60,"timeUnit":"TIME_UNIT_MINUTE"},"detail":{"limit":"100","remaining":"100"}}]}]}"""); Assert.Null(q.Rolling); }
    [Fact] public void FixtureParsesWithoutFallbacks() { var q = UsageNormalizer.ParseUsage(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "fixtures/usage.json"))); Assert.Equal(.68, q.Rolling?.Ratio); Assert.Equal(.58, q.Weekly?.Ratio); Assert.Equal(.74, q.Total?.Ratio); }
    [Fact] public void MissingObjectIsNotDemoData() { var q = UsageNormalizer.ParseUsage("{}"); Assert.Null(q.Total); Assert.Null(q.Rolling); Assert.Null(UsageNormalizer.ParsePlan("{}")); }
}
