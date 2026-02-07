namespace VertexCommerce.Modules.Basket.Persistence;

public sealed class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string BasketsCollectionName { get; set; } = "baskets";
}
