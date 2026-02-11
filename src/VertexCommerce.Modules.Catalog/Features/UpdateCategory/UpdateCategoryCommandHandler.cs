using VertexCommerce.Modules.Catalog.Domain.Repositories;
using VertexCommerce.Modules.Catalog.Persistence;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.UpdateCategory;

internal sealed class UpdateCategoryCommandHandler : ICommandHandler<UpdateCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICatalogUnitOfWork _unitOfWork;

    public UpdateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        ICatalogUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateCategoryCommand command, CancellationToken ct)
    {
        var category = await _categoryRepository.GetByIdAsync(command.Id, ct);
        if (category is null)
            return Result.Failure(Error.NotFound("Category", command.Id));

        if (command.ParentId.HasValue && command.ParentId.Value == command.Id)
            return Result.Failure(Error.Validation("Category cannot be its own parent"));

        if (command.ParentId.HasValue)
        {
            var parentExists = await _categoryRepository.ExistsAsync(command.ParentId.Value, ct);
            if (!parentExists)
                return Result.Failure(Error.NotFound("Parent Category", command.ParentId.Value));
        }

        var nameExists = await _categoryRepository.NameExistsAsync(command.Name, command.Id, ct);
        if (nameExists)
            return Result.Failure(Error.Conflict("Category with this name already exists"));

        category.Update(command.Name, command.Description, command.ParentId, command.SortOrder);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
