using System.Security.Claims;
using eShop.Auth.Api.Types;
using Microsoft.AspNetCore.Authentication;

namespace eShop.Auth.Api.Interfaces;

public interface ISignInManager
{
    public ValueTask<AuthenticationResult> AuthenticateAsync(HttpContext context, string scheme,
        CancellationToken cancellationToken = default);
}