using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using VertexCommerce.Modules.Notifications.Services.Models;

namespace VertexCommerce.Modules.Notifications.Services;

public sealed class SseConnectionManager(ILogger<SseConnectionManager> logger) : ISseConnectionManager
{
    private readonly Dictionary<Guid, List<Channel<NotificationMessage>>> _connectionsByUser = [];
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
            if (!_connectionsByUser.TryGetValue(userId, out var connections))
            {
                connections = [];
                _connectionsByUser.Add(userId, connections);
            }

            connections.Add(channel);
        }

        logger.LogInformation("SSE client registered for user {UserId}. Active client channels: {Count}",
            userId, ActiveConnectionsCount);

        return channel;
    }

    public void UnregisterClient(Guid userId, Channel<NotificationMessage> channel)
    {
        lock (_lock)
        {
            if (_connectionsByUser.TryGetValue(userId, out var connections))
            {
                connections.Remove(channel);
                if (connections.Count == 0)
                {
                    _connectionsByUser.Remove(userId);
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
            if (!_connectionsByUser.TryGetValue(userId, out var connections) || connections.Count == 0)
            {
                return;
            }

            channels = [.. connections];
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
                return _connectionsByUser.Values.Sum(connections => connections.Count);
            }
        }
    }
}
