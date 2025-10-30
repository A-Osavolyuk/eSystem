namespace eAccount.Security.Cookies;

public interface ICookieAccessor
{
    public string? Get(string key);
}