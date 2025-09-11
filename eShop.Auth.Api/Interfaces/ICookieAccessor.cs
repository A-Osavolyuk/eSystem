namespace eShop.Auth.Api.Interfaces;

public interface ICookieAccessor
{
    public bool? Exists(string key);
    public string? Get(string key);
    public void Set(string key, string value, CookieOptions options);
    public void Remove(string key, CookieOptions options);
}