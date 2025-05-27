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

    public async ValueTask<List<string>> GetOAuthSchemasAsync(CancellationToken cancellationToken = default)
    {
        var schemes = await schemeProvider.GetAllSchemesAsync();
        var result = schemes
            .Where(s => !string.IsNullOrEmpty(s.DisplayName))
            .Select(x => x.Name).ToList();
        
        return result;
    }

    public AuthenticationProperties ConfigureOAuthProperties(string provider, string redirectUri)
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = redirectUri,
            Items =
            {
                ["LoginProvider"] = provider,
            }
        };
        
        return properties;
    }
}