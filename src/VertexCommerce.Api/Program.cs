using Serilog;
using VertexCommerce.Api.Endpoints;
using VertexCommerce.Api.Extensions;
using VertexCommerce.Api.Middleware;
using VertexCommerce.Modules.Basket;
using VertexCommerce.Modules.Catalog;
using VertexCommerce.Modules.Customers;
using VertexCommerce.Modules.Identity;
using VertexCommerce.Modules.Orders;
using VertexCommerce.Shared.Behaviors;

var builder = WebApplication.CreateBuilder(args);

ConfigureHost(builder);
ConfigureServices(builder);

var app = builder.Build();

ConfigureMiddleware(app);
ConfigureEndpoints(app);

await InitializeApplicationAsync(app);

app.Run();


// =======================
// Host
// =======================

static void ConfigureHost(WebApplicationBuilder builder)
{
    builder.Host.UseSerilog((context, config) =>
        config.ReadFrom.Configuration(context.Configuration));
}


// =======================
// Services
// =======================

static void ConfigureServices(WebApplicationBuilder builder)
{
    var services = builder.Services;
    var configuration = builder.Configuration;

    // Core
    services.AddExceptionHandler<GlobalExceptionHandler>();
    services.AddProblemDetails();
    services.AddAntiforgery();
    services.AddEndpointsApiExplorer();

    // Infrastructure
    services.AddVertexCors(configuration);
    services.AddMongoDb(configuration);

    // OpenApi / Swagger
    services.AddVertexOpenApi();

    // MediatR Pipeline
    services.AddTransient(typeof(MediatR.IPipelineBehavior<,>),
        typeof(ValidationBehavior<,>));

    // Modules
    services.AddBasketModule(configuration);
    services.AddCatalogModule(configuration);
    services.AddCustomersModule(configuration);
    services.AddIdentityModule(configuration);
    services.AddOrdersModule(configuration);

    // GraphQL
    services.AddVertexGraphQL();

    // Media
    services.AddVertexMedia(builder.Environment);
}


// =======================
// Middleware
// =======================

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


// =======================
// Endpoints
// =======================

static void ConfigureEndpoints(WebApplication app)
{
    app.MapBasketEndpoints();
    app.MapCatalogEndpoints();
    app.MapCustomerEndpoints();
    app.MapIdentityEndpoints();
    app.MapCheckoutEndpoints();
    app.MapOrdersEndpoints();
    app.MapMediaEndpoints();

    app.MapGraphQL();
}


// =======================
// Initialization
// =======================

static async Task InitializeApplicationAsync(WebApplication app)
{
    await app.Services.InitializeMongoDbAsync();

    if (app.Environment.IsDevelopment())
    {
        app.ApplyDatabaseMigrations();
    }
}
