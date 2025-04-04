namespace eShop.Domain.Requests.Api.Auth;

public class ResendEmailVerificationCodeRequest
{
    public string Email { get; set; } = string.Empty;
}