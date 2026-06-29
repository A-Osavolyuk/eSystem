using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Cryptography.Tokens.Logout;

public sealed class LogoutTokenFactoryContext : TokenFactoryContext
{
    public required Guid UserId { get; init; }
    public required Guid SessionId { get; init; }
}