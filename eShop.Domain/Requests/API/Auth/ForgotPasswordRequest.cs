namespace eShop.Domain.Requests.Api.Auth;

public class ForgotPasswordRequest
{
    public string Email { get; set; } = string.Empty;
}