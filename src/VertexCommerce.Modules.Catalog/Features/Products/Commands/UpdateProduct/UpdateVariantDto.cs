namespace VertexCommerce.Modules.Catalog.Features.Products.Commands.UpdateProduct;

public sealed record UpdateVariantDto(
    Guid? Id,
    decimal Price,
    string? Currency,
    int StockQuantity,
    int SortOrder,
    string? Sku,
    List<UpdateProductAttributeDto> Attributes
);
