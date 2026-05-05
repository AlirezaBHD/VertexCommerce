using VertexCommerce.Modules.Catalog.Domain.Categories;
using VertexCommerce.Modules.Catalog.Domain.Products.ValueObjects;
using VertexCommerce.Modules.Catalog.Persistence.Postgres;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Categories.Commands.CreateCategory;

public sealed class CreateCategoryCommandHandler : ICommandHandler<CreateCategoryCommand, Guid>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICatalogUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        ICatalogUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateCategoryCommand command, CancellationToken ct)
    {
        if (command.ParentId.HasValue)
        {
            var parentExists = await _categoryRepository.ExistsAsync(command.ParentId.Value, ct);
            if (!parentExists)
            {
                return Result.Failure<Guid>(Error.NotFound("Parent Category", command.ParentId.Value));
            }
        }

        var nameExists = await _categoryRepository.NameExistsAsync(command.Name, null, ct);
        if (nameExists)
        {
            return Result.Failure<Guid>(Error.Conflict($"Category with name '{command.Name}' already exists."));
        }

        var slugExists = await _categoryRepository.SlugExistsAsync(command.Name, null, ct);
        if (slugExists)
        {
            return Result.Failure<Guid>(Error.Conflict($"Category with slug '{command.Seo.Slug}' already exists."));
        }

        var seo = SeoMetadata.Create(
            slug: command.Seo.Slug,
            metaTitle: command.Seo.MetaTitle,
            metaDescription: command.Seo.MetaDescription,
            keywords: command.Seo.Keywords);

        var category = Category.Create(
            name: command.Name,
            description: command.Description,
            seoMetadata: seo,
            iconPath: command.IconPath,
            coverImagePath: command.CoverImagePath,
            imageAltText: command.ImageAltText,
            parentId: command.ParentId,
            isActive: command.IsActive,
            showOnHome: command.ShowOnHome,
            includeInMenu: command.IncludeInMenu,
            sortOrder: command.SortOrder
        );

        await _categoryRepository.AddAsync(category, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success(category.Id);
    }
}
