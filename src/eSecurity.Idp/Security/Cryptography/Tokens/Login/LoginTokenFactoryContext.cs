using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Cryptography.Tokens.Login;

public sealed class LoginTokenFactoryContext : TokenFactoryContext
{
    public required Guid UserId { get; init; }
    public Guid? SessionId { get; init; }
}