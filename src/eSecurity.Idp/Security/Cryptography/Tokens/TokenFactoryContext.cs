using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Cryptography.Tokens;

public abstract class TokenFactoryContext
{
    public required ClientEntity Client { get; set; }
}