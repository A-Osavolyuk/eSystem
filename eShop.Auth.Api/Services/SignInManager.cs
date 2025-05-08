using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace eShop.Auth.Api.Services;

public class SignInManager : ISignInManager
{
    public async ValueTask<ClaimsPrincipal> AuthenticateAsync(HttpContext context, string scheme, CancellationToken cancellationToken = default)
    {
        var result = await context.AuthenticateAsync(scheme);
        var principal = result?.Principal!;
        return principal;
    }

    public AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl)
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = redirectUrl,
            Items = { ["LoginProvider"] = provider }
        };
        
        return properties;
    }
}