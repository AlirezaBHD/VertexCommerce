using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Modules.Catalog.Domain.Products.ValueObjects;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Products.Documents;

namespace VertexCommerce.Modules.Catalog.Sync;

internal static class ProductReadModelMapper
{
    public static ProductReadModel Map(
        Product product,
        string categoryName,
        string? categoryPath)
    {
        var variants = MapVariants(product.Variants);
        var activeVariants = variants.Where(v => v.IsActive).ToList();

        var prices = activeVariants.Select(v => v.Price).ToList();
        var minPrice = prices.Count > 0 ? prices.Min() : 0m;
        var maxPrice = prices.Count > 0 ? prices.Max() : 0m;
        var totalStock = activeVariants.Sum(v => v.StockQuantity);

        var attributes = product.Attributes.ToDictionary(a => a.Key, a => a.Value);
        var availableOptions = BuildAvailableOptions(variants);
        var searchText = BuildSearchText(product, categoryName, variants, attributes);

        return new ProductReadModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            MinPrice = minPrice ?? 0,
            MaxPrice = maxPrice ?? 0,
            TotalStock = totalStock,
            IsActive = product.IsActive,
            CategoryId = product.CategoryId,
            CategoryName = categoryName,
            CategoryPath = categoryPath,
            Variants = variants,
            AvailableOptions = availableOptions, Attributes = attributes,
            SearchText = searchText,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt,
            SyncedAt = DateTime.UtcNow
        };
    }

    private static List<ProductVariantReadModel> MapVariants(
        IReadOnlyCollection<ProductVariant> variants)
    {
        return variants.Select(v => new ProductVariantReadModel
        {
            Id = v.Id,
            Sku = v.Sku.Value,
            Price = v.Price.Amount,
            StockQuantity = v.StockQuantity,
            IsActive = v.IsActive,
            Order = v.Order,
            Options = v.Options.ToDictionary(o => o.Name, o => o.Value),
            Media = MapMedia(v.Media)
        }).ToList();
    }

    private static List<ProductMediaReadModel> MapMedia(
        IReadOnlyCollection<ProductMedia>? mediaList)
    {
        if (mediaList is null || mediaList.Count == 0)
            return [];

        return mediaList
            .OrderBy(m => m.Order)
            .Select(m => new ProductMediaReadModel
            {
                Path = m.Path,
                Type = m.Type.ToString(),
                AltText = m.AltText,
                Order = m.Order
            }).ToList();
    }

    private static Dictionary<string, List<string>> BuildAvailableOptions(
        List<ProductVariantReadModel> variants)
    {
        var options = new Dictionary<string, List<string>>();

        foreach (var variant in variants.Where(v => v.IsActive))
        {
            foreach (var (key, value) in variant.Options)
            {
                if (!options.TryGetValue(key, out var values))
                {
                    values = [];
                    options[key] = values;
                }

                if (!values.Contains(value))
                    values.Add(value);
            }
        }

        return options;
    }

    private static string BuildSearchText(
        Product product,
        string categoryName,
        List<ProductVariantReadModel> variants,
        Dictionary<string, string> attributes)
    {
        var parts = new List<string> { product.Name, categoryName };

        if (!string.IsNullOrWhiteSpace(product.Description))
            parts.Add(product.Description);

        foreach (var variant in variants)
        {
            parts.Add(variant.Sku);
            parts.AddRange(variant.Options.Values);
        }

        parts.AddRange(attributes.Values);

        return string.Join(" ", parts).ToLowerInvariant();
    }
}
