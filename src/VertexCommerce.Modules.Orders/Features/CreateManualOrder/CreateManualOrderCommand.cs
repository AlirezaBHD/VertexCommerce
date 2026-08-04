using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.CreateManualOrder;

public sealed record CreateManualOrderCommand(
    Guid CustomerId,
    ManualOrderAddressDto ShippingAddress,
    ManualOrderAddressDto? BillingAddress,
    IReadOnlyList<ManualOrderItemDto> Items,
    decimal ShippingCost = 0,
    string? Notes = null
) : ICommand<CreateManualOrderResponse>;

public sealed record ManualOrderAddressDto(
    string Province,
    string City,
    string PostalAddress,
    string PostalCode,
    decimal Latitude = 0,
    decimal Longitude = 0,
    string? Label = null);

public sealed record ManualOrderItemDto(
    Guid ProductId,
    Guid VariantId,
    int Quantity);

public sealed record CreateManualOrderResponse(
    Guid OrderId,
    string OrderNumber,
    decimal TotalAmount,
    string Currency);
