namespace eShop.Domain.Types;

public class SecurityToken
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}