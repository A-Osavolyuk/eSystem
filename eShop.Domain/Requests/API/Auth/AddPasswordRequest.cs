namespace eShop.Domain.Requests.API.Auth;

public class AddPasswordRequest
{
    public Guid UserId { get; set; }
    public string Password { get; set; } = string.Empty;
}