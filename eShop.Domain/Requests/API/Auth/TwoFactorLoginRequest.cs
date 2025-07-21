namespace eShop.Domain.Requests.API.Auth;

public record TwoFactorLoginRequest
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
}