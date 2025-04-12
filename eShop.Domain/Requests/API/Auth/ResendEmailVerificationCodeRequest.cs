namespace eShop.Domain.Requests.API.Auth;

public class ResendEmailVerificationCodeRequest
{
    public string Email { get; set; } = string.Empty;
}