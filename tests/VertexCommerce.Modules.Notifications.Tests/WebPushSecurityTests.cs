using System.Net;
using Microsoft.Extensions.Options;
using NSubstitute;
using VertexCommerce.Modules.Notifications.Services;
using VertexCommerce.Modules.Notifications.Services.Delivery;
using Xunit;
using PushSubscription = VertexCommerce.Modules.Notifications.Domain.Entities.PushSubscription;

namespace VertexCommerce.Modules.Notifications.Tests;

public sealed class WebPushSecurityTests
{
    [Theory]
    [InlineData("https://fcm.googleapis.com/fcm/send/test", true)]
    [InlineData("https://updates.push.services.mozilla.com/wpush/v2/test", true)]
    [InlineData("https://web.push.apple.com/test", true)]
    [InlineData("https://wns1.notify.windows.com/test", true)]
    [InlineData("http://fcm.googleapis.com/test", false)]
    [InlineData("https://fcm.googleapis.com:444/test", false)]
    [InlineData("https://fcm.googleapis.com.untrusted.test/test", false)]
    [InlineData("https://unrelatedpush.apple.com/test", false)]
    [InlineData("https://user@fcm.googleapis.com/test", false)]
    [InlineData("https://fcm.googleapis.com/test#fragment", false)]
    [InlineData("https://127.0.0.1/test", false)]
    public void Endpoint_policy_allows_only_supported_https_destinations(string endpoint, bool allowed)
    {
        Assert.Equal(allowed, WebPushEndpointPolicy.IsAllowed(endpoint));
    }

    [Theory]
    [InlineData("8.8.8.8", true)]
    [InlineData("2607:f8b0::1", true)]
    [InlineData("127.0.0.1", false)]
    [InlineData("10.0.0.1", false)]
    [InlineData("100.64.0.1", false)]
    [InlineData("169.254.1.1", false)]
    [InlineData("192.168.1.1", false)]
    [InlineData("::1", false)]
    [InlineData("::ffff:127.0.0.1", false)]
    [InlineData("fc00::1", false)]
    [InlineData("fe80::1", false)]
    [InlineData("2001:db8::1", false)]
    public void Transport_connects_only_to_public_addresses(string address, bool allowed)
    {
        Assert.Equal(allowed, WebPushTransport.IsPublicAddress(IPAddress.Parse(address)));
    }

    [Fact]
    public void Transport_disables_redirects_and_proxy_lookup()
    {
        using var handler = WebPushTransport.CreateHandler();

        Assert.False(handler.AllowAutoRedirect);
        Assert.False(handler.UseProxy);
        Assert.NotNull(handler.ConnectCallback);
    }

    [Theory]
    [InlineData(201, "Accepted")]
    [InlineData(404, "SubscriptionExpired")]
    [InlineData(410, "SubscriptionExpired")]
    [InlineData(429, "Retryable")]
    [InlineData(500, "Retryable")]
    [InlineData(408, "Retryable")]
    [InlineData(403, "Rejected")]
    [InlineData(302, "Rejected")]
    public async Task Sender_classifies_provider_responses_and_sets_the_delivery_topic(int status, string expected)
    {
        using var handler = new RecordingHandler((HttpStatusCode)status);
        using var client = new HttpClient(handler, disposeHandler: false);
        var service = CreateService(client);
        var request = TestPushSubscription.Create();
        var subscription = PushSubscription.Create(Guid.NewGuid(), request.Endpoint, request.P256dh, request.Auth);
        var now = DateTimeOffset.UtcNow;
        var deliveryId = Guid.NewGuid();

        var result = await service.SendAsync(deliveryId, "{}", now.AddMinutes(5), subscription, CancellationToken.None);

        Assert.Equal(expected, result.Status.ToString());
        Assert.Equal(status, result.HttpStatus);
        Assert.Equal(request.Endpoint, handler.Destination);
        Assert.Equal(deliveryId.ToString("N"), handler.Topic);
        Assert.InRange(handler.Ttl, 1, 300);
        if (status == 429)
        {
            Assert.True(result.RetryAfter >= now.AddMinutes(2));
        }
    }

    [Fact]
    public async Task Sender_skips_unsupported_legacy_destinations()
    {
        using var handler = new RecordingHandler(HttpStatusCode.OK);
        using var client = new HttpClient(handler, disposeHandler: false);
        var service = CreateService(client);
        var request = TestPushSubscription.Create("https://untrusted.example.test/private-endpoint");
        var subscription = PushSubscription.Create(Guid.NewGuid(), request.Endpoint, request.P256dh, request.Auth);
        var now = DateTimeOffset.UtcNow;
        var deliveryId = Guid.NewGuid();

        var result = await service.SendAsync(deliveryId, "{}", now.AddMinutes(5), subscription, CancellationToken.None);

        Assert.Equal(WebPushSendStatus.InvalidSubscription, result.Status);
        Assert.Null(handler.Destination);
    }

    private static WebPushSender CreateService(HttpClient client)
    {
        var clientFactory = Substitute.For<IHttpClientFactory>();
        clientFactory.CreateClient(WebPushSender.HttpClientName).Returns(client);
        var vapid = new WebPushOptions { Subject = "mailto:test@example.test" };
        vapid.Initialize(isDevelopment: true);
        return new WebPushSender(Options.Create(vapid), clientFactory, TimeProvider.System);
    }

    private sealed class RecordingHandler(HttpStatusCode status) : HttpMessageHandler
    {
        public string? Destination { get; private set; }
        public string? Topic { get; private set; }
        public int Ttl { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Destination = request.RequestUri?.AbsoluteUri;
            Topic = request.Headers.GetValues("Topic").Single();
            Ttl = int.Parse(request.Headers.GetValues("TTL").Single(), System.Globalization.CultureInfo.InvariantCulture);
            var response = new HttpResponseMessage(status);
            if (status == HttpStatusCode.TooManyRequests)
            {
                response.Headers.RetryAfter = new System.Net.Http.Headers.RetryConditionHeaderValue(TimeSpan.FromMinutes(2));
            }

            return Task.FromResult(response);
        }
    }
}
