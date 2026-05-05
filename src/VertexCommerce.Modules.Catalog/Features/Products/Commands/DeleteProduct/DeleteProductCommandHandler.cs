using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Modules.Catalog.Persistence.Postgres;
using VertexCommerce.Modules.Catalog.Sync;
using VertexCommerce.Modules.Catalog.Sync.Products;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Products.Commands.DeleteProduct;

internal sealed class DeleteProductCommandHandler(
    IProductRepository productRepository,
    ICatalogUnitOfWork unitOfWork)
    : ICommandHandler<DeleteProductCommand>
{
    public async Task<Result> Handle(DeleteProductCommand command, CancellationToken ct)
    {
        var product = await productRepository.GetByIdAsync(command.Id, ct);
        if (product is null)
            return Result.Failure(Error.NotFound("Product", command.Id));
        
        product.Delete();
        
        productRepository.Delete(product);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
