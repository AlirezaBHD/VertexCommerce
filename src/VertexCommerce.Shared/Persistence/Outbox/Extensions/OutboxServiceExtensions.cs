using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace VertexCommerce.Shared.Persistence.Outbox.Extensions;

public static class OutboxServiceExtensions
{
    public static IServiceCollection AddOutbox<TDbContext>(
        this IServiceCollection services,
        Action<OutboxProcessorOptions>? configure = null)
        where TDbContext : DbContext, IOutboxDbContext
    {
        var options = new OutboxProcessorOptions();
        configure?.Invoke(options);

        var signal = new OutboxSignal();

        services.AddSingleton(signal);
        services.AddSingleton<OutboxInterceptor>(sp => new OutboxInterceptor(signal));
        services.AddSingleton(options);
        services.AddHostedService<OutboxProcessor<TDbContext>>();

        return services;
    }
}
