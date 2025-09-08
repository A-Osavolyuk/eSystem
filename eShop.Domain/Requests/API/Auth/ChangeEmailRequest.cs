namespace eShop.Domain.Requests.API.Auth;

public record ChangeEmailRequest
{
    public required Guid UserId { get; set; }
    public required string CurrentEmail { get; set; }
    public required string NewEmail { get; set; }
}