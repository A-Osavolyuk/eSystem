namespace eSecurity.Security.Identity.SignUp;

public interface ISignUpStrategy
{
    public ValueTask<Result> SignUpAsync(SignUpPayload payload, CancellationToken cancellationToken = default);
}

public abstract class SignUpPayload() {}