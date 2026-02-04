using VertexCommerce.Modules.Catalog.Domain.Entities;
using VertexCommerce.Modules.Catalog.Domain.Repositories;
using VertexCommerce.Shared.CQRS;
using VertexCommerce.Shared.Persistence;

namespace VertexCommerce.Modules.Catalog.Features.CreateCategory;

public sealed class CreateCategoryCommandHandler : ICommandHandler<CreateCategoryCommand, Guid>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateCategoryCommand command, CancellationToken ct)
    {
        // Check if parent exists (if provided)
        if (command.ParentId.HasValue)
        {
            var parentExists = await _categoryRepository.ExistsAsync(command.ParentId.Value, ct);
            if (!parentExists)
            {
                return Result.Failure<Guid>(Error.NotFound("Parent Category", command.ParentId.Value));
            }
        }

        // Check if name already exists
        var nameExists = await _categoryRepository.NameExistsAsync(command.Name, null, ct);
        if (nameExists)
        {
            return Result.Failure<Guid>(Error.Conflict($"Category with name '{command.Name}' already exists."));
        }

        // Create category
        var category = Category.Create(
            command.Name,
            command.Description,
            command.ParentId,
            command.SortOrder
        );

        // Persist
        await _categoryRepository.AddAsync(category, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success(category.Id);
    }
}
