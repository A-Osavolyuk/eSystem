namespace eShop.Domain.Requests.API.Auth;

public class DisconnectLinkedAccountRequest
{
    public Guid UserId { get; set; }
    public Guid ProviderId { get; set; }
}