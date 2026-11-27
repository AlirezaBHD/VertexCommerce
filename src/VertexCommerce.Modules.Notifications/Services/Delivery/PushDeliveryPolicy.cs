namespace VertexCommerce.Modules.Notifications.Services.Delivery;

internal static class PushDeliveryPolicy
{
    public static readonly TimeSpan JobLifetime = TimeSpan.FromHours(24);
    public static readonly TimeSpan LeaseDuration = TimeSpan.FromMinutes(1);

    private static readonly TimeSpan[] RetryDelays =
    [
        TimeSpan.FromSeconds(15),
        TimeSpan.FromMinutes(1),
        TimeSpan.FromMinutes(5),
        TimeSpan.FromMinutes(30)
    ];

    public static DateTimeOffset? NextRetry(int attempts, DateTimeOffset now, DateTimeOffset? retryAfter)
    {
        if (attempts > RetryDelays.Length)
        {
            return null;
        }

        var next = now + RetryDelays[attempts - 1];
        return retryAfter > next ? retryAfter : next;
    }
}
