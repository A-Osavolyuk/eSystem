using Microsoft.AspNetCore.Http;

namespace eShop.Blazor.Server.Infrastructure.Security;

public class TokenProvider(IHttpContextAccessor httpContextAccessor)
{
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    public string? AccessToken { get; set; }
    public string? RefreshToken => httpContextAccessor.HttpContext?.Request.Cookies["eAccount.Authentication.RefreshToken"];

    public void Clear()
    {
        AccessToken = null;
    }
}