using Microsoft.AspNetCore.Routing;

namespace VertexCommerce.Api.Endpoints;

public static class HealthEndpoints
{
    public static void MapHealthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapHealthChecks("/healthz")
            .WithMetadata(new HttpMethodMetadata(["GET", "HEAD"]))
            .AllowAnonymous()
            .WithTags("Health")
            .WithName("HealthCheck")
            .WithSummary("Health check endpoint supporting GET and HEAD");
    }
}
