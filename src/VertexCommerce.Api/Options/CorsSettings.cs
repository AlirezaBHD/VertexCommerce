namespace VertexCommerce.Api.Options;

internal sealed class CorsSettings
{
    public const string SectionName = "Cors";

    public string[] AllowedOrigins { get; init; } = [];
    public string[] AllowedMethods { get; init; } = [];
    public string[] AllowedHeaders { get; init; } = [];
    public string[] ExposedHeaders { get; init; } = [];
    public bool AllowCredentials { get; init; } = true;
    public int MaxAgeSeconds { get; init; } = 600;
}
