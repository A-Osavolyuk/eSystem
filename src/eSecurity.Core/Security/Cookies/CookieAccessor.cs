using Microsoft.AspNetCore.Http;

namespace eSecurity.Core.Security.Cookies;

public class CookieAccessor(IHttpContextAccessor httpContextAccessor) : ICookieAccessor
{
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public string? Get(string key) => _httpContext.Request.Cookies[key];
}