using VertexCommerce.Modules.Catalog.Features.Products.Commands.CreateProduct;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Categories.Commands.CreateCategory;

public sealed record CreateCategoryCommand(
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
) : ICommand<Guid>;
