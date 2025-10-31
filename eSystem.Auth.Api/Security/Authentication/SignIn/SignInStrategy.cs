using eSystem.Core.Security.Authentication.SignIn;

namespace eSystem.Auth.Api.Security.Authentication.SignIn;

public abstract class SignInStrategy
{
    public abstract ValueTask<Result> SignInAsync(SignInPayload payload, 
        CancellationToken cancellationToken = default);
}