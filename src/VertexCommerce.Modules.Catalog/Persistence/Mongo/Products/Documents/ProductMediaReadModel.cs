namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Products.Documents;

public sealed class ProductMediaReadModel
{
    public string Path { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public string? AltText { get; set; }
    public string? AssociatedAttributeCode { get; set; }
    public string? AssociatedOptionCode { get; set; }
}
