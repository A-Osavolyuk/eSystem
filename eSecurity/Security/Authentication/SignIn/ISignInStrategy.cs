namespace eSecurity.Security.Authentication.SignIn;

public interface ISignInStrategy
{
    public ValueTask<Result> ExecuteAsync(SignInPayload payload, 
        CancellationToken cancellationToken = default);
}