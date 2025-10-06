namespace eShop.Domain.Requests.Auth;

public record VerifyEmailRequest
{
    public required Guid UserId { get; set; }
    public required string Email { get; set; }
}