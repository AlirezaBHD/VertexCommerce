using VertexCommerce.Modules.Catalog.Domain.Categories;
using VertexCommerce.Modules.Catalog.Domain.Products.ValueObjects;
using VertexCommerce.Modules.Catalog.Persistence.Postgres;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Categories.Commands.UpdateCategory;

internal sealed class UpdateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    ICatalogUnitOfWork unitOfWork)
    : ICommandHandler<UpdateCategoryCommand>
{
    public async Task<Result> Handle(UpdateCategoryCommand command, CancellationToken ct)
    {
        var category = await categoryRepository.GetByIdAsync(command.Id, ct);
        if (category is null)
            return Result.Failure(Error.NotFound("Category", command.Id));


        if (command.ParentId.HasValue)
        {
            if (command.ParentId.Value == command.Id)
                return Result.Failure(Error.Validation("Category cannot be its own parent"));

            var parentExists = await categoryRepository.ExistsAsync(command.ParentId.Value, ct);
            if (!parentExists)
                return Result.Failure(Error.NotFound("Parent Category", command.ParentId.Value));
        }

        var nameExists = await categoryRepository.NameExistsAsync(command.Name, command.Id, ct);
        if (nameExists)
        {
            return Result.Failure(Error.Conflict($"Category with name '{command.Name}' already exists."));
        }

        var slugExists = await categoryRepository.SlugExistsAsync(command.Name,command.Id, ct);
        if (slugExists)
        {
            return Result.Failure(Error.Conflict($"Category with slug '{command.Seo.Slug}' already exists."));
        }

        var seo = SeoMetadata.Create(
            slug: command.Seo.Slug,
            metaTitle: command.Seo.MetaTitle,
            metaDescription: command.Seo.MetaDescription,
            keywords: command.Seo.Keywords);

        category.Update(
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
            sortOrder: category.SortOrder
        );
        
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
