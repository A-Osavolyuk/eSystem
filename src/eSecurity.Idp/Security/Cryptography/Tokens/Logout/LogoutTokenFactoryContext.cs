using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Cryptography.Tokens.Logout;

public sealed class LogoutTokenFactoryContext : TokenFactoryContext
{
    public required UserEntity User { get; set; }
    public required SessionEntity Session { get; set; }
}