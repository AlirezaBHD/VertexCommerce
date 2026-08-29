using VertexCommerce.Modules.Orders.Domain.Entities;
using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Modules.Orders.Domain.ValueObjects;
using VertexCommerce.Modules.Orders.Persistence;
using VertexCommerce.Shared.Contracts.Catalog;
using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.CreateManualOrder;

internal sealed class CreateManualOrderCommandHandler(
    IOrderRepository orderRepository,
    IOrdersUnitOfWork unitOfWork,
    ICustomerService customerService,
    IProductService productService,
    IStockService stockService)
    : ICommandHandler<CreateManualOrderCommand, CreateManualOrderResponse>
{
    public async Task<Result<CreateManualOrderResponse>> Handle(
        CreateManualOrderCommand command,
        CancellationToken ct)
    {
        var customer = await customerService.GetCustomerInfo(command.CustomerId, ct);
        if (customer is null)
        {
            return Result.Failure<CreateManualOrderResponse>(
                Error.NotFound("Customer", command.CustomerId));
        }

        var shippingAddress = CreateAddress(command.ShippingAddress);
        var billingAddress = CreateAddress(command.BillingAddress ?? command.ShippingAddress);

        var order = Order.CreateManual(
            customerId: command.CustomerId,
            customerPhoneNumber: customer.PhoneNumber,
            shippingAddress: shippingAddress,
            billingAddress: billingAddress,
            notes: command.Notes);

        foreach (var item in command.Items)
        {
            var variant = await productService.GetProductVariantInfoAsync(item.ProductId, item.VariantId, ct);
            if (variant is null)
            {
                return Result.Failure<CreateManualOrderResponse>(
                    Error.NotFound("ProductVariant", item.VariantId));
            }

            if (item.Quantity > variant.StockQuantity)
            {
                return Result.Failure<CreateManualOrderResponse>(
                    Error.Validation(
                        "Stock.Insufficient",
                        $"Insufficient stock for '{variant.Name}' ({variant.Sku}). " +
                        $"Requested: {item.Quantity}, Available: {variant.StockQuantity}."));
            }

            order.AddItem(
                productId: variant.ProductId,
                variantId: variant.VariantId,
                productName: variant.Name,
                productSku: variant.Sku,
                unitPrice: Money.Create(variant.Price),
                quantity: item.Quantity);
        }

        if (!order.Items.Any())
        {
            return Result.Failure<CreateManualOrderResponse>(
                Error.Validation("Order.EmptyItems", "Order has no valid items."));
        }

        if (command.ShippingCost > 0)
        {
            order.SetShippingCost(Money.Create(command.ShippingCost));
        }

        var stockRequests = order.Items.Select(i => new StockDeductionRequest(i.VariantId, i.Quantity));
        var deductResult = await stockService.DeductStocksAsync(stockRequests, ct);
        if (deductResult.IsFailure)
        {
            return Result.Failure<CreateManualOrderResponse>(deductResult.Error);
        }

        await orderRepository.AddAsync(order, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new CreateManualOrderResponse(
            order.Id,
            order.OrderNumber,
            order.TotalAmount.Amount,
            order.TotalAmount.Currency));
    }

    private static Address CreateAddress(ManualOrderAddressDto dto)
    {
        return Address.Create(
            province: dto.Province,
            city: dto.City,
            postalAddress: dto.PostalAddress,
            postalCode: dto.PostalCode,
            latitude: dto.Latitude,
            longitude: dto.Longitude,
            label: dto.Label);
    }
}
