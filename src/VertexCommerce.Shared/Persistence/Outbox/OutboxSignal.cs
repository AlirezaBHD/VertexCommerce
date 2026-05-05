using System.Threading.Channels;

namespace VertexCommerce.Shared.Persistence.Outbox;

public sealed class OutboxSignal
{
    private readonly Channel<bool> _channel = Channel.CreateBounded<bool>(
        new BoundedChannelOptions(1)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleReader = true,
            SingleWriter = false
        });

    public void Trigger() => _channel.Writer.TryWrite(true);

    public ValueTask<bool> WaitAsync(CancellationToken ct) => _channel.Reader.ReadAsync(ct);
}
