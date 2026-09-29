using System.Collections.Concurrent;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using VertexCommerce.Modules.Notifications.Services.Models;

namespace VertexCommerce.Modules.Notifications.Services;

public sealed class SseConnectionManager : ISseConnectionManager
{
    private readonly ConcurrentDictionary<Guid, List<Channel<NotificationMessage>>> _clients = new();
    private readonly object _lock = new();
    private readonly ILogger<SseConnectionManager> _logger;

    public SseConnectionManager(ILogger<SseConnectionManager> logger)
    {
        _logger = logger;
    }

    public Channel<NotificationMessage> RegisterClient(Guid userId)
    {
        var channel = Channel.CreateUnbounded<NotificationMessage>(new UnboundedChannelOptions
        {
            SingleWriter = false,
            SingleReader = true
        });

        lock (_lock)
        {
            var list = _clients.GetOrAdd(userId, _ => []);
            list.Add(channel);
        }

        _logger.LogInformation("SSE client registered for user {UserId}. Active client channels: {Count}",
            userId, ActiveConnectionsCount);

        return channel;
    }

    public void UnregisterClient(Guid userId, Channel<NotificationMessage> channel)
    {
        lock (_lock)
        {
            if (_clients.TryGetValue(userId, out var list))
            {
                list.Remove(channel);
                if (list.Count == 0)
                {
                    _clients.TryRemove(userId, out _);
                }
            }
        }

        channel.Writer.TryComplete();

        _logger.LogInformation("SSE client unregistered for user {UserId}. Active client channels: {Count}",
            userId, ActiveConnectionsCount);
    }

    public async Task BroadcastToUserAsync(Guid userId, NotificationMessage message, CancellationToken ct = default)
    {
        List<Channel<NotificationMessage>> channels;

        lock (_lock)
        {
            if (!_clients.TryGetValue(userId, out var list) || list.Count == 0)
            {
                return;
            }

            channels = list.ToList();
        }

        _logger.LogInformation("Broadcasting SSE notification {Id} to {Count} connection(s) for user {UserId}",
            message.Id, channels.Count, userId);

        foreach (var channel in channels)
        {
            try
            {
                await channel.Writer.WriteAsync(message, ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to write SSE notification to channel for user {UserId}", userId);
            }
        }
    }

    public int ActiveConnectionsCount
    {
        get
        {
            lock (_lock)
            {
                return _clients.Values.Sum(list => list.Count);
            }
        }
    }
}
