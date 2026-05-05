using VertexCommerce.Modules.Catalog.Features.Products.Commands.CreateProduct;

namespace VertexCommerce.Modules.Catalog.Features.Categories.Commands.UpdateCategory;

public sealed record UpdateCategoryRequest(
    string Name,
    string Description,
    SeoMetadataRequest Seo,
    string? IconPath,
    string CoverImagePath,
    string? ImageAltText,
    Guid? ParentId,
    bool IsActive,
    bool ShowOnHome,
    bool IncludeInMenu,
    int SortOrder
);
