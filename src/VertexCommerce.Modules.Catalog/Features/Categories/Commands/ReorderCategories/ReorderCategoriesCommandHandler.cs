using VertexCommerce.Modules.Catalog.Domain.Categories;
using VertexCommerce.Modules.Catalog.Persistence.Postgres;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Categories.Commands.ReorderCategories;

internal sealed class ReorderCategoriesCommandHandler(
    ICategoryRepository categoryRepository,
    ICatalogUnitOfWork unitOfWork)
    : ICommandHandler<ReorderCategoriesCommand>
{
    public async Task<Result> Handle(ReorderCategoriesCommand command, CancellationToken ct)
    {
        var allCategories = await categoryRepository.GetAllAsync(ct);
        var categoryDict = allCategories.ToDictionary(c => c.Id);

        foreach (var item in command.Items)
        {
            if (!categoryDict.ContainsKey(item.CategoryId))
                return Result.Failure(Error.NotFound("Category", item.CategoryId));
        }

        foreach (var item in command.Items)
        {
            categoryDict[item.CategoryId].SetSortOrder(item.SortOrder);
        }

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
