using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace eShop.Auth.Api.Interfaces;

public interface ISignInManager
{
    public ValueTask<ClaimsPrincipal> AuthenticateAsync(HttpContext context, string scheme,
        CancellationToken cancellationToken = default);
    public ValueTask<List<string>> GetExternalAuthenticationSchemasAsync(CancellationToken cancellationToken = default);
    public AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUri);
}