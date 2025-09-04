using eShop.Blazor.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace eShop.Blazor.Infrastructure.Implementations;

public class TokenProvider(
    ICookieManager cookieManager,
    IHttpContextAccessor httpContextAccessor) : ITokenProvider
{
    private readonly ICookieManager cookieManager = cookieManager;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private const string Key = "access-token";

    public async ValueTask<string?> GetAsync()
    {
        var context = httpContextAccessor.HttpContext;
        var token = context?.Request.Cookies[Key];
        return await Task.FromResult(token);
    }

    public async ValueTask RemoveAsync()
    {
        await cookieManager.RemoveAsync(Key);
    }

    public async ValueTask SetAsync(string token)
    {
        await cookieManager.SetAsync(Key, token, 30);
    }
}