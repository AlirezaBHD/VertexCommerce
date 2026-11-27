using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using VertexCommerce.Modules.Notifications.Services.Delivery;
using WebPush;
using PushSubscription = VertexCommerce.Modules.Notifications.Domain.Entities.PushSubscription;

namespace VertexCommerce.Modules.Notifications.Services;

internal sealed class WebPushSender(
    IOptions<WebPushOptions> options, IHttpClientFactory httpClientFactory, TimeProvider clock) : IWebPushSender
{
    internal const string HttpClientName = "Notifications.WebPush";

    [SuppressMessage("Design", "CA1031:Do not catch general exception types",
        Justification = "Unexpected provider errors are retried without logging exceptions that can contain subscription credentials.")]
    public async Task<WebPushSendResult> SendAsync(
        Guid deliveryId, string payload, DateTimeOffset expiresAt, PushSubscription subscription, CancellationToken ct)
    {
        if (!WebPushEndpointPolicy.IsAllowed(subscription.Endpoint))
        {
            return new WebPushSendResult(WebPushSendStatus.InvalidSubscription);
        }

        try
        {
            var settings = options.Value;
            var remainingLifetime = expiresAt - clock.GetUtcNow();
            var requestOptions = new Dictionary<string, object>(StringComparer.Ordinal)
            {
                ["vapidDetails"] = new VapidDetails(settings.Subject, settings.PublicKey, settings.PrivateKey),
                ["TTL"] = Math.Max(0, (int)Math.Floor(remainingLifetime.TotalSeconds))
            };

            using var webPushClient = new WebPushClient();
            using var httpClient = httpClientFactory.CreateClient(HttpClientName);
            var destination = new WebPush.PushSubscription(subscription.Endpoint, subscription.P256dhKey, subscription.AuthKey);
            using var request = webPushClient.GenerateRequestDetails(destination, payload, requestOptions);
            request.Headers.Add("Topic", deliveryId.ToString("N"));
            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
            return ClassifyResponse(response);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (HttpRequestException)
        {
            return new WebPushSendResult(WebPushSendStatus.Retryable);
        }
        catch (OperationCanceledException)
        {
            return new WebPushSendResult(WebPushSendStatus.Retryable);
        }
        catch (Exception ex) when (ex is ArgumentException or FormatException or CryptographicException)
        {
            return new WebPushSendResult(WebPushSendStatus.InvalidSubscription);
        }
        catch (Exception)
        {
            return new WebPushSendResult(WebPushSendStatus.Retryable);
        }
    }

    private WebPushSendResult ClassifyResponse(HttpResponseMessage response)
    {
        var status = (int)response.StatusCode;
        if (response.IsSuccessStatusCode)
        {
            return new WebPushSendResult(WebPushSendStatus.Accepted, status);
        }

        if (response.StatusCode is HttpStatusCode.Gone or HttpStatusCode.NotFound)
        {
            return new WebPushSendResult(WebPushSendStatus.SubscriptionExpired, status);
        }

        if (response.StatusCode is HttpStatusCode.RequestTimeout or HttpStatusCode.TooManyRequests || status is >= 500 and < 600)
        {
            var header = response.Headers.RetryAfter;
            var retryAfter = header?.Delta is { } delay ? clock.GetUtcNow() + delay : header?.Date;
            return new WebPushSendResult(WebPushSendStatus.Retryable, status, retryAfter);
        }

        return new WebPushSendResult(WebPushSendStatus.Rejected, status);
    }
}
