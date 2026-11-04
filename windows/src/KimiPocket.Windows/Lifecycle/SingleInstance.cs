using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Text;
namespace KimiPocket.Windows.Lifecycle;

public sealed class SingleInstance : IDisposable
{
    private readonly Mutex mutex; private readonly string pipe; private readonly CancellationTokenSource stop = new();
    public bool IsPrimary { get; }
    public SingleInstance(string suffix = "")
    {
        var user = WindowsIdentity.GetCurrent().User?.Value ?? Environment.UserName;
        pipe = "KimiPocket-" + user + suffix;
        // Ownership is thread-affine; created and disposed on the App dispatcher.
        mutex = new Mutex(true, @"Local\" + pipe, out var created); IsPrimary = created;
    }
    public async Task SendAsync(string message)
    {
        using var client = new NamedPipeClientStream(".", pipe, PipeDirection.Out, PipeOptions.Asynchronous | PipeOptions.CurrentUserOnly);
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(3));
        await client.ConnectAsync(timeout.Token); await client.WriteAsync(Encoding.UTF8.GetBytes(message), timeout.Token);
    }
    public async Task ListenAsync(Action<string> received)
    {
        while (!stop.IsCancellationRequested)
        {
            try
            {
                using var server = new NamedPipeServerStream(pipe, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous | PipeOptions.CurrentUserOnly);
                await server.WaitForConnectionAsync(stop.Token).ConfigureAwait(false); var bytes = new byte[64]; var n = await server.ReadAsync(bytes, stop.Token).ConfigureAwait(false); received(Encoding.UTF8.GetString(bytes, 0, n));
            }
            catch (OperationCanceledException) { break; }
            catch (IOException) { }
        }
    }
    public void Dispose() { stop.Cancel(); stop.Dispose(); if (IsPrimary) mutex.ReleaseMutex(); mutex.Dispose(); }
}
