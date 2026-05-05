using MongoDB.Driver;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories.Documents;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories;

internal sealed class CategoryIndexManager(IMongoDatabase database)
{
    private readonly IMongoCollection<CategoryReadModel> _collection = database.GetCollection<CategoryReadModel>("categories");

    public async Task EnsureIndexesAsync(
        CancellationToken ct = default)
    {
        var indexes = new List<CreateIndexModel<CategoryReadModel>>
        {
            // new(
            //     Builders<CategoryReadModel>.IndexKeys.Ascending(c => c.Slug),
            //     new CreateIndexOptions 
            //     { 
            //         Name = "IX_Slug_Unique", 
            //         Unique = true
            //     }
            // ),
            
            new(
                Builders<CategoryReadModel>.IndexKeys.Ascending(c => c.ParentId),
                new CreateIndexOptions { Name = "IX_ParentId" }
            ),

            new(
                Builders<CategoryReadModel>.IndexKeys
                    .Ascending(c => c.IsActive)
                    .Ascending(c => c.SortOrder),
                new CreateIndexOptions { Name = "IX_IsActive_SortOrder" }
            ),

            new(
                Builders<CategoryReadModel>.IndexKeys.Ascending(c => c.Path),
                new CreateIndexOptions { Name = "IX_Path" }
            ),

            new(
                Builders<CategoryReadModel>.IndexKeys.Ascending(c => c.Depth),
                new CreateIndexOptions { Name = "IX_Depth" }
            )
        };

        await _collection.Indexes.CreateManyAsync(indexes, ct);
    }
}
