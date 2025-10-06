namespace eShop.Domain.Requests.Auth;

public record ChangeUsernameRequest
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
}