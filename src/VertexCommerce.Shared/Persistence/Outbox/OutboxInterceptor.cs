// VertexCommerce.Shared/Persistence/Outbox/OutboxInterceptor.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;
using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Shared.Persistence.Outbox;

public sealed class OutboxInterceptor(OutboxSignal signal) : SaveChangesInterceptor
{
    private static readonly JsonSerializerSettings SerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    };

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

        var outboxMessages = domainEvents.Select(e => new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = e.GetType().Name,
            Content = JsonConvert.SerializeObject(e, SerializerSettings),
            OccurredOn = e.OccurredOn,
            RetryCount = 0
        }).ToList();

        await dbContext.Set<OutboxMessage>().AddRangeAsync(outboxMessages, cancellationToken);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        // Signal فقط بعد از commit موفق
        signal.Trigger();

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}
