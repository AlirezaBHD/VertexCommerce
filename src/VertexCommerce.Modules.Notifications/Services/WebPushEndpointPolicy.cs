namespace VertexCommerce.Modules.Notifications.Services;

internal static class WebPushEndpointPolicy
{
    public static bool IsAllowed(string endpoint)
    {
        return Uri.TryCreate(endpoint.Trim(), UriKind.Absolute, out var uri) &&
            uri.IsWellFormedOriginalString() &&
            string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.Ordinal) &&
            uri.Port == 443 &&
            uri.UserInfo.Length == 0 &&
            uri.Fragment.Length == 0 &&
            uri.HostNameType == UriHostNameType.Dns &&
            IsAllowedHost(uri.IdnHost);
    }

    public static bool IsAllowedHost(string host)
    {
        return string.Equals(host, "fcm.googleapis.com", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(host, "android.googleapis.com", StringComparison.OrdinalIgnoreCase) ||
            host.EndsWith(".push.services.mozilla.com", StringComparison.OrdinalIgnoreCase) ||
            host.EndsWith(".push.apple.com", StringComparison.OrdinalIgnoreCase) ||
            host.EndsWith(".notify.windows.com", StringComparison.OrdinalIgnoreCase);
    }
}
