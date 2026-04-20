using System.Threading.Channels;

namespace AleonAPI.Services;

public class ImportTaskQueue
{
    private readonly Channel<string> _queue;

    public ImportTaskQueue()
    {
        // Restrict queue size to 1000 file uploads to prevent severe memory leaks.
        // FullMode.Wait guarantees the API endpoint will wait safely if the queue is overloaded.
        var options = new BoundedChannelOptions(1000) { FullMode = BoundedChannelFullMode.Wait };
        _queue = Channel.CreateBounded<string>(options);
    }

    public async ValueTask QueueFileAsync(string filePath, CancellationToken cancellationToken)
    {
        await _queue.Writer.WriteAsync(filePath, cancellationToken);
    }

    public IAsyncEnumerable<string> ReadAllAsync(CancellationToken cancellationToken)
    {
        // This is a powerful feature in .NET! 
        // It streams out files as they arrive, and "sleeps" when the queue is empty!
        return _queue.Reader.ReadAllAsync(cancellationToken);
    }
}
