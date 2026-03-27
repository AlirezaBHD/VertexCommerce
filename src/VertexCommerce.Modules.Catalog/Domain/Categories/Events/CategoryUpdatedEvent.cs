using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Catalog.Domain.Categories.Events;

public sealed record CategoryUpdatedEvent(
    Guid CategoryId,
    string Name,
    Guid? ParentId,
    bool IsActive,
    int SortOrder)
    : DomainEvent;
