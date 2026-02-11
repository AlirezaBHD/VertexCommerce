using VertexCommerce.Modules.Catalog.Domain.Repositories;
using VertexCommerce.Modules.Catalog.Domain.ValueObjects;
using VertexCommerce.Modules.Catalog.Persistence;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.UpdateProduct;

internal sealed class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICatalogUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        ICatalogUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateProductCommand command, CancellationToken ct)
    {
        var product = await _productRepository.GetByIdAsync(command.Id, ct);
        if (product is null)
            return Result.Failure(Error.NotFound("Product", command.Id));

        var categoryExists = await _categoryRepository.ExistsAsync(command.CategoryId, ct);
        if (!categoryExists)
            return Result.Failure(Error.NotFound("Category", command.CategoryId));

        var price = Money.Create(command.Price, command.Currency);

        product.Update(command.Name, command.Description, price, command.CategoryId);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
