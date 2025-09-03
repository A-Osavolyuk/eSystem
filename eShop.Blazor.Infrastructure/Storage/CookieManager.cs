using eShop.Blazor.Domain.Interfaces;
using Microsoft.JSInterop;

namespace eShop.Blazor.Infrastructure.Storage;

public class CookieManager(IJSRuntime jsRuntime) : ICookieManager
{
    private readonly IJSRuntime jsRuntime = jsRuntime;
    
    public async Task<string?> GetAsync(string key)
    {
        var value = await jsRuntime.InvokeAsync<string>("getCookie", key);
        return value;
    }

    public async Task SetAsync(string key, string value, int days)
    {
        await jsRuntime.InvokeVoidAsync("setCookie", key, value, days);
    }

    public async Task RemoveAsync(string key)
    {
        await jsRuntime.InvokeVoidAsync("deleteCookie", key);
    }
}