using eShop.Auth.Api.Security.Authentication;
using eShop.Auth.Api.Security.Authentication.Results;

namespace eShop.Auth.Api.Interfaces;

public interface ISignInManager
{
    public ValueTask<AuthenticationResult> AuthenticateAsync(string scheme,
        CancellationToken cancellationToken = default);
}