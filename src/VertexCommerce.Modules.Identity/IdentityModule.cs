using System.Text;
using FluentValidation;
using HotChocolate.Execution.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using VertexCommerce.Modules.Identity.Domain.Repositories;
using VertexCommerce.Modules.Identity.Endpoints;
using VertexCommerce.Modules.Identity.Features.Commands.Registration.SendOpt;
using VertexCommerce.Modules.Identity.Persistence;
using VertexCommerce.Modules.Identity.Services;
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
}
