using FluentValidation;
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
using VertexCommerce.Modules.Notifications.Services.Dispatching;
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
        services.AddScoped<INotificationDispatcher, NotificationDispatcher>();

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
        var db = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<NotificationsModule>>();

        try
        {
            const string migrationFixSql =
                "DO $$\n" +
                "BEGIN\n" +
                "    IF EXISTS (\n" +
                "        SELECT 1 FROM information_schema.tables\n" +
                "        WHERE table_schema = 'notifications' AND table_name = '__EFMigrationsHistory'\n" +
                "    ) THEN\n" +
                "        UPDATE notifications.\"__EFMigrationsHistory\"\n" +
                "        SET \"MigrationId\" = '20260920090000_InitialNotifications'\n" +
                "        WHERE \"MigrationId\" = '20260920120000_InitialNotifications';\n" +
                "    END IF;\n" +
                "END $$;";
            await db.Database.ExecuteSqlRawAsync(migrationFixSql, ct);
            await db.Database.MigrateAsync(ct);
            logger.LogInformation("Notifications migrations applied successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to apply migrations for Notifications module.");
            throw new InvalidOperationException("Failed to apply migrations for Notifications module.", ex);
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
