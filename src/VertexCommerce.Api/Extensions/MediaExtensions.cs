using VertexCommerce.Shared.Services;
using Path = System.IO.Path;

namespace VertexCommerce.Api.Extensions;

public static class MediaExtensions
{
    public static IServiceCollection AddVertexMedia(
        this IServiceCollection services,
        IWebHostEnvironment environment,
        IConfiguration configuration)
    {
        services.Configure<MediaOptions>(options =>
        {
            options.RootPath = environment.WebRootPath ??
                               Path.Combine(environment.ContentRootPath, "wwwroot");
        });

        var providerOptions = new MediaProviderOptions();
        configuration.GetSection(MediaProviderOptions.SectionName).Bind(providerOptions);

        if (string.Equals(providerOptions.Provider, "Cloudinary", StringComparison.OrdinalIgnoreCase))
        {
            services.Configure<CloudinaryOptions>(configuration.GetSection("Cloudinary"));
            services.AddSingleton<IMediaService, CloudinaryMediaService>();
        }
        else
        {
            services.AddSingleton<IMediaService, LocalMediaService>();
        }

        return services;
    }
}
