using System.Security.Cryptography;

namespace eSecurity.Security.Cryptography.Keys.SigningKey;

public class SigningKey
{
    public Guid Id { get; set; }
    public required RSA PrivateKey { get; set; }
    public required RSA PublicKey { get; set; }
}