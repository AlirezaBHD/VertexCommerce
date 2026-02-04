using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Catalog.Domain.Events;

public sealed record ProductUpdatedEvent(
    Guid ProductId,
    string Name,
    decimal Price
) : DomainEvent;
