using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.CreateOrder;

public sealed record CreateOrderCommand(
    Guid CustomerId,
    string CustomerEmail,
    AddressDto ShippingAddress,
    AddressDto BillingAddress,
    List<OrderItemDto> Items,
    string Currency,
    string? Notes
) : ICommand<Guid>;

public sealed record AddressDto(
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode
);

public sealed record OrderItemDto(
    Guid ProductId,
    string ProductName,
    string ProductSku,
    decimal UnitPrice,
    int Quantity
);
