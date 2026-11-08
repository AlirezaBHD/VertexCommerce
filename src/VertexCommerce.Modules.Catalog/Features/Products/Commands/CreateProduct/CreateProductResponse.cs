namespace VertexCommerce.Modules.Catalog.Features.Products.Commands.CreateProduct;

public sealed record CreateProductResponse(
    Guid ProductId,
    List<VariantInfo> Variants
);
