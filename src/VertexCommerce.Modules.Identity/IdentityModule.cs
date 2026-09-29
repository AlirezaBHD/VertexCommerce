using System.Text;
using FluentValidation;
using HotChocolate.Execution.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using VertexCommerce.Modules.Identity.Domain.Repositories;
using VertexCommerce.Modules.Identity.Endpoints;
using VertexCommerce.Modules.Identity.Persistence;
using VertexCommerce.Modules.Identity.Infrastructure.Authentication;
using VertexCommerce.Modules.Identity.Infrastructure.Cryptography;
using VertexCommerce.Modules.Identity.Infrastructure.Identity;
using VertexCommerce.Modules.Identity.Persistence.Repositories;
using VertexCommerce.Shared.Contracts;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.Persistence.Outbox;
using VertexCommerce.Shared.Persistence.Outbox.Extensions;

namespace VertexCommerce.Modules.Identity;

public class IdentityModule : IModule
{
    public string Name => "Identity";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();
        
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();

        services.AddScoped<IOtpService, OtpService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IIdentityUnitOfWork>(sp => sp.GetRequiredService<IdentityDbContext>());

        services.AddOutbox<IdentityDbContext>(opts =>
        {
            opts.ModuleName = Name;
            opts.BatchSize = 25;
            opts.MaxRetryCount = 5;
            opts.PollingInterval = TimeSpan.FromSeconds(30);
        });

        services.AddDbContext<IdentityDbContext>((sp, options) =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("IdentityDb"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "identity"));

            options.AddInterceptors(sp.GetRequiredService<OutboxInterceptor>());
        });

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtService, JwtService>();

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()!;

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/api/notifications/stream"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AppRoles.Admin, policy =>
                policy.RequireRole(AppRoles.Admin));
        });

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(IdentityModule).Assembly));

        services.AddValidatorsFromAssembly(typeof(IdentityModule).Assembly);
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapIdentityEndpoints();
    }

    public void ConfigureGraphQl(IRequestExecutorBuilder builder)
    {
    }

    public async Task InitializeAsync(IServiceProvider serviceProvider, CancellationToken ct = default)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<IdentityModule>>();

        try
        {
            logger.LogInformation("Applying migrations for Identity module...");
            await db.Database.MigrateAsync(ct);
            logger.LogInformation("Identity migrations applied successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to apply migrations for Identity module.");
            throw;
        }
    }
}
