using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.CreateProduct;

public sealed record CreateProductCommand(
    string Name,
    string? Description,
    string Sku,
    decimal Price,
    string Currency,
    int StockQuantity,
    Guid CategoryId
) : ICommand<Guid>;
