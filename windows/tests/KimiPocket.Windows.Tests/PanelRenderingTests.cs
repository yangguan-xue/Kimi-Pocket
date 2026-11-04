using System.Windows;
using System.Windows.Threading;
using KimiPocket.App.Design;
using KimiPocket.App.ViewModels;
using KimiPocket.App.Views;
namespace KimiPocket.Windows.Tests;

public class PanelRenderingTests
{
    [Theory]
    [InlineData("normal")]
    [InlineData("offline")]
    [InlineData("expired")]
    [InlineData("forbidden")]
    [InlineData("rate-limited")]
    [InlineData("missing")]
    [InlineData("low")]
    [InlineData("empty")]
    [InlineData("unconfigured")]
    [InlineData("loading")]
    public void ApprovedSampleCanLayoutWithoutBindingExceptions(string state)
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                var now = DateTimeOffset.UtcNow;
                var model = new UsagePanelViewModel(); model.Update(UsagePanelPreview.Create(state, now), state != "unconfigured", state == "loading", now, DateTimeOffset.MinValue);
                var view = new UsagePanel { DataContext = model };
                view.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/KimiPocket;component/Resources/Theme.xaml", UriKind.Relative) });
                view.Measure(new Size(360, double.PositiveInfinity)); view.Arrange(new Rect(0, 0, 360, view.DesiredSize.Height)); view.UpdateLayout();
                Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.ApplicationIdle);
                Assert.Equal(360, view.ActualWidth); Assert.True(view.ActualHeight >= 376);
                if (state == "normal") { Assert.Equal(376, view.ActualHeight); Assert.Equal("68", model.HeroNumber); Assert.Equal("剩余 58%", model.WeeklyText); }
            }
            catch (Exception error) { failure = error; }
        }); thread.SetApartmentState(ApartmentState.STA); thread.Start(); Assert.True(thread.Join(TimeSpan.FromSeconds(10))); Assert.Null(failure);
    }
}
