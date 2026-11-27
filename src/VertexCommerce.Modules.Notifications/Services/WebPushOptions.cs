using System.Buffers.Text;
using System.Security.Cryptography;
using WebPush;

namespace VertexCommerce.Modules.Notifications.Services;

public sealed class WebPushOptions
{
    public const string SectionName = "WebPush";

    public bool? Enabled { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string PublicKey { get; set; } = string.Empty;
    public string PrivateKey { get; set; } = string.Empty;

    internal bool IsEnabled => Enabled == true;
    internal bool HasInvalidCredentials { get; private set; }
    internal bool UsesTemporaryKeys { get; private set; }

    internal void Initialize(bool isDevelopment)
    {
        if (Enabled == false)
        {
            return;
        }

        var hasConfiguredKeys = !string.IsNullOrWhiteSpace(PublicKey) || !string.IsNullOrWhiteSpace(PrivateKey);
        if (isDevelopment && string.IsNullOrWhiteSpace(Subject))
        {
            Subject = "mailto:development@example.invalid";
        }

        if (hasConfiguredKeys && HasValidCredentials())
        {
            Enabled = true;
            return;
        }

        HasInvalidCredentials = hasConfiguredKeys;
        if (Enabled == true && (!isDevelopment || hasConfiguredKeys))
        {
            throw new InvalidOperationException("Web Push requires a valid VAPID subject and matching public and private keys.");
        }

        if (isDevelopment)
        {
            Subject = "mailto:development@example.invalid";
            GenerateDevelopmentKeys();
            UsesTemporaryKeys = true;
            Enabled = true;
            return;
        }

        Enabled = false;
    }

    private void GenerateDevelopmentKeys()
    {
        using var key = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var generated = key.ExportParameters(includePrivateParameters: true);
        var secret = generated.D ?? throw new CryptographicException("Could not generate a VAPID key.");
        try
        {
            Span<byte> publicKey = stackalloc byte[65];
            publicKey[0] = 4;
            generated.Q.X.AsSpan().CopyTo(publicKey[1..33]);
            generated.Q.Y.AsSpan().CopyTo(publicKey[33..]);
            PublicKey = Base64Url.EncodeToString(publicKey);
            PrivateKey = Base64Url.EncodeToString(secret);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(secret);
        }
    }

    private bool HasValidCredentials()
    {
        try
        {
            VapidHelper.ValidateSubject(Subject);
            VapidHelper.ValidatePublicKey(PublicKey);
            VapidHelper.ValidatePrivateKey(PrivateKey);
            return IsKeyPairValid();
        }
        catch (Exception ex) when (ex is ArgumentException or FormatException or CryptographicException)
        {
            // The library may include configured keys in an exception message.
            return false;
        }
    }

    private bool IsKeyPairValid()
    {
        var publicKey = Base64Url.DecodeFromChars(PublicKey);
        var privateKey = Base64Url.DecodeFromChars(PrivateKey);
        try
        {
            if (publicKey.Length != 65 || publicKey[0] != 4 || privateKey.Length != 32)
            {
                return false;
            }

            var parameters = new ECParameters
            {
                Curve = ECCurve.NamedCurves.nistP256,
                Q = new ECPoint { X = publicKey[1..33], Y = publicKey[33..65] }
            };
            using var verifier = ECDsa.Create(parameters);
            parameters.D = privateKey;
            using var signer = ECDsa.Create(parameters);
            var message = "VertexCommerce Web Push"u8;
            var signature = signer.SignData(message, HashAlgorithmName.SHA256);
            return verifier.VerifyData(message, signature, HashAlgorithmName.SHA256);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(privateKey);
        }
    }
}
