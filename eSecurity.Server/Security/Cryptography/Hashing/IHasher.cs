namespace eSecurity.Server.Security.Cryptography.Hashing;

public interface IHasher
{
    public string Hash(string value);
    public bool VerifyHash(string value, string hash);
}