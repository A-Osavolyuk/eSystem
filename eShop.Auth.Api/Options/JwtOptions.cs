namespace eShop.Auth.Api.Options;

public class JwtOptions
{
    public int AccessTokenExpirationMinutes { get; init; }
    public int RefreshTokenExpirationDays { get; init; }
    public string Secret { get; init; } = "";
    public string Audience { get; init; } = "";
    public string Issuer { get; init; } = "";
}