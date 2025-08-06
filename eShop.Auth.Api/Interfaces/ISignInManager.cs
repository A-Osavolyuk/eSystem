using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace eShop.Auth.Api.Interfaces;

public interface ISignInManager
{
    public ValueTask<ClaimsPrincipal> AuthenticateAsync(HttpContext context, string scheme,
        CancellationToken cancellationToken = default);

    public AuthenticationProperties ConfigureAuthenticationProperties(string redirectUri,
        Dictionary<string, string?> items);
}