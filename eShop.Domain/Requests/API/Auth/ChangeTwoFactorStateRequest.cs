namespace eShop.Domain.Requests.API.Auth;

public record ChangeTwoFactorStateRequest
{
    public string Email { get; set; } = string.Empty;
}