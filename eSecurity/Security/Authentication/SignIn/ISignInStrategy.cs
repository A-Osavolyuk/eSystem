namespace eSecurity.Security.Authentication.SignIn;

public interface ISignInStrategy
{
    public ValueTask<Result> SignInAsync(SignInPayload payload, 
        CancellationToken cancellationToken = default);
}