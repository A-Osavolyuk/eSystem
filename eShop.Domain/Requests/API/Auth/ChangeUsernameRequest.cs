namespace eShop.Domain.Requests.API.Auth;

public record ChangeUsernameRequest
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
}