using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Catalog.Domain.Categories.Events;

public sealed record CategoryDeletedEvent(
    Guid CategoryId
) : DomainEvent;
