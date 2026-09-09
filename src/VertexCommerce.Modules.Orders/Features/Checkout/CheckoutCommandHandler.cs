using VertexCommerce.Modules.Orders.Domain.Entities;
using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Modules.Orders.Domain.ValueObjects;
using VertexCommerce.Modules.Orders.Persistence;
using VertexCommerce.Shared.Contracts.Baskets;
using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.Contracts.Catalog;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.CQRS;
using VertexCommerce.Modules.Orders.Domain.Errors;

namespace VertexCommerce.Modules.Orders.Features.Checkout;

public sealed class CheckoutCommandHandler(
    IOrderRepository orderRepository,
    IBasketService basketService,
    IOrdersUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    ICustomerResolver customerResolver,
    ICustomerService customerService,
    IProductService productService,
    IStockService stockService)
    : ICommandHandler<CheckoutCommand, CheckoutResponse>
{
    public async Task<Result<CheckoutResponse>> Handle(CheckoutCommand command, CancellationToken ct)
    {
        var customerId = await customerResolver.GetCustomerIdByUserIdAsync(currentUser.UserId, ct);

        var basket = await basketService.GetBasketAsync(
            customerId: customerId,
            ct);

        if (basket is null || basket.Items.Count == 0)
        {
            return Result.Failure<CheckoutResponse>(
                OrderErrors.EmptyBasket);
        }

        var customer = await customerService.GetCustomerInfo(
            customerId: customerId,
            ct);

        if (customer is null)
        {
            return Result.Failure<CheckoutResponse>(
                OrderErrors.CustomerNotFound);
        }

        var shippingAddressResult = CreateShippingAddress(customer);
        if (shippingAddressResult.IsFailure)
        {
            return Result.Failure<CheckoutResponse>(shippingAddressResult.Error);
        }

        var billingAddressResult = CreateBillingAddress(customer);
        if (billingAddressResult.IsFailure)
        {
            return Result.Failure<CheckoutResponse>(billingAddressResult.Error);
        }

        var order = Order.Create(
            customerId: customerId,
            customerPhoneNumber: PhoneNumber.Create(customer.PhoneNumber),
            shippingAddress: shippingAddressResult.Value,
            billingAddress: billingAddressResult.Value,
            notes: command.Notes
        );

        foreach (var item in basket.Items)
        {
            if (item.Quantity <= 0)
            {
                continue;
            }

            var variant = await productService.GetProductVariantInfoAsync(item.ProductId, item.VariantId, ct);
            if (variant is null)
            {
                return Result.Failure<CheckoutResponse>(
                    OrderErrors.ProductVariantNotFound(item.VariantId.ToString()));
            }

            if (variant.StockQuantity < item.Quantity)
            {
                return Result.Failure<CheckoutResponse>(
                    OrderErrors.InsufficientStock(variant.Name, variant.Sku, item.Quantity, variant.StockQuantity));
            }

            var unitPrice = Money.Create(variant.Price);

            order.AddItem(
                productId: variant.ProductId,
                variantId: variant.VariantId,
                productName: variant.Name,
                productSku: variant.Sku,
                unitPrice: unitPrice,
                quantity: item.Quantity
            );
        }

        if (!order.Items.Any())
        {
            return Result.Failure<CheckoutResponse>(
                OrderErrors.EmptyItems);
        }

        var stockRequests = order.Items.Select(i => new StockDeductionRequest(i.VariantId, i.Quantity));
        var reserveResult = await stockService.ReserveStocksAsync(stockRequests, ct);
        if (reserveResult.IsFailure)
        {
            return Result.Failure<CheckoutResponse>(reserveResult.Error);
        }

        await orderRepository.AddAsync(order, ct);
        await unitOfWork.SaveChangesAsync(ct);

        await basketService.ClearBasketAsync(customerId, ct);

        return Result.Success(new CheckoutResponse(
            order.Id,
            order.OrderNumber.Value,
            order.TotalAmount.Amount,
            order.TotalAmount.Currency,
            order.ExpiresAt
        ));
    }

    private static Result<Address> CreateShippingAddress(CustomerInfoDto customer)
    {
        var csa = customer.ShippingAddress;
        if (csa is null)
        {
            return Result.Failure<Address>(
                OrderErrors.ShippingAddressNotSet);
        }

        var address = Address.Create(Province.Create(csa.Province), City.Create(csa.City), PostalAddress.Create(csa.PostalAddress), PostalCode.Create(csa.PostalCode), GeoLocation.Create(csa.Latitude, csa.Longitude), AddressLabel.CreateOrNull(csa.Label));

        return Result.Success(address);
    }

    private static Result<Address> CreateBillingAddress(CustomerInfoDto customer)
    {
        var cba = customer.BillingAddress ?? customer.ShippingAddress;
        if (cba is null)
        {
            return Result.Failure<Address>(
                OrderErrors.BillingAddressNotSet);
        }

        var address = Address.Create(Province.Create(cba.Province), City.Create(cba.City), PostalAddress.Create(cba.PostalAddress), PostalCode.Create(cba.PostalCode), GeoLocation.Create(cba.Latitude, cba.Longitude), AddressLabel.CreateOrNull(cba.Label));

        return Result.Success(address);
    }
}
