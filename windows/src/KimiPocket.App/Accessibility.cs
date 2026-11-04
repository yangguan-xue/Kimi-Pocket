using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.Win32;
namespace KimiPocket.App;

internal static class Accessibility
{
    private sealed record FontMetrics(double Size, double LineHeight);
    private static readonly ConditionalWeakTable<DependencyObject, FontMetrics> originals = new();
    public static void ApplyTheme(ResourceDictionary resources)
    {
        var colors = new Dictionary<string, string> { { "Surface", "#141414" }, { "Hover", "#262626" }, { "Primary", "#F2F2F2" }, { "Secondary", "#A6A6A6" }, { "Muted", "#929292" }, { "Fill", "#DEDEDE" }, { "Track", "#333333" }, { "Edge", "#303030" } };
        foreach (var (name, color) in colors) resources[name] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
        if (!SystemParameters.HighContrast) return;
        foreach (var name in new[] { "Surface", "Hover" }) resources[name] = SystemColors.WindowBrush;
        foreach (var name in new[] { "Primary", "Secondary", "Muted", "Fill", "Edge" }) resources[name] = SystemColors.WindowTextBrush;
        resources["Track"] = SystemColors.GrayTextBrush;
    }
    public static void ApplyTextScale(DependencyObject root)
    {
        using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Accessibility");
        var factor = Math.Clamp((key?.GetValue("TextScaleFactor") as int? ?? 100) / 100d, 1, 2.25);
        Scale(root, factor);
    }
    private static void Scale(DependencyObject root, double factor)
    {
        if (root is TextBlock text)
        {
            var original = originals.GetValue(text, _ => new(text.FontSize, text.LineHeight)); text.FontSize = original.Size * factor; if (!double.IsNaN(original.LineHeight)) text.LineHeight = original.LineHeight * factor;
            foreach (var run in text.Inlines.OfType<Run>()) if (run.ReadLocalValue(TextElement.FontSizeProperty) != DependencyProperty.UnsetValue) { var font = originals.GetValue(run, _ => new(run.FontSize, double.NaN)); run.FontSize = font.Size * factor; }
        }
        else if (root is Button or CheckBox or TextBox or PasswordBox) { var control = (Control)root; var original = originals.GetValue(control, _ => new(control.FontSize, double.NaN)); control.FontSize = original.Size * factor; }
        foreach (var child in LogicalTreeHelper.GetChildren(root).OfType<DependencyObject>()) if (child is not Run) Scale(child, factor);
    }
}
