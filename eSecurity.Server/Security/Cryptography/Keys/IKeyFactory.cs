namespace eSecurity.Server.Security.Cryptography.Keys;

public interface IKeyFactory
{
    public string Create(int length);
}