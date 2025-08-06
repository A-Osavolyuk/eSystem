using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace eShop.Auth.Api.Services;

[Injectable(typeof(ISignInManager), ServiceLifetime.Scoped)]
public sealed class SignInManager(IAuthenticationSchemeProvider schemeProvider) : ISignInManager
{
    private readonly IAuthenticationSchemeProvider schemeProvider = schemeProvider;

    public async ValueTask<ClaimsPrincipal> AuthenticateAsync(HttpContext context, string scheme, CancellationToken cancellationToken = default)
    {
        var result = await context.AuthenticateAsync(scheme);
        var principal = result?.Principal!;
        return principal;
    }

    public AuthenticationProperties ConfigureOAuthProperties(string provider, string redirectUri)
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = redirectUri,
            Items =
            {
                ["provider"] = provider,
            }
        };
        
        return properties;
    }
}