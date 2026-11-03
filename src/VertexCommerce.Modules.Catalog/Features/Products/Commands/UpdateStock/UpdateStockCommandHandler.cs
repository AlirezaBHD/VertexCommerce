using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Modules.Catalog.Persistence.Postgres;
using VertexCommerce.Modules.Catalog.Sync;
using VertexCommerce.Modules.Catalog.Sync.Products;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Products.Commands.UpdateStock;

internal sealed class UpdateStockCommandHandler(
    IProductRepository productRepository,
    ICatalogUnitOfWork unitOfWork,
    IProductSyncService syncService)
    : ICommandHandler<UpdateStockCommand>
{
    public async Task<Result> Handle(UpdateStockCommand command, CancellationToken ct)
    {
        //TODO
        var product = await productRepository.GetByIdAsync(command.ProductId, ct);
        if (product is null)
            return Result.Failure(Error.NotFound("Product", command.ProductId));

        // product.SetStock(command.Quantity);
        await unitOfWork.SaveChangesAsync(ct);
        await syncService.SyncProductAsync(command.ProductId, ct);

        return Result.Success();
    }
}