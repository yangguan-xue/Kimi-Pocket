using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using KimiPocket.Windows.Interop;
using KimiPocket.Windows.Positioning;
namespace KimiPocket.App.Views;

public sealed class PanelWindow : Window
{
    public UsagePanel Panel { get; } = new();
    public bool KeyboardMode { get; private set; }
    public event Action? DismissRequested;
    private HwndSource? source;
    public PanelWindow()
    {
        Title = "Kimi Pocket"; Width = 360; SizeToContent = SizeToContent.Height; WindowStyle = WindowStyle.None; ResizeMode = ResizeMode.NoResize;
        AllowsTransparency = true; Background = Brushes.Transparent; ShowInTaskbar = false; ShowActivated = false; Topmost = true; Content = Panel;
        SourceInitialized += (_, _) => { source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle); source.AddHook(Hook); SetActivation(false); };
        PreviewKeyDown += (_, e) => { if (e.Key == Key.Escape) { e.Handled = true; DismissRequested?.Invoke(); } };
        Deactivated += (_, _) => { if (KeyboardMode) DismissRequested?.Invoke(); };
    }
    private nint Hook(nint hwnd, int message, nint w, nint l, ref bool handled)
    {
        if (message == 0x21 && !KeyboardMode) { handled = true; return 3; }
        return 0;
    }
    private void SetActivation(bool keyboard)
    {
        KeyboardMode = keyboard; var hwnd = new WindowInteropHelper(this).EnsureHandle(); var style = (long)Native.GetWindowLongPtr(hwnd, -20) | 0x80;
        Native.SetWindowLongPtr(hwnd, -20, (nint)(keyboard ? style & ~0x08000000 : style | 0x08000000)); ShowActivated = keyboard;
    }
    public void ShowAt(PixelRect anchor, bool keyboard)
    {
        SetActivation(keyboard); Place(anchor); Show(); UpdateLayout(); Place(anchor);
        if (keyboard) { Activate(); Native.SetForegroundWindow(new WindowInteropHelper(this).Handle); Panel.FocusFirst(); }
    }
    public void Place(PixelRect anchor)
    {
        var rect = new Native.Rect { Left = (int)anchor.Left, Top = (int)anchor.Top, Right = (int)anchor.Right, Bottom = (int)anchor.Bottom };
        var monitor = Native.MonitorFromRect(ref rect, 2); var info = new Native.MonitorInfo { Size = (uint)System.Runtime.InteropServices.Marshal.SizeOf<Native.MonitorInfo>() };
        if (!Native.GetMonitorInfo(monitor, ref info)) return;
        Native.GetDpiForMonitor(monitor, 0, out var dpi, out _); var scale = dpi > 0 ? dpi / 96d : 1;
        var work = new PixelRect(info.Work.Left, info.Work.Top, info.Work.Right, info.Work.Bottom);
        Width = Math.Min(360, Math.Max(1, work.Width / scale - 16)); MaxHeight = Math.Max(1, work.Height / scale - 16);
        Panel.Measure(new Size(Width, double.PositiveInfinity)); var height = Math.Min(Panel.DesiredSize.Height, MaxHeight);
        var p = PopupPlacement.Calculate(anchor, work, Width, height, scale);
        Native.SetWindowPos(new WindowInteropHelper(this).EnsureHandle(), -1, (int)Math.Round(p.Left), (int)Math.Round(p.Top), (int)Math.Round(p.Width), (int)Math.Round(p.Height), 0x10 | 0x200);
    }
    public bool Contains(Native.Point point)
    {
        if (!IsVisible) return false; Native.GetWindowRect(new WindowInteropHelper(this).Handle, out var r); return point.X >= r.Left && point.X < r.Right && point.Y >= r.Top && point.Y < r.Bottom;
    }
    protected override void OnClosed(EventArgs e) { source?.RemoveHook(Hook); base.OnClosed(e); }
}
