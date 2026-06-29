using eSecurity.Idp.Security.Authentication.Client;

namespace eSecurity.Idp.Security.Cryptography.Tokens.Access;

public sealed class AccessTokenFactoryContext : TokenFactoryContext
{
    public Guid? UserId { get; init; }
    public Guid? SessionId { get; init; }
    public required AccessTokenType TokenType { get; init; }
}