namespace eAccount.Domain.Interfaces;

public interface ICookieAccessor
{
    public string? Get(string key);
}