using HotChocolate.Execution.Configuration;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace VertexCommerce.Shared.Contracts;

public interface IModule
{
    string Name { get; }

    void RegisterServices(IServiceCollection services, IConfiguration configuration);

    void MapEndpoints(IEndpointRouteBuilder endpoints);

    void ConfigureGraphQl(IRequestExecutorBuilder builder);

    Task InitializeAsync(IServiceProvider serviceProvider, CancellationToken ct = default) => Task.CompletedTask;
}
