using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Identity.SignUp;

public interface ISignUpStrategy
{
    public ValueTask<Result> ExecuteAsync(SignUpPayload payload, CancellationToken cancellationToken = default);
}

public abstract class SignUpPayload() {}