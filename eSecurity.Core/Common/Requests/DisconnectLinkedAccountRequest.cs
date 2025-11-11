using eSecurity.Core.Security.Authorization.OAuth;

namespace eSecurity.Core.Common.Requests;

public class DisconnectLinkedAccountRequest
{
    public Guid UserId { get; set; }
    public LinkedAccountType Type { get; set; }
}