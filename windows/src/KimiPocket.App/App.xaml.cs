using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Win32;
using KimiPocket.App.Design;
using KimiPocket.App.ViewModels;
using KimiPocket.App.Views;
using KimiPocket.Core.Api;
using KimiPocket.Core.Auth;
using KimiPocket.Core.Interaction;
using KimiPocket.Core.Usage;
using KimiPocket.Windows.Lifecycle;
using KimiPocket.Windows.Security;
namespace KimiPocket.App;

public partial class App : Application
{
    private PanelWindow? panel; private readonly UsagePanelViewModel model = new(); private PreferencesWindow? settings;
    private TrayController? tray; private SingleInstance? instance; private HttpClient? http; private KimiUsageClient? client; private RefreshCoordinator? coordinator;
    private readonly CredentialManager credentials = new(); private bool preview, exiting, suspended, locked;
    private readonly DateTimeOffset previewNow = DateTimeOffset.UtcNow; private UsageStore? previewStore; private string previewState = "normal"; private string iconState = "";
    private readonly DispatcherTimer uiTimer = new() { Interval = TimeSpan.FromSeconds(1) };
    private readonly DispatcherTimer refreshTimer = new() { Interval = TimeSpan.FromSeconds(5) };
    private DateTimeOffset? lastRollingReset, lastWeeklyReset;
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e); DispatcherUnhandledException += (_, error) => { error.Handled = true; if (e.Args.Contains("--design-preview")) { File.WriteAllText(Path.Combine(Path.GetTempPath(), "kimi-preview-error.txt"), error.Exception.ToString()); Shutdown(1); return; } MessageBox.Show("应用遇到问题，请重新启动 Kimi Pocket。", "Kimi Pocket"); Shutdown(1); };
        Accessibility.ApplyTheme(Resources); preview = e.Args.Contains("--design-preview");
        if (preview) { var i = Array.IndexOf(e.Args, "--preview-state"); if (i >= 0 && i + 1 < e.Args.Length) previewState = e.Args[i + 1]; if (!UsagePanelPreview.States.Contains(previewState)) { MessageBox.Show("未知设计预览状态"); Shutdown(2); return; } }
        if (!preview)
        {
            instance = new SingleInstance();
            if (!instance.IsPrimary) { try { await instance.SendAsync(e.Args.Contains("--startup") ? "startup" : e.Args.Contains("--show") ? "show" : "settings"); } catch (Exception err) when (err is IOException or TimeoutException or OperationCanceledException) { MessageBox.Show("已有实例正在启动，请稍后重试。", "Kimi Pocket"); } Shutdown(); return; }
            _ = instance.ListenAsync(message => Dispatcher.BeginInvoke(() => { if (message == "show") tray?.OpenKeyboard(); else if (message == "settings") OpenSettings(); }));
            http = new HttpClient(new SocketsHttpHandler { AllowAutoRedirect = false, ConnectTimeout = TimeSpan.FromSeconds(10) }) { Timeout = Timeout.InfiniteTimeSpan }; client = new KimiUsageClient(http); coordinator = new RefreshCoordinator(client); coordinator.Changed += OnDataChanged;
            NetworkChange.NetworkAvailabilityChanged += NetworkChanged; SystemEvents.PowerModeChanged += PowerChanged; SystemEvents.SessionSwitch += SessionChanged; SystemEvents.UserPreferenceChanged += PreferencesChanged;
        }
        panel = new PanelWindow(); panel.Panel.DataContext = model; panel.Panel.RefreshRequested += () => Refresh(RefreshReason.Manual); panel.Panel.SettingsRequested += OpenSettings;
        Accessibility.ApplyTextScale(panel.Panel); panel.DismissRequested += () => { if (preview && !e.Args.Contains("--tray")) Shutdown(); };
        uiTimer.Tick += (_, _) => { Render(); CheckReset(); }; refreshTimer.Tick += (_, _) => { Refresh(RefreshReason.Automatic); CheckReset(); };
        if (preview) previewStore = UsagePanelPreview.Create(previewState, previewNow);
        if (!preview || e.Args.Contains("--tray"))
        {
            tray = new TrayController(panel); tray.Opened += () => { Render(); uiTimer.Start(); Refresh(RefreshReason.Hover); }; tray.Closed += () => uiTimer.Stop(); tray.RefreshRequested += () => Refresh(RefreshReason.Manual); tray.SettingsRequested += OpenSettings; tray.ExitRequested += () => Shutdown();
        }
        if (preview)
        {
            Render(); if (!e.Args.Contains("--tray")) { panel.ShowActivated = true; panel.WindowStartupLocation = WindowStartupLocation.CenterScreen; panel.Show(); panel.Panel.FocusFirst(); }
            var capture = Array.IndexOf(e.Args, "--capture");
            if (capture >= 0 && capture + 1 < e.Args.Length)
            {
                var scale = 2d; var si = Array.IndexOf(e.Args, "--capture-scale"); if (si >= 0 && si + 1 < e.Args.Length && double.TryParse(e.Args[si + 1], System.Globalization.CultureInfo.InvariantCulture, out var value)) scale = Math.Clamp(value, 1, 3);
                await Dispatcher.InvokeAsync(() => Capture(panel, e.Args[capture + 1], scale), DispatcherPriority.ApplicationIdle); Shutdown();
            }
            return;
        }
        string? token = null; string? storageError = null;
        try { token = credentials.Read(); if (token != null) JwtPrecheck.Parse(token, DateTimeOffset.UtcNow); }
        catch (Exception err) when (err is Win32Exception or TokenExpiredException or FormatException) { storageError = err is Win32Exception ? "无法读取 Windows 凭据管理器，请重试" : "登录凭证已失效，请更新 Token"; token = null; }
        coordinator!.SetToken(token); Render(); refreshTimer.Start(); UpdatePause();
        if (token is null || e.Args.Contains("--settings")) { OpenSettings(); if (storageError != null) settings?.SetStatus(storageError); } else Refresh(RefreshReason.Automatic);
    }
    private void OnDataChanged() { if (!exiting) Dispatcher.BeginInvoke(() => { if (exiting) return; if (panel?.IsVisible == true) Render(); UpdateIcon(); }); }
    private void Render()
    {
        var store = preview ? previewStore : coordinator?.Store; if (store == null) return; var now = preview ? previewNow : DateTimeOffset.UtcNow;
        model.Update(store, preview ? previewState != "unconfigured" : coordinator!.HasToken, preview ? previewState == "loading" : coordinator!.Loading, now, preview ? store.Usage.RetryAt ?? DateTimeOffset.MinValue : coordinator!.RateLimitUntil);
    }
    private void UpdateIcon()
    {
        if (coordinator == null || tray == null) return; var usage = coordinator.Store.Usage; var now = DateTimeOffset.UtcNow;
        var state = !coordinator.HasToken || usage.Error is Failure.InvalidToken or Failure.Expired or Failure.Unauthorized ? "unavailable" : usage.Value?.Rolling == null ? "unavailable" : usage.Error != Failure.None || usage.UpdatedAt is DateTimeOffset t && now - t > TimeSpan.FromMinutes(2) ? "stale" : usage.Value.Rolling.Ratio <= .05 ? "critical" : usage.Value.Rolling.Ratio <= .2 ? "low" : "normal";
        if (state != iconState) { iconState = state; tray.UpdateIcon(state); }
    }
    private void Refresh(RefreshReason reason) { if (!preview && !exiting && coordinator != null) _ = coordinator.RefreshAsync(reason); }
    private void CheckReset()
    {
        if (preview || coordinator == null || coordinator.Paused) return; UpdateIcon(); var data = coordinator.Store.Usage.Value; var now = DateTimeOffset.UtcNow; bool due = false;
        if (data?.Rolling?.ResetTime is DateTimeOffset rolling && rolling <= now && rolling != lastRollingReset) { lastRollingReset = rolling; due = true; }
        if (data?.Weekly?.ResetTime is DateTimeOffset weekly && weekly <= now && weekly != lastWeeklyReset) { lastWeeklyReset = weekly; due = true; }
        if (due) Refresh(RefreshReason.Reset);
    }
    private void OpenSettings()
    {
        if (exiting) return; if (settings != null) { settings.Activate(); return; }
        tray?.Suppress(SuppressionReason.Settings); settings = new PreferencesWindow(SaveCandidateAsync, DeleteCredential, preview);
        settings.Closed += (_, _) => { settings = null; tray?.Release(SuppressionReason.Settings); }; settings.Loaded += (_, _) => { if (settings != null) Accessibility.ApplyTextScale(settings); }; settings.Show(); settings.Activate();
    }
    private async Task<string> SaveCandidateAsync(string token, CancellationToken cancellationToken)
    {
        if (preview || client == null || coordinator == null) return "设计预览模式";
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken); timeout.CancelAfter(TimeSpan.FromSeconds(15)); FetchResult result;
        try { result = await client.FetchAsync(token, timeout.Token); } catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested) { return "验证超时，请重试；原凭据未修改"; }
        cancellationToken.ThrowIfCancellationRequested();
        if (!result.Successful)
        {
            var f = result.Plan.Error != Failure.None ? result.Plan.Error : result.Usage.Error; return f switch
            {
                Failure.InvalidToken or Failure.Expired or Failure.Unauthorized => "凭据无效或已过期，请重新获取",
                Failure.Forbidden => "账号暂无访问权限，请检查套餐；原凭据未修改",
                Failure.RateLimited => "请求较频繁，请稍后重试；原凭据未修改",
                Failure.InvalidData => "无法识别接口数据，请稍后重试；原凭据未修改",
                _ => "网络不可用，请重试；原凭据未修改"
            };
        }
        credentials.Save(token); coordinator.SetToken(token, result); lastRollingReset = null; lastWeeklyReset = null; UpdatePause(); Render(); UpdateIcon(); return "已验证并安全保存";
    }
    private string DeleteCredential()
    {
        if (preview) return "设计预览模式";
        try { credentials.Delete(); coordinator?.SetToken(null); lastRollingReset = null; lastWeeklyReset = null; Render(); UpdateIcon(); return "已删除凭据，额度数据已清除"; } catch (Win32Exception) { return "删除失败，请重试；当前凭据保持不变"; }
    }
    private void NetworkChanged(object? sender, NetworkAvailabilityEventArgs e) => Dispatcher.BeginInvoke(() => { UpdatePause(); if (e.IsAvailable) Refresh(RefreshReason.Recovery); });
    private void PowerChanged(object sender, PowerModeChangedEventArgs e) => Dispatcher.BeginInvoke(() => { suspended = e.Mode == PowerModes.Suspend; if (suspended) tray?.Dismiss(); UpdatePause(); if (e.Mode == PowerModes.Resume) Refresh(RefreshReason.Recovery); });
    private void SessionChanged(object sender, SessionSwitchEventArgs e) => Dispatcher.BeginInvoke(() => { if (e.Reason == SessionSwitchReason.SessionLock) { locked = true; tray?.Suppress(SuppressionReason.SessionLock); } else if (e.Reason == SessionSwitchReason.SessionUnlock) { locked = false; tray?.Release(SuppressionReason.SessionLock); } UpdatePause(); if (!locked) Refresh(RefreshReason.Recovery); });
    private void PreferencesChanged(object sender, UserPreferenceChangedEventArgs e) => Dispatcher.BeginInvoke(() => { if (exiting) return; Accessibility.ApplyTheme(Resources); if (panel != null) Accessibility.ApplyTextScale(panel.Panel); if (settings != null) Accessibility.ApplyTextScale(settings); });
    private void UpdatePause() { if (coordinator != null) coordinator.Paused = suspended || locked || !NetworkInterface.GetIsNetworkAvailable(); }
    private static void Capture(PanelWindow window, string path, double scale)
    {
        window.UpdateLayout(); var visual = window.Panel; var image = new RenderTargetBitmap((int)Math.Ceiling(visual.ActualWidth * scale), (int)Math.Ceiling(visual.ActualHeight * scale), 96 * scale, 96 * scale, PixelFormats.Pbgra32); image.Render(visual);
        var encoder = new PngBitmapEncoder(); encoder.Frames.Add(BitmapFrame.Create(image)); using var output = File.Create(Path.GetFullPath(path)); encoder.Save(output);
    }
    protected override void OnExit(ExitEventArgs e)
    {
        exiting = true; uiTimer.Stop(); refreshTimer.Stop(); settings?.Close(); tray?.Dispose(); coordinator?.Dispose(); http?.Dispose();
        if (!preview) { NetworkChange.NetworkAvailabilityChanged -= NetworkChanged; SystemEvents.PowerModeChanged -= PowerChanged; SystemEvents.SessionSwitch -= SessionChanged; SystemEvents.UserPreferenceChanged -= PreferencesChanged; }
        panel?.Close(); instance?.Dispose(); base.OnExit(e);
    }
}
