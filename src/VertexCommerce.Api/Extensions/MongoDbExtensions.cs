using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using VertexCommerce.Modules.Catalog;

namespace VertexCommerce.Api.Extensions;

public static class MongoDbExtensions
{
    public static IServiceCollection AddMongoDb(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var conventionPack = new ConventionPack
        {
            new CamelCaseElementNameConvention(),
            new IgnoreExtraElementsConvention(true),
            new EnumRepresentationConvention(BsonType.String)
        };
        ConventionRegistry.Register("defaults", conventionPack, _ => true);

        BsonSerializer.RegisterSerializer(
            new GuidSerializer(GuidRepresentation.Standard));
        
        

        var connectionString = configuration
                                   .GetConnectionString("MongoDb")
                               ?? throw new InvalidOperationException(
                                   "MongoDb connection string is missing");

        var mongoUrl = new MongoUrl(connectionString);
        var client = new MongoClient(mongoUrl);
        var database = client.GetDatabase(
            mongoUrl.DatabaseName ?? "vertex_commerce");

        services.AddSingleton<IMongoClient>(client);
        services.AddSingleton(database);

        return services;
    }

    public static async Task InitializeMongoDbAsync(
        this IServiceProvider services)
    {
        using var scope = services.CreateScope();

        await services.InitializeCatalogIndexesAsync();
    }
}
