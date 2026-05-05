using VertexCommerce.Modules.Basket.Persistence.Documents;
using VertexCommerce.Shared.Contracts.Catalog;

namespace VertexCommerce.Modules.Basket.Features;

internal static class BasketItemMapper
{
    public static BasketItemDocument ToDocument(
        ProductVariantInfo variant,
        int quantity)
    {
        return new BasketItemDocument
        {
            ProductId = variant.ProductId,
            VariantId = variant.VariantId,
            Sku = variant.Sku,
            ProductName = variant.Name,
            Price = variant.Price,
            TotalPrice = variant.Price *  quantity,
            Quantity = quantity,
            StockQuantity = variant.StockQuantity,
            ImagePath = variant.ImagePath,
            Attributes = variant.Attributes.Select(a => new BasketItemAttributeDocument
            {
                AttributeCode = a.AttributeCode,
                OptionCode = a.OptionCode
            }).ToList(),
            AddedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
