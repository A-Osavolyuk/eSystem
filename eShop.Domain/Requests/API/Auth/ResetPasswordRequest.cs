namespace eShop.Domain.Requests.Api.Auth;

public record class ResetPasswordRequest
{
    public string Email { get; set; } = string.Empty;
}