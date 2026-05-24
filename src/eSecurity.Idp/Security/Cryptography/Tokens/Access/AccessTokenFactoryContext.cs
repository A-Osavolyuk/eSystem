using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Cryptography.Tokens.Access;

public sealed class AccessTokenFactoryContext : TokenFactoryContext
{
    public UserEntity? User { get; set; }
    public SessionEntity? Session { get; set; }
}