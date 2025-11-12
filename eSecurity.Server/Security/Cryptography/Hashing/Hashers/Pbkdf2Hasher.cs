using System.Security.Cryptography;

namespace eSecurity.Server.Security.Cryptography.Hashing.Hashers;

public class Pbkdf2Hasher : Hasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 10000;

    public override string Hash(string value)
    {
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[SaltSize];
        rng.GetBytes(salt);

        var hash = Rfc2898DeriveBytes.Pbkdf2(value, salt, 
            Iterations, HashAlgorithmName.SHA256, KeySize);
        
        var hashBytes = new byte[1 + 4 + SaltSize + KeySize];
        hashBytes[0] = 0x01;
        BitConverter.GetBytes(Iterations).CopyTo(hashBytes, 1);
        salt.CopyTo(hashBytes, 5);
        hash.CopyTo(hashBytes, 5 + SaltSize);

        return Convert.ToBase64String(hashBytes);
    }

    public override bool VerifyHash(string value, string hash)
    {
        var hashBytes = Convert.FromBase64String(hash);

        if (hashBytes[0] != 0x01)
            throw new FormatException("Unsupported hash format");

        var iterations = BitConverter.ToInt32(hashBytes, 1);
        var salt = new byte[SaltSize];
        var storedSubkey = new byte[KeySize];
        Array.Copy(hashBytes, 5, salt, 0, SaltSize);
        Array.Copy(hashBytes, 5 + SaltSize, storedSubkey, 0, KeySize);

        var computedSubkey = Rfc2898DeriveBytes.Pbkdf2(value, salt, 
            iterations, HashAlgorithmName.SHA256, KeySize);
        
        return CryptographicOperations.FixedTimeEquals(storedSubkey, computedSubkey);
    }
}