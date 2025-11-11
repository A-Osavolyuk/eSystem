namespace eSecurity.Server.Security.Cryptography.Hashing;

public abstract class Hasher
{
    public abstract string Hash(string value);
    public abstract bool VerifyHash(string value, string hash);
}