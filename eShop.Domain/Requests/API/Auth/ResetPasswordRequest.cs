namespace eShop.Domain.Requests.API.Auth;

public class ResetPasswordRequest
{
    public string Email { get; set; } = string.Empty;
}