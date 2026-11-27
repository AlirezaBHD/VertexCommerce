using System.Globalization;
using System.Text.Json;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using VertexCommerce.Modules.Notifications.Domain.Entities;
using VertexCommerce.Modules.Notifications.Services.Dispatching;
using VertexCommerce.Modules.Notifications.Services;
using VertexCommerce.Modules.Notifications.Services.Models;
using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.IntegrationEvents;
using Xunit;

namespace VertexCommerce.Modules.Notifications.Tests;

public sealed class OrderNotificationTests
{
    [Fact]
    public async Task Order_creation_persists_the_customer_and_admin_batch_before_delivering_each_notification()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        var ct = timeout.Token;
        var connections = Substitute.For<ISseConnectionManager>();
        await using var host = await NotificationsTestHost.StartAsync(ct, connections);
        var dependencies = host.Dependencies;
        var order = new OrderCreatedIntegrationEvent(Guid.NewGuid(), Guid.NewGuid(), "VC-100", 1250, "IRR");
        var customerId = Guid.NewGuid();
        var firstAdmin = Guid.NewGuid();
        var secondAdmin = Guid.NewGuid();
        dependencies.Customers.GetCustomerInfo(order.CustomerId, ct)
            .Returns(new CustomerInfoDto("test-phone", "آوا", "احمدی", null, null, customerId));
        dependencies.Identity.GetAdminUserIdsAsync(ct).Returns([firstAdmin, secondAdmin, firstAdmin]);

        var storedEventIds = new List<Guid>();
        dependencies.OrderInbox.BeforeStoreAsync = async (eventId, notifications) =>
        {
            storedEventIds.Add(eventId);
            Assert.Equal(3, notifications.Count);
            await AssertNoLiveDeliveryAsync(connections, ct);
        };

        await host.PublishAsync(order, ct);

        Assert.Equal(order.EventId, Assert.Single(storedEventIds));
        var payload = JsonSerializer.Serialize(order);
        var batch = dependencies.OrderInbox.GetBatch(order.EventId);
        var customer = Assert.Single(batch, item => item.Type == "OrderCreated");
        var adminNotifications = batch.Where(item => item.Type == "AdminOrderCreated").ToArray();
        Assert.Equal(customerId, customer.UserId);
        Assert.Equal("OrderCreated", customer.Type);
        Assert.Equal("سفارش شما ثبت شد", customer.Title);
        Assert.Equal("سفارش جدید شما با شماره VC-100 با موفقیت ثبت شد.", customer.Message);
        Assert.Equal(payload, customer.PayloadJson);
        Assert.Equal(new[] { firstAdmin, secondAdmin }, adminNotifications.Select(item => item.UserId));
        var amount = order.TotalAmount.ToString("N0", CultureInfo.CurrentCulture);
        Assert.All(adminNotifications, item =>
        {
            Assert.Equal("AdminOrderCreated", item.Type);
            Assert.Equal("سفارش جدید ثبت شد", item.Title);
            Assert.Equal($"کاربر آوا احمدی سفارش جدید با شماره VC-100 به مبلغ {amount} IRR ثبت کرد.", item.Message);
            Assert.Equal(payload, item.PayloadJson);
        });
        var customerData = JsonSerializer.Serialize(new { orderId = order.OrderId, orderNumber = order.OrderNumber });
        var adminData = JsonSerializer.Serialize(new { orderId = order.OrderId, orderNumber = order.OrderNumber, isAdmin = true });

        var messages = dependencies.OrderInbox.GetMessages(order.EventId);
        AssertPushPayload(Assert.Single(messages, item => item.Notification.Id == customer.Id), "/account", customerData);
        Assert.All(messages.Where(item => item.Notification.Id != customer.Id), item =>
            AssertPushPayload(item, "/orders", adminData));

