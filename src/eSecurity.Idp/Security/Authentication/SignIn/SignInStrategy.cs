using eSecurity.Core.Security.Authentication.SignIn;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authentication.SignIn;

public abstract class SignInStrategy<TPayload> : ISignInStrategy<TPayload> 
    where TPayload : SignInPayload
{
    public abstract Type PayloadType { get; }
    
    public virtual async ValueTask<Result> ExecuteAsync(SignInPayload payload, 
        CancellationToken cancellationToken = default)
    {
        if (payload is not TPayload typedPayload)
            throw new InvalidOperationException("Invalid payload type");
        
        return await ExecuteAsync(typedPayload, cancellationToken);
    }

    public abstract ValueTask<Result> ExecuteAsync(TPayload payload, CancellationToken cancellationToken = default);
}