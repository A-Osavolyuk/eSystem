namespace eSecurity.Idp.Security.Cryptography.Tokens;

public sealed class TokenFactoryOptions
{
    public List<string> AllowedScopes { get; set; } = [];
    public string? Nonce { get; set; }
}