        Received.InOrder(() =>
        {
            _ = connections.BroadcastToUserAsync(customerId, Arg.Is<NotificationMessage>(message => Matches(customer, message)), ct);
            foreach (var admin in adminNotifications)
            {
                _ = connections.BroadcastToUserAsync(admin.UserId, Arg.Is<NotificationMessage>(message => Matches(admin, message)), ct);
            }
        });
    }

    [Theory]
    [InlineData("PaymentPending", null, null, "در انتظار پرداخت", "سفارش شماره VC-100 در انتظار ثبت فیش واریزی یا پرداخت است.")]
    [InlineData("PaymentUnderReview", null, null, "رسید پرداخت در حال بررسی", "فیش واریزی سفارش شماره VC-100 با موفقیت ثبت شد و در حال بررسی توسط پشتیبانی است.")]
    [InlineData("Processing", null, null, "پرداخت تایید شد", "پرداخت سفارش شماره VC-100 تایید شد و سفارش وارد مرحله پردازش و آماده‌سازی شد.")]
    [InlineData("Shipped", null, null, "سفارش ارسال شد", "سفارش شماره VC-100 تحویل پست/پیک شد.")]
    [InlineData("Shipped", "TR-100", null, "سفارش ارسال شد", "سفارش شماره VC-100 تحویل پست شد. کد پیگیری مرسوله: TR-100")]
    [InlineData("Delivered", null, null, "سفارش تحویل داده شد", "سفارش شماره VC-100 با موفقیت تحویل داده شد. از خرید شما سپاسگزاریم.")]
    [InlineData("Cancelled", null, null, "سفارش لغو شد", "سفارش شماره VC-100 لغو شد.")]
    [InlineData("Cancelled", null, "تکمیل نشد", "سفارش لغو شد", "سفارش شماره VC-100 لغو شد. علت: تکمیل نشد")]
    [InlineData("PaymentRejected", null, null, "رسید پرداخت رد شد", "فیش واریزی سفارش شماره VC-100 مورد تایید قرار نگرفت. لطفاً با پشتیبانی تماس بگیرید.")]
    [InlineData("PaymentRejected", null, "مبلغ نادرست", "رسید پرداخت رد شد", "فیش واریزی سفارش شماره VC-100 رد شد. علت: مبلغ نادرست")]
    [InlineData("Unknown", null, null, "بروزرسانی وضعیت سفارش", "وضعیت سفارش شماره VC-100 به 'Unknown' تغییر یافت.")]
    public async Task Order_status_uses_the_current_message_and_customer_lookup_fallback(
        string status, string? trackingNumber, string? reason, string title, string body)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        var ct = timeout.Token;
        var connections = Substitute.For<ISseConnectionManager>();
        await using var host = await NotificationsTestHost.StartAsync(ct, connections);
        var dependencies = host.Dependencies;
        var order = NewStatusEvent(status, trackingNumber, reason);
        dependencies.Customers.GetCustomerInfo(order.CustomerId, ct).ThrowsAsync(new InvalidOperationException("Lookup failed."));
        var payload = JsonSerializer.Serialize(order);
        var pushData = JsonSerializer.Serialize(new { orderId = order.OrderId, orderNumber = order.OrderNumber, status });

        await host.PublishAsync(order, ct);

        var saved = Assert.Single(dependencies.OrderInbox.GetBatch(order.EventId));
        Assert.Equal(order.CustomerId, saved.UserId);
        Assert.Equal("OrderStatusChanged", saved.Type);
        Assert.Equal(title, saved.Title);
        Assert.Equal(body, saved.Message);
        Assert.Equal(payload, saved.PayloadJson);
        AssertPushPayload(Assert.Single(dependencies.OrderInbox.GetMessages(order.EventId)), "/account", pushData);
        await connections.Received(1).BroadcastToUserAsync(order.CustomerId,
            Arg.Is<NotificationMessage>(message => Matches(saved, message)), ct);
    }

    [Fact]
    public async Task Payment_receipt_creates_admin_notifications_and_a_duplicate_skips_both_audiences()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        var ct = timeout.Token;
        var connections = Substitute.For<ISseConnectionManager>();
        await using var host = await NotificationsTestHost.StartAsync(ct, connections);
        var dependencies = host.Dependencies;
        var order = NewStatusEvent("PaymentUnderReview");
        var adminId = Guid.NewGuid();
        const string phone = "test-phone";
        dependencies.Customers.GetCustomerInfo(order.CustomerId, ct)
            .Returns(new CustomerInfoDto(phone, " ", null, null, null));
        dependencies.Identity.GetAdminUserIdsAsync(ct).Returns([adminId]);

        await host.PublishAsync(order, ct);
        await host.PublishAsync(order, ct);

        const string title = "رسید پرداخت جدید دریافت شد";
        const string body = "کاربر test-phone فیش واریزی برای سفارش VC-100 را ارسال کرد و در انتظار بررسی است.";
        var payload = JsonSerializer.Serialize(order);
        var pushData = JsonSerializer.Serialize(new { orderId = order.OrderId, orderNumber = order.OrderNumber, isAdmin = true });
        var batch = dependencies.OrderInbox.GetBatch(order.EventId);
        Assert.Equal(2, batch.Count);
        var admin = Assert.Single(batch, item => item.Type == "AdminPaymentReceiptSubmitted");
        Assert.Equal(adminId, admin.UserId);
        Assert.Equal(title, admin.Title);
        Assert.Equal(body, admin.Message);
        Assert.Equal(payload, admin.PayloadJson);
        await connections.Received(2).BroadcastToUserAsync(
            Arg.Any<Guid>(), Arg.Any<NotificationMessage>(), ct);
        AssertPushPayload(Assert.Single(dependencies.OrderInbox.GetMessages(order.EventId),
            item => item.Notification.Id == admin.Id), "/orders", pushData);
    }

    [Fact]
    public async Task Failed_save_stops_sse_and_push_for_unsaved_notifications()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        var ct = timeout.Token;
        var connections = Substitute.For<ISseConnectionManager>();
        await using var host = await NotificationsTestHost.StartAsync(ct, connections);
        var dependencies = host.Dependencies;
        var order = new OrderCreatedIntegrationEvent(Guid.NewGuid(), Guid.NewGuid(), "VC-100", 1250, "IRR");
        dependencies.Identity.GetAdminUserIdsAsync(ct).Returns([Guid.NewGuid(), Guid.NewGuid()]);
        dependencies.OrderInbox.BeforeStoreAsync = (_, _) => Task.FromException(new InvalidOperationException("Storage failed."));

        await host.PublishAsync(order, ct);

        Assert.Empty(dependencies.OrderInbox.GetBatch(order.EventId));
        await AssertNoLiveDeliveryAsync(connections, ct);
    }

    [Fact]
    public async Task Sse_failure_leaves_the_batch_committed_and_other_recipients_are_still_notified()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        var ct = timeout.Token;
        var connections = Substitute.For<ISseConnectionManager>();
        await using var host = await NotificationsTestHost.StartAsync(ct, connections);
        var dependencies = host.Dependencies;
        var order = new OrderCreatedIntegrationEvent(Guid.NewGuid(), Guid.NewGuid(), "VC-100", 1250, "IRR");
        var failedAdmin = Guid.NewGuid();
        var otherAdmin = Guid.NewGuid();
        dependencies.Identity.GetAdminUserIdsAsync(ct).Returns([failedAdmin, otherAdmin]);
        connections.BroadcastToUserAsync(failedAdmin, Arg.Any<NotificationMessage>(), ct)
            .ThrowsAsync(new InvalidOperationException("SSE failed."));

        await host.PublishAsync(order, ct);
        await host.PublishAsync(order, ct);

        Assert.Equal(3, dependencies.OrderInbox.GetBatch(order.EventId).Count);
        await connections.Received(3).BroadcastToUserAsync(Arg.Any<Guid>(), Arg.Any<NotificationMessage>(), ct);
        await connections.Received(1).BroadcastToUserAsync(otherAdmin, Arg.Any<NotificationMessage>(), ct);
    }

    private static Task AssertNoLiveDeliveryAsync(ISseConnectionManager connections, CancellationToken ct)
    {
        return connections.DidNotReceive().BroadcastToUserAsync(
            Arg.Any<Guid>(), Arg.Any<NotificationMessage>(), ct);
    }

    private static void AssertPushPayload(InboxNotification item, string url, string expectedData)
    {
        using var payload = JsonDocument.Parse(item.PushPayload);
        var json = payload.RootElement;
        Assert.Equal(item.Notification.UserId, json.GetProperty("userId").GetGuid());
        Assert.Equal(item.Notification.Title, json.GetProperty("title").GetString());
        Assert.Equal(item.Notification.Message, json.GetProperty("body").GetString());
        Assert.Equal("/logo.ico", json.GetProperty("icon").GetString());
        Assert.Equal("/logo.ico", json.GetProperty("badge").GetString());
        Assert.Equal(url, json.GetProperty("url").GetString());
        Assert.Equal(expectedData, json.GetProperty("data").GetRawText());
    }

    private static OrderStatusChangedIntegrationEvent NewStatusEvent(
        string status, string? trackingNumber = null, string? reason = null)
    {
        return new OrderStatusChangedIntegrationEvent(
            Guid.NewGuid(), Guid.NewGuid(), "VC-100", "PaymentPending", status, 1250, "IRR", trackingNumber, reason);
    }

    private static bool Matches(Notification notification, NotificationMessage message)
    {
        return notification.Id == message.Id && notification.UserId == message.UserId &&
            notification.Title == message.Title && notification.Message == message.Message &&
            notification.Type == message.Type && notification.PayloadJson == message.PayloadJson &&
            notification.IsRead == message.IsRead && notification.CreatedAt == message.CreatedAt;
    }
}
