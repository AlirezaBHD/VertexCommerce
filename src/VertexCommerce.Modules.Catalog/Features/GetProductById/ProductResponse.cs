namespace VertexCommerce.Modules.Catalog.Features.GetProductById;

public sealed record ProductResponse(
    Guid Id,
    string Name,
    string? Description,
    string Sku,
    decimal Price,
    string Currency,
    int StockQuantity,
    bool IsActive,
    Guid CategoryId,
    string? CategoryName,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    IReadOnlyList<ProductAttributeResponse> Attributes
);

public sealed record ProductAttributeResponse(
    string Key,
    string Value,
    string? Type
);
