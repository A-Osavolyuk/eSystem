using eShop.Auth.Api.Security.Authentication;

namespace eShop.Auth.Api.Interfaces;

public interface ISignInManager
{
    public ValueTask<AuthenticationResult> AuthenticateAsync(string scheme,
        CancellationToken cancellationToken = default);
}