namespace eShop.Domain.Requests.Auth;

public class DisconnectLinkedAccountRequest
{
    public Guid UserId { get; set; }
    public LinkedAccountType Type { get; set; }
}