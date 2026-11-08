namespace VertexCommerce.Modules.Catalog.Features.Products.Commands.UpdateProduct;

public sealed record UpdateProductAttributeDto(
    string AttributeCode,
    string OptionCode
);
