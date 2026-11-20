using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using VertexCommerce.Modules.Notifications.Domain.Repositories;
using VertexCommerce.Modules.Notifications.Services.Dispatching;
using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.IntegrationEvents;

namespace VertexCommerce.Modules.Notifications.EventHandlers;

public sealed class OrderStatusChangedNotificationHandler(
    INotificationRepository notificationRepository,
    INotificationDispatcher dispatcher,
    ICustomerService customerService,
    ILogger<OrderStatusChangedNotificationHandler> logger)
    : INotificationHandler<OrderStatusChangedIntegrationEvent>
{
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Integration event handler should not throw and abort event pipeline")]
    public async Task Handle(OrderStatusChangedIntegrationEvent notification, CancellationToken cancellationToken)
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

            // 1. Idempotency Check for customer
            var exists = await notificationRepository.ExistsForEventAsync(
                customerUserId,
                "OrderStatusChanged",
                notification.OrderNumber,
                notification.NewStatus,
                cancellationToken);

            if (exists)
            {
                logger.LogInformation("Notification already created for Order {OrderNumber} status {Status}. Skipping duplicate.",
                    notification.OrderNumber, notification.NewStatus);
                return;
            }

            // 2. Format Persian messages for customer
            var (title, message) = FormatStatusMessage(notification);
            var payloadJson = JsonSerializer.Serialize(notification);

            // 3. Dispatch to Customer
            var customerPushData = new
            {
                orderId = notification.OrderId,
                orderNumber = notification.OrderNumber,
                status = notification.NewStatus
            };

            await dispatcher.DispatchToUserAsync(
                userId: customerUserId,
                title: title,
                message: message,
                type: "OrderStatusChanged",
                payloadJson: payloadJson,
                pushUrl: "/account",
                pushData: customerPushData,
                ct: cancellationToken);

            // 4. If payment receipt was submitted (PaymentUnderReview), also notify Admins!
            if (notification.NewStatus == "PaymentUnderReview")
            {
                const string adminTitle = "رسید پرداخت جدید دریافت شد";
                var adminMessage = $"کاربر {customerName} فیش واریزی برای سفارش {notification.OrderNumber} را ارسال کرد و در انتظار بررسی است.";
                var adminPushData = new
                {
                    orderId = notification.OrderId,
                    orderNumber = notification.OrderNumber,
                    isAdmin = true
                };

                await dispatcher.DispatchToAdminsAsync(
                    title: adminTitle,
                    message: adminMessage,
                    type: "AdminPaymentReceiptSubmitted",
                    payloadJson: payloadJson,
                    pushUrl: "/orders",
                    pushData: adminPushData,
                    ct: cancellationToken);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling OrderStatusChangedIntegrationEvent for Order {OrderId}", notification.OrderId);
        }
    }

    private static (string Title, string Message) FormatStatusMessage(OrderStatusChangedIntegrationEvent ev)
    {
        return ev.NewStatus switch
        {
            "PaymentPending" => (
                "در انتظار پرداخت",
                $"سفارش شماره {ev.OrderNumber} در انتظار ثبت فیش واریزی یا پرداخت است."
            ),
            "PaymentUnderReview" => (
                "رسید پرداخت در حال بررسی",
                $"فیش واریزی سفارش شماره {ev.OrderNumber} با موفقیت ثبت شد و در حال بررسی توسط پشتیبانی است."
            ),
            "Processing" => (
                "پرداخت تایید شد",
                $"پرداخت سفارش شماره {ev.OrderNumber} تایید شد و سفارش وارد مرحله پردازش و آماده‌سازی شد."
            ),
            "Shipped" => (
                "سفارش ارسال شد",
                string.IsNullOrWhiteSpace(ev.TrackingNumber)
                    ? $"سفارش شماره {ev.OrderNumber} تحویل پست/پیک شد."
                    : $"سفارش شماره {ev.OrderNumber} تحویل پست شد. کد پیگیری مرسوله: {ev.TrackingNumber}"
            ),
            "Delivered" => (
                "سفارش تحویل داده شد",
                $"سفارش شماره {ev.OrderNumber} با موفقیت تحویل داده شد. از خرید شما سپاسگزاریم."
            ),
            "Cancelled" => (
                "سفارش لغو شد",
                string.IsNullOrWhiteSpace(ev.CancellationReason)
                    ? $"سفارش شماره {ev.OrderNumber} لغو شد."
                    : $"سفارش شماره {ev.OrderNumber} لغو شد. علت: {ev.CancellationReason}"
            ),
            "PaymentRejected" => (
                "رسید پرداخت رد شد",
                string.IsNullOrWhiteSpace(ev.CancellationReason)
                    ? $"فیش واریزی سفارش شماره {ev.OrderNumber} مورد تایید قرار نگرفت. لطفاً با پشتیبانی تماس بگیرید."
                    : $"فیش واریزی سفارش شماره {ev.OrderNumber} رد شد. علت: {ev.CancellationReason}"
            ),
            _ => (
                "بروزرسانی وضعیت سفارش",
                $"وضعیت سفارش شماره {ev.OrderNumber} به '{ev.NewStatus}' تغییر یافت."
            )
        };
    }
}
