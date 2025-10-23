namespace eSystem.Auth.Api.Interfaces;

public interface ISessionStorage
{
    public void Set(string key, string value);
    public string? Get(string key);
}