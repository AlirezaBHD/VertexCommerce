using Microsoft.Extensions.Logging;
using VertexCommerce.Modules.Notifications.Domain.Entities;
using VertexCommerce.Modules.Notifications.Domain.Repositories;
using VertexCommerce.Modules.Notifications.Persistence;
using VertexCommerce.Modules.Notifications.Persistence.Repositories;

namespace VertexCommerce.Modules.Notifications.Services.Delivery;

internal sealed class PushDeliveryProcessor(
    PushDeliveryQueue queue, IPushSubscriptionRepository subscriptions, IWebPushSender sender,
    TimeProvider clock, ILogger<PushDeliveryProcessor> logger)
{
    public async Task<bool> ProcessNextAsync(CancellationToken ct)
    {
        var delivery = await queue.ClaimAsync(clock.GetUtcNow(), PushDeliveryPolicy.LeaseDuration, ct);
        if (delivery is null)
        {
            return false;
        }

        await ProcessAsync(delivery, ct);
        return true;
    }

    private async Task ProcessAsync(PushDelivery delivery, CancellationToken ct)
    {
        var now = clock.GetUtcNow();
        if (delivery.ExpiresAt <= now)
        {
            await SetResultAsync(delivery, delivery.Attempts, delivery.LastHttpStatus, now, PushDeliveryOutcome.Expired, ct);
            return;
        }

        var subscription = await subscriptions.GetByIdAsync(delivery.SubscriptionId, ct);
        if (subscription is null)
        {
            await SetResultAsync(delivery, delivery.Attempts, delivery.LastHttpStatus, now, PushDeliveryOutcome.SubscriptionRemoved, ct);
            return;
        }

        var result = await sender.SendAsync(delivery.Id, delivery.Payload, delivery.ExpiresAt, subscription, ct);
        now = clock.GetUtcNow();
        var attempts = delivery.Attempts + 1;
        var expiredSubscription = result.Status == WebPushSendStatus.SubscriptionExpired ? subscription : null;
        if (result.Status == WebPushSendStatus.Retryable)
        {
            var retryAt = PushDeliveryPolicy.NextRetry(attempts, now, result.RetryAfter);
            if (retryAt is { } next && next < delivery.ExpiresAt)
            {
                await SetResultAsync(delivery, attempts, result.HttpStatus, next, null, ct);
                return;
            }

            var reason = retryAt is null ? PushDeliveryOutcome.AttemptsExhausted : PushDeliveryOutcome.Expired;
            await SetResultAsync(delivery, attempts, result.HttpStatus, now, reason, ct);
            return;
        }

        var outcome = result.Status switch
        {
            WebPushSendStatus.Accepted => PushDeliveryOutcome.Accepted,
            WebPushSendStatus.SubscriptionExpired => PushDeliveryOutcome.SubscriptionExpired,
            WebPushSendStatus.InvalidSubscription => PushDeliveryOutcome.InvalidSubscription,
            _ => PushDeliveryOutcome.Rejected
        };

        await SetResultAsync(delivery, attempts, result.HttpStatus, now, outcome, ct, expiredSubscription);
    }

    private async Task SetResultAsync(
        PushDelivery delivery, int attempts, int? httpStatus, DateTimeOffset nextAttemptAt,
        PushDeliveryOutcome? outcome, CancellationToken ct, PushSubscription? expiredSubscription = null)
    {
        var updated = await queue.SetResultAsync(
            delivery, attempts, httpStatus, nextAttemptAt, outcome, clock.GetUtcNow(), expiredSubscription, ct);

        if (!updated)
        {
            logger.LogDebug("The lease changed while processing Web Push delivery {DeliveryId}", delivery.Id);
        }
        else if (outcome is PushDeliveryOutcome.Accepted or PushDeliveryOutcome.SubscriptionExpired or PushDeliveryOutcome.SubscriptionRemoved)
        {
            logger.LogInformation("Web Push delivery {DeliveryId} ended as {Outcome} with status {Status}", delivery.Id, outcome, httpStatus);
        }
        else if (outcome.HasValue)
        {
            logger.LogWarning("Web Push delivery {DeliveryId} ended as {Outcome} after {Attempts} attempts with status {Status}",
                delivery.Id, outcome, attempts, httpStatus);
        }
        else
        {
            logger.LogInformation("Web Push delivery {DeliveryId} will retry at {NextAttemptAt} after status {Status}",
                delivery.Id, nextAttemptAt, httpStatus);
        }
    }
}
