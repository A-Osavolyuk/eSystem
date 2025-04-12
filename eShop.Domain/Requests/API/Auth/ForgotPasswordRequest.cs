namespace eShop.Domain.Requests.API.Auth;

public class ForgotPasswordRequest
{
    public string Email { get; set; } = string.Empty;
}