namespace eShop.Domain.Requests.API.Auth;

public class ConfirmDisallowLinkedAccountRequest
{
    public Guid UserId { get; set; }
    public Guid ProviderId { get; set; }
    public string Code { get; set; } = string.Empty;
}