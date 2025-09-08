namespace eShop.Domain.Requests.API.Auth;

public class RemoveEmailRequest
{
    public required Guid UserId { get; set; }
    public required string Email { get; set; }
}