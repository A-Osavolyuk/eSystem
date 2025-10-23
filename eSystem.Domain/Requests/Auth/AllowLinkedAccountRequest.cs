using eSystem.Domain.Security.Authorization.OAuth;

namespace eSystem.Domain.Requests.Auth;

public class AllowLinkedAccountRequest
{
    public Guid UserId { get; set; }
    public LinkedAccountType Type { get; set; }
}