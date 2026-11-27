using System.Buffers.Text;
using System.Security.Cryptography;
using VertexCommerce.Modules.Notifications.Endpoints;

namespace VertexCommerce.Modules.Notifications.Tests;

internal static class TestPushSubscription
{
    public const string Endpoint = "https://fcm.googleapis.com/fcm/send/test-subscription";

    public static PushSubscribeRequest Create(string endpoint = Endpoint, string? userAgent = "test-agent")
    {
        using var key = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256);
        var point = key.ExportParameters(includePrivateParameters: false).Q;
        Span<byte> publicKey = stackalloc byte[65];
        publicKey[0] = 4;
        point.X.AsSpan().CopyTo(publicKey[1..33]);
        point.Y.AsSpan().CopyTo(publicKey[33..]);

        return new PushSubscribeRequest(
            endpoint, Base64Url.EncodeToString(publicKey), Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(16)),
            userAgent);
    }
}
