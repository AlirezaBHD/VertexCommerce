using MongoDB.Bson.Serialization;
using VertexCommerce.Modules.Basket.Persistence.Documents;

namespace VertexCommerce.Modules.Basket.Persistence.Configuration;

public static class MongoDbConfiguration
{
    private static bool _configured;
    private static readonly object Lock = new();

    public static void Configure()
    {
        lock (Lock)
        {
            if (_configured) return;

            // ✅ فقط ClassMap های مربوط به Basket Module
            if (!BsonClassMap.IsClassMapRegistered(typeof(BasketDocument)))
            {
                BsonClassMap.RegisterClassMap<BasketDocument>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Id);
                    cm.SetIgnoreExtraElements(true);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(BasketItemDocument)))
            {
                BsonClassMap.RegisterClassMap<BasketItemDocument>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }
            
            if (!BsonClassMap.IsClassMapRegistered(typeof(BasketItemAttributeDocument)))
            {
                BsonClassMap.RegisterClassMap<BasketItemAttributeDocument>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }

            _configured = true;
        }
    }
}
