using System.Windows;
using System.Windows.Media;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using KimiPocket.Core.Presentation;
namespace KimiPocket.App.Controls;

public sealed class QuotaBar : FrameworkElement
{
    public static readonly DependencyProperty RatioProperty = DependencyProperty.Register(nameof(Ratio), typeof(double?), typeof(QuotaBar), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty SegmentsProperty = DependencyProperty.Register(nameof(Segments), typeof(int), typeof(QuotaBar), new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsRender));
    public double? Ratio { get => (double?)GetValue(RatioProperty); set => SetValue(RatioProperty, value); }
    public int Segments { get => (int)GetValue(SegmentsProperty); set => SetValue(SegmentsProperty, value); }
    protected override void OnRender(DrawingContext dc)
    {
        var count = Math.Max(1, Segments); var gap = count == 1 ? 0 : 4; var available = Math.Max(0, ActualWidth - gap * (count - 1)); var width = available / count;
        var fills = QuotaPresentation.Segments(Ratio, count); var track = (Brush)FindResource("Track"); var fill = (Brush)FindResource("Fill"); var scale = VisualTreeHelper.GetDpi(this).DpiScaleX;
        for (var i = 0; i < count; i++)
        {
            var x = Math.Round(i * (width + gap) * scale) / scale; var right = Math.Round((i * (width + gap) + width) * scale) / scale; var rect = new Rect(x, 0, Math.Max(0, right - x), ActualHeight);
            var geometry = new RectangleGeometry(rect, 2, 2); dc.DrawGeometry(track, null, geometry);
            dc.PushClip(geometry); if (fills[i] > 0) dc.DrawRectangle(fill, null, new Rect(x, 0, rect.Width * fills[i], ActualHeight)); dc.Pop();
        }
    }
    protected override AutomationPeer OnCreateAutomationPeer() => new QuotaBarPeer(this);
    private sealed class QuotaBarPeer(QuotaBar owner) : FrameworkElementAutomationPeer(owner), IRangeValueProvider
    {
        protected override string GetClassNameCore() => "QuotaBar";
        protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.ProgressBar;
        public override object? GetPattern(PatternInterface pattern) => pattern == PatternInterface.RangeValue && owner.Ratio.HasValue ? this : base.GetPattern(pattern);
        public bool IsReadOnly => true; public double LargeChange => 0; public double SmallChange => 0; public double Maximum => 100; public double Minimum => 0; public double Value => (owner.Ratio ?? 0) * 100;
        public void SetValue(double value) => throw new InvalidOperationException("额度为只读");
    }
}
