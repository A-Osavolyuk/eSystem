namespace eShop.Auth.Api.Security.Tokens.Jwt;

public class JwtOptions
{
    public int AccessTokenExpirationMinutes { get; init; }
    public int RefreshTokenExpirationDays { get; init; }
    public string Secret { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
}