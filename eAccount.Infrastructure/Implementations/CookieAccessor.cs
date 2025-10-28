using Microsoft.AspNetCore.Http;

namespace eAccount.Infrastructure.Implementations;

public class CookieAccessor(IHttpContextAccessor httpContextAccessor) : Domain.Interfaces.ICookieAccessor
{
    private readonly HttpContext httpContext = httpContextAccessor.HttpContext!;

    public string? Get(string key) => httpContext.Request.Cookies[key];
}