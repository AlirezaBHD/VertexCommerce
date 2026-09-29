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

public sealed class OrderCreatedNotificationHandler : INotificationHandler<OrderCreatedIntegrationEvent>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationsUnitOfWork _unitOfWork;
    private readonly ISseConnectionManager _sseManager;
    private readonly IWebPushService _webPushService;
    private readonly ICustomerService _customerService;
    private readonly IIdentityService _identityService;
    private readonly ILogger<OrderCreatedNotificationHandler> _logger;

    public OrderCreatedNotificationHandler(
        INotificationRepository notificationRepository,
        INotificationsUnitOfWork unitOfWork,
        ISseConnectionManager sseManager,
        IWebPushService webPushService,
        ICustomerService customerService,
        IIdentityService identityService,
        ILogger<OrderCreatedNotificationHandler> logger)
    {
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
        _sseManager = sseManager;
        _webPushService = webPushService;
        _customerService = customerService;
        _identityService = identityService;
        _logger = logger;
    }

    public async Task Handle(OrderCreatedIntegrationEvent notification, CancellationToken ct)
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

            var title = "سفارش شما ثبت شد";
            var message = $"سفارش جدید شما با شماره {notification.OrderNumber} با موفقیت ثبت شد.";
            var payloadJson = JsonSerializer.Serialize(notification);

            // 1. Notify Customer
            try
            {
                var entity = Notification.Create(
                    userId: customerUserId,
                    title: title,
                    message: message,
                    type: "OrderCreated",
                    payloadJson: payloadJson);

                await _notificationRepository.AddAsync(entity, ct);
                await _unitOfWork.SaveChangesAsync(ct);

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

                await _webPushService.SendPushToUserAsync(
                    userId: entity.UserId,
                    title: title,
                    body: message,
                    url: "/account",
                    data: new
                    {
                        orderId = notification.OrderId,
                        orderNumber = notification.OrderNumber
                    },
                    ct: ct);

                _logger.LogInformation("OrderCreated notification dispatched for Customer {UserId} on Order {OrderNumber}",
                    customerUserId, notification.OrderNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to notify customer for OrderCreated {OrderNumber}", notification.OrderNumber);
            }

            // 2. Notify Admins
            try
            {
                var adminTitle = "سفارش جدید ثبت شد";
                var adminMessage = $"کاربر {customerName} سفارش جدید با شماره {notification.OrderNumber} به مبلغ {notification.TotalAmount:N0} {notification.Currency} ثبت کرد.";

                var adminIds = await _identityService.GetAdminUserIdsAsync(ct);
                _logger.LogInformation("Found {Count} admin user(s) to notify for Order {OrderNumber}", adminIds.Count, notification.OrderNumber);

                foreach (var adminId in adminIds)
                {
                    var adminNotification = Notification.Create(
                        userId: adminId,
                        title: adminTitle,
                        message: adminMessage,
                        type: "AdminOrderCreated",
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

                _logger.LogInformation("OrderCreated notification dispatched to {Count} admin(s) for Order {OrderNumber}",
                    adminIds.Count, notification.OrderNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send admin notification for OrderCreated {OrderNumber}", notification.OrderNumber);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling OrderCreatedIntegrationEvent for Order {OrderId}", notification.OrderId);
        }
    }
}
