using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Catalog.Domain.Products.Events;

public sealed record ProductCreatedEvent(
    Guid ProductId,
    string Name
) : DomainEvent;
