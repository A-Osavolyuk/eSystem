namespace eSystem.Auth.Api.Storage.Session;

public interface ISessionStorage
{
    public void Set(string key, string value);
    public string? Get(string key);
}