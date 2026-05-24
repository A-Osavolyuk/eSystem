using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Cryptography.Tokens.Login;

public sealed class LoginTokenFactoryContext : TokenFactoryContext
{
    public required UserEntity User { get; set; }
    public SessionEntity? Session { get; set; }
}