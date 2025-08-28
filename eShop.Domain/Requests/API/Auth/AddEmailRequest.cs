namespace eShop.Domain.Requests.API.Auth;

public class AddEmailRequest
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
}