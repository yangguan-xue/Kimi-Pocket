using KimiPocket.Core.Interaction;
namespace KimiPocket.Core.Tests;

public class HoverCoordinatorTests
{
    [Fact] public void OpenAndCloseRespectExactBoundaries() { var h = new HoverCoordinator(); h.Move(true, false, 0); h.Tick(179); Assert.False(h.IsVisible); h.Tick(180); Assert.True(h.IsVisible); h.Move(false, false, 200); h.Tick(499); Assert.True(h.IsVisible); h.Tick(500); Assert.False(h.IsVisible); }
    [Fact] public void CrossingGapCancelsClose() { var h = new HoverCoordinator(); h.Toggle(0); h.Move(false, false, 10); h.Move(false, true, 200); h.Tick(500); Assert.True(h.IsVisible); }
    [Fact] public void ClickCloseRequiresLeavingIcon() { var h = new HoverCoordinator(); h.Move(true, false, 0); h.Tick(180); h.Toggle(190); h.Move(true, false, 200); h.Tick(1000); Assert.False(h.IsVisible); h.Move(false, false, 1100); h.Move(true, false, 1200); h.Tick(1380); Assert.True(h.IsVisible); }
    [Fact] public void SuppressionAndKeyboardAreIndependentOfPointer() { var h = new HoverCoordinator(); h.Suppress(); h.Move(true, false, 0); h.Tick(200); Assert.False(h.IsVisible); h.Release(); h.Move(false, false, 201); h.OpenKeyboard(); h.Move(false, false, 300); h.Tick(1000); Assert.True(h.IsVisible); h.Hide(); h.Tick(2000); Assert.False(h.IsVisible); }
    [Theory]
    [InlineData("click")]
    [InlineData("keyboard")]
    [InlineData("dismiss")]
    public void SuppressionSurvivesOtherEntryPoints(string action)
    {
        var hover = new HoverCoordinator();
        hover.Suppress();
        if (action == "click") hover.Toggle(0);
        else if (action == "keyboard") hover.OpenKeyboard();
        else hover.Hide();
        hover.Move(true, false, 100);
        hover.Tick(1000);
        Assert.Equal(HoverState.Suppressed, hover.State);
        Assert.False(hover.IsVisible);
        Assert.False(hover.NeedsProbe);
    }
    [Theory]
    [InlineData(SuppressionReason.Menu, SuppressionReason.Settings)]
    [InlineData(SuppressionReason.SessionLock, SuppressionReason.Settings)]
    [InlineData(SuppressionReason.Settings, SuppressionReason.SessionLock)]
    public void ReleasingOneOwnerKeepsOtherOwnerSuppressed(SuppressionReason first, SuppressionReason second)
    {
        var hover = new HoverCoordinator();
        hover.Suppress(first);
        hover.Suppress(second);
        hover.Release(first);
        hover.Move(true, false, 0);
        hover.Toggle(1);
        hover.OpenKeyboard();
        hover.Tick(1000);
        Assert.Equal(HoverState.Suppressed, hover.State);
        hover.Release(second);
        Assert.Equal(HoverState.Hidden, hover.State);
        hover.Move(false, false, 1100);
        hover.Move(true, false, 1200);
        hover.Tick(1380);
        Assert.True(hover.IsVisible);
    }

    [Fact]
    public void RepeatedOwnerNotificationsAreIdempotent()
    {
        var hover = new HoverCoordinator();
        hover.Suppress(SuppressionReason.SessionLock);
        hover.Suppress(SuppressionReason.SessionLock);
        hover.Release(SuppressionReason.Menu);
        Assert.Equal(HoverState.Suppressed, hover.State);
        hover.Release(SuppressionReason.SessionLock);
        Assert.Equal(HoverState.Hidden, hover.State);
    }
}
