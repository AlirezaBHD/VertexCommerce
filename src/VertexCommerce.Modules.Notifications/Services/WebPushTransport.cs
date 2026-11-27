using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;

namespace VertexCommerce.Modules.Notifications.Services;

internal static class WebPushTransport
{
    private static readonly IPNetwork GlobalIpv6 = IPNetwork.Parse("2000::/3");
    private static readonly IPNetwork[] NonPublicNetworks =
    [
        IPNetwork.Parse("0.0.0.0/8"),
        IPNetwork.Parse("10.0.0.0/8"),
        IPNetwork.Parse("100.64.0.0/10"),
        IPNetwork.Parse("127.0.0.0/8"),
        IPNetwork.Parse("169.254.0.0/16"),
        IPNetwork.Parse("172.16.0.0/12"),
        IPNetwork.Parse("192.0.0.0/24"),
        IPNetwork.Parse("192.0.2.0/24"),
        IPNetwork.Parse("192.88.99.0/24"),
        IPNetwork.Parse("192.168.0.0/16"),
        IPNetwork.Parse("198.18.0.0/15"),
        IPNetwork.Parse("198.51.100.0/24"),
        IPNetwork.Parse("203.0.113.0/24"),
        IPNetwork.Parse("224.0.0.0/3"),
        IPNetwork.Parse("2001::/23"),
        IPNetwork.Parse("2001:db8::/32"),
        IPNetwork.Parse("2002::/16"),
        IPNetwork.Parse("3fff::/20")
    ];

    public static SocketsHttpHandler CreateHandler()
    {
        return new SocketsHttpHandler
        {
            AllowAutoRedirect = false,
            UseProxy = false,
            UseCookies = false,
            ActivityHeadersPropagator = null,
            ConnectCallback = ConnectAsync,
            ConnectTimeout = TimeSpan.FromSeconds(5),
            PooledConnectionLifetime = TimeSpan.FromMinutes(5)
        };
    }

    internal static bool IsPublicAddress(IPAddress address)
    {
        if (address.IsIPv4MappedToIPv6)
        {
            address = address.MapToIPv4();
        }

        return (address.AddressFamily is AddressFamily.InterNetwork || GlobalIpv6.Contains(address)) &&
            !NonPublicNetworks.Any(network => network.Contains(address));
    }

    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope",
        Justification = "The returned NetworkStream owns the socket. The failure path disposes it directly.")]
    private static async ValueTask<Stream> ConnectAsync(SocketsHttpConnectionContext context, CancellationToken ct)
    {
        var endpoint = context.DnsEndPoint;
        if (endpoint.Port != 443 || !WebPushEndpointPolicy.IsAllowedHost(endpoint.Host))
        {
            throw new HttpRequestException("Web Push destination is not allowed.");
        }

        var addresses = await Dns.GetHostAddressesAsync(endpoint.Host, ct);
        if (addresses.Length == 0 || addresses.Any(address => !IsPublicAddress(address)))
        {
            throw new HttpRequestException("Web Push destination did not resolve to public addresses.");
        }

        // Connect to the checked addresses while the HTTP handler still uses the original host for TLS.
        var socket = new Socket(SocketType.Stream, ProtocolType.Tcp) { NoDelay = true };
        try
        {
            await socket.ConnectAsync(addresses, endpoint.Port, ct);
            return new NetworkStream(socket, ownsSocket: true);
        }
        catch
        {
            socket.Dispose();
            throw;
        }
    }
}
