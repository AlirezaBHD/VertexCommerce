using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Catalog.Domain.Events;

public sealed record ProductDeletedEvent(
    Guid ProductId
) : DomainEvent;
