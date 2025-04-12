namespace eShop.Domain.Requests.API.Auth;

public record class ResetPasswordRequest
{
    public string Email { get; set; } = string.Empty;
}