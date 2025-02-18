namespace eShop.Auth.Api.Options;

public class JwtOptions
{
    public int ExpirationDays { get; init; }
    public string Key { get; init; } = "";
    public string Audience { get; init; } = "";
    public string Issuer { get; init; } = "";
}