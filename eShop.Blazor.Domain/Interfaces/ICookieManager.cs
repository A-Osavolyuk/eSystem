namespace eShop.Blazor.Domain.Interfaces;

public interface ICookieManager
{
    public Task<string?> GetAsync(string key);
    public Task SetAsync(string key, string value, int days);
    public Task RemoveAsync(string key);
}