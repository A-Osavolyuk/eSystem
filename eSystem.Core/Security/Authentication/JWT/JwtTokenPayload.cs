using eSystem.Core.Security.Cryptography.Tokens;

namespace eSystem.Core.Security.Authentication.JWT;

public class JwtTokenPayload : TokenPayload
{
    public required string Audience { get; set; }
    public required string Subject { get; set; }
    public required DateTimeOffset IssuedAt { get; set; }
    public required DateTimeOffset ExpiresAt { get; set; }
}