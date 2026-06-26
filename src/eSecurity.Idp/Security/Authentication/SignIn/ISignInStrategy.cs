using eSecurity.Core.Security.Authentication.SignIn;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authentication.SignIn;

public interface ISignInStrategy<in TPayload> : ISignInStrategy 
    where TPayload : SignInPayload
{
    ValueTask<Result> ExecuteAsync(TPayload payload, CancellationToken cancellationToken = default);
}

public interface ISignInStrategy
{
    public Type PayloadType { get; }
    
    ValueTask<Result> ExecuteAsync(SignInPayload payload, CancellationToken cancellationToken = default);
}