using VertexCommerce.Modules.Catalog.Domain.Categories;
using VertexCommerce.Modules.Catalog.Domain.ValueObjects;
using VertexCommerce.Modules.Catalog.Persistence.Postgres;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Categories.Commands.CreateCategory;

public sealed class CreateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    ICatalogUnitOfWork unitOfWork)
    : ICommandHandler<CreateCategoryCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateCategoryCommand command, CancellationToken ct)
    {
        if (command.ParentId.HasValue)
        {
            var parentExists = await categoryRepository.ExistsAsync(command.ParentId.Value, ct);
            if (!parentExists)
            {
                return Result.Failure<Guid>(Error.NotFound("Parent Category", command.ParentId.Value));
            }
        }

        var nameExists = await categoryRepository.NameExistsAsync(command.Name, null, ct);
        if (nameExists)
        {
            return Result.Failure<Guid>(Error.Conflict($"Category with name '{command.Name}' already exists."));
        }

        var slugExists = await categoryRepository.SlugExistsAsync(command.Seo.Slug, null, ct);
        if (slugExists)
        {
            return Result.Failure<Guid>(Error.Conflict($"Category with slug '{Slug.Create(command.Seo.Slug)}' already exists."));
        }

        var seo = SeoMetadata.Create(
            slug: Slug.Create(command.Seo.Slug),
            metaTitle: MetaTitle.CreateOrNull(command.Seo.MetaTitle),
            metaDescription: MetaDescription.CreateOrNull(command.Seo.MetaDescription),
            keywords: SeoKeywords.CreateOrNull(command.Seo.Keywords));

        var category = Category.Create(
            name: CategoryName.Create(command.Name),
            description: CategoryDescription.Create(command.Description),
            seoMetadata: seo,
            iconPath: ImagePath.CreateOrNull(command.IconPath),
            coverImagePath: ImagePath.Create(command.CoverImagePath),
            imageAltText: AltText.CreateOrNull(command.ImageAltText),
            parentId: command.ParentId,
            isActive: command.IsActive,
            showOnHome: command.ShowOnHome,
            includeInMenu: command.IncludeInMenu,
            sortOrder: command.SortOrder
        );

        await categoryRepository.AddAsync(category, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(category.Id);
    }
}
