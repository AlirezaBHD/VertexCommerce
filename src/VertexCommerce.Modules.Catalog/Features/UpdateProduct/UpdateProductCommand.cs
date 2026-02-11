using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.UpdateProduct;

public sealed record UpdateProductCommand(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    string Currency,
    Guid CategoryId
) : ICommand;
