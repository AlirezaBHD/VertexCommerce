using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Modules.Catalog.Persistence.Postgres;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Products.Commands.UpdateStock;

internal sealed class AddStockCommandHandler(
    IProductRepository productRepository,
    ICatalogUnitOfWork unitOfWork)
    : ICommandHandler<AddStockCommand>
{
    public async Task<Result> Handle(AddStockCommand command, CancellationToken ct)
    {
        var product = await productRepository.GetByIdAsync(command.ProductId, ct);
        if (product is null)
            return Result.Failure(Error.NotFound("Product", command.ProductId));

        // product.AddStock(command.Quantity);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}