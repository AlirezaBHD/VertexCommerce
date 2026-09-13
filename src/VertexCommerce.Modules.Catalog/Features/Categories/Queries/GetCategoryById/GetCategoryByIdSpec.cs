using VertexCommerce.Modules.Catalog.Domain.Categories;
using VertexCommerce.Modules.Catalog.Domain.ValueObjects;
using VertexCommerce.Modules.Catalog.Features.Categories.Queries.GetCategoryById.DTOs;
using VertexCommerce.Modules.Catalog.Features.Products.Queries.GetProductById.DTOs;
using VertexCommerce.Shared.Specifications;

namespace VertexCommerce.Modules.Catalog.Features.Categories.Queries.GetCategoryById;

public sealed class GetCategoryByIdSpec : BaseSpecification<Category, CategoryDto>
{
    public GetCategoryByIdSpec(Guid categoryId)
    {
        Where(c => c.Id == categoryId);

        // Include(c => c.Seo);

        Select(p => new CategoryDto(
            Name: p.Name.Value,
            Description: p.Description.Value,
            Seo: MapSeoMetadata(p.Seo),
            IconPath: p.IconPath.HasValue ? p.IconPath.Value.Value : null,
            CoverImagePath: p.CoverImagePath.Value,
            ImageAltText: p.ImageAltText.HasValue ? p.ImageAltText.Value.Value : null,
            ParentId: p.ParentId,
            IsActive: p.IsActive,
            ShowOnHome: p.ShowOnHome,
            IncludeInMenu: p.IncludeInMenu,
            SortOrder: p.SortOrder
        ));
    }

    private static SeoMetadataResponse MapSeoMetadata(SeoMetadata seo) =>
        new(seo.Slug.Value, seo.MetaTitle.HasValue ? seo.MetaTitle.Value.Value : string.Empty, seo.MetaDescription.HasValue ? seo.MetaDescription.Value.Value : string.Empty, seo.Keywords.HasValue ? seo.Keywords.Value.Value : null);
}
