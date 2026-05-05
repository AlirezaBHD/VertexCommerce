namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Products.Documents;

public sealed class ProductAttributeReadModel
{
    public string AttributeCode { get; set; } = string.Empty;
    public string OptionCode { get; set; } = string.Empty;
}
