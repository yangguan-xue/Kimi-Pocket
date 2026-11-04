using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using KimiPocket.Core.Usage;
namespace KimiPocket.Windows.Security;

public sealed class CredentialManager(string target = "KimiPocket/access-token") : ICredentialStore
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct Credential
    {
        public uint Flags, Type; public string TargetName; public string? Comment; public long LastWritten; public uint BlobSize; public nint Blob; public uint Persist, AttributeCount; public nint Attributes; public string? Alias, UserName;
    }
    [DllImport("advapi32.dll", EntryPoint = "CredReadW", CharSet = CharSet.Unicode, SetLastError = true)] private static extern bool ReadNative(string target, uint type, uint flags, out nint credential);
    [DllImport("advapi32.dll", EntryPoint = "CredWriteW", CharSet = CharSet.Unicode, SetLastError = true)] private static extern bool WriteNative(ref Credential credential, uint flags);
    [DllImport("advapi32.dll", EntryPoint = "CredDeleteW", CharSet = CharSet.Unicode, SetLastError = true)] private static extern bool DeleteNative(string target, uint type, uint flags);
    [DllImport("advapi32.dll")] private static extern void CredFree(nint buffer);
    public string? Read()
    {
        if (!ReadNative(target, 1, 0, out var ptr)) { var e = Marshal.GetLastWin32Error(); if (e == 1168) return null; throw new Win32Exception(e, "无法读取 Windows 凭据管理器"); }
        try { var c = Marshal.PtrToStructure<Credential>(ptr); var bytes = new byte[c.BlobSize]; Marshal.Copy(c.Blob, bytes, 0, bytes.Length); try { return Encoding.UTF8.GetString(bytes); } finally { Array.Clear(bytes); } } finally { CredFree(ptr); }
    }
    public void Save(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token); if (bytes.Length > 2560) { Array.Clear(bytes); throw new InvalidOperationException("Token 超出 Windows 凭据存储长度限制"); }
        var ptr = Marshal.AllocHGlobal(bytes.Length);
        try { Marshal.Copy(bytes, 0, ptr, bytes.Length); var c = new Credential { Type = 1, TargetName = target, UserName = "KimiPocket", BlobSize = (uint)bytes.Length, Blob = ptr, Persist = 2 }; if (!WriteNative(ref c, 0)) throw new Win32Exception(Marshal.GetLastWin32Error(), "无法保存到 Windows 凭据管理器"); }
        finally { for (int i = 0; i < bytes.Length; i++) Marshal.WriteByte(ptr, i, 0); Marshal.FreeHGlobal(ptr); Array.Clear(bytes); }
    }
    public void Delete() { if (!DeleteNative(target, 1, 0) && Marshal.GetLastWin32Error() != 1168) throw new Win32Exception(Marshal.GetLastWin32Error(), "无法删除 Windows 凭据"); }
}
