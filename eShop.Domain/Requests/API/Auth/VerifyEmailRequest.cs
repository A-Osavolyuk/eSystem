namespace eShop.Domain.Requests.API.Auth;

public record VerifyEmailRequest
{
    public required Guid UserId { get; set; }
    public required string Email { get; set; }
}