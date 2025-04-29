namespace eShop.Domain.Requests.API.Auth;

public record VerifyEmailRequest
{
    public string Email { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}