namespace eShop.Domain.Requests.API.Auth;

public class CheckEmailRequest
{
    public required Guid UserId { get; set; }
    public required string Email { get; set; }
}