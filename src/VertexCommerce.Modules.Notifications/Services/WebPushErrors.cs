using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Notifications.Services;

internal static class WebPushErrors
{
    public static readonly Error Unavailable = new("Notifications.WebPushUnavailable", "Web Push is currently unavailable.");
}
