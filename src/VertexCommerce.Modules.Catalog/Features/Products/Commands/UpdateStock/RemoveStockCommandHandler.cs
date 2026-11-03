using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Modules.Catalog.Persistence.Postgres;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Products.Commands.UpdateStock;

internal sealed class RemoveStockCommandHandler(
    IProductRepository productRepository,
    ICatalogUnitOfWork unitOfWork)
    : ICommandHandler<RemoveStockCommand>
{
    public async Task<Result> Handle(RemoveStockCommand command, CancellationToken ct)
    {
        var product = await productRepository.GetByIdAsync(command.ProductId, ct);
        if (product is null)
            return Result.Failure(Error.NotFound("Product", command.ProductId));

        // if (product.StockQuantity < command.Quantity)
        // return Result.Failure(Error.Validation("Insufficient stock"));

        // product.RemoveStock(command.Quantity);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}