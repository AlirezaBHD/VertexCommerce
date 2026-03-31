using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Shared.Specifications;

namespace VertexCommerce.Modules.Catalog.Features.Products.Queries.GetProductById;

public sealed class ProductByIdSpec : BaseSpecification<Product, ProductResponse>
{
    public ProductByIdSpec(Guid productId)
    {
        Where(p => p.Id == productId);
        
        Include(p => p.Category!);
        Include(p => p.Attributes);
        Include(p => p.Seo);
        Include("Variants.Options");
        Include("Variants.Media");
        
        Select(p => new ProductResponse(
            p.Name,
            p.Description,
            p.IsActive,
            p.CategoryId,
            p.CreatedAt,
            p.UpdatedAt,
            p.Attributes.Select(a => new ProductAttributeResponse(
                a.Key,
                a.Value,
                a.Type
            )).ToList(),
            new SeoMetadataResponse(
                p.Seo.Slug,
                p.Seo.MetaTitle,
                p.Seo.MetaDescription,
                p.Seo.Keywords
            ),
            p.Variants.Select(v => new ProductVariantResponse(
                v.Id,
                v.Price.Amount,
                v.StockQuantity,
                v.Order,
                v.Options.Select(o => new VariantOptionDto(o.Name, o.Value)).ToList(),
                v.Media.Select(m => new MediaDto(m.Path, m.Order)).ToList()
            )).ToList()
        ));
    }
}
