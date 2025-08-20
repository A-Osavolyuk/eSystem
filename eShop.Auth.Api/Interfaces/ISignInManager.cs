using eShop.Auth.Api.Types;

namespace eShop.Auth.Api.Interfaces;

public interface ISignInManager
{
    public ValueTask<AuthenticationResult> AuthenticateAsync(HttpContext context, string scheme,
        CancellationToken cancellationToken = default);
}