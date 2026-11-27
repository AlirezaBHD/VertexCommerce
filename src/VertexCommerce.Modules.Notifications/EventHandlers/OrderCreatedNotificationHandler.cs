using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using VertexCommerce.Modules.Notifications.Services.Dispatching;
using VertexCommerce.Shared.IntegrationEvents;

namespace VertexCommerce.Modules.Notifications.EventHandlers;

internal sealed class OrderCreatedNotificationHandler(
    INotificationDispatcher dispatcher,
    OrderNotificationRecipientResolver recipientResolver,
    ILogger<OrderCreatedNotificationHandler> logger)
    : INotificationHandler<OrderCreatedIntegrationEvent>
{
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Integration event handler should not throw and abort event pipeline")]
    public async Task Handle(OrderCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var recipient = await recipientResolver.ResolveAsync(notification.CustomerId, cancellationToken);
            var payloadJson = JsonSerializer.Serialize(notification);

            var customerDraft = new NotificationDraft(
                Title: "سفارش شما ثبت شد",
                Message: $"سفارش جدید شما با شماره {notification.OrderNumber} با موفقیت ثبت شد.",
                Type: OrderNotificationTypes.OrderCreated,
                PayloadJson: payloadJson,
                PushUrl: "/account",
                PushData: new { orderId = notification.OrderId, orderNumber = notification.OrderNumber });

            var adminDraft = new NotificationDraft(
                Title: "سفارش جدید ثبت شد",
                Message: $"کاربر {recipient.DisplayName} سفارش جدید با شماره {notification.OrderNumber} به مبلغ {notification.TotalAmount:N0} {notification.Currency} ثبت کرد.",
                Type: OrderNotificationTypes.AdminOrderCreated,
                PayloadJson: payloadJson,
                PushUrl: "/orders",
                PushData: new { orderId = notification.OrderId, orderNumber = notification.OrderNumber, isAdmin = true });

            await dispatcher.DispatchOrderEventAsync(
                notification.EventId, recipient.UserId, customerDraft, adminDraft, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process order creation event {EventId} for order {OrderId}",
                notification.EventId, notification.OrderId);
        }
    }
}
