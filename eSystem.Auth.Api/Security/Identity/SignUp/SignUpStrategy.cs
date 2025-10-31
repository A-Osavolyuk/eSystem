namespace eSystem.Auth.Api.Security.Identity.SignUp;

public abstract class SignUpStrategy
{
    public abstract ValueTask<Result> SignUpAsync(SignUpPayload payload, CancellationToken cancellationToken = default);
}

public abstract class SignUpPayload() {}