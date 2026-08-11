using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Modules.Catalog.Domain.Products.ValueObjects;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories.Documents;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Products.Documents;

namespace VertexCommerce.Modules.Catalog.Sync.Products;

internal static class ProductReadModelMapper
{
    public static ProductReadModel Map(
        Product product,
        string categoryName,
        List<CategoryBreadcrumb> breadcrumb)
    {
        var variants = MapVariants(product.Variants);
        var activeVariants = variants.Where(v => v.IsActive).ToList();

        var prices = activeVariants.Select(v => v.Price).ToList();
        var minPrice = prices.Count > 0 ? prices.Min() : 0m;
        var maxPrice = prices.Count > 0 ? prices.Max() : 0m;
        var totalStock = activeVariants.Sum(v => v.StockQuantity);

        var availableOptions = BuildAvailableOptions(variants);
        var searchText = BuildSearchText(product, categoryName, variants);

        return new ProductReadModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            TotalStock = totalStock,
            IsActive = product.IsActive,
            CategoryId = product.CategoryId,
            CategoryName = categoryName,
            Breadcrumb = breadcrumb,
            Variants = variants,
            AvailableOptions = availableOptions,
            Media = MapMedia(product.Media),
            SearchText = searchText,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt,
            SyncedAt = DateTime.UtcNow,
            Slug = product.Seo.Slug,
            MetaTitle = product.Seo.MetaTitle,
            MetaDescription = product.Seo.MetaDescription,
            Keywords = product.Seo.Keywords
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
            SortOrder = v.SortOrder,
            Attributes = v.Attributes.Select(a => new ProductAttributeReadModel
            {
                AttributeCode = a.AttributeCode,
                OptionCode = a.OptionCode
            }).ToList()
        }).ToList();
    }

    private static List<ProductMediaReadModel> MapMedia(
        IReadOnlyCollection<ProductMedia> mediaList)
    {
        return mediaList
            .OrderBy(m => m.SortOrder)
            .Select(m => new ProductMediaReadModel
            {
                Path = m.Path,
                Type = m.Type.ToString(),
                AltText = m.AltText,
                SortOrder = m.SortOrder,
                AssociatedAttributeCode = m.AssociatedAttributeCode,
                AssociatedOptionCode = m.AssociatedOptionCode
            }).ToList();
    }

    private static Dictionary<string, List<string>> BuildAvailableOptions(
        List<ProductVariantReadModel> variants)
    {
        var options = new Dictionary<string, List<string>>();

        foreach (var variant in variants.Where(v => v.IsActive))
        {
            foreach (var attr in variant.Attributes)
            {
                if (!options.TryGetValue(attr.AttributeCode, out var values))
                {
                    values = [];
                    options[attr.AttributeCode] = values;
                }

                if (!values.Contains(attr.OptionCode))
                    values.Add(attr.OptionCode);
            }
        }

        return options
            .OrderBy(x => x.Key)
            .ToDictionary(
                x => x.Key, 
                x => x.Value.OrderBy(v => v).ToList()
            );    }

    private static string BuildSearchText(
        Product product,
        string categoryName,
        List<ProductVariantReadModel> variants)
    {
        var parts = new List<string> { product.Name, categoryName };

        if (!string.IsNullOrWhiteSpace(product.Description))
            parts.Add(product.Description);

        foreach (var variant in variants)
        {
            parts.Add(variant.Sku);
            parts.AddRange(variant.Attributes.Select(a => $"{a.AttributeCode} {a.OptionCode}"));
        }

        return string.Join(" ", parts).ToLowerInvariant();
    }
}
