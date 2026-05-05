using HotChocolate;
using HotChocolate.Data;
using MongoDB.Driver;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories.Documents;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Products.Documents;
using VertexCommerce.Shared.Contracts.Catalog;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Products;

internal sealed class ProductReadModelRepository(IMongoDatabase database) : IProductReadModelRepository
{
    private readonly IMongoCollection<ProductReadModel> _collection =
        database.GetCollection<ProductReadModel>("products");


    public IExecutable<ProductReadModel> GetFilteredProducts(
        string? searchTerm = null,
        Guid? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool? isActive = null)
    {
        var filter = ProductQueryFilterBuilder.BuildSearchFilter(
            searchTerm, categoryId, minPrice, maxPrice, isActive);

        return _collection
            .Find(filter)
            .AsExecutable();
    }


    public async Task UpsertAsync(
        ProductReadModel model,
        CancellationToken ct = default)
    {
        model.SyncedAt = DateTime.UtcNow;

        await _collection.ReplaceOneAsync(
            Builders<ProductReadModel>.Filter.Eq(p => p.Id, model.Id),
            model,
            new ReplaceOptions { IsUpsert = true },
            ct);
    }

    public async Task UpsertManyAsync(
        IEnumerable<ProductReadModel> models,
        CancellationToken ct = default)
    {
        var writes = models.Select(model =>
        {
            model.SyncedAt = DateTime.UtcNow;
            return new ReplaceOneModel<ProductReadModel>(
                    Builders<ProductReadModel>.Filter.Eq(p => p.Id, model.Id),
                    model)
                { IsUpsert = true };
        }).ToList();

        if (writes.Count > 0)
        {
            await _collection.BulkWriteAsync(writes, cancellationToken: ct);
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await _collection.DeleteOneAsync(
            Builders<ProductReadModel>.Filter.Eq(p => p.Id, id), ct);
    }

    public IExecutable<ProductReadModel> GetLatestProducts(int limit)
    {
        var query = _collection.AsQueryable()
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .Take(limit);

        return query.AsExecutable();
    }

    public IExecutable<ProductReadModel> GetBySlugAsync(string slug)
    {
        var query = _collection.AsQueryable()
            .Where(p => p.Slug == slug);

        return query.AsExecutable();
    }

    public IExecutable<ProductReadModel> GetAll()
    {
        return _collection.AsExecutable();
    }

    public async Task<ProductVariantInfo?> GetProductVariantInfoAsync(Guid productId, Guid variantId,
        CancellationToken ct = default)
    {
        var result = await _collection.Aggregate()
            .Match(p => p.Id == productId && p.IsActive)
            .Project(p => new
            {
                p.Id,
                p.Name,
                p.Media,
                Variant = p.Variants.FirstOrDefault(v => v.Id == variantId)
            })
            .FirstOrDefaultAsync(ct);

        if (result?.Variant is null || !result.Variant.IsActive)
            return null;

        var variant = result.Variant;
        var atr = variant.Attributes.First(); //TODO
        var imagePath = result.Media
            .FirstOrDefault(m => m.AssociatedAttributeCode == atr.AttributeCode && m.AssociatedOptionCode == atr.OptionCode)?.Path ?? result.Media.First().Path;

        return new ProductVariantInfo(
            ProductId: result.Id,
            VariantId: variant.Id,
            Name: result.Name,
            Sku: variant.Sku,
            ImagePath: imagePath,
            Price: variant.Price,
            StockQuantity: variant.StockQuantity,
            Attributes: variant.Attributes.Select(a =>
                new ProductInfoAttribute(a.AttributeCode, a.OptionCode)).ToList()
        );
    }
}
