namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Products.Documents;

public sealed class ProductMediaReadModel
{
    public string Path { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Order { get; set; }
    public string? AltText { get; set; }
}
