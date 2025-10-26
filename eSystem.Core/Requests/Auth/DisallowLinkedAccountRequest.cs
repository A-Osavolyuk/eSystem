using eSystem.Core.Security.Authorization.OAuth;

namespace eSystem.Core.Requests.Auth;

public class DisallowLinkedAccountRequest
{
    public Guid UserId { get; set; }
    public LinkedAccountType Type { get; set; }
}