using VertexCommerce.Modules.Catalog.Features.Products.Commands.CreateProduct;
using VertexCommerce.Modules.Catalog.Features.Products.Queries.GetProductById.DTOs;

namespace VertexCommerce.Modules.Catalog.Features.Categories.Queries.GetCategoryById.DTOs;

public sealed record CategoryDto(
    string Name,
    string Description,
    SeoMetadataResponse Seo,
    string? IconPath,
    string CoverImagePath,
    string? ImageAltText,
    Guid? ParentId,
    bool IsActive,
    bool ShowOnHome,
    bool IncludeInMenu,
    int SortOrder
);