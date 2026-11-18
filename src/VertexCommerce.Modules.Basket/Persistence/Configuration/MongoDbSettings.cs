namespace VertexCommerce.Modules.Basket.Persistence.Configuration;

/// <summary>
/// Basket-specific MongoDB configuration. The connection string and database are owned
/// by the host (ConnectionStrings:MongoDb via AddMongoDb); the module only names its collection.
/// </summary>
public sealed class MongoDbSettings
{
    public string BasketsCollectionName { get; set; } = "baskets";
}
