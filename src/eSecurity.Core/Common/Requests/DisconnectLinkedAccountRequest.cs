using eSecurity.Core.Security.Authorization.OAuth;

namespace eSecurity.Core.Common.Requests;

public sealed class DisconnectLinkedAccountRequest
{
    [JsonPropertyName("type")]
    public LinkedAccountType Type { get; set; }
}