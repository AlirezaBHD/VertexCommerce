using HotChocolate.Data;
using VertexCommerce.Api.GraphQL.Basket;
using VertexCommerce.Api.GraphQL.Catalog;
using VertexCommerce.Api.GraphQL.Orders;
using VertexCommerce.Modules.Basket.Domain.Repositories;
using VertexCommerce.Modules.Catalog.Domain.Repositories;
using VertexCommerce.Modules.Orders.Domain.Enums;
using VertexCommerce.Modules.Orders.Domain.Repositories;

namespace VertexCommerce.Api.GraphQL;

public sealed class Query
{
    /* ==================== CATALOG ==================== */

    [UsePaging(MaxPageSize = 50, DefaultPageSize = 10)]
    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<ProductType>> GetProducts(
        [Service] IProductRepository productRepository,
        CancellationToken ct)
    {
        var products = await productRepository.GetAllAsync(ct);

        return products.Select(p => new ProductType
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Sku = p.Sku.Value,
            Price = p.Price.Amount,
            Currency = p.Price.Currency,
            StockQuantity = p.StockQuantity,
            IsActive = p.IsActive,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        });
    }

    public async Task<ProductType?> GetProductById(
        Guid id,
        [Service] IProductRepository productRepository,
        CancellationToken ct)
    {
        var product = await productRepository.GetByIdAsync(id, ct);

        if (product is null) return null;

        return new ProductType
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Sku = product.Sku.Value,
            Price = product.Price.Amount,
            Currency = product.Price.Currency,
            StockQuantity = product.StockQuantity,
            IsActive = product.IsActive,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }

    public async Task<ProductType?> GetProductBySku(
        string sku,
        [Service] IProductRepository productRepository,
        CancellationToken ct)
    {
        var product = await productRepository.GetBySkuAsync(sku, ct);

        if (product is null) return null;

        return new ProductType
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Sku = product.Sku.Value,
            Price = product.Price.Amount,
            Currency = product.Price.Currency,
            StockQuantity = product.StockQuantity,
            IsActive = product.IsActive,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }

    [UsePaging(MaxPageSize = 50, DefaultPageSize = 10)]
    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<CategoryType>> GetCategories(
        [Service] ICategoryRepository categoryRepository,
        CancellationToken ct)
    {
        var categories = await categoryRepository.GetAllAsync(ct);

        return categories.Select(c => new CategoryType
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            ParentId = c.ParentId,
            IsActive = c.IsActive,
            SortOrder = c.SortOrder,
            CreatedAt = c.CreatedAt
        });
    }

    public async Task<CategoryType?> GetCategoryById(
        Guid id,
        [Service] ICategoryRepository categoryRepository,
        CancellationToken ct)
    {
        var category = await categoryRepository.GetByIdAsync(id, ct);

        if (category is null) return null;

        return new CategoryType
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ParentId = category.ParentId,
            IsActive = category.IsActive,
            SortOrder = category.SortOrder,
            CreatedAt = category.CreatedAt
        };
    }

    /* ==================== ORDERS ==================== */

    [UsePaging(MaxPageSize = 50, DefaultPageSize = 10)]
    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<OrderType>> GetOrders(
        [Service] IOrderRepository orderRepository,
        CancellationToken ct)
    {
        var orders = await orderRepository.GetAllAsync(ct);

        return orders.Select(MapToOrderType);
    }

    public async Task<OrderType?> GetOrderById(
        Guid id,
        [Service] IOrderRepository orderRepository,
        CancellationToken ct)
    {
        var order = await orderRepository.GetByIdAsync(id, ct);

        if (order is null) return null;

        return MapToOrderType(order);
    }

    public async Task<OrderType?> GetOrderByNumber(
        string orderNumber,
        [Service] IOrderRepository orderRepository,
        CancellationToken ct)
    {
        var order = await orderRepository.GetByOrderNumberAsync(orderNumber, ct);

        if (order is null) return null;

        return MapToOrderType(order);
    }

    public async Task<IEnumerable<OrderType>> GetOrdersByCustomer(
        Guid customerId,
        [Service] IOrderRepository orderRepository,
        CancellationToken ct)
    {
        var orders = await orderRepository.GetByCustomerIdAsync(customerId, ct);

        return orders.Select(MapToOrderType);
    }

    public async Task<IEnumerable<OrderType>> GetOrdersByStatus(
        OrderStatus status,
        [Service] IOrderRepository orderRepository,
        CancellationToken ct)
    {
        var orders = await orderRepository.GetByStatusAsync(status, ct);

        return orders.Select(MapToOrderType);
    }

    /* ==================== BASKET ==================== */

    public async Task<BasketType?> GetBasket(
        Guid customerId,
        [Service] IBasketRepository basketRepository,
        CancellationToken ct)
    {
        var basket = await basketRepository.GetByCustomerIdAsync(customerId, ct);

        if (basket is null) return null;

        return new BasketType
        {
            Id = basket.Id,
            CustomerId = basket.CustomerId,
            Currency = basket.Currency,
            TotalAmount = basket.TotalAmount,
            TotalItems = basket.TotalItems,
            CreatedAt = basket.CreatedAt,
            ExpiresAt = basket.ExpiresAt,
            Items = basket.Items.Select(i => new BasketItemType
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                ProductSku = i.ProductSku,
                ImageUrl = i.ImageUrl,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                TotalPrice = i.TotalPrice,
                AddedAt = i.AddedAt
            }).ToList()
        };
    }

    /* ==================== HELPERS ==================== */

    private static OrderType MapToOrderType(Modules.Orders.Domain.Entities.Order order)
    {
        return new OrderType
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            CustomerId = order.CustomerId,
            CustomerEmail = order.CustomerEmail,
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
}