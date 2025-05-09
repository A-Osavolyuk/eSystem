namespace eShop.Auth.Api.Utilities;

using System.Security.Cryptography;

public static class PasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 10000;

    public static string HashPassword(string password)
    {
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[SaltSize];
        rng.GetBytes(salt);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(KeySize);

        var hashBytes = new byte[1 + 4 + SaltSize + KeySize];
        hashBytes[0] = 0x01;
        BitConverter.GetBytes(Iterations).CopyTo(hashBytes, 1);
        salt.CopyTo(hashBytes, 5);
        hash.CopyTo(hashBytes, 5 + SaltSize);

        return Convert.ToBase64String(hashBytes);
    }

    public static bool VerifyPassword(string password, string storedHash)
    {
        var hashBytes = Convert.FromBase64String(storedHash);

        if (hashBytes[0] != 0x01)
            throw new FormatException("Unsupported hash format");

        var iterations = BitConverter.ToInt32(hashBytes, 1);
        var salt = new byte[SaltSize];
        var storedSubkey = new byte[KeySize];
        Array.Copy(hashBytes, 5, salt, 0, SaltSize);
        Array.Copy(hashBytes, 5 + SaltSize, storedSubkey, 0, KeySize);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
        var computedSubkey = pbkdf2.GetBytes(KeySize);

        return CryptographicOperations.FixedTimeEquals(storedSubkey, computedSubkey);
    }
}