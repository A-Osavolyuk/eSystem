namespace eShop.AppHost.Options;

public class JwtOptions
{
    public int ExpirationDays { get; init; }
    public string Secret { get; init; } = "";
    public string Audience { get; init; } = "";
    public string Issuer { get; init; } = "";
}