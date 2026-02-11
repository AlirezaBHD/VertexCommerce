using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Serilog;
using VertexCommerce.Api.Endpoints;
using VertexCommerce.Api.GraphQL;
using VertexCommerce.Api.Middleware;
using VertexCommerce.Modules.Basket;
using VertexCommerce.Modules.Catalog;
using VertexCommerce.Modules.Customers;
using VertexCommerce.Modules.Identity;
using VertexCommerce.Modules.Orders;
using VertexCommerce.Shared.Behaviors;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    // ...

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

builder.Services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddBasketModule(builder.Configuration);
builder.Services.AddCatalogModule(builder.Configuration);
builder.Services.AddCustomersModule(builder.Configuration);
builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddOrdersModule(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi("v1", options =>
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


builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddFiltering()
    .AddSorting()
    .AddProjections();



var app = builder.Build();

app.UseExceptionHandler();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapScalarApiReference();
}
app.UseAuthentication();
app.UseAuthorization();

app.MapBasketEndpoints();
app.MapCatalogEndpoints();
app.MapCustomerEndpoints();
app.MapIdentityEndpoints();
app.MapCheckoutEndpoints();
app.MapOrdersEndpoints();

app.MapGraphQL();

app.Run();
