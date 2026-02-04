using VertexCommerce.Modules.Catalog.Domain.Entities;
using VertexCommerce.Modules.Catalog.Domain.Repositories;
using VertexCommerce.Modules.Catalog.Domain.ValueObjects;
using VertexCommerce.Shared.CQRS;
using VertexCommerce.Shared.Persistence;

namespace VertexCommerce.Modules.Catalog.Features.CreateProduct;

public sealed class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, Guid>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateProductCommand command, CancellationToken ct)
    {
        var categoryExists = await _categoryRepository.ExistsAsync(command.CategoryId, ct);
        if (!categoryExists)
        {
            return Result.Failure<Guid>(Error.NotFound("Category", command.CategoryId));
        }

        var sku = string.IsNullOrWhiteSpace(command.Sku)
            ? Sku.Generate()
            : Sku.Create(command.Sku);

        var skuExists = await _productRepository.SkuExistsAsync(sku.Value, ct);
        if (skuExists)
        {
            return Result.Failure<Guid>(Error.Conflict($"Product with SKU '{sku.Value}' already exists."));
        }

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

        return Result.Success(product.Id);
    }
}