namespace eSystem.Auth.Api.Storage.Session;

public class SessionStorage(IHttpContextAccessor httpContextAccessor) : ISessionStorage
{
    private readonly HttpContext httpContext = httpContextAccessor.HttpContext!;
    
    public void Set(string key, string value) => httpContext.Session.SetString(key, value);

    public string? Get(string key) => httpContext.Session.GetString(key);
}