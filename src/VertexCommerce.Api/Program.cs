using Scalar.AspNetCore;
using Serilog;
using VertexCommerce.Api.Endpoints;
using VertexCommerce.Api.Middleware;
using VertexCommerce.Modules.Basket;
using VertexCommerce.Modules.Catalog;
using VertexCommerce.Modules.Orders;
using VertexCommerce.Shared.Behaviors;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddCatalogModule(builder.Configuration);
builder.Services.AddOrdersModule(builder.Configuration);
builder.Services.AddBasketModule(builder.Configuration);


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

app.MapCatalogEndpoints();
app.MapOrdersEndpoints();
app.MapBasketEndpoints();

app.Run();
