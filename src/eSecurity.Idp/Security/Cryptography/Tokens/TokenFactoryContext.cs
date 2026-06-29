using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Cryptography.Tokens;

public abstract class TokenFactoryContext
{
    public Guid ClientId { get; init; }
    public required TimeSpan? TokenLifetime { get; init; }
}