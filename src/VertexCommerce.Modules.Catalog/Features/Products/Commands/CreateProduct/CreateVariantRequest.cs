namespace VertexCommerce.Modules.Catalog.Features.Products.Commands.CreateProduct;

public sealed record CreateVariantRequest(
    decimal Price,
    string? Currency,
    int StockQuantity,
    int SortOrder,
    List<ProductAttributeRequest> Attributes
);
