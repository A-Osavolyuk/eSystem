using eSystem.Core.Security.Authorization.OAuth;

namespace eSystem.Core.Requests.Auth;

public class AllowLinkedAccountRequest
{
    public Guid UserId { get; set; }
    public LinkedAccountType Type { get; set; }
}