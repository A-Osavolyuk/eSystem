using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Identity.SignUp;

public interface ISignUpStrategy
{
    public ValueTask<Result> ExecuteAsync(SignUpPayload payload, CancellationToken cancellationToken = default);
}

public abstract class SignUpPayload() {}