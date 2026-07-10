using VertexCommerce.Modules.Catalog.Domain.Categories;
using VertexCommerce.Modules.Catalog.Domain.Products.ValueObjects;
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
            Name: p.Name,
            Description: p.Description,
            Seo: MapSeoMetadata(p.Seo),
            IconPath: p.IconPath,
            CoverImagePath: p.CoverImagePath,
            ImageAltText: p.ImageAltText,
            ParentId: p.ParentId,
            IsActive: p.IsActive,
            ShowOnHome: p.ShowOnHome,
            IncludeInMenu: p.IncludeInMenu,
            SortOrder: p.SortOrder
        ));
    }

    private static SeoMetadataResponse MapSeoMetadata(SeoMetadata seo) =>
        new(seo.Slug, seo.MetaTitle, seo.MetaDescription, seo.Keywords);
}
