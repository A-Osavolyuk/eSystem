using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Cryptography.Tokens.Id;

public sealed class IdTokenFactoryContext : TokenFactoryContext
{
    public required Guid UserId { get; init; }
    public required Guid SessionId { get; init; }
}