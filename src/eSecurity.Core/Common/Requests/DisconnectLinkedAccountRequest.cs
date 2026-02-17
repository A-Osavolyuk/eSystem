using eSecurity.Core.Security.Authorization.OAuth;

namespace eSecurity.Core.Common.Requests;

public sealed class DisconnectLinkedAccountRequest
{
    public required string Subject { get; set; }
    public LinkedAccountType Type { get; set; }
}