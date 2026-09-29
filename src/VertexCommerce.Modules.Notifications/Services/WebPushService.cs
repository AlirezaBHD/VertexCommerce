using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebPush;
using VertexCommerce.Modules.Notifications.Domain.Repositories;
using VertexCommerce.Modules.Notifications.Persistence;

namespace VertexCommerce.Modules.Notifications.Services;

internal sealed class WebPushService : IWebPushService
{
    private readonly WebPushOptions _options;
    private readonly IPushSubscriptionRepository _subscriptionRepository;
    private readonly INotificationsUnitOfWork _unitOfWork;
    private readonly ILogger<WebPushService> _logger;

    public WebPushService(
        IOptions<WebPushOptions> options,
        IPushSubscriptionRepository subscriptionRepository,
        INotificationsUnitOfWork unitOfWork,
        ILogger<WebPushService> logger)
    {
        _options = options.Value;
        _subscriptionRepository = subscriptionRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public string PublicKey => _options.PublicKey;

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
            _logger.LogWarning("VAPID keys are not configured. Web push notification skipped for user {UserId}", userId);
            return;
        }

        var subscriptions = await _subscriptionRepository.GetByUserIdAsync(userId, ct);
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
        var client = new WebPushClient();

        foreach (var sub in subscriptions)
        {
            try
            {
                var pushSub = new WebPush.PushSubscription(sub.Endpoint, sub.P256dhKey, sub.AuthKey);
                await client.SendNotificationAsync(pushSub, payload, vapidDetails, ct);
                _logger.LogInformation("Push notification sent successfully to endpoint {Endpoint} for user {UserId}",
                    sub.Endpoint[..Math.Min(sub.Endpoint.Length, 35)], userId);
            }
            catch (WebPushException ex) when (ex.StatusCode is HttpStatusCode.Gone or HttpStatusCode.NotFound)
            {
                _logger.LogInformation("Push subscription has expired or unsubscribed (status {Status}). Removing endpoint {Endpoint}",
                    ex.StatusCode, sub.Endpoint[..Math.Min(sub.Endpoint.Length, 35)]);

                await _subscriptionRepository.RemoveAsync(sub, ct);
                await _unitOfWork.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send WebPush notification to user {UserId} endpoint {Endpoint}",
                    userId, sub.Endpoint[..Math.Min(sub.Endpoint.Length, 35)]);
            }
        }
    }
}
