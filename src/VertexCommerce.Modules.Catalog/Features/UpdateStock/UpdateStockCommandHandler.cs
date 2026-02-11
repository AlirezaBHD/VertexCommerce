using VertexCommerce.Modules.Catalog.Domain.Repositories;
using VertexCommerce.Modules.Catalog.Persistence;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.UpdateStock;

internal sealed class UpdateStockCommandHandler : ICommandHandler<UpdateStockCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly ICatalogUnitOfWork _unitOfWork;

    public UpdateStockCommandHandler(
        IProductRepository productRepository,
        ICatalogUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateStockCommand command, CancellationToken ct)
    {
        var product = await _productRepository.GetByIdAsync(command.ProductId, ct);
        if (product is null)
            return Result.Failure(Error.NotFound("Product", command.ProductId));

        product.SetStock(command.Quantity);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}

internal sealed class AddStockCommandHandler : ICommandHandler<AddStockCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly ICatalogUnitOfWork _unitOfWork;

    public AddStockCommandHandler(
        IProductRepository productRepository,
        ICatalogUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AddStockCommand command, CancellationToken ct)
    {
        var product = await _productRepository.GetByIdAsync(command.ProductId, ct);
        if (product is null)
            return Result.Failure(Error.NotFound("Product", command.ProductId));

        product.AddStock(command.Quantity);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}

internal sealed class RemoveStockCommandHandler : ICommandHandler<RemoveStockCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly ICatalogUnitOfWork _unitOfWork;

    public RemoveStockCommandHandler(
        IProductRepository productRepository,
        ICatalogUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemoveStockCommand command, CancellationToken ct)
    {
        var product = await _productRepository.GetByIdAsync(command.ProductId, ct);
        if (product is null)
            return Result.Failure(Error.NotFound("Product", command.ProductId));

        if (product.StockQuantity < command.Quantity)
            return Result.Failure(Error.Validation("Insufficient stock"));

        product.RemoveStock(command.Quantity);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}