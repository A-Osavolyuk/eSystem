namespace eShop.Infrastructure.Services;

public class TokenProvider(ICookieManager cookieManager) : ITokenProvider
{
    private readonly ICookieManager cookieManager = cookieManager;
    private const string Key = "access-token";

    public async ValueTask<string> GetTokenAsync()
    {
        var token = await cookieManager.GetAsync(Key);
        return token;
    }

    public async ValueTask RemoveAsync()
    {
        await cookieManager.RemoveAsync(Key);
    }

    public async ValueTask SetTokenAsync(string token)
    {
        await cookieManager.SetAsync(Key, token, 30);
    }
}