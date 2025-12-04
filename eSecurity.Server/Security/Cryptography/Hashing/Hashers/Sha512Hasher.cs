using System.Security.Cryptography;

namespace eSecurity.Server.Security.Cryptography.Hashing.Hashers;

public class Sha512Hasher : Hasher
{
    public override string Hash(string value)
    {
        using var sha = SHA512.Create();
        var bytes = Encoding.UTF8.GetBytes(value);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToHexString(hash);
    }

    public override bool VerifyHash(string value, string hash)
    {
        var incomingHash = Hash(value);
        return CryptographicOperations.FixedTimeEquals(
            Convert.FromHexString(incomingHash),
            Convert.FromHexString(hash)
        );
    }
}