using MongoDB.Driver;
using VertexCommerce.Modules.Catalog.Domain.Banners;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories.Documents;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Products.Documents;

namespace VertexCommerce.Modules.Catalog.Services;

public interface IBannerService
{
    string? ResolveHref(BannerTarget target, out bool isExternal);
}

internal sealed class BannerService(IMongoDatabase database) : IBannerService
{
    private readonly IMongoCollection<ProductReadModel> _products =
        database.GetCollection<ProductReadModel>("products");

    private readonly IMongoCollection<CategoryReadModel> _categories =
        database.GetCollection<CategoryReadModel>("categories");

    public string? ResolveHref(BannerTarget target, out bool isExternal)
    {
        isExternal = false;

        switch (target.Type)
        {
            case TargetType.None:
                return null;

            case TargetType.Product:
                var slug = target.ProductSlugSnapshot;
                if (string.IsNullOrEmpty(slug))
                {
                    // Try to resolve from DB using ProductId
                    if (target.ProductId.HasValue)
                    {
                        var product = _products.Find(p => p.Id == target.ProductId.Value).FirstOrDefault();
                        slug = product?.Slug;
                    }
                }
                return string.IsNullOrEmpty(slug) ? null : $"/products/{slug}";

            case TargetType.Category:
                var catSlug = target.CategorySlugSnapshot;
                if (string.IsNullOrEmpty(catSlug))
                {
                    if (target.CategoryId.HasValue)
                    {
                        var category = _categories.Find(c => c.Id == target.CategoryId.Value).FirstOrDefault();
                        catSlug = category?.Slug;
                    }
                }
                return string.IsNullOrEmpty(catSlug) ? null : $"/categories/{catSlug}";

            case TargetType.InternalPath:
                return target.InternalPath;

            case TargetType.ExternalUrl:
                isExternal = true;
                return target.ExternalUrl;

            default:
                return null;
        }
    }
}
