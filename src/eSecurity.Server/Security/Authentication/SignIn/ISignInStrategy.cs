using eSecurity.Core.Security.Authentication.SignIn;
using eSystem.Core.Primitives;

namespace eSecurity.Server.Security.Authentication.SignIn;

public interface ISignInStrategy
{
    public ValueTask<Result> ExecuteAsync(SignInPayload payload, 
        CancellationToken cancellationToken = default);
}