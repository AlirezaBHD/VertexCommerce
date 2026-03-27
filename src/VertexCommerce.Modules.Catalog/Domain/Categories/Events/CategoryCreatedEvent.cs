using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Catalog.Domain.Categories.Events;

public sealed record CategoryCreatedEvent(
    Guid CategoryId,
    string Name,
    Guid? ParentId
) : DomainEvent;
