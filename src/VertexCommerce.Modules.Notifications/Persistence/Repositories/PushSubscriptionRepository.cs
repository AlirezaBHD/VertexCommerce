using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using VertexCommerce.Modules.Notifications.Domain.Entities;
using VertexCommerce.Modules.Notifications.Domain.Repositories;

namespace VertexCommerce.Modules.Notifications.Persistence.Repositories;

internal sealed class PushSubscriptionRepository(NotificationsDbContext context) : IPushSubscriptionRepository
{
    private const string UpsertSql =
        "INSERT INTO notifications.\"PushSubscriptions\" AS existing\n" +
        "    (\"Id\", \"UserId\", \"Endpoint\", \"P256dhKey\", \"AuthKey\", \"UserAgent\", \"CreatedAt\", \"IsDeleted\")\n" +
        "VALUES ({0}, {1}, {2}, {3}, {4}, @user_agent, {5}, FALSE)\n" +
        "ON CONFLICT (\"Endpoint\") DO UPDATE SET\n" +
        "    \"P256dhKey\" = EXCLUDED.\"P256dhKey\", \"AuthKey\" = EXCLUDED.\"AuthKey\",\n" +
        "    \"UserAgent\" = EXCLUDED.\"UserAgent\", \"UpdatedAt\" = statement_timestamp()\n" +
        "WHERE existing.\"UserId\" = EXCLUDED.\"UserId\";";

    public async Task<bool> TryUpsertOwnedAsync(PushSubscription subscription, CancellationToken ct = default)
    {
        var userAgent = new NpgsqlParameter("user_agent", NpgsqlDbType.Varchar)
        {
            Value = (object?)subscription.UserAgent ?? DBNull.Value
        };
        var affectedRows = await context.Database.ExecuteSqlRawAsync(UpsertSql,
            [subscription.Id, subscription.UserId, subscription.Endpoint, subscription.P256dhKey,
                subscription.AuthKey, subscription.CreatedAt, userAgent], ct);
        return affectedRows == 1;
    }

    public async Task RemoveOwnedByEndpointAsync(Guid userId, string endpoint, CancellationToken ct = default)
    {
        await context.PushSubscriptions
            .Where(subscription => subscription.UserId == userId && subscription.Endpoint == endpoint)
            .ExecuteDeleteAsync(ct);
    }

    public Task<PushSubscription?> GetByIdAsync(Guid subscriptionId, CancellationToken ct = default)
    {
        return context.PushSubscriptions.AsNoTracking().SingleOrDefaultAsync(subscription => subscription.Id == subscriptionId, ct);
    }

    public async Task<IReadOnlyList<PushSubscription>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await context.PushSubscriptions
            .AsNoTracking()
            .Where(subscription => subscription.UserId == userId)
            .ToListAsync(ct);
    }

    public async Task RemoveIfUnchangedAsync(PushSubscription snapshot, CancellationToken ct = default)
    {
        // A delayed provider response must leave a refreshed subscription alone.
        await context.PushSubscriptions
            .Where(subscription => subscription.Id == snapshot.Id && subscription.UpdatedAt == snapshot.UpdatedAt)
            .ExecuteDeleteAsync(ct);
    }
}
