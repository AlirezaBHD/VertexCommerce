namespace VertexCommerce.Modules.Identity.Infrastructure.Authentication;

public sealed class JwtSettings
{
    public const string SectionName = "Jwt";
    public string Secret { get; init; } = default!;
    public string Issuer { get; init; } = default!;
    public string Audience { get; init; } = default!;
    public int AccessTokenExpirationMinutes { get; init; } = 15;
    public int RefreshTokenExpirationDays { get; init; } = 7;
}