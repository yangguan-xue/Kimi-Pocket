namespace KimiPocket.Windows.Positioning;

public readonly record struct PixelRect(double Left, double Top, double Right, double Bottom)
{
    public double Width => Right - Left; public double Height => Bottom - Top;
    public bool Contains(double x, double y) => x >= Left && x < Right && y >= Top && y < Bottom;
}
public static class PopupPlacement
{
    public static PixelRect Calculate(PixelRect icon, PixelRect work, double widthDip, double heightDip, double scale)
    {
        var gap = 8 * scale; var w = Math.Max(1, Math.Min(widthDip * scale, work.Width - 2 * gap)); var h = Math.Max(1, Math.Min(heightDip * scale, work.Height - 2 * gap));
        var x = icon.Right - w; var y = icon.Top - gap - h;
        if (y < work.Top + gap && icon.Bottom + gap + h <= work.Bottom - gap) y = icon.Bottom + gap;
        x = Math.Clamp(x, work.Left + Math.Min(gap, work.Width / 2), Math.Max(work.Left + gap, work.Right - gap - w));
        y = Math.Clamp(y, work.Top + Math.Min(gap, work.Height / 2), Math.Max(work.Top + gap, work.Bottom - gap - h));
        return new(x, y, x + w, y + h);
    }
}
