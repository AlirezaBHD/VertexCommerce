using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using VertexCommerce.Modules.Notifications.Services.Models;

namespace VertexCommerce.Modules.Notifications.Services;

public sealed class SseConnectionManager(ILogger<SseConnectionManager> logger) : ISseConnectionManager
{
    private readonly ConcurrentDictionary<Guid, List<Channel<NotificationMessage>>> _clients = new();
    private readonly Lock _lock = new();

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

        logger.LogInformation("SSE client registered for user {UserId}. Active client channels: {Count}",
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

        logger.LogInformation("SSE client unregistered for user {UserId}. Active client channels: {Count}",
            userId, ActiveConnectionsCount);
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "SSE channel writing errors should not abort broadcasts to remaining connections")]
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

        logger.LogInformation("Broadcasting SSE notification {Id} to {Count} connection(s) for user {UserId}",
            message.Id, channels.Count, userId);

        foreach (var channel in channels)
        {
            try
            {
                await channel.Writer.WriteAsync(message, ct);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to write SSE notification to channel for user {UserId}", userId);
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
