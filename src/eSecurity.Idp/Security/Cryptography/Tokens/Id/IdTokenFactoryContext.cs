using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Cryptography.Tokens.Id;

public sealed class IdTokenFactoryContext : TokenFactoryContext
{
    public required UserEntity User { get; set; }
    public required SessionEntity Session { get; set; }
}