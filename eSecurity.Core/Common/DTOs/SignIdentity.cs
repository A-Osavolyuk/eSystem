namespace eSecurity.Core.Common.DTOs;

public class SignIdentity
{
    public required string RefreshToken { get; set; }
    public required List<ClaimValue> Claims { get; set; }
    public required string Scheme { get; set; }
}

public class ClaimValue
{
    public required string Type { get; set; }
    public required string Value { get; set; }
}