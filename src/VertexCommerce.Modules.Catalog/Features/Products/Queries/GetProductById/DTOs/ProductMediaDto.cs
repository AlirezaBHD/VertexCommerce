namespace VertexCommerce.Modules.Catalog.Features.Products.Queries.GetProductById.DTOs;

public sealed record ProductMediaDto(
    string Path,
    string MediaType,
    int SortOrder,
    string? AltText,
    string? AssociatedAttributeCode,
    string? AssociatedOptionCode
);