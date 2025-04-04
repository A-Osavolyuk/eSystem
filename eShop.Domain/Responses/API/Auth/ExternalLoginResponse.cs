namespace eShop.Domain.Responses.Api.Auth;

public class ExternalLoginResponse
{
    public AuthenticationProperties AuthenticationProperties { get; set; } = null!;
    public string Provider { get; set; } = string.Empty;
}