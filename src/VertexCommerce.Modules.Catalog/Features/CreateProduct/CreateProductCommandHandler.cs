using VertexCommerce.Modules.Catalog.Domain.Entities;
using VertexCommerce.Modules.Catalog.Domain.Repositories;
using VertexCommerce.Modules.Catalog.Domain.ValueObjects;
using VertexCommerce.Modules.Catalog.Persistence;
using VertexCommerce.Modules.Catalog.Sync;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.CreateProduct;

internal sealed class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, Guid>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICatalogUnitOfWork _unitOfWork;
    private readonly IProductSyncService _syncService;

    public CreateProductCommandHandler(
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

    public async Task<Result<Guid>> Handle(CreateProductCommand command, CancellationToken ct)
    {
        var skuExists = await _productRepository.SkuExistsAsync(command.Sku, ct);
        if (skuExists)
            return Result.Failure<Guid>(Error.Conflict("Product with this SKU already exists"));

        var categoryExists = await _categoryRepository.ExistsAsync(command.CategoryId, ct);
        if (!categoryExists)
            return Result.Failure<Guid>(Error.NotFound("Category", command.CategoryId));

        var sku = Sku.Create(command.Sku);
        var price = Money.Create(command.Price, command.Currency);

        var product = Product.Create(
            command.Name,
            command.Description,
            sku,
            price,
            command.StockQuantity,
            command.CategoryId
        );

        await _productRepository.AddAsync(product, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        // Sync to MongoDB
        await _syncService.SyncProductAsync(product, ct);

        return Result.Success(product.Id);
    }
}