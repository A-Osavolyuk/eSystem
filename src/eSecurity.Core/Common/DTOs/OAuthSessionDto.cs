using eSecurity.Core.Security.Authorization.OAuth;

namespace eSecurity.Core.Common.DTOs;

public sealed class OAuthSessionDto
{
    public OAuthFlow Flow { get; set; }
    public Guid UserId { get; set; }
    public bool RequireTwoFactor { get; set; }
    public required string Provider { get; set; }
    public required string[] AuthenticationMethods { get; set; }
}