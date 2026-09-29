using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using VertexCommerce.Modules.Notifications.Domain.Entities;
using VertexCommerce.Modules.Notifications.Domain.Repositories;
using VertexCommerce.Modules.Notifications.Persistence;
using VertexCommerce.Modules.Notifications.Services;
using VertexCommerce.Modules.Notifications.Services.Models;
using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.IntegrationEvents;

namespace VertexCommerce.Modules.Notifications.EventHandlers;

public sealed class OrderStatusChangedNotificationHandler : INotificationHandler<OrderStatusChangedIntegrationEvent>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationsUnitOfWork _unitOfWork;
    private readonly ISseConnectionManager _sseManager;
    private readonly IWebPushService _webPushService;
    private readonly ICustomerService _customerService;
    private readonly IIdentityService _identityService;
    private readonly ILogger<OrderStatusChangedNotificationHandler> _logger;

    public OrderStatusChangedNotificationHandler(
        INotificationRepository notificationRepository,
        INotificationsUnitOfWork unitOfWork,
        ISseConnectionManager sseManager,
        IWebPushService webPushService,
        ICustomerService customerService,
        IIdentityService identityService,
        ILogger<OrderStatusChangedNotificationHandler> logger)
    {
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
        _sseManager = sseManager;
        _webPushService = webPushService;
        _customerService = customerService;
        _identityService = identityService;
        _logger = logger;
    }

    public async Task Handle(OrderStatusChangedIntegrationEvent notification, CancellationToken ct)
    {
        try
        {
            CustomerInfoDto? customer = null;
            try
            {
                customer = await _customerService.GetCustomerInfo(notification.CustomerId, ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not fetch customer info for CustomerId {CustomerId}", notification.CustomerId);
            }

            var customerUserId = customer?.UserId ?? notification.CustomerId;
            var customerName = customer is not null && (!string.IsNullOrWhiteSpace(customer.FirstName) || !string.IsNullOrWhiteSpace(customer.LastName))
                ? $"{customer.FirstName} {customer.LastName}".Trim()
                : customer?.PhoneNumber ?? "مشتری";

            // 1. Idempotency Check for customer
            var exists = await _notificationRepository.ExistsForEventAsync(
                customerUserId,
                "OrderStatusChanged",
                notification.OrderNumber,
                notification.NewStatus,
                ct);

            if (exists)
            {
                _logger.LogInformation("Notification already created for Order {OrderNumber} status {Status}. Skipping duplicate.",
                    notification.OrderNumber, notification.NewStatus);
                return;
            }

            // 2. Format Persian messages for customer
            var (title, message) = FormatStatusMessage(notification);
            var payloadJson = JsonSerializer.Serialize(notification);

            // 3. Persist Notification in DB for Customer
            try
            {
                var entity = Notification.Create(
                    userId: customerUserId,
                    title: title,
                    message: message,
                    type: "OrderStatusChanged",
                    payloadJson: payloadJson);

                await _notificationRepository.AddAsync(entity, ct);
                await _unitOfWork.SaveChangesAsync(ct);

                // 4. Real-time In-App notification via SSE for Customer
                var sseMessage = new NotificationMessage(
                    Id: entity.Id,
                    UserId: entity.UserId,
                    Title: entity.Title,
                    Message: entity.Message,
                    Type: entity.Type,
                    PayloadJson: entity.PayloadJson,
                    IsRead: entity.IsRead,
                    CreatedAt: entity.CreatedAt);

                await _sseManager.BroadcastToUserAsync(entity.UserId, sseMessage, ct);

                // 5. Background Web Push notification to browser for Customer
                var orderUrl = $"/account";
                await _webPushService.SendPushToUserAsync(
                    userId: entity.UserId,
                    title: title,
                    body: message,
                    url: orderUrl,
                    data: new
                    {
                        orderId = notification.OrderId,
                        orderNumber = notification.OrderNumber,
                        status = notification.NewStatus
                    },
                    ct: ct);

                _logger.LogInformation("Notification dispatched successfully to customer {UserId} for Order {OrderNumber} status {Status}",
                    customerUserId, notification.OrderNumber, notification.NewStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send customer notification for OrderStatusChanged {OrderNumber}", notification.OrderNumber);
            }

            // 6. If payment receipt was submitted (PaymentUnderReview), also notify Admins!
            if (notification.NewStatus == "PaymentUnderReview")
            {
                try
                {
                    var adminTitle = "رسید پرداخت جدید دریافت شد";
                    var adminMessage = $"کاربر {customerName} فیش واریزی برای سفارش {notification.OrderNumber} را ارسال کرد و در انتظار بررسی است.";

                    var adminIds = await _identityService.GetAdminUserIdsAsync(ct);
                    _logger.LogInformation("Found {Count} admin user(s) to notify for receipt submission on Order {OrderNumber}", adminIds.Count, notification.OrderNumber);

                    foreach (var adminId in adminIds)
                    {
                        var adminNotification = Notification.Create(
                            userId: adminId,
                            title: adminTitle,
                            message: adminMessage,
                            type: "AdminPaymentReceiptSubmitted",
                            payloadJson: payloadJson);

                        await _notificationRepository.AddAsync(adminNotification, ct);
                        await _unitOfWork.SaveChangesAsync(ct);

                        var adminSseMsg = new NotificationMessage(
                            Id: adminNotification.Id,
                            UserId: adminNotification.UserId,
                            Title: adminNotification.Title,
                            Message: adminNotification.Message,
                            Type: adminNotification.Type,
                            PayloadJson: adminNotification.PayloadJson,
                            IsRead: adminNotification.IsRead,
                            CreatedAt: adminNotification.CreatedAt);

                        await _sseManager.BroadcastToUserAsync(adminId, adminSseMsg, ct);

                        await _webPushService.SendPushToUserAsync(
                            userId: adminId,
                            title: adminTitle,
                            body: adminMessage,
                            url: "/orders",
                            data: new
                            {
                                orderId = notification.OrderId,
                                orderNumber = notification.OrderNumber,
                                isAdmin = true
                            },
                            ct: ct);
                    }

                    _logger.LogInformation("Receipt submission notification dispatched to {Count} admin(s) for Order {OrderNumber}",
                        adminIds.Count, notification.OrderNumber);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send admin notification for receipt submission on Order {OrderNumber}", notification.OrderNumber);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling OrderStatusChangedIntegrationEvent for Order {OrderId}", notification.OrderId);
        }
    }

    private static (string Title, string Message) FormatStatusMessage(OrderStatusChangedIntegrationEvent ev)
    {
        return ev.NewStatus switch
        {
            "PaymentUnderReview" => (
                "رسید پرداخت ثبت شد",
                $"رسید پرداخت برای سفارش {ev.OrderNumber} با موفقیت ثبت شد و در انتظار بررسی است."
            ),
            "Confirmed" => (
                "سفارش شما تأیید شد",
                $"سفارش شما با شماره {ev.OrderNumber} با موفقیت تأیید شد و به زودی وارد مرحله آماده‌سازی می‌شود."
            ),
            "Processing" => (
                "سفارش در حال آماده‌سازی",
                $"سفارش {ev.OrderNumber} در حال آماده‌سازی و بسته‌بندی در انبار است."
            ),
            "Shipped" => (
                "سفارش شما تحویل پست شد",
                string.IsNullOrWhiteSpace(ev.TrackingNumber)
                    ? $"سفارش {ev.OrderNumber} تحویل شرکت پست گردید و در مسیر ارسال است."
                    : $"سفارش {ev.OrderNumber} تحویل شرکت پست گردید. کد رهگیری: {ev.TrackingNumber}"
            ),
            "Delivered" => (
                "سفارش تحویل داده شد",
                $"سفارش {ev.OrderNumber} با موفقیت تحویل داده شد. از خرید شما سپاسگزاریم."
            ),
            "Cancelled" => (
                "سفارش شما لغو شد",
                string.IsNullOrWhiteSpace(ev.CancellationReason)
                    ? $"سفارش {ev.OrderNumber} لغو شد."
                    : $"سفارش {ev.OrderNumber} لغو شد. علت: {ev.CancellationReason}"
            ),
            _ => (
                "تغییر وضعیت سفارش",
                $"وضعیت سفارش شما ({ev.OrderNumber}) به {ev.NewStatus} تغییر یافت."
            )
        };
    }
}
