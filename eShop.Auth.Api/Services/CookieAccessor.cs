namespace eShop.Auth.Api.Services;

[Injectable(typeof(ICookieAccessor), ServiceLifetime.Scoped)]
public class CookieAccessor(IHttpContextAccessor httpContextAccessor) : ICookieAccessor
{
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

    public bool? Exists(string key) => httpContextAccessor.HttpContext?.Request.Cookies.ContainsKey(key);
    public string? Get(string key) => httpContextAccessor.HttpContext?.Request.Cookies[key];

    public void Set(string key, string value, CookieOptions options)
        => httpContextAccessor.HttpContext?.Response.Cookies.Append(key, value, options);

    public void Remove(string key, CookieOptions options)
        => httpContextAccessor.HttpContext?.Response.Cookies.Delete(key, options);
}