using VertexCommerce.Api.Options;

namespace VertexCommerce.Api.Extensions;

public static class CorsExtensions
{
    public const string PolicyName = "VertexCorsPolicy";

    public static IServiceCollection AddVertexCors(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var settings = configuration
            .GetSection(CorsSettings.SectionName)
            .Get<CorsSettings>()
            ?? throw new InvalidOperationException("Cors settings are missing in configuration.");

        services.AddCors(options =>
        {
            options.AddPolicy(PolicyName, policy =>
            {
                if (settings.AllowedOrigins.Contains("*"))
                    policy.AllowAnyOrigin();
                else
                    policy.WithOrigins(settings.AllowedOrigins)
                          .SetIsOriginAllowedToAllowWildcardSubdomains();

                if (settings.AllowedMethods.Contains("*"))
                    policy.AllowAnyMethod();
                else
                    policy.WithMethods(settings.AllowedMethods);

                if (settings.AllowedHeaders.Contains("*"))
                    policy.AllowAnyHeader();
                else
                    policy.WithHeaders(settings.AllowedHeaders);

                if (settings.ExposedHeaders.Length > 0)
                    policy.WithExposedHeaders(settings.ExposedHeaders);

                if (settings.AllowCredentials && !settings.AllowedOrigins.Contains("*"))
                    policy.AllowCredentials();
                else
                    policy.DisallowCredentials();

                policy.SetPreflightMaxAge(TimeSpan.FromSeconds(settings.MaxAgeSeconds));
            });
        });

        return services;
    }
}
