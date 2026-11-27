using VertexCommerce.Modules.Notifications.Services;
using Xunit;

namespace VertexCommerce.Modules.Notifications.Tests;

public sealed class WebPushOptionsTests
{
    [Fact]
    public void Production_without_keys_disables_push()
    {
        var options = new WebPushOptions();

        options.Initialize(isDevelopment: false);

        Assert.False(options.IsEnabled);
        Assert.Empty(options.PublicKey);
    }

    [Fact]
    public void Production_requires_valid_matching_keys_when_push_is_enabled()
    {
        var first = new WebPushOptions();
        first.Initialize(isDevelopment: true);
        var second = new WebPushOptions();
        second.Initialize(isDevelopment: true);
        var options = new WebPushOptions
        {
            Enabled = true,
            Subject = "mailto:test@example.test",
            PublicKey = first.PublicKey,
            PrivateKey = second.PrivateKey
        };

        Assert.Throws<InvalidOperationException>(() => options.Initialize(isDevelopment: false));
        options.PrivateKey = first.PrivateKey;
        options.Initialize(isDevelopment: false);
        Assert.True(options.IsEnabled);
    }

    [Fact]
    public void Invalid_implicit_credentials_do_not_prevent_production_startup()
    {
        var options = new WebPushOptions { PublicKey = "invalid", PrivateKey = "invalid" };

        options.Initialize(isDevelopment: false);

        Assert.False(options.IsEnabled);
        Assert.True(options.HasInvalidCredentials);
        Assert.False(options.UsesTemporaryKeys);
    }

    [Fact]
    public void Development_can_generate_temporary_keys()
    {
        var options = new WebPushOptions();

        options.Initialize(isDevelopment: true);

        Assert.True(options.IsEnabled);
        Assert.True(options.UsesTemporaryKeys);
        Assert.NotEmpty(options.PublicKey);
        Assert.NotEmpty(options.PrivateKey);
    }
}
