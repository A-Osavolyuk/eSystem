using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Cryptography.Tokens.Refresh;

public sealed class RefreshTokenFactoryContext : TokenFactoryContext
{
    public Guid? UserId { get; init; }
    public Guid? SessionId { get; init; }
}