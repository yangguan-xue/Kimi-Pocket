using KimiPocket.Windows.Tray;
namespace KimiPocket.Windows.Tests;

public class TrayCallbackTests
{
    [Fact] public void VersionFourDecodesSignedCoordinatesAndIdentifier() { var c = TrayCallback.Decode(unchecked((nint)((uint)(ushort)-120 | ((uint)(ushort)-40 << 16))), (nint)(0x0400 | (7 << 16))); Assert.Equal(-120, c.X); Assert.Equal(-40, c.Y); Assert.Equal(7, c.Id); Assert.Equal(0x400, c.Message); }
}
