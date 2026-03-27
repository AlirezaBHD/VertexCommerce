using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Modules.Catalog.Persistence.Postgres;
using VertexCommerce.Modules.Catalog.Sync;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Products.Commands.DeleteProduct;

internal sealed class DeleteProductCommandHandler(
    IProductRepository productRepository,
    ICatalogUnitOfWork unitOfWork,
    IProductSyncService  syncService)
    : ICommandHandler<DeleteProductCommand>
{
    public async Task<Result> Handle(DeleteProductCommand command, CancellationToken ct)
    {
        var product = await productRepository.GetByIdAsync(command.Id, ct);
        if (product is null)
            return Result.Failure(Error.NotFound("Product", command.Id));

        productRepository.Delete(product);
        await unitOfWork.SaveChangesAsync(ct);
        await syncService.DeleteProductAsync(command.Id, ct);

        return Result.Success();
    }
}
