using System.Windows;
using System.Windows.Controls;
namespace KimiPocket.App.Views;

public partial class UsagePanel : UserControl
{
    public event Action? RefreshRequested; public event Action? SettingsRequested;
    public UsagePanel() { InitializeComponent(); }
    private void RefreshClick(object sender, RoutedEventArgs e) => RefreshRequested?.Invoke();
    private void SettingsClick(object sender, RoutedEventArgs e) => SettingsRequested?.Invoke();
    public void FocusFirst() { if (RefreshButton.IsEnabled) RefreshButton.Focus(); else SettingsButton.Focus(); }
}
