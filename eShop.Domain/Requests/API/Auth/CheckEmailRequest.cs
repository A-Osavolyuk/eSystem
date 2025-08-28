namespace eShop.Domain.Requests.API.Auth;

public class CheckEmailRequest
{
    public Guid UserId { get; set; }
    public required string Email { get; set; }
}