using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VertexCommerce.Modules.Notifications.Services;
using VertexCommerce.Modules.Notifications.Services.Delivery;
using VertexCommerce.Modules.Notifications.Services.Dispatching;

namespace VertexCommerce.Modules.Notifications.Persistence.Repositories;

internal sealed class OrderNotificationInbox(
    IDbContextFactory<NotificationsDbContext> contextFactory, IOptions<WebPushOptions> webPush, TimeProvider clock)
    : IOrderNotificationInbox
{
    private const string ClaimEventSql =
        "INSERT INTO notifications.\"ProcessedOrderEvents\" (\"EventId\", \"ProcessedAt\")\n" +
        "VALUES ({0}, {1})\n" +
        "ON CONFLICT (\"EventId\") DO NOTHING;";

    public async Task<bool> TryStoreAsync(Guid eventId, IReadOnlyList<InboxNotification> notifications, CancellationToken ct)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        await using var transaction = await context.Database.BeginTransactionAsync(ct);

        var now = clock.GetUtcNow();
        var claimed = await context.Database.ExecuteSqlRawAsync(ClaimEventSql, [eventId, now], ct);

        if (claimed == 0)
        {
            return false;
        }

        context.Notifications.AddRange(notifications.Select(item => item.Notification));
        if (webPush.Value.IsEnabled)
        {
            await EnqueueDeliveriesAsync(context, notifications, now, ct);
        }

        await context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return true;
    }

    private static async Task EnqueueDeliveriesAsync(
        NotificationsDbContext context, IReadOnlyList<InboxNotification> notifications, DateTimeOffset now, CancellationToken ct)
    {
        var recipients = notifications.Select(item => item.Notification.UserId).Distinct().ToArray();
        var subscriptions = await context.PushSubscriptions
            .Where(subscription => recipients.Contains(subscription.UserId))
            .Select(subscription => new { subscription.Id, subscription.UserId })
            .ToListAsync(ct);
        var byRecipient = subscriptions.ToLookup(subscription => subscription.UserId);
        var expiresAt = now + PushDeliveryPolicy.JobLifetime;

        foreach (var item in notifications)
        {
            foreach (var subscription in byRecipient[item.Notification.UserId])
            {
                context.Set<PushDelivery>().Add(new PushDelivery(
                    Guid.NewGuid(), item.Notification.Id, subscription.Id, item.PushPayload, now, expiresAt));
            }
        }
    }
}
