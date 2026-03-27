using MongoDB.Bson.Serialization;
using VertexCommerce.Modules.Basket.Domain.Entities;

namespace VertexCommerce.Modules.Basket.Persistence;

public static class MongoDbConfiguration
{
    private static bool _configured;
    private static readonly object Lock = new();

    public static void Configure()
    {
        lock (Lock)
        {
            if (_configured) return;

            // ❌ حذف شد - Convention و GuidSerializer
            // اینا دیگه اینجا نیستن!

            // ✅ فقط ClassMap های مربوط به Basket Module
            if (!BsonClassMap.IsClassMapRegistered(typeof(CustomerBasket)))
            {
                BsonClassMap.RegisterClassMap<CustomerBasket>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Id);
                    cm.SetIgnoreExtraElements(true);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(BasketItem)))
            {
                BsonClassMap.RegisterClassMap<BasketItem>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }

            _configured = true;
        }
    }
}
