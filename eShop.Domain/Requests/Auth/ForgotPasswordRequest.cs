namespace eShop.Domain.Requests.Auth;

public class ForgotPasswordRequest
{
    public string Email { get; set; } = string.Empty;
}