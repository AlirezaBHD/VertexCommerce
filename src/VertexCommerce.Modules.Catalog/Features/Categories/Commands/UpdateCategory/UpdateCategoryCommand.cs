using VertexCommerce.Modules.Catalog.Features.Products.Commands.CreateProduct;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Categories.Commands.UpdateCategory;

public sealed record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string Description,
    SeoMetadataRequest Seo,
    string? IconPath,
    string CoverImagePath,
    string? ImageAltText,
    Guid? ParentId,
    bool IsActive,
    bool ShowOnHome,
    bool IncludeInMenu
) : ICommand;
