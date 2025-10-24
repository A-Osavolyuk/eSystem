namespace eSystem.Domain.Requests.Auth;

public class AuthorizeRequest
{
    public Guid UserId { get; set; }
    public string ClientId { get; set; } = string.Empty;
}