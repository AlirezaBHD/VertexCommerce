using VertexCommerce.Modules.Catalog.Domain.Categories;
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

        var category = Category.Create(
            command.Name,
            command.Description,
            command.ParentId,
            command.SortOrder
        );

        await _categoryRepository.AddAsync(category, ct);
        var count = await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success(category.Id);
    }
}
