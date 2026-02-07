using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
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

            // Convention pack
            var conventionPack = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreExtraElementsConvention(true),
                new EnumRepresentationConvention(BsonType.String)
            };

            ConventionRegistry.Register("VertexCommerceConventions", conventionPack, _ => true);

            // Guid serialization
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            // CustomerBasket mapping
            if (!BsonClassMap.IsClassMapRegistered(typeof(CustomerBasket)))
            {
                BsonClassMap.RegisterClassMap<CustomerBasket>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Id);
                    cm.SetIgnoreExtraElements(true);
                });
            }

            // BasketItem mapping
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
