using HotChocolate;
using VertexCommerce.Api.GraphQL.Orders.Types;
using VertexCommerce.Modules.Orders.Domain.Repositories;

namespace VertexCommerce.Api.GraphQL.Orders;

[ExtendObjectType(typeof(Query))]
public sealed class OrderQueries{
    public async Task<OrderType?> GetOrderById(
        [Service] IOrderRepository orderRepository,
        Guid id,
        CancellationToken ct = default)
    {
        var order = await orderRepository.GetByIdAsync(id, ct);
        return order is null ? null : MapToOrderType(order);
    }

    public async Task<OrderType?> GetOrderByNumber(
        [Service] IOrderRepository orderRepository,
        string orderNumber,
        CancellationToken ct = default)
    {
        var order = await orderRepository.GetByOrderNumberAsync(orderNumber, ct);
        return order is null ? null : MapToOrderType(order);
    }

    public async Task<IEnumerable<OrderType>> GetOrdersByCustomer(
        [Service] IOrderRepository orderRepository,
        Guid customerId,
        CancellationToken ct = default)
    {
        var orders = await orderRepository.GetByCustomerIdAsync(customerId, ct);
        return orders.Select(MapToOrderType);
    }

    private static OrderType MapToOrderType(Modules.Orders.Domain.Entities.Order order) => new()
    {
        Id = order.Id,
        OrderNumber = order.OrderNumber,
        CustomerId = order.CustomerId,
        CustomerEmail = order.CustomerPhoneNumber,
        Status = order.Status,
        PaymentStatus = order.PaymentStatus,
        SubTotal = order.SubTotal.Amount,
        ShippingCost = order.ShippingCost.Amount,
        Tax = order.Tax.Amount,
        TotalAmount = order.TotalAmount.Amount,
        Currency = order.TotalAmount.Currency,
        CreatedAt = order.CreatedAt,
        ShippedAt = order.ShippedAt,
        DeliveredAt = order.DeliveredAt,
        Items = order.Items.Select(i => new OrderItemType
        {
            Id = i.Id,
            ProductId = i.ProductId,
            ProductName = i.ProductName,
            ProductSku = i.ProductSku,
            UnitPrice = i.UnitPrice.Amount,
            Quantity = i.Quantity,
            TotalPrice = i.TotalPrice.Amount
        }).ToList()
    };
}