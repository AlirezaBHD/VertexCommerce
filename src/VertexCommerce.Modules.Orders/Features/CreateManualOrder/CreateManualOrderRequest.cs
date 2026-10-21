namespace VertexCommerce.Modules.Orders.Features.CreateManualOrder;

internal sealed record CreateManualOrderRequest(
    Guid CustomerId,
    CreateManualOrderRequest.Address ShippingAddress,
    CreateManualOrderRequest.Address? BillingAddress,
    IReadOnlyList<CreateManualOrderRequest.Item> Items,
    decimal ShippingCost = 0,
    string? Notes = null)
{
    public CreateManualOrderCommand ToCommand() => new(
        CustomerId,
        ShippingAddress.ToCommandAddress(),
        BillingAddress?.ToCommandAddress(),
        [.. Items.Select(i => new CreateManualOrderCommand.Item(i.ProductId, i.VariantId, i.Quantity))],
        ShippingCost,
        Notes);

    internal sealed record Address(
        string Province,
        string City,
        string PostalAddress,
        string PostalCode,
        decimal Latitude = 0,
        decimal Longitude = 0,
        string? Label = null)
    {
        public CreateManualOrderCommand.Address ToCommandAddress() =>
            new(Province, City, PostalAddress, PostalCode, Latitude, Longitude, Label);
    }

    internal sealed record Item(
        Guid ProductId,
        Guid VariantId,
        int Quantity);
}
