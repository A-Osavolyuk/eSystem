namespace eShop.Domain.Requests.API.Auth;

public record ResetPasswordRequest
{
    public string Email { get; set; } = string.Empty;
}