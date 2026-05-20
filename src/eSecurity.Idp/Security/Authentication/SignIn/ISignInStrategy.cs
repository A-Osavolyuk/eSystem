using eSecurity.Core.Security.Authentication.SignIn;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authentication.SignIn;

public interface ISignInStrategy
{
    public ValueTask<Result> ExecuteAsync(SignInPayload payload, 
        CancellationToken cancellationToken = default);
}