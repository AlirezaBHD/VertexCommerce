using VertexCommerce.Modules.Catalog.Domain.Categories;
using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Modules.Catalog.Domain.Products.ValueObjects;
using VertexCommerce.Modules.Catalog.Persistence.Postgres;
using VertexCommerce.Modules.Catalog.Sync;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Products.Commands.UpdateProduct;

internal sealed class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICatalogUnitOfWork _unitOfWork;
    private readonly IProductSyncService _syncService;

    public UpdateProductCommandHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        ICatalogUnitOfWork unitOfWork,
        IProductSyncService syncService)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _syncService = syncService;

    }

    public async Task<Result> Handle(UpdateProductCommand command, CancellationToken ct)
    {
        //TODO
        var product = await _productRepository.GetByIdAsync(command.Id, ct);
        if (product is null)
            return Result.Failure(Error.NotFound("Product", command.Id));

        var categoryExists = await _categoryRepository.ExistsAsync(command.CategoryId, ct);
        if (!categoryExists)
            return Result.Failure(Error.NotFound("Category", command.CategoryId));

        var price = Money.Create(command.Price, command.Currency);

        // product.Update(command.Name, command.Description, price, command.CategoryId);

        await _unitOfWork.SaveChangesAsync(ct);
        await _syncService.SyncProductAsync(command.Id, ct);

        return Result.Success();
    }
}
