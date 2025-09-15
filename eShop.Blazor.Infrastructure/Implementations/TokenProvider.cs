using eShop.Blazor.Domain.Interfaces;
using eShop.Blazor.Infrastructure.Security;
using Microsoft.AspNetCore.Http;

namespace eShop.Blazor.Infrastructure.Implementations;

public class TokenProvider(TokenStore tokenStore) : ITokenProvider
{
    private readonly TokenStore tokenStore = tokenStore;

    public string? Get()
    {
        return tokenStore.Token;
    }

    public void Set(string accessToken)
    {
        tokenStore.Token = accessToken;
    }

    public void Clear()
    {
        tokenStore.Token = null;
    }
}