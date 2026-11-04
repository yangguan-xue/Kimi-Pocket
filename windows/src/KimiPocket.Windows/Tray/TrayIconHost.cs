using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Microsoft.Win32;
using KimiPocket.Windows.Interop;
using KimiPocket.Windows.Positioning;
namespace KimiPocket.Windows.Tray;

public sealed class TrayIconHost : IDisposable
{
    private const uint CallbackMessage = 0x8001;
    private readonly HwndSource source;
    private Native.NotifyIconData data;
    private readonly uint taskbarCreated = Native.RegisterWindowMessage("TaskbarCreated");
    private readonly Func<bool, string, nint> loadIcon;
    private nint icon; private string status = "normal"; private bool disposed;
    public event Action<TrayCallback>? Callback;
    public event Action? DisplayChanged;
    public bool IsRegistered { get; private set; }
    public TrayIconHost(Func<bool, string, nint> loader)
    {
        loadIcon = loader;
        source = new HwndSource(new HwndSourceParameters("Kimi Pocket tray host") { Width = 0, Height = 0, PositionX = -32000, PositionY = -32000, WindowStyle = unchecked((int)0x80000000) });
        source.AddHook(WndProc);
        data = new Native.NotifyIconData { Size = (uint)Marshal.SizeOf<Native.NotifyIconData>(), Window = source.Handle, Id = 1, Flags = 1 | 2 | 4, Callback = CallbackMessage, Tip = "Kimi Pocket", Info = "", InfoTitle = "" };
        UpdateIcon("normal"); Register();
    }
    public void UpdateIcon(string nextStatus)
    {
        status = nextStatus; using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
        var light = (key?.GetValue("SystemUsesLightTheme") as int?) == 1;
        var next = loadIcon(light, status); if (next == 0) return;
        data.Icon = next;
        if (IsRegistered) Native.Shell_NotifyIconW(1, ref data);
        var old = icon; icon = next; if (old != 0) Native.DestroyIcon(old);
    }
    private void Register() { IsRegistered = Native.Shell_NotifyIconW(0, ref data); if (IsRegistered) { data.Version = 4; Native.Shell_NotifyIconW(4, ref data); } }
    public PixelRect? GetRect()
    {
        var id = new Native.IconIdentifier { Size = (uint)Marshal.SizeOf<Native.IconIdentifier>(), Window = source.Handle, Id = 1 };
        return Native.Shell_NotifyIconGetRect(ref id, out var r) == 0 && r.Right > r.Left && r.Bottom > r.Top ? new(r.Left, r.Top, r.Right, r.Bottom) : null;
    }
    public void ReturnFocus() { Native.Shell_NotifyIconW(3, ref data); }
    private nint WndProc(nint hwnd, int message, nint w, nint l, ref bool handled)
    {
        if ((uint)message == taskbarCreated) { IsRegistered = false; Register(); }
        else if (message == 0x1A) { UpdateIcon(status); DisplayChanged?.Invoke(); }
        else if (message == 0x7E) DisplayChanged?.Invoke();
        else if ((uint)message == CallbackMessage) { handled = true; var c = TrayCallback.Decode(w, l); if (c.Id == 1) Callback?.Invoke(c); }
        return 0;
    }
    public void Dispose() { if (disposed) return; disposed = true; Native.Shell_NotifyIconW(2, ref data); IsRegistered = false; source.RemoveHook(WndProc); source.Dispose(); if (icon != 0) { Native.DestroyIcon(icon); icon = 0; } }
}
