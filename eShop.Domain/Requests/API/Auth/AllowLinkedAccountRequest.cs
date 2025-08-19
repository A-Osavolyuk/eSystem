namespace eShop.Domain.Requests.API.Auth;

public class AllowLinkedAccountRequest
{
    public Guid UserId { get; set; }
    public Guid ProviderId { get; set; }
}