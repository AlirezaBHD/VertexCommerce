namespace VertexCommerce.Modules.Catalog.Features.Products.Commands.CreateProduct;

public sealed record CreateMediaRequest(
    string Path,
    int SortOrder,
    string? AltText,
    string? AssociatedAttributeCode,
    string? AssociatedOptionCode
);
