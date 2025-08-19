namespace eShop.Domain.Requests.API.Auth;

public class ConfirmDisconnectLinkedAccountRequest
{
    public Guid UserId { get; set; }
    public Guid ProviderId { get; set; }
    public string Code { get; set; } = string.Empty;
}