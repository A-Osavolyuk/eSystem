namespace eShop.Domain.Requests.API.Auth;

public class AllowLinkedAccountRequest
{
    public Guid UserId { get; set; }
    public string Provider { get; set; } = string.Empty;
}