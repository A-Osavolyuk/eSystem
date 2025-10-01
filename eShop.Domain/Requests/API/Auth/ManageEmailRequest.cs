namespace eShop.Domain.Requests.API.Auth;

public class ManageEmailRequest
{
    public required Guid UserId { get; set; }
    public required string Email { get; set; }
    public required EmailType Type { get; set; }
}