using Microsoft.OpenApi;

namespace VertexCommerce.Api.Extensions;

public static class OpenApiExtensions
{
    public static IServiceCollection AddVertexOpenApi(this IServiceCollection services)
    {
        
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "JWT Authorization header using the Bearer scheme."
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("bearer", document)] = []
            });
        });
        
        services.AddOpenApi("v1", options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Components ??= new OpenApiComponents();

                document.Components.SecuritySchemes?.Add("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your token"
                });

                document.Security ??= new List<OpenApiSecurityRequirement>();

                return Task.CompletedTask;
            });
        });

        return services;
    }

    public static void UseVertexDeveloperTools(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
}
