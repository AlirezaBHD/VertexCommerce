using Microsoft.EntityFrameworkCore.Diagnostics;
using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Shared.Persistence.Outbox;

public sealed class OutboxInterceptor(OutboxSignal signal) : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
            return await base.SavingChangesAsync(eventData, result, cancellationToken);

        var dbContext = eventData.Context;

        var domainEvents = dbContext.ChangeTracker
            .Entries<IHasDomainEvents>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Count > 0)
            .SelectMany(e =>
            {
                var events = e.DomainEvents.ToList();
                e.ClearDomainEvents();
                return events;
            })
            .ToList();

        if (domainEvents.Count == 0)
            return await base.SavingChangesAsync(eventData, result, cancellationToken);

        var outboxMessages = domainEvents.ConvertAll(e => new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = e.GetType().FullName ?? e.GetType().Name,
            Content = OutboxSerializer.Serialize(e),
            OccurredOn = e.OccurredOn,
            RetryCount = 0
        });

        await dbContext.Set<OutboxMessage>().AddRangeAsync(outboxMessages, cancellationToken);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        signal.Trigger();

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}
