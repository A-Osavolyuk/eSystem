namespace eSecurity.Server.Common.Storage.Session;

public class SessionStorage(IHttpContextAccessor httpContextAccessor) : ISessionStorage
{
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    
    public void Set(string key, string value) => _httpContext.Session.SetString(key, value);

    public string? Get(string key) => _httpContext.Session.GetString(key);
}