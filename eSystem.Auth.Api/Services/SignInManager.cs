using eSystem.Auth.Api.Security.Authentication.Results;
using eSystem.Core.Attributes;
using Microsoft.AspNetCore.Authentication;

namespace eSystem.Auth.Api.Services;

[Injectable(typeof(ISignInManager), ServiceLifetime.Scoped)]
public sealed class SignInManager(IHttpContextAccessor httpContextAccessor) : ISignInManager
{
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

    public async ValueTask<AuthenticationResult> AuthenticateAsync( 
        string scheme, CancellationToken cancellationToken = default)
    {
        var authenticateResult = await httpContextAccessor.HttpContext!.AuthenticateAsync(scheme);
        
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