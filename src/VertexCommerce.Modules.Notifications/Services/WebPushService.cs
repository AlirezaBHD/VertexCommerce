using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebPush;
using VertexCommerce.Modules.Notifications.Domain.Entities;
using VertexCommerce.Modules.Notifications.Domain.Repositories;
using VertexCommerce.Modules.Notifications.Persistence;

namespace VertexCommerce.Modules.Notifications.Services;

internal sealed class WebPushService(
    IOptions<WebPushOptions> options,
    IPushSubscriptionRepository subscriptionRepository,
    INotificationsUnitOfWork unitOfWork,
    ILogger<WebPushService> logger)
    : IWebPushService
{
    private readonly WebPushOptions _options = options.Value;

    public string PublicKey => _options.PublicKey;

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "WebPush individual delivery errors must not abort delivery to remaining subscriptions")]
    public async Task SendPushToUserAsync(
        Guid userId,
        string title,
        string body,
        string? url = null,
        object? data = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_options.PublicKey) || string.IsNullOrWhiteSpace(_options.PrivateKey))
        {
            logger.LogWarning("VAPID keys are not configured. Web push notification skipped for user {UserId}", userId);
            return;
        }

        var subscriptions = await subscriptionRepository.GetByUserIdAsync(userId, ct);
        if (subscriptions.Count == 0)
        {
            return;
        }

        var payload = JsonSerializer.Serialize(new
        {
            title,
            body,
            icon = "/logo.ico",
            badge = "/logo.ico",
            url = url ?? "/account",
            data
        });

        var vapidDetails = new VapidDetails(_options.Subject, _options.PublicKey, _options.PrivateKey);
        using var client = new WebPushClient();

        List<Domain.Entities.PushSubscription>? expiredSubscriptions = null;

        foreach (var sub in subscriptions)
        {
            try
            {
                var pushSub = new WebPush.PushSubscription(sub.Endpoint, sub.P256dhKey, sub.AuthKey);
                await client.SendNotificationAsync(pushSub, payload, vapidDetails, ct);
                logger.LogInformation("Push notification sent successfully to endpoint {Endpoint} for user {UserId}",
                    sub.Endpoint[..Math.Min(sub.Endpoint.Length, 35)], userId);
            }
            catch (WebPushException ex) when (ex.StatusCode is HttpStatusCode.Gone or HttpStatusCode.NotFound)
            {
                logger.LogInformation(ex, "Push subscription has expired or unsubscribed (status {Status}). Removing endpoint {Endpoint}",
                    ex.StatusCode, sub.Endpoint[..Math.Min(sub.Endpoint.Length, 35)]);

                expiredSubscriptions ??= [];
                expiredSubscriptions.Add(sub);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send WebPush notification to user {UserId} endpoint {Endpoint}",
                    userId, sub.Endpoint[..Math.Min(sub.Endpoint.Length, 35)]);
            }
        }

        if (expiredSubscriptions is { Count: > 0 })
        {
            foreach (var expired in expiredSubscriptions)
            {
                await subscriptionRepository.RemoveAsync(expired, ct);
            }

            await unitOfWork.SaveChangesAsync(ct);
        }
    }
}
