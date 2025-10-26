using eSystem.Core.Security.Authorization.OAuth;

namespace eSystem.Core.Requests.Auth;

public class DisconnectLinkedAccountRequest
{
    public Guid UserId { get; set; }
    public LinkedAccountType Type { get; set; }
}