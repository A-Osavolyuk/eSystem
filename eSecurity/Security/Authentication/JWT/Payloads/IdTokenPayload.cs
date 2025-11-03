using eSystem.Core.Security.Authentication.JWT;

namespace eSecurity.Security.Authentication.JWT.Payloads;

public sealed class IdTokenPayload : JwtTokenPayload
{
    public required List<string> Scopes { get; set; }
    public required DateTimeOffset AuthTime { get; set; }
    public string? Nonce { get; set; }

    public string? Email { get; set; }
    public bool EmailVerified { get; set; }

    public string? PhoneNumber { get; set; }
    public bool PhoneNumberVerified { get; set; }
    
    public string? Name { get; set; }
    public string? FamilyName { get; set; }
    public string? GivenName { get; set; }
    public string? MiddleName { get; set; }
    public string? Nickname { get; set; }
    public string? PreferredUsername { get; set; }
    public string? Profile { get; set; }
    public string? Picture { get; set; }
    public string? Gender { get; set; }
    public DateTimeOffset? Birthdate { get; set; }
    public string? ZoneInfo { get; set; }
    public string? Locale { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}