using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Catalog.Domain.Products.Events;

public sealed record ProductDeletedEvent(
    Guid ProductId
) : DomainEvent;
