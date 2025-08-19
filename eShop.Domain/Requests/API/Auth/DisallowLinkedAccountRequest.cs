namespace eShop.Domain.Requests.API.Auth;

public class DisallowLinkedAccountRequest
{
    public Guid UserId { get; set; }
    public Guid ProviderId { get; set; }
}