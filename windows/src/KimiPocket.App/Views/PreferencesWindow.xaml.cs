using System.Windows;
using System.Windows.Input;
using KimiPocket.Core.Auth;
using KimiPocket.Core.Usage;
using KimiPocket.Windows.Lifecycle;
namespace KimiPocket.App.Views;

public partial class PreferencesWindow : Window
{
    private readonly Func<string, CancellationToken, Task<string>> save;
    private readonly Func<string> delete;
    private readonly CancellationTokenSource lifetime = new();
    private readonly bool preview;
    public PreferencesWindow(Func<string, CancellationToken, Task<string>> saveToken, Func<string> deleteToken, bool design = false)
    {
        InitializeComponent(); save = saveToken; delete = deleteToken; preview = design; MaxHeight = SystemParameters.WorkArea.Height;
        StartupBox.IsChecked = !preview && StartupPreference.Enabled; StartupBox.IsEnabled = !preview;
        if (!preview && StartupPreference.HasStaleEntry) StartupHint.Text = "开机启动路径已变化，请重新开启以更新到当前位置。";
        PreviewKeyDown += (_, e) => { if (e.Key == Key.Escape) { e.Handled = true; Close(); } };
        Closed += (_, _) => { lifetime.Cancel(); TokenBox.Clear(); VisibleTokenBox.Clear(); lifetime.Dispose(); };
    }
    private void RevealChanged(object sender, RoutedEventArgs e)
    {
        if (RevealBox.IsChecked == true) { VisibleTokenBox.Text = TokenBox.Password; TokenBox.Clear(); VisibleTokenBox.Visibility = Visibility.Visible; TokenBox.Visibility = Visibility.Collapsed; }
        else { TokenBox.Password = VisibleTokenBox.Text; VisibleTokenBox.Clear(); VisibleTokenBox.Visibility = Visibility.Collapsed; TokenBox.Visibility = Visibility.Visible; }
    }
    private async void SaveClick(object sender, RoutedEventArgs e)
    {
        if (preview) { ResultText.Text = "设计预览模式，不访问网络或保存凭据"; return; }
        var candidate = (RevealBox.IsChecked == true ? VisibleTokenBox.Text : TokenBox.Password).Trim();
        SaveButton.IsEnabled = false; DeleteButton.IsEnabled = false; ResultText.Text = "正在验证…";
        try { JwtPrecheck.Parse(candidate, DateTimeOffset.UtcNow); var result = await save(candidate, lifetime.Token); if (!lifetime.IsCancellationRequested) { ResultText.Text = result; TokenBox.Clear(); VisibleTokenBox.Clear(); } }
        catch (OperationCanceledException) { }
        catch (Exception error) when (error is FormatException or TokenExpiredException or System.ComponentModel.Win32Exception or InvalidOperationException) { if (!lifetime.IsCancellationRequested) ResultText.Text = error.Message; }
        finally { if (!lifetime.IsCancellationRequested) { SaveButton.IsEnabled = true; DeleteButton.IsEnabled = true; } }
    }
    private void DeleteClick(object sender, RoutedEventArgs e) { if (preview) { ResultText.Text = "设计预览模式，不修改凭据"; return; } ResultText.Text = delete(); TokenBox.Clear(); VisibleTokenBox.Clear(); }
    private void StartupClick(object sender, RoutedEventArgs e)
    {
        if (preview) return;
        try { StartupPreference.Set(StartupBox.IsChecked == true); }
        catch (Exception error) when (error is UnauthorizedAccessException or System.Security.SecurityException or System.IO.IOException) { StartupBox.IsChecked = StartupPreference.Enabled; ResultText.Text = "无法更新开机启动设置，请重试"; }
    }
    public void SetStatus(string message) => ResultText.Text = message;
    private void DoneClick(object sender, RoutedEventArgs e) => Close();
}
