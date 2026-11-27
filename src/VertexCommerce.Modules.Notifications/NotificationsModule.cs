using FluentValidation;
using HotChocolate.Execution.Configuration;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VertexCommerce.Modules.Notifications.Domain.Repositories;
using VertexCommerce.Modules.Notifications.Endpoints;
using VertexCommerce.Modules.Notifications.EventHandlers;
using VertexCommerce.Modules.Notifications.Persistence;
using VertexCommerce.Modules.Notifications.Persistence.Repositories;
using VertexCommerce.Modules.Notifications.Services;
using VertexCommerce.Modules.Notifications.Services.Dispatching;
using VertexCommerce.Modules.Notifications.Services.Delivery;
using VertexCommerce.Shared.Contracts;

namespace VertexCommerce.Modules.Notifications;

public class NotificationsModule : IModule
{
    public string Name => "Notifications";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextFactory<NotificationsDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("NotificationsDb")
                    ?? configuration.GetConnectionString("IdentityDb"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "notifications")),
            ServiceLifetime.Scoped);

        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IOrderNotificationInbox, OrderNotificationInbox>();
        services.AddScoped<IPushSubscriptionRepository, PushSubscriptionRepository>();
        services.AddScoped<INotificationsUnitOfWork>(sp => sp.GetRequiredService<NotificationsDbContext>());

        services.AddSingleton<ISseConnectionManager, SseConnectionManager>();

        services.TryAddSingleton(TimeProvider.System);
        services.AddOptions<WebPushOptions>()
            .Bind(configuration.GetSection(WebPushOptions.SectionName))
            .PostConfigure<IServiceProvider>((options, services) =>
                options.Initialize(services.GetService<IHostEnvironment>()?.IsDevelopment() == true));
        services.AddHttpClient(WebPushSender.HttpClientName, client => client.Timeout = TimeSpan.FromSeconds(10))
            .ConfigurePrimaryHttpMessageHandler(WebPushTransport.CreateHandler)
            .RemoveAllLoggers();
        services.AddScoped<IWebPushSender, WebPushSender>();
        services.AddScoped<PushDeliveryQueue>();
        services.AddScoped<PushDeliveryProcessor>();
        services.AddHostedService<PushDeliveryWorker>();
        services.AddScoped<INotificationDispatcher, NotificationDispatcher>();
        services.AddScoped<OrderNotificationRecipientResolver>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(NotificationsModule).Assembly));

        services.AddValidatorsFromAssembly(typeof(NotificationsModule).Assembly);
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapNotificationEndpoints();
    }

    public void ConfigureGraphQl(IRequestExecutorBuilder builder)
    {
    }

    public async Task InitializeAsync(IServiceProvider serviceProvider, CancellationToken ct = default)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var webPush = scope.ServiceProvider.GetRequiredService<IOptions<WebPushOptions>>().Value;
        var db = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<NotificationsModule>>();

        try
        {
            await NotificationsDatabaseInitializer.ApplyMigrationsAsync(db, ct);
            logger.LogInformation("Notifications migrations applied successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to apply migrations for Notifications module.");
            throw new InvalidOperationException("Failed to apply migrations for Notifications module.", ex);
        }

        if (webPush.HasInvalidCredentials)
        {
            logger.LogWarning("Configured VAPID credentials are invalid. Set WebPush__Enabled=true with a valid subject and matching keys.");
        }

        if (webPush.UsesTemporaryKeys)
        {
            logger.LogInformation("Web Push is using temporary development keys.");
        }

        logger.LogInformation("Web Push delivery is {State}", webPush.IsEnabled ? "enabled" : "disabled");
    }
}
