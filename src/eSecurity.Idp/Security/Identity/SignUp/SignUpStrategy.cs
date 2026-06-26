using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Identity.SignUp;

public abstract class SignUpStrategy<TPayload> : ISignUpStrategy<TPayload> where TPayload : SignUpPayload
{
    public abstract Type PayloadType { get; }
    
    public virtual async ValueTask<Result> ExecuteAsync(SignUpPayload payload, 
        CancellationToken cancellationToken = default)
    {
        if (payload is not TPayload typedPayload)
            throw new InvalidOperationException("Invalid payload type");
        
        return await ExecuteAsync(typedPayload, cancellationToken);
    }

    public abstract ValueTask<Result> ExecuteAsync(TPayload payload,
        CancellationToken cancellationToken = default);
}