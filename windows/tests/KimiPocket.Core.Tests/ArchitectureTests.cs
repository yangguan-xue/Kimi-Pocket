using KimiPocket.Core.Usage;
namespace KimiPocket.Core.Tests;

public class ArchitectureTests
{
    [Fact] public void CoreHasNoWindowsOrWpfDependency() { var names = typeof(UsageStore).Assembly.GetReferencedAssemblies().Select(a => a.Name ?? ""); Assert.DoesNotContain(names, n => n.StartsWith("Presentation") || n == "WindowsBase" || n.StartsWith("KimiPocket.Windows") || n.StartsWith("KimiPocket.App")); }
}
