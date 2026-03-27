using VertexCommerce.Modules.Basket.Domain.Repositories;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Basket.Features.AddItem;

internal sealed class AddItemCommandHandler : ICommandHandler<AddItemCommand>
{
    private readonly IBasketRepository _basketRepository;

    public AddItemCommandHandler(
        IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }

    public Task<Result> Handle(AddItemCommand command, CancellationToken ct)
    {
        throw new NotImplementedException();
        // var product = await _productService.GetProductInfoAsync(command.ProductId, ct);
        //
        // if (product is null)
        //     return Result.Failure(Error.NotFound("Product", command.ProductId));
        //
        // if (!product.IsActive)
        //     return Result.Failure(Error.Validation("Product is not available"));
        //
        // if (product.StockQuantity < command.Quantity)
        //     return Result.Failure(Error.Validation($"Insufficient stock. Available: {product.StockQuantity}"));
        //
        // var basket = await _basketRepository.GetByCustomerIdAsync(command.CustomerId, ct);
        //
        // if (basket is null)
        // {
        //     basket = CustomerBasket.Create(command.CustomerId, product.Currency);
        // }
        //
        // basket.AddItem(
        //     command.ProductId,
        //     product.Name,
        //     product.Sku,
        //     product.ImageUrl,
        //     product.Price,
        //     command.Quantity
        // );
        //
        // await _basketRepository.UpdateAsync(basket, ct);
        //
        // return Result.Success();
    }
}
