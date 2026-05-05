namespace VertexCommerce.Modules.Catalog.Features.Products.Queries.GetProductById.DTOs;

public sealed record ProductAttributeDto(
    string AttributeCode,
    string OptionCode
);