namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Products.Documents;

public sealed class ProductVariantReadModel
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = default!;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int ReservedQuantity { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
    public List<ProductAttributeReadModel> Attributes { get; set; } = [];
}