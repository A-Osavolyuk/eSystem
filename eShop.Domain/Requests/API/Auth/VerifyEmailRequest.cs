namespace eShop.Domain.Requests.API.Auth;

public record VerifyEmailRequest
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
}