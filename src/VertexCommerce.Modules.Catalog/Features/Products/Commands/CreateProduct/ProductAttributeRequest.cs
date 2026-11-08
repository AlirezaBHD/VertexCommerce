namespace VertexCommerce.Modules.Catalog.Features.Products.Commands.CreateProduct;

public sealed record ProductAttributeRequest(
    string AttributeCode,
    string OptionCode
);
