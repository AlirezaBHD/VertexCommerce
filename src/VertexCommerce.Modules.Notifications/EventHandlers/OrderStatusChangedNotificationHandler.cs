using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using VertexCommerce.Modules.Notifications.Services.Dispatching;
using VertexCommerce.Shared.IntegrationEvents;

namespace VertexCommerce.Modules.Notifications.EventHandlers;

internal sealed class OrderStatusChangedNotificationHandler(
    INotificationDispatcher dispatcher,
    OrderNotificationRecipientResolver recipientResolver,
    ILogger<OrderStatusChangedNotificationHandler> logger)
    : INotificationHandler<OrderStatusChangedIntegrationEvent>
{
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Integration event handler should not throw and abort event pipeline")]
    public async Task Handle(OrderStatusChangedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var recipient = await recipientResolver.ResolveAsync(notification.CustomerId, cancellationToken);
            var (title, message) = FormatStatusMessage(notification);
            var payloadJson = JsonSerializer.Serialize(notification);
            var customerDraft = new NotificationDraft(
                Title: title,
                Message: message,
                Type: OrderNotificationTypes.OrderStatusChanged,
                PayloadJson: payloadJson,
                PushUrl: "/account",
                PushData: new { orderId = notification.OrderId, orderNumber = notification.OrderNumber, status = notification.NewStatus });

            NotificationDraft? adminDraft = null;
            if (notification.NewStatus == "PaymentUnderReview")
            {
                adminDraft = new NotificationDraft(
                    Title: "رسید پرداخت جدید دریافت شد",
                    Message: $"کاربر {recipient.DisplayName} فیش واریزی برای سفارش {notification.OrderNumber} را ارسال کرد و در انتظار بررسی است.",
                    Type: OrderNotificationTypes.AdminPaymentReceiptSubmitted,
                    PayloadJson: payloadJson,
                    PushUrl: "/orders",
                    PushData: new { orderId = notification.OrderId, orderNumber = notification.OrderNumber, isAdmin = true });
            }

            await dispatcher.DispatchOrderEventAsync(
                notification.EventId, recipient.UserId, customerDraft, adminDraft, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process order status event {EventId} for order {OrderId}",
                notification.EventId, notification.OrderId);
        }
    }

    private static (string Title, string Message) FormatStatusMessage(OrderStatusChangedIntegrationEvent order)
    {
        return order.NewStatus switch
        {
            "PaymentPending" => (
                "در انتظار پرداخت",
                $"سفارش شماره {order.OrderNumber} در انتظار ثبت فیش واریزی یا پرداخت است."
            ),
            "PaymentUnderReview" => (
                "رسید پرداخت در حال بررسی",
                $"فیش واریزی سفارش شماره {order.OrderNumber} با موفقیت ثبت شد و در حال بررسی توسط پشتیبانی است."
            ),
            "Processing" => (
                "پرداخت تایید شد",
                $"پرداخت سفارش شماره {order.OrderNumber} تایید شد و سفارش وارد مرحله پردازش و آماده‌سازی شد."
            ),
            "Shipped" => (
                "سفارش ارسال شد",
                string.IsNullOrWhiteSpace(order.TrackingNumber)
                    ? $"سفارش شماره {order.OrderNumber} تحویل پست/پیک شد."
                    : $"سفارش شماره {order.OrderNumber} تحویل پست شد. کد پیگیری مرسوله: {order.TrackingNumber}"
            ),
            "Delivered" => (
                "سفارش تحویل داده شد",
                $"سفارش شماره {order.OrderNumber} با موفقیت تحویل داده شد. از خرید شما سپاسگزاریم."
            ),
            "Cancelled" => (
                "سفارش لغو شد",
                string.IsNullOrWhiteSpace(order.CancellationReason)
                    ? $"سفارش شماره {order.OrderNumber} لغو شد."
                    : $"سفارش شماره {order.OrderNumber} لغو شد. علت: {order.CancellationReason}"
            ),
            "PaymentRejected" => (
                "رسید پرداخت رد شد",
                string.IsNullOrWhiteSpace(order.CancellationReason)
                    ? $"فیش واریزی سفارش شماره {order.OrderNumber} مورد تایید قرار نگرفت. لطفاً با پشتیبانی تماس بگیرید."
                    : $"فیش واریزی سفارش شماره {order.OrderNumber} رد شد. علت: {order.CancellationReason}"
            ),
            _ => (
                "بروزرسانی وضعیت سفارش",
                $"وضعیت سفارش شماره {order.OrderNumber} به '{order.NewStatus}' تغییر یافت."
            )
        };
    }
}
