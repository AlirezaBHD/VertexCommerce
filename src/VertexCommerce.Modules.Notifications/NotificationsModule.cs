using HotChocolate.Execution.Configuration;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebPush;
using VertexCommerce.Modules.Notifications.Domain.Repositories;
using VertexCommerce.Modules.Notifications.Endpoints;
using VertexCommerce.Modules.Notifications.Persistence;
using VertexCommerce.Modules.Notifications.Persistence.Repositories;
using VertexCommerce.Modules.Notifications.Services;
using VertexCommerce.Shared.Contracts;

namespace VertexCommerce.Modules.Notifications;

public class NotificationsModule : IModule
{
    public string Name => "Notifications";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<NotificationsDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("NotificationsDb")
                    ?? configuration.GetConnectionString("IdentityDb"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "notifications")));

        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IPushSubscriptionRepository, PushSubscriptionRepository>();
        services.AddScoped<INotificationsUnitOfWork>(sp => sp.GetRequiredService<NotificationsDbContext>());

        services.AddSingleton<ISseConnectionManager, SseConnectionManager>();

        services.Configure<WebPushOptions>(configuration.GetSection(WebPushOptions.SectionName));
        services.AddScoped<IWebPushService, WebPushService>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(NotificationsModule).Assembly));
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
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<NotificationsModule>>();

        try
        {
            logger.LogInformation("Applying migrations for Notifications module...");
            await db.Database.ExecuteSqlRawAsync("""
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.tables
                        WHERE table_schema = 'notifications' AND table_name = '__EFMigrationsHistory'
                    ) THEN
                        UPDATE notifications."__EFMigrationsHistory"
                        SET "MigrationId" = '20260920090000_InitialNotifications'
                        WHERE "MigrationId" = '20260920120000_InitialNotifications';
                    END IF;
                END $$;
                """, ct);
            await db.Database.MigrateAsync(ct);
            logger.LogInformation("Notifications migrations applied successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to apply migrations for Notifications module.");
            throw;
        }

        var options = scope.ServiceProvider.GetRequiredService<IOptions<WebPushOptions>>().Value;

        if (string.IsNullOrWhiteSpace(options.PublicKey) || string.IsNullOrWhiteSpace(options.PrivateKey))
        {
            logger.LogWarning("WebPush VAPID keys are not configured in appsettings.json. Generating temporary keys for session...");
            var keys = VapidHelper.GenerateVapidKeys();
            options.PublicKey = keys.PublicKey;
            options.PrivateKey = keys.PrivateKey;
            logger.LogInformation("Generated VAPID PublicKey: {PublicKey}", keys.PublicKey);
        }
    }
}
