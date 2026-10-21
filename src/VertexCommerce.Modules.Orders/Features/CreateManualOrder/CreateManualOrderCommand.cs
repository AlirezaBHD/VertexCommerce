using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.CreateManualOrder;

internal sealed record CreateManualOrderCommand(
    Guid CustomerId,
    CreateManualOrderCommand.Address ShippingAddress,
    CreateManualOrderCommand.Address? BillingAddress,
    IReadOnlyList<CreateManualOrderCommand.Item> Items,
    decimal ShippingCost,
    string? Notes
) : ICommand<CreateManualOrderResponse>
{
    internal sealed record Address(
        string Province,
        string City,
        string PostalAddress,
        string PostalCode,
        decimal Latitude,
        decimal Longitude,
        string? Label);

    internal sealed record Item(
        Guid ProductId,
        Guid VariantId,
        int Quantity);
}
