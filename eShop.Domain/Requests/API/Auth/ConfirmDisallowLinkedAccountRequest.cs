namespace eShop.Domain.Requests.API.Auth;

public class ConfirmDisallowLinkedAccountRequest
{
    public Guid UserId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}