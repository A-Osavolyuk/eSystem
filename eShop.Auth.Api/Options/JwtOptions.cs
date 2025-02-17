namespace eShop.Auth.Api.Options;

public class JwtOptions
{
    public int ExpirationDays { get; set; }
    public string Key { get; set; } = "";
    public string Audience { get; set; } = "";
    public string Issuer { get; set; } = "";
}