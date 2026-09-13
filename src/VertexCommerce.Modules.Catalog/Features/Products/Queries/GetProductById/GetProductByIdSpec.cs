using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Modules.Catalog.Domain.ValueObjects;
using VertexCommerce.Modules.Catalog.Features.Products.Queries.GetProductById.DTOs;
using VertexCommerce.Shared.Specifications;

namespace VertexCommerce.Modules.Catalog.Features.Products.Queries.GetProductById;

public sealed class GetProductByIdSpec : BaseSpecification<Product, ProductResponse>
{
    public GetProductByIdSpec(Guid productId)
    {
        Where(p => p.Id == productId);
        
        Include(p => p.Category!);
        Include(p => p.Media);
        Include(p => p.Seo);
        Include(p => p.Variants);
        Select(p => new ProductResponse(
            p.Name.Value,
            p.Description.HasValue ? p.Description.Value.Value : null,
            p.IsActive,
            p.CategoryId,
            p.CreatedAt,
            p.UpdatedAt,
            MapSeoMetadata(p.Seo),
            MapVariants(p.Variants),
            MapMedia(p.Media)
        ));
    }

    private static SeoMetadataResponse MapSeoMetadata(SeoMetadata seo) =>
        new(seo.Slug.Value, seo.MetaTitle.HasValue ? seo.MetaTitle.Value.Value : string.Empty, seo.MetaDescription.HasValue ? seo.MetaDescription.Value.Value : string.Empty, seo.Keywords.HasValue ? seo.Keywords.Value.Value : null);

    private static List<ProductVariantDto> MapVariants(IEnumerable<ProductVariant> variants) =>
        variants.Select(v => new ProductVariantDto(
            v.Id,
            v.Sku.ToString(),
            v.Price.Amount,
            v.StockQuantity,
            v.IsActive,
            v.SortOrder,
            MapAttributes(v.Attributes)
        )).ToList();

    private static List<ProductAttributeDto> MapAttributes(IEnumerable<ProductAttribute> attributes) =>
        attributes.Select(a => new ProductAttributeDto(a.AttributeCode.Value, a.OptionCode.Value)).ToList();

    private static List<ProductMediaDto> MapMedia(IEnumerable<ProductMedia> media) =>
        media.Select(m => new ProductMediaDto(m.Path.Value, m.Type.ToString(), m.SortOrder, m.AltText?.Value, m.AssociatedAttributeCode?.Value, m.AssociatedOptionCode?.Value)).ToList();
}
