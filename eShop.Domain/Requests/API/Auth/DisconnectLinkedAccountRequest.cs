namespace eShop.Domain.Requests.API.Auth;

public class DisconnectLinkedAccountRequest
{
    public Guid UserId { get; set; }
    public string Provider { get; set; } = string.Empty;
}