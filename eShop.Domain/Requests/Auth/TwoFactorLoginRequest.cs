namespace eShop.Domain.Requests.Auth;

public record TwoFactorLoginRequest
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
    public TwoFactorMethod Type { get; set; }
}