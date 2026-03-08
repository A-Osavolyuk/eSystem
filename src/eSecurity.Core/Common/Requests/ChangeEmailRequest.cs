using eSecurity.Core.Security.Identity;

namespace eSecurity.Core.Common.Requests;

public sealed class ChangeEmailRequest
{
    [JsonPropertyName("email")]
    public required string Email { get; set; }
    
    [JsonPropertyName("type")]
    public required EmailType Type { get; set; }
}