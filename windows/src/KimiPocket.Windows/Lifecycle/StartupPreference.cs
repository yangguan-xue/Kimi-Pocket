using Microsoft.Win32;
namespace KimiPocket.Windows.Lifecycle;

public static class StartupPreference
{
    private const string Key = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string Name = "KimiPocket";
    private static string Command => $"\"{Environment.ProcessPath}\" --startup";
    public static bool Enabled { get { using var key = Registry.CurrentUser.OpenSubKey(Key); return key?.GetValue(Name) as string == Command; } }
    public static bool HasStaleEntry { get { using var key = Registry.CurrentUser.OpenSubKey(Key); return key?.GetValue(Name) is string s && s != Command; } }
    public static void Set(bool enabled) { using var key = Registry.CurrentUser.CreateSubKey(Key); if (enabled) key.SetValue(Name, Command, RegistryValueKind.String); else key.DeleteValue(Name, false); }
}
