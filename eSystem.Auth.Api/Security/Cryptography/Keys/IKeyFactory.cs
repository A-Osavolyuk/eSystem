namespace eSystem.Auth.Api.Security.Cryptography.Keys;

public interface IKeyFactory
{
    public string Create(int length);
}