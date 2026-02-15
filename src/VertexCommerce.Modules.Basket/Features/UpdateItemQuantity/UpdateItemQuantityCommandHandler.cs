using VertexCommerce.Modules.Basket.Domain.Repositories;
using VertexCommerce.Shared.CQRS;
using VertexCommerce.Shared.Services;

namespace VertexCommerce.Modules.Basket.Features.UpdateItemQuantity;

public sealed class UpdateItemQuantityCommandHandler : ICommandHandler<UpdateItemQuantityCommand>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IProductService _productService;

    public UpdateItemQuantityCommandHandler(IBasketRepository basketRepository, IProductService productService)
    {
        _basketRepository = basketRepository;
        _productService = productService;
    }

    public async Task<Result> Handle(UpdateItemQuantityCommand command, CancellationToken ct)
    {
        var basket = await _basketRepository.GetByCustomerIdAsync(command.CustomerId, ct);

        if (basket is null)
        {
            return Result.Failure(Error.NotFound("Basket", command.CustomerId));
        }

        if (!basket.HasItem(command.ProductId))
        {
            return Result.Failure(Error.NotFound("BasketItem", command.ProductId));
        }
        var product = await _productService.GetProductInfoAsync(command.ProductId, ct);
        
        if (product is null)
            return Result.Failure(Error.NotFound("Product", command.ProductId));

        if (product.StockQuantity < command.Quantity)
            return Result.Failure(Error.Validation($"Insufficient stock. Available: {product.StockQuantity}"));


        basket.UpdateItemQuantity(command.ProductId, command.Quantity);

        await _basketRepository.UpdateAsync(basket, ct);

        return Result.Success();
    }
}
