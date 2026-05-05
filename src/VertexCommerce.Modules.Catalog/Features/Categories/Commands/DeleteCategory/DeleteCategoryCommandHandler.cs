using VertexCommerce.Modules.Catalog.Domain.Categories;
using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Modules.Catalog.Persistence.Postgres;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Categories.Commands.DeleteCategory;

internal sealed class DeleteCategoryCommandHandler : ICommandHandler<DeleteCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICatalogUnitOfWork _unitOfWork;

    public DeleteCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IProductRepository productRepository,
        ICatalogUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteCategoryCommand command, CancellationToken ct)
    {
        var category = await _categoryRepository.GetByIdAsync(command.Id, ct);
        if (category is null)
            return Result.Failure(Error.NotFound("Category", command.Id));

        var hasProducts = await _productRepository.HasProductsInCategoryAsync(command.Id, ct);
        if (hasProducts)
            return Result.Failure(Error.Validation("Cannot delete category with products"));

        var hasChildren = await _categoryRepository.HasChildrenAsync(command.Id, ct);
        if (hasChildren)
            return Result.Failure(Error.Validation("Cannot delete category with subcategories"));
        
        category.Delete();
        
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
