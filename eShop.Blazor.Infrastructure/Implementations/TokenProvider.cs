using eShop.Blazor.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace eShop.Blazor.Infrastructure.Implementations;

public class TokenProvider(IHttpContextAccessor httpContextAccessor) : ITokenProvider
{
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

    public async ValueTask<string?> GetAsync()
    {
        const string key = "AccessToken";
        var context = httpContextAccessor.HttpContext;
        var token = context?.Request.Cookies[key];
        return await Task.FromResult(token);
    }
}