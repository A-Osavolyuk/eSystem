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

    public AuthenticationProperties ConfigureAuthenticationProperties(string redirectUri, Dictionary<string, string?> items)
    {
        var properties = new AuthenticationProperties { RedirectUri = redirectUri };

        foreach (var item in items)
        {
            properties.Items.Add(item.Key, item.Value);
        }
        
        return properties;
    }
}