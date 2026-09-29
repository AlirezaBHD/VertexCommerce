# Stage 1: Build & Publish
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy central package and build configuration files
COPY Directory.Build.props Directory.Packages.props ./

# Copy project files for layer-cached restore
COPY src/VertexCommerce.Shared/VertexCommerce.Shared.csproj src/VertexCommerce.Shared/
COPY src/VertexCommerce.Modules.Basket/VertexCommerce.Modules.Basket.csproj src/VertexCommerce.Modules.Basket/
COPY src/VertexCommerce.Modules.Catalog/VertexCommerce.Modules.Catalog.csproj src/VertexCommerce.Modules.Catalog/
COPY src/VertexCommerce.Modules.Customers/VertexCommerce.Modules.Customers.csproj src/VertexCommerce.Modules.Customers/
COPY src/VertexCommerce.Modules.Identity/VertexCommerce.Modules.Identity.csproj src/VertexCommerce.Modules.Identity/
COPY src/VertexCommerce.Modules.Notifications/VertexCommerce.Modules.Notifications.csproj src/VertexCommerce.Modules.Notifications/
COPY src/VertexCommerce.Modules.Orders/VertexCommerce.Modules.Orders.csproj src/VertexCommerce.Modules.Orders/
COPY src/VertexCommerce.Api/VertexCommerce.Api.csproj src/VertexCommerce.Api/

RUN dotnet restore src/VertexCommerce.Api/VertexCommerce.Api.csproj

# Copy the rest of the source code and publish
COPY src/ src/
RUN dotnet publish src/VertexCommerce.Api/VertexCommerce.Api.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false \
    --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Ensure wwwroot exists for static files / local media fallback
RUN mkdir -p /app/wwwroot

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "VertexCommerce.Api.dll"]
