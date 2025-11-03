using eSystem.Core.Security.Authentication.JWT;

namespace eSecurity.Security.Authentication.JWT.Payloads;

public sealed class AccessTokenPayload : JwtTokenPayload
{
    public required List<string> Scopes { get; set; }
    public string? Nonce { get; set; }
}