using VertexCommerce.Shared.Services;
using Path = System.IO.Path;

namespace VertexCommerce.Api.Extensions;

public static class MediaExtensions
{
    public static IServiceCollection AddVertexMedia(
        this IServiceCollection services,
        IWebHostEnvironment environment)
    {
        services.Configure<MediaOptions>(options =>
        {
            options.RootPath = environment.WebRootPath ??
                               Path.Combine(environment.ContentRootPath, "wwwroot");
        });

        services.AddSingleton<IMediaService, LocalMediaService>();

        return services;
    }
}
