namespace eShop.Domain.Requests.Auth;

public class CheckEmailRequest
{
    public required Guid UserId { get; set; }
    public required string Email { get; set; }
}