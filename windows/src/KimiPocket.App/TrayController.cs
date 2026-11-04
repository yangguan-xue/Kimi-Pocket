using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using KimiPocket.App.Views;
using KimiPocket.Core.Interaction;
using KimiPocket.Windows.Interop;
using KimiPocket.Windows.Tray;
using KimiPocket.Windows.Positioning;
namespace KimiPocket.App;

public sealed class TrayController : IDisposable
{
    private readonly TrayIconHost tray; private readonly PanelWindow window; private readonly HoverCoordinator hover = new();
    private readonly DispatcherTimer probe = new() { Interval = TimeSpan.FromMilliseconds(40) };
    private PixelRect? fallback; private bool previousLeft, previousRight; private bool showing; private bool disposed;
    public event Action? RefreshRequested; public event Action? SettingsRequested; public event Action? Opened; public event Action? Closed; public event Action? ExitRequested;
    public bool IsVisible => window.IsVisible;
    public TrayController(PanelWindow panel)
    {
        window = panel; tray = new TrayIconHost(LoadIcon); tray.Callback += OnCallback; tray.DisplayChanged += Reposition; probe.Tick += Probe;
        window.Panel.SizeChanged += (_, _) => Reposition();
        window.DismissRequested += Dismiss; window.Closed += (_, _) => probe.Stop();
    }
    private nint LoadIcon(bool light, string status)
    {
        var name = light ? "dark" : "light"; if (status != "normal") name += "-" + status;
        var dpi = 96u; var rect = tray?.GetRect(); if (rect is PixelRect a) { var r = new Native.Rect { Left = (int)a.Left, Top = (int)a.Top, Right = (int)a.Right, Bottom = (int)a.Bottom }; Native.GetDpiForMonitor(Native.MonitorFromRect(ref r, 2), 0, out dpi, out _); }
        var size = (int)Math.Round(16 * dpi / 96d);
        return Native.LoadImage(0, System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", "Tray", name + ".ico"), 1, size, size, 0x10);
    }
    public void UpdateIcon(string status) => tray.UpdateIcon(status);
    private void OnCallback(TrayCallback c)
    {
        if (disposed) return;
        if (c.Message is 0x400 or 0x401 or 0x7B) { fallback = c.X == -1 && c.Y == -1 ? null : new PixelRect(c.X - 8, c.Y - 8, c.X + 8, c.Y + 8); }
        if (c.Message == 0x401) { hover.OpenKeyboard(); Sync(true); }
        else if (c.Message == 0x400) { hover.Toggle(Environment.TickCount64); Sync(); }
        else if (c.Message == 0x7B) OpenMenu();
        else if (c.Message is 0x200 or 0x406) { Native.GetCursorPos(out var p); var rect = tray.GetRect(); hover.Move(rect?.Contains(p.X, p.Y) == true, window.Contains(p), Environment.TickCount64); Sync(); }
        else if (c.Message == 0x407) { Native.GetCursorPos(out var p); hover.Move(false, window.Contains(p), Environment.TickCount64); Sync(); }
    }
    private void Probe(object? sender, EventArgs e)
    {
        Native.GetCursorPos(out var point); var anchor = tray.GetRect(); var icon = anchor?.Contains(point.X, point.Y) == true; var panel = window.Contains(point);
        var left = (Native.GetAsyncKeyState(1) & 0x8000) != 0; var right = (Native.GetAsyncKeyState(2) & 0x8000) != 0;
        if ((left && !previousLeft || right && !previousRight) && !icon && !panel) hover.Hide();
        previousLeft = left; previousRight = right;
        if (anchor is null && fallback is null) hover.Hide();
        hover.Move(icon, panel, Environment.TickCount64); hover.Tick(Environment.TickCount64); Sync();
    }
    private void Sync(bool keyboard = false)
    {
        if (hover.IsVisible && !showing)
        {
            var anchor = tray.GetRect() ?? (keyboard || fallback != null ? fallback : null); if (anchor is null) { hover.Hide(); return; }
            showing = true; previousLeft = (Native.GetAsyncKeyState(1) & 0x8000) != 0; previousRight = (Native.GetAsyncKeyState(2) & 0x8000) != 0;
            window.ShowAt(anchor.Value, keyboard); Opened?.Invoke();
        }
        else if (!hover.IsVisible && showing) { var wasKeyboard = window.KeyboardMode; showing = false; window.Hide(); Closed?.Invoke(); if (wasKeyboard) tray.ReturnFocus(); }
        if (hover.NeedsProbe) probe.Start(); else probe.Stop();
    }
    public void Dismiss() { hover.Hide(); fallback = null; Sync(); }
    public void Suppress(SuppressionReason reason) { hover.Suppress(reason); Sync(); }
    public void Release(SuppressionReason reason) { hover.Release(reason); Native.GetCursorPos(out var p); if (tray.GetRect()?.Contains(p.X, p.Y) != true) hover.Move(false, false, Environment.TickCount64); Sync(); }
    public void OpenKeyboard() { hover.OpenKeyboard(); var anchor = tray.GetRect(); if (anchor is null) { SettingsRequested?.Invoke(); return; } Sync(true); }
    private void Reposition() { if (window.IsVisible && tray.GetRect() is PixelRect anchor) window.Place(anchor); }
    private void OpenMenu()
    {
        Suppress(SuppressionReason.Menu); var menu = new ContextMenu { Placement = PlacementMode.MousePoint };
        var refresh = new MenuItem { Header = "刷新额度" }; refresh.Click += (_, _) => RefreshRequested?.Invoke();
        var settings = new MenuItem { Header = "设置" }; settings.Click += (_, _) => SettingsRequested?.Invoke();
        var exit = new MenuItem { Header = "退出 Kimi Pocket" }; exit.Click += (_, _) => ExitRequested?.Invoke();
        menu.Items.Add(refresh); menu.Items.Add(settings); menu.Items.Add(new Separator()); menu.Items.Add(exit);
        menu.Closed += (_, _) => { Release(SuppressionReason.Menu); if (hover.State != HoverState.Suppressed) tray.ReturnFocus(); };
        // A native tray context menu needs a foreground owner so an outside click dismisses it.
        Native.SetForegroundWindow(new System.Windows.Interop.WindowInteropHelper(window).EnsureHandle()); menu.IsOpen = true;
    }
    public void Dispose() { if (disposed) return; disposed = true; probe.Stop(); tray.Dispose(); }
}
