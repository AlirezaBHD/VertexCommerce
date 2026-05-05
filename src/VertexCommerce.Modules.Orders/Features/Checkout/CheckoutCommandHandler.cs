using VertexCommerce.Modules.Orders.Domain.Entities;
using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Modules.Orders.Domain.ValueObjects;
using VertexCommerce.Modules.Orders.Persistence;
using VertexCommerce.Shared.Contracts.Baskets;
using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.Checkout;

public sealed class CheckoutCommandHandler(
    IOrderRepository orderRepository,
    IBasketService basketService,
    IOrdersUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    ICustomerResolver customerResolver,
    ICustomerService customerService)
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
                Error.Validation("Basket.Empty", "Basket is empty. Cannot checkout."));
        }

        var customer = await customerService.GetCustomerInfo(
            customerId: customerId,
            ct);

        if (customer is null)
        {
            return Result.Failure<CheckoutResponse>(
                Error.NotFound("Customer.NotFound", "Customer not found."));
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
            customerPhoneNumber: customer.PhoneNumber,
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

            var unitPrice = Money.Create(item.UnitPrice);

            order.AddItem(
                productId: item.ProductId,
                variantId: item.VariantId,
                productName: item.ProductName,
                productSku: item.Sku,
                unitPrice: unitPrice,
                quantity: item.Quantity
            );
        }

        if (!order.Items.Any())
        {
            return Result.Failure<CheckoutResponse>(
                Error.Validation("Order.EmptyItems", "Order has no valid items."));
        }

        await orderRepository.AddAsync(order, ct);
        await unitOfWork.SaveChangesAsync(ct);

        await basketService.ClearBasketAsync(customerId, ct);

        return Result.Success(new CheckoutResponse(
            order.Id,
            order.OrderNumber,
            order.TotalAmount.Amount,
            order.TotalAmount.Currency
        ));
    }

    private static Result<Address> CreateShippingAddress(CustomerInfoDto customer)
    {
        var csa = customer.ShippingAddress;
        if (csa is null)
        {
            return Result.Failure<Address>(
                Error.Validation("ShippingAddress.NotSet", "Shipping address is not set. Cannot checkout."));
        }

        var address = Address.Create(
            province: csa.Province,
            city: csa.City,
            postalAddress: csa.PostalAddress,
            postalCode: csa.PostalCode,
            latitude: csa.Latitude,
            longitude: csa.Longitude,
            label: csa.Label);

        return Result.Success(address);
    }

    private static Result<Address> CreateBillingAddress(CustomerInfoDto customer)
    {
        var cba = customer.BillingAddress ?? customer.ShippingAddress;
        if (cba is null)
        {
            return Result.Failure<Address>(
                Error.Validation("BillingAddress.NotSet", "Billing address is not set. Cannot checkout."));
        }

        var address = Address.Create(
            province: cba.Province,
            city: cba.City,
            postalAddress: cba.PostalAddress,
            postalCode: cba.PostalCode,
            latitude: cba.Latitude,
            longitude: cba.Longitude,
            label: cba.Label);

        return Result.Success(address);
    }
}
