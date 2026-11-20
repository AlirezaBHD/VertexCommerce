using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using VertexCommerce.Modules.Notifications.Services.Dispatching;
using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.IntegrationEvents;

namespace VertexCommerce.Modules.Notifications.EventHandlers;

public sealed class OrderCreatedNotificationHandler(
    INotificationDispatcher dispatcher,
    ICustomerService customerService,
    ILogger<OrderCreatedNotificationHandler> logger)
    : INotificationHandler<OrderCreatedIntegrationEvent>
{
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Integration event handler should not throw and abort event pipeline")]
    public async Task Handle(OrderCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            CustomerInfoDto? customer = null;
            try
            {
                customer = await customerService.GetCustomerInfo(notification.CustomerId, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Could not fetch customer info for CustomerId {CustomerId}", notification.CustomerId);
            }

            var customerUserId = customer?.UserId ?? notification.CustomerId;
            var customerName = customer is not null && (!string.IsNullOrWhiteSpace(customer.FirstName) || !string.IsNullOrWhiteSpace(customer.LastName))
                ? $"{customer.FirstName} {customer.LastName}".Trim()
                : customer?.PhoneNumber ?? "مشتری";

            var payloadJson = JsonSerializer.Serialize(notification);

            // 1. Notify Customer
            const string title = "سفارش شما ثبت شد";
            var message = $"سفارش جدید شما با شماره {notification.OrderNumber} با موفقیت ثبت شد.";
            var pushData = new
            {
                orderId = notification.OrderId,
                orderNumber = notification.OrderNumber
            };

            await dispatcher.DispatchToUserAsync(
                userId: customerUserId,
                title: title,
                message: message,
                type: "OrderCreated",
                payloadJson: payloadJson,
                pushUrl: "/account",
                pushData: pushData,
                ct: cancellationToken);

            // 2. Notify Admins
            const string adminTitle = "سفارش جدید ثبت شد";
            var adminMessage = $"کاربر {customerName} سفارش جدید با شماره {notification.OrderNumber} به مبلغ {notification.TotalAmount:N0} {notification.Currency} ثبت کرد.";
            var adminPushData = new
            {
                orderId = notification.OrderId,
                orderNumber = notification.OrderNumber,
                isAdmin = true
            };

            await dispatcher.DispatchToAdminsAsync(
                title: adminTitle,
                message: adminMessage,
                type: "AdminOrderCreated",
                payloadJson: payloadJson,
                pushUrl: "/orders",
                pushData: adminPushData,
                ct: cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling OrderCreatedIntegrationEvent for Order {OrderId}", notification.OrderId);
        }
    }
}
