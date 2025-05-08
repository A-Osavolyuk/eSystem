namespace eShop.Domain.Requests.API.Auth;

public record TwoFactorLoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
}