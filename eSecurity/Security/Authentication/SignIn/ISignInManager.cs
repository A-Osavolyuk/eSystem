namespace eSecurity.Security.Authentication.SignIn;

public interface ISignInManager
{
    public ValueTask<AuthenticationResult> AuthenticateAsync(string scheme,
        CancellationToken cancellationToken = default);
}