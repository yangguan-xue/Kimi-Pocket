namespace KimiPocket.Windows.Tray;

public readonly record struct TrayCallback(int Message, int Id, int X, int Y)
{
    public static TrayCallback Decode(nint w, nint l) => new((int)((long)l & 0xffff), (int)(((long)l >> 16) & 0xffff), unchecked((short)((long)w & 0xffff)), unchecked((short)(((long)w >> 16) & 0xffff)));
}
