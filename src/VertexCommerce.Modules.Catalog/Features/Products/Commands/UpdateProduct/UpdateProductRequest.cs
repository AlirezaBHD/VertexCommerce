namespace VertexCommerce.Modules.Catalog.Features.Products.Commands.UpdateProduct;

public sealed record UpdateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    string Currency,
    Guid CategoryId
);