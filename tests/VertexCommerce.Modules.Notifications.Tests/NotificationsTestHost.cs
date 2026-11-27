using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using VertexCommerce.Modules.Identity;
using VertexCommerce.Modules.Notifications.Services;
using VertexCommerce.Shared.Behaviors;
using VertexCommerce.Shared.Exceptions;

namespace VertexCommerce.Modules.Notifications.Tests;

internal sealed class NotificationsTestHost(
    WebApplication app,
    NotificationTestDependencies dependencies,
    SymmetricSecurityKey signingKey) : IAsyncDisposable
{
    private const string Issuer = "notifications-contract-tests";
    private const string Audience = "notifications-contract-client";

    private readonly SocketsHttpHandler _handler = new() { MaxResponseDrainSize = 0 };

    public NotificationTestDependencies Dependencies => dependencies;
    public IServiceProvider Services => app.Services;
    public string VapidPublicKey => app.Services.GetRequiredService<IOptions<WebPushOptions>>().Value.PublicKey;
    public ISseConnectionManager Connections => app.Services.GetRequiredService<ISseConnectionManager>();

    public static async Task<NotificationsTestHost> StartAsync(
        CancellationToken cancellationToken, ISseConnectionManager? connections = null, string? postgresConnection = null,
        bool webPushEnabled = true, IWebPushSender? hostedPushSender = null)
    {
        var keys = new WebPushOptions();
        keys.Initialize(isDevelopment: true);
        var signingSecret = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        var builder = WebApplication.CreateSlimBuilder();
        builder.Configuration.Sources.Clear();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Jwt:Secret"] = signingSecret,
            ["Jwt:Issuer"] = Issuer,
            ["Jwt:Audience"] = Audience,
            ["ConnectionStrings:NotificationsDb"] = postgresConnection,
            ["WebPush:Enabled"] = webPushEnabled ? "true" : "false",
            ["WebPush:Subject"] = "mailto:test@example.invalid",
            ["WebPush:PublicKey"] = keys.PublicKey,
            ["WebPush:PrivateKey"] = keys.PrivateKey
        });
        builder.Logging.ClearProviders();
        builder.WebHost.ConfigureKestrel(options => options.Listen(IPAddress.Loopback, 0));

        var notifications = new NotificationsModule();
        new IdentityModule().RegisterServices(builder.Services, builder.Configuration);
        // Identity's worker is unrelated to notification tests.
        builder.Services.RemoveAll<IHostedService>();
        notifications.RegisterServices(builder.Services, builder.Configuration);
        if (hostedPushSender is null)
        {
            builder.Services.RemoveAll<IHostedService>();
        }
        else
        {
            builder.Services.AddSingleton(hostedPushSender);
        }
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        var dependencies = new NotificationTestDependencies();
        dependencies.Register(builder.Services, usePostgres: postgresConnection is not null);
        if (connections is not null)
        {
            builder.Services.AddSingleton(connections);
        }

        var app = builder.Build();
        app.UseExceptionHandler(handler => handler.Run(async context =>
        {
            if (context.Features.Get<IExceptionHandlerFeature>()?.Error is ValidationException validation)
            {
                await Results.ValidationProblem(new Dictionary<string, string[]>(validation.Errors)).ExecuteAsync(context);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
        }));
        app.UseAuthentication();
        app.UseAuthorization();
        notifications.MapEndpoints(app);

        try
        {
            if (postgresConnection is not null)
            {
                await notifications.InitializeAsync(app.Services, cancellationToken);
            }

            await app.StartAsync(cancellationToken);
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingSecret));
            return new NotificationsTestHost(app, dependencies, signingKey);
        }
        catch
        {
            await app.DisposeAsync();
            throw;
        }
    }

    public HttpClient CreateClient()
    {
        return new HttpClient(_handler, disposeHandler: false)
        {
            BaseAddress = new Uri(app.Urls.Single())
        };
    }

    public string CreateToken(Guid userId)
    {
        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: [new Claim(JwtRegisteredClaimNames.Sub, userId.ToString())],
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task PublishAsync(INotification notification, CancellationToken cancellationToken)
    {
        await using var scope = app.Services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<IPublisher>().Publish(notification, cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        _handler.Dispose();
        return app.DisposeAsync();
    }
}
