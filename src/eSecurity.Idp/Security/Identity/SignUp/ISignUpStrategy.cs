using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Identity.SignUp;

public interface ISignUpStrategy
{
    public Type PayloadType { get; }
    
    ValueTask<Result> ExecuteAsync(SignUpPayload payload, CancellationToken cancellationToken = default);
}

public interface ISignUpStrategy<in TPayload> : ISignUpStrategy 
    where TPayload : SignUpPayload
{
    ValueTask<Result> ExecuteAsync(TPayload payload, CancellationToken cancellationToken = default);
}

