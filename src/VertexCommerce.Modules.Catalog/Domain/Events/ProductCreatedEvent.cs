using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Catalog.Domain.Events;

public sealed record ProductCreatedEvent(
    Guid ProductId,
    string Name,
    string Sku,
    decimal Price,
    string Currency
) : DomainEvent;
