using eShop.Auth.Api.Types;
using Microsoft.AspNetCore.Authentication;

namespace eShop.Auth.Api.Services;

[Injectable(typeof(ISignInManager), ServiceLifetime.Scoped)]
public sealed class SignInManager() : ISignInManager
{
    public async ValueTask<AuthenticationResult> AuthenticateAsync(HttpContext context, 
        string scheme, CancellationToken cancellationToken = default)
    {
        var authenticateResult = await context.AuthenticateAsync(scheme);
        
        var principal = authenticateResult.Principal ?? throw new NullReferenceException("Principal is null");
        var properties = authenticateResult.Properties ?? throw new NullReferenceException("Properties is null");

        var result = new AuthenticationResult()
        {
            Principal = principal,
            Properties = properties,
        };
        
        return result;
    }
}