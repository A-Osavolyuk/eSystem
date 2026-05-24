using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Cryptography.Tokens.Refresh;

public sealed class RefreshTokenFactoryContext : TokenFactoryContext
{
    public UserEntity? User { get; set; }
    public SessionEntity? Session { get; set; }
}