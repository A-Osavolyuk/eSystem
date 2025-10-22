using eShop.Domain.Security.Authorization.OAuth;

namespace eShop.Domain.Requests.Auth;

public class DisallowLinkedAccountRequest
{
    public Guid UserId { get; set; }
    public LinkedAccountType Type { get; set; }
}