using KimiPocket.Core.Presentation;
namespace KimiPocket.Core.Tests;

public class QuotaPresentationTests
{
    [Theory]
    [InlineData(0, "0%")]
    [InlineData(1, "100%")]
    [InlineData(.001, "<1%")]
    [InlineData(.999, ">99%")]
    [InlineData(.58, "58%")]
    public void PercentagePreservesEndpoints(double ratio, string expected) => Assert.Equal(expected, QuotaPresentation.Percent(ratio));
    [Fact] public void WeeklyPartialsAreNotRoundedUp() { var s = QuotaPresentation.Segments(.58, 7); Assert.Equal(7, s.Length); Assert.Equal(1, s[3]); Assert.Equal(.06, s[4], 8); Assert.Equal(0, s[5]); }
    [Fact] public void MissingIsNotFull() { Assert.Equal("—", QuotaPresentation.Percent(null)); Assert.All(QuotaPresentation.Segments(null, 4), v => Assert.Equal(0, v)); }
    [Fact] public void ExpiredCountdownDoesNotInventQuota() => Assert.Equal("等待额度更新", QuotaPresentation.Countdown(DateTimeOffset.UnixEpoch, DateTimeOffset.UtcNow));
    [Fact] public void CoreHasNoDesktopDependencies() => Assert.DoesNotContain(typeof(QuotaPresentation).Assembly.GetReferencedAssemblies(), a => a.Name is "PresentationFramework" or "KimiPocket.Windows");
}
