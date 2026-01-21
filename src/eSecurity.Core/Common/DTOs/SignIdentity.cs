using Microsoft.AspNetCore.Authentication;

namespace eSecurity.Core.Common.DTOs;

public class SignIdentity
{
    public required List<ClaimValue> Claims { get; set; }
    public required List<AuthenticationToken> Tokens { get; set; }
}

public class ClaimValue
{
    public required string Type { get; set; }
    public required string Value { get; set; }
}