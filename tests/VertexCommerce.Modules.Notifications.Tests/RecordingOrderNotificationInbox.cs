using System.Collections.Concurrent;
using VertexCommerce.Modules.Notifications.Domain.Entities;
using VertexCommerce.Modules.Notifications.Services.Dispatching;

namespace VertexCommerce.Modules.Notifications.Tests;

internal sealed class RecordingOrderNotificationInbox : IOrderNotificationInbox
{
    private readonly ConcurrentDictionary<Guid, IReadOnlyList<InboxNotification>> _batches = new();

    public Func<Guid, IReadOnlyList<InboxNotification>, Task>? BeforeStoreAsync { get; set; }

    public IReadOnlyList<Notification> GetBatch(Guid eventId)
    {
        return GetMessages(eventId).Select(item => item.Notification).ToArray();
    }

    public IReadOnlyList<InboxNotification> GetMessages(Guid eventId)
    {
        return _batches.TryGetValue(eventId, out var notifications) ? notifications : [];
    }

    public async Task<bool> TryStoreAsync(Guid eventId, IReadOnlyList<InboxNotification> notifications, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        if (BeforeStoreAsync is not null)
        {
            await BeforeStoreAsync(eventId, notifications);
        }

        return _batches.TryAdd(eventId, notifications);
    }
}
