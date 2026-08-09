using System.Diagnostics;
using Serilog;
using VertexCommerce.Api.Endpoints;
using VertexCommerce.Api.Extensions;
using VertexCommerce.Api.Middleware;
using VertexCommerce.Shared.Behaviors;

var builder = WebApplication.CreateBuilder(args);

ConfigureHost(builder);
ConfigureServices(builder);

var app = builder.Build();

ConfigureMiddleware(app);
ConfigureEndpoints(app);


static void ConfigureHost(WebApplicationBuilder builder)
{
    builder.Host.UseSerilog((context, config) =>
        config.ReadFrom.Configuration(context.Configuration));
}

static void ConfigureServices(WebApplicationBuilder builder)
{
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails(options =>
    {
        options.CustomizeProblemDetails = ctx =>
        {
            ctx.ProblemDetails.Instance = ctx.HttpContext.Request.Path;
            ctx.ProblemDetails.Extensions["traceId"] = Activity.Current?.TraceId.ToString();
            ctx.ProblemDetails.Extensions["requestId"] = ctx.HttpContext.TraceIdentifier;
            ctx.ProblemDetails.Extensions.TryAdd("requestId", System.Diagnostics.Activity.Current?.Id);
        };
    });
    builder.Services.AddAntiforgery();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

    builder.Services.AddVertexCors(builder.Configuration);
    builder.Services.AddMongoDb(builder.Configuration);
    builder.Services.AddVertexOpenApi();
    builder.Services.AddVertexMedia(builder.Environment);

    var graphQlBuilder = builder.Services
        .AddGraphQLServer()
        .AddQueryType(d => d.Name("Query"))
        .AddMongoDbProjections()
        .AddMongoDbFiltering()
        .AddMongoDbPagingProviders()
        .AddMongoDbSorting()
        .ModifyCostOptions(options => { options.MaxFieldCost = 10000; });
    
    builder.Services.RegisterModules(builder.Configuration);
    graphQlBuilder.ConfigureModulesGraphQl();
}

static void ConfigureMiddleware(WebApplication app)
{
    app.UseExceptionHandler();
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseVertexDeveloperTools();
    }

    app.UseCors(CorsExtensions.PolicyName);
    app.UseStaticFiles();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseAntiforgery();
}

static void ConfigureEndpoints(WebApplication app)
{
    app.MapMediaEndpoints();
    app.MapGraphQL();
    app.MapModulesEndpoints();
}

await app.Services.InitializeModulesAsync();

app.Run();
