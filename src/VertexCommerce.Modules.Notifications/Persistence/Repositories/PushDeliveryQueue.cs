using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Notifications.Domain.Entities;
using VertexCommerce.Modules.Notifications.Domain.Repositories;
using VertexCommerce.Modules.Notifications.Services.Delivery;

namespace VertexCommerce.Modules.Notifications.Persistence.Repositories;

internal sealed class PushDeliveryQueue(NotificationsDbContext context, IPushSubscriptionRepository subscriptions)
{
    private const string ClaimSql =
        "UPDATE notifications.\"PushDeliveries\" AS delivery\n" +
        "SET \"LeaseId\" = {0}, \"LeaseExpiresAt\" = {1}\n" +
        "WHERE delivery.\"Id\" = (\n" +
        "    SELECT pending.\"Id\" FROM notifications.\"PushDeliveries\" AS pending\n" +
        "    WHERE pending.\"CompletedAt\" IS NULL AND pending.\"NextAttemptAt\" <= {2}\n" +
        "        AND (pending.\"LeaseExpiresAt\" IS NULL OR pending.\"LeaseExpiresAt\" <= {2})\n" +
        "    ORDER BY pending.\"NextAttemptAt\", pending.\"Id\"\n" +
        "    FOR UPDATE SKIP LOCKED LIMIT 1\n" +
        ")\n" +
        "RETURNING delivery.*;";

    public async Task<PushDelivery?> ClaimAsync(DateTimeOffset now, TimeSpan leaseDuration, CancellationToken ct)
    {
        var claimed = await context.Set<PushDelivery>()
            .FromSqlRaw(ClaimSql, Guid.NewGuid(), now + leaseDuration, now)
            .AsNoTracking()
            .ToListAsync(ct);

        return claimed.SingleOrDefault();
    }

    public async Task<bool> SetResultAsync(
        PushDelivery delivery, int attempts, int? httpStatus, DateTimeOffset nextAttemptAt,
        PushDeliveryOutcome? outcome, DateTimeOffset now, PushSubscription? expiredSubscription, CancellationToken ct)
    {
        var leaseId = delivery.LeaseId ?? throw new InvalidOperationException("A delivery result requires an active lease.");
        // Keep the provider call outside the transaction. The lease guards the completion update and cleanup.
        await using var transaction = await context.Database.BeginTransactionAsync(ct);
        var updated = await context.Set<PushDelivery>()
            .Where(row => row.Id == delivery.Id && row.LeaseId == leaseId && row.CompletedAt == null)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(row => row.Attempts, attempts)
                .SetProperty(row => row.LastHttpStatus, httpStatus)
                .SetProperty(row => row.NextAttemptAt, nextAttemptAt)
                .SetProperty(row => row.Outcome, outcome)
                .SetProperty(row => row.CompletedAt, outcome.HasValue ? now : (DateTimeOffset?)null)
                .SetProperty(row => row.LeaseId, (Guid?)null)
                .SetProperty(row => row.LeaseExpiresAt, (DateTimeOffset?)null), ct);

        if (updated == 1 && expiredSubscription is not null)
        {
            await subscriptions.RemoveIfUnchangedAsync(expiredSubscription, ct);
        }

        await transaction.CommitAsync(ct);
        return updated == 1;
    }
}
