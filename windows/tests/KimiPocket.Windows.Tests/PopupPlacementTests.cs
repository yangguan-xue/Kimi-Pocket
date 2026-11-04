using KimiPocket.Windows.Positioning;
namespace KimiPocket.Windows.Tests;

public class PopupPlacementTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(1.25)]
    [InlineData(1.5)]
    [InlineData(2)]
    public void NegativeDisplayStaysInsideWorkArea(double scale) { var area = new PixelRect(-1920, -500, 0, 580); var icon = new PixelRect(-50, 580, -20, 610); var p = PopupPlacement.Calculate(icon, area, 360, 376, scale); Assert.True(p.Left >= area.Left + 8 * scale); Assert.True(p.Top >= area.Top + 8 * scale); Assert.True(p.Right <= area.Right - 8 * scale); Assert.True(p.Bottom <= area.Bottom - 8 * scale); }
    [Fact] public void TinyWorkAreaClampsDimensions() { var p = PopupPlacement.Calculate(new(90, 90, 100, 100), new(0, 0, 200, 200), 360, 376, 1); Assert.True(p.Width <= 184); Assert.True(p.Height <= 184); }
}
