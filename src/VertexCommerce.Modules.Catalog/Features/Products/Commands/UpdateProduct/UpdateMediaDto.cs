namespace VertexCommerce.Modules.Catalog.Features.Products.Commands.UpdateProduct;

public sealed record UpdateMediaDto(
    string Path,
    int SortOrder,
    string? AltText,
    string? AssociatedAttributeCode,
    string? AssociatedOptionCode
);
