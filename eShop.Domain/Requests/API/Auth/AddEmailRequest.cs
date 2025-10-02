namespace eShop.Domain.Requests.API.Auth;

public class AddEmailRequest
{
    public required Guid UserId { get; set; }
    public required string Email { get; set; } = string.Empty;
}