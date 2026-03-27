namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Products.Documents;

public sealed class ProductVariantReadModel
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = default!;
    public decimal? Price { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }
    public int Order { get; set; }
    public Dictionary<string, string> Options { get; set; } = new();
    public List<ProductMediaReadModel> Media { get; set; } = new();
}
