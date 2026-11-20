using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Shared.Persistence;

public sealed class DomainEventInterceptor : SaveChangesInterceptor
{
    private readonly IMediator _mediator;
    private readonly ILogger<DomainEventInterceptor> _logger;

    public DomainEventInterceptor(
        IMediator mediator,
        ILogger<DomainEventInterceptor> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            await DispatchDomainEventsAsync(eventData.Context, cancellationToken);
        }

        return result;
    }

    private async Task DispatchDomainEventsAsync(
        DbContext context,
        CancellationToken cancellationToken)
    {
        var entities = context.ChangeTracker
            .Entries<IHasDomainEvents>()
            .Where(e => e.Entity.DomainEvents.Count != 0)
            .Select(e => e.Entity)
            .ToList();

        if (entities.Count == 0) return;

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        entities.ForEach(e => e.ClearDomainEvents());

        _logger.LogInformation(
            "Dispatching {Count} domain event(s)...", domainEvents.Count);

        foreach (var domainEvent in domainEvents)
        {
            _logger.LogInformation(
                "  → Publishing {EventType}", domainEvent.GetType().Name);

            await _mediator.Publish(domainEvent, cancellationToken);
        }

        _logger.LogInformation("All domain events dispatched.");
    }
}
