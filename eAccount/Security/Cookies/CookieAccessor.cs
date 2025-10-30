namespace eAccount.Security.Cookies;

public class CookieAccessor(IHttpContextAccessor httpContextAccessor) : ICookieAccessor
{
    private readonly HttpContext httpContext = httpContextAccessor.HttpContext!;

    public string? Get(string key) => httpContext.Request.Cookies[key];
}