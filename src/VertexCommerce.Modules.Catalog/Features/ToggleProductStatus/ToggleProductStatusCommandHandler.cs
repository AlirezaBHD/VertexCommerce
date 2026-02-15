using VertexCommerce.Modules.Catalog.Domain.Repositories;
using VertexCommerce.Modules.Catalog.Persistence;
using VertexCommerce.Modules.Catalog.Sync;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.ToggleProductStatus;

internal sealed class ActivateProductCommandHandler(
    IProductRepository productRepository,
    ICatalogUnitOfWork unitOfWork,
    IProductSyncService syncService)
    : ICommandHandler<ActivateProductCommand>
{
    public async Task<Result> Handle(ActivateProductCommand command, CancellationToken ct)
    {
        var product = await productRepository.GetByIdAsync(command.Id, ct);
        if (product is null)
            return Result.Failure(Error.NotFound("Product", command.Id));

        product.Activate();
        await unitOfWork.SaveChangesAsync(ct);
        await syncService.SyncProductAsync(command.Id, ct);

        return Result.Success();
    }
}

internal sealed class DeactivateProductCommandHandler(
    IProductRepository productRepository,
    ICatalogUnitOfWork unitOfWork,
    IProductSyncService syncService)
    : ICommandHandler<DeactivateProductCommand>
{
    public async Task<Result> Handle(DeactivateProductCommand command, CancellationToken ct)
    {
        var product = await productRepository.GetByIdAsync(command.Id, ct);
        if (product is null)
            return Result.Failure(Error.NotFound("Product", command.Id));

        product.Deactivate();
        await unitOfWork.SaveChangesAsync(ct);
        await syncService.SyncProductAsync(command.Id, ct);

        return Result.Success();
    }
}
