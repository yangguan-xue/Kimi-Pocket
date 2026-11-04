using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using KimiPocket.App.Views;
using KimiPocket.App.Design;
using KimiPocket.App.ViewModels;
using KimiPocket.Windows.Interop;
using KimiPocket.Windows.Lifecycle;
using KimiPocket.Windows.Positioning;
using KimiPocket.Windows.Security;
using KimiPocket.Windows.Tray;
namespace KimiPocket.Windows.Tests;

public class NativeIntegrationTests(Xunit.Abstractions.ITestOutputHelper output)
{
    [DllImport("user32.dll")] private static extern uint GetGuiResources(nint process, uint flags);
    [Fact] public void CredentialManagerRoundtripUsesAnIsolatedTarget() { var target = "KimiPocket/tests/" + Guid.NewGuid(); var store = new CredentialManager(target); try { Assert.Null(store.Read()); store.Save("synthetic-test-value"); Assert.Equal("synthetic-test-value", store.Read()); store.Save("replacement"); Assert.Equal("replacement", store.Read()); store.Delete(); Assert.Null(store.Read()); store.Delete(); } finally { store.Delete(); } }
    [Fact] public void CredentialManagerRejectsOversizedTokensBeforeSaving() { var store = new CredentialManager("KimiPocket/tests/" + Guid.NewGuid()); Assert.Throws<InvalidOperationException>(() => store.Save(new string('x', 2561))); Assert.Null(store.Read()); }
    [Fact] public void SingleInstanceRelaysToPrimary() { Exception? failure = null; var thread = new Thread(() => { try { var suffix = "-tests-" + Guid.NewGuid(); using var primary = new SingleInstance(suffix); Assert.True(primary.IsPrimary); var got = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously); var listen = primary.ListenAsync(s => got.TrySetResult(s)); using var secondary = new SingleInstance(suffix); Assert.False(secondary.IsPrimary); secondary.SendAsync("settings").GetAwaiter().GetResult(); Assert.Equal("settings", got.Task.WaitAsync(TimeSpan.FromSeconds(3)).GetAwaiter().GetResult()); } catch (Exception e) { failure = e; } }); thread.Start(); Assert.True(thread.Join(TimeSpan.FromSeconds(8))); Assert.Null(failure); }
    [Fact]
    public void NativeTrayAndPassivePanelSurviveOneHundredCycles()
    {
        Exception? failure = null; var thread = new Thread(() =>
        {
            PanelWindow? window = null; try
            {
                var iconFile = Path.Combine(AppContext.BaseDirectory, "Assets", "Tray", "light.ico"); Assert.True(File.Exists(iconFile));
                using var tray = new TrayIconHost((_, _) => Native.LoadImage(0, iconFile, 1, 16, 16, 0x10)); Assert.True(tray.IsRegistered); Assert.NotNull(tray.GetRect());
                window = new PanelWindow(); window.Panel.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/KimiPocket;component/Resources/Theme.xaml", UriKind.Relative) });
                var now = DateTimeOffset.UtcNow; var vm = new UsagePanelViewModel(); vm.Update(UsagePanelPreview.Create("normal", now), true, false, now, DateTimeOffset.MinValue); window.Panel.DataContext = vm;
                var anchor = tray.GetRect()!.Value; var foreground = Native.GetForegroundWindow();
                window.ShowAt(anchor, false); window.Hide();
                using var process = System.Diagnostics.Process.GetCurrentProcess();
                var gdiBefore = GetGuiResources(process.Handle, 0); var userBefore = GetGuiResources(process.Handle, 1);
                for (int i = 0; i < 100; i++) { window.ShowAt(anchor, false); Assert.Equal(foreground, Native.GetForegroundWindow()); Assert.True(window.IsVisible); window.Hide(); tray.UpdateIcon("normal"); }
                var gdiAfter = GetGuiResources(process.Handle, 0); var userAfter = GetGuiResources(process.Handle, 1);
                output.WriteLine($"100 passive cycles: GDI {gdiBefore}->{gdiAfter}, USER {userBefore}->{userAfter}");
                Assert.True(gdiAfter <= gdiBefore + 2); Assert.True(userAfter <= userBefore + 2);
                var handle = new WindowInteropHelper(window).Handle; var style = (long)Native.GetWindowLongPtr(handle, -20); Assert.NotEqual(0, style & 0x08000000); Assert.NotEqual(0, style & 0x80);
            }
            catch (Exception e) { failure = e; }
            finally { window?.Close(); }
        }); thread.SetApartmentState(ApartmentState.STA); thread.Start(); Assert.True(thread.Join(TimeSpan.FromSeconds(30))); Assert.Null(failure);
    }
}
