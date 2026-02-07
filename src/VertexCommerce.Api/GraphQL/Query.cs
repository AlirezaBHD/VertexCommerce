using VertexCommerce.Api.GraphQL.Catalog;
using VertexCommerce.Api.GraphQL.Orders;
using VertexCommerce.Modules.Catalog.Domain.Repositories;
using VertexCommerce.Modules.Orders.Domain.Repositories;

namespace VertexCommerce.Api.GraphQL;

public sealed class Query
{
    /* ---------- Catalog ---------- */

    public async Task<IEnumerable<ProductGql>> Products(
        [Service] IProductRepository productRepository,
        CancellationToken ct)
    {
        var products = await productRepository.GetAllAsync(ct);

        return products.Select(p => new ProductGql
        {
            Id = p.Id,
            Name = p.Name,
            Sku = p.Sku.Value,
            Price = p.Price.Amount,
            Currency = p.Price.Currency,
            StockQuantity = p.StockQuantity,
            IsActive = p.IsActive,
            CategoryName = p.Category?.Name
        });
    }

    /* ---------- Orders ---------- */

    public async Task<IEnumerable<OrderGql>> Orders(
        [Service] IOrderRepository orderRepository,
        CancellationToken ct)
    {
        var orders = await orderRepository.GetAllAsync(ct);

        return orders.Select(o => new OrderGql
        {
            Id = o.Id,
            OrderNumber = o.OrderNumber,
            Status = o.Status,
            TotalAmount = o.TotalAmount.Amount,
            Currency = o.TotalAmount.Currency,
            CreatedAt = o.CreatedAt
        });
    }
}
