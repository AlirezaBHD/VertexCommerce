namespace VertexCommerce.Modules.Notifications.Domain.Entities;

internal static class PushSubscriptionLimits
{
    public const int Endpoint = 2048;
    public const int P256dhKey = 256;
    public const int AuthKey = 128;
    public const int UserAgent = 512;
}